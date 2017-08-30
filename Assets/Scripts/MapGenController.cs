using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using UnityEngine.AI;




public class MapGenController : MonoBehaviour
{
    
    public int NumPisos;
    public int NumHabitaciones;
    private int cantHabitacionesTotal;
    public GameObject room;
    public int dificultad;
    public GameObject player;
    public RoomController.listaEstilos estilo;
    private List<int> probDificultad;
    private List<estructuraPisos> listaPisos;
    public List<condicionesVictoria> objetivosMision;
    public int[,] matrizHabitaciones;
    public float timeToForgetPlayer;

    private List<GameObject> roomList;

    public void init(int dif, int pisos, int numHab, RoomController.listaEstilos style)
    {
        dificultad = dif;
        NumPisos = pisos;
        room = Resources.Load("Prefabs/Room") as GameObject;
        NumHabitaciones = numHab;
        estilo = style;
    }



    public struct estructuraPisos
    {
        public List<estructuraHabitacion> habitaciones;
        public int escaleraEn;

    }

    public struct estructuraHabitacion
    {
        public GameObject habitacion;
        public List<estructuraHabitacion> conectaCon;
        public Vector2 coordenadas;
        public int tam;
        public bool tall;
        public int id;
    }

    public enum condicionesVictoria
    {
        conseguirDocumentos = 1,
        conseguirArmas = 2,
        conseguirPiezas = 3,
        desactivarTrampas = 4,
        eliminarATodosLosEnemigos = 5,
    }

    // Use this for initialization
    void Start()
    {

        timeToForgetPlayer = dificultad * 2;
        cantHabitacionesTotal = 0;
        if(objetivosMision.Count == 0)
            objetivosMision = new List<condicionesVictoria>();
        roomList = new List<GameObject>();
        listaPisos = new List<estructuraPisos>();
        probDificultad = new List<int>();
        matrizHabitaciones = new int[NumPisos, NumHabitaciones * 2];
        matrixInitialator();
        calculoCondicionVictoria();
        lecturaDificultad();
        mapGeneration();
        contruccionMatrizPisos(-1);
        rellenarParedes();

        comprobacionEdificio();

        print("Numero total de habitaciones " + cantHabitacionesTotal);
        //printListaHab ();
        //lecDif ();

        /*for(int j = 0; j < cantHabitacionesTotal; j++)
        {
            print("Hola");
            //transform.GetChild(j).GetComponent<RoomController>().bakeNavMesh();




        }*/

    }

    /*private void comprobacionEdificio()
    {
        List<estructuraHabitacion> idConectados = new List<estructuraHabitacion>();
        List<int> auxLista = new List<int>();
        idConectados.Add(listaPisos[0].habitaciones[0]);


        for (int pisoActual = 0; pisoActual < listaPisos.Count; pisoActual++)
        {
            for (int habActual = 1; habActual < listaPisos[pisoActual].habitaciones.Count; habActual++)
            {
                if (isTouchingThisRoom(idConectados, listaPisos[pisoActual].habitaciones[habActual]))
                {
                    //print("Meto " + listaPisos[pisoActual].habitaciones[habActual].id);
                    idConectados.Add(listaPisos[pisoActual].habitaciones[habActual]);
                }
            }
        }


        string lista = "las habitaciones conectadas son: ";
        for (int i = 0; i < idConectados.Count; i++)
            lista += idConectados[i].id + ", ";
        print(lista);
    }*/


    private void comprobacionEdificio()
    {
        int pisoActual = 0;
        int habActual = 0;
        int cont = 0;
        bool fin = false;

        List<estructuraHabitacion> idConectados = new List<estructuraHabitacion>();
        List<int> auxLista = new List<int>();
        idConectados.Add(listaPisos[pisoActual].habitaciones[habActual]);

        while (cont < idConectados.Count)
        {
            for (int i = 0; i < idConectados[cont].conectaCon.Count; i++)
            {
                bool yaEstaDentro = false;

                for (int j = 0; j < idConectados.Count; j++)
                {
                    if (idConectados[j].id == idConectados[cont].conectaCon[i].id || idConectados[cont].conectaCon[i].id == -1)
                        yaEstaDentro = true;
                }

                if (!yaEstaDentro)
                    idConectados.Add(idConectados[cont].conectaCon[i]);
            }
            cont++;
        }

        string lista = "las habitaciones conectadas son: ";
        for (int i = 0; i < idConectados.Count; i++)
            lista += idConectados[i].id + ", ";
        print(lista);

        if (objetivosMision.Contains(condicionesVictoria.conseguirDocumentos))
            idConectados[idConectados.Count - 1].habitacion.GetComponent<RoomController>().reward = ObjetoRecompensa.tipoRecompensa.documentos;

        if (objetivosMision.Contains(condicionesVictoria.conseguirPiezas))
        {
            int auxID = cantHabitacionesTotal * 2 / 3;
            bool done = false;
            while (!done)
            {
                if (getRoomObjectByID(auxID) != null)
                {
                    if (getRoomObjectByID(auxID).GetComponent<RoomController>().reward == ObjetoRecompensa.tipoRecompensa.ninguno)
                    {
                        getRoomObjectByID(auxID).GetComponent<RoomController>().reward = ObjetoRecompensa.tipoRecompensa.piezasSecretas;
                        print("Genero piezas en "+auxID);
                        done = true;
                    }
                    else 
                        auxID++;
                }

            }


        }
    }

    private void matrixInitialator()
    {
        matrizHabitaciones.Initialize();
        for (int i = 0; i < NumPisos; i++)
        {
            for (int j = 0; j < NumHabitaciones * 2; j++)
            {
                matrizHabitaciones[i, j] = -1;
            }
        }
    }

    private void printListaHab()
    {
        for (int i = 0; i < NumPisos; i++)
        {
            string linea = "Piso " + i + "(";
            for (int j = 0; j < NumHabitaciones * 2; j++)
            {
                linea += matrizHabitaciones[i, j] + ", ";
            }
            linea += ")";
            print(linea);
        }

    }

    private void mapGeneration()
    {
        int cantHabitaciones = NumPisos * NumHabitaciones;

        int idRoom;
        if (room == null)
            print("Room es null");

        int espaciosRestantes;
        Vector3 posicionActual = new Vector3(0, 0, 0);

        idRoom = 0;
        for (int pisoActual = 0; pisoActual < NumPisos; pisoActual++)
        {
            posicionActual.x = 0;

            espaciosRestantes = NumHabitaciones * 2;
            estructuraPisos auxPiso = new estructuraPisos(); //Creo el piso auxiliar donde guardar los datos en la estructura de pisos
            auxPiso.habitaciones = new List<estructuraHabitacion>();
            auxPiso.escaleraEn = -1;

            int habActual = 0;

            while (espaciosRestantes > 0)
            { // Mientras que sigan habiendo espacios / Pequenya 1 - Mediana 2 . Grande 4

                int coordenadasActuales = (NumHabitaciones * 2 - espaciosRestantes);

                float tHab = RoomController.T_HAB;

                if (puedoConstruir(pisoActual, coordenadasActuales))
                {

                    //print ("En el piso "+pisoActual+" hay " + espaciosRestantes + " espacios restantes");
                    estructuraHabitacion auxHab = new estructuraHabitacion();
                    if (pisoActual == 0 && espaciosRestantes == cantHabitaciones * 2)
                    {
                        auxHab.coordenadas = new Vector2(0, pisoActual);
                    }
                    else
                        auxHab.coordenadas = new Vector2(coordenadasActuales, pisoActual);

                    auxHab.conectaCon = new List<estructuraHabitacion>();
                    GameObject habitacion = Instantiate(room, new Vector3(posicionActual.x, posicionActual.y, posicionActual.z), Quaternion.identity);
                    if (habActual < 10)
                        habitacion.transform.name = "Habitacion: " + pisoActual + "0" + habActual;
                    else
                        habitacion.transform.name = "Habitacion: " + pisoActual + "" + habActual;

                    habitacion.GetComponent<RoomController>().ladderReceived = -1;
                    habitacion.GetComponent<RoomController>().id = idRoom;
                    habitacion.GetComponent<RoomController>().piso = pisoActual;
                    habitacion.transform.Rotate(new Vector3(0, 180, 0));

                    if (pisoActual != NumPisos - 1)
                        habitacion.GetComponent<RoomController>().tall = Random.Range(0, 3) == 1 ? true : false;
                    else
                        habitacion.GetComponent<RoomController>().tall = false;

                    auxHab.tall = habitacion.GetComponent<RoomController>().tall;

                    auxHab.id = idRoom;
                    auxHab.habitacion = habitacion;

                    int contRandom = 0;

                    if (espaciosRestantes > 3)
                        contRandom = 3;
                    else if (espaciosRestantes > 1)
                        contRandom = 2;
                    else if (espaciosRestantes == 1)
                        contRandom = 1;



                    bool listo = false;
                    while (!listo)
                    {

                        switch (Random.Range(1, (contRandom + 1)))
                        {
                            case 2:
                                if (puedoConstruir(pisoActual, coordenadasActuales) && puedoConstruir(pisoActual, coordenadasActuales + 1))
                                {
                                    habitacion.GetComponent<RoomController>().tamanyo = RoomController.roomSize.medium;
                                    posicionActual.x = posicionActual.x + tHab;
                                    espaciosRestantes = espaciosRestantes - 2;
                                    auxHab.tam = 2;
                                    listo = true;
                                }
                                break;
                            case 3:
                                if (puedoConstruir(pisoActual, coordenadasActuales) && puedoConstruir(pisoActual, coordenadasActuales + 1) && puedoConstruir(pisoActual, coordenadasActuales + 2) && puedoConstruir(pisoActual, coordenadasActuales + 3))
                                {
                                    habitacion.GetComponent<RoomController>().tamanyo = RoomController.roomSize.large;
                                    posicionActual.x = posicionActual.x + tHab * 2f;
                                    espaciosRestantes = espaciosRestantes - 4;
                                    auxHab.tam = 4;
                                    listo = true;
                                }
                                break;
                            default:
                                if (puedoConstruir(pisoActual, coordenadasActuales))
                                {
                                    habitacion.GetComponent<RoomController>().tamanyo = RoomController.roomSize.small;
                                    espaciosRestantes--;
                                    posicionActual.x = posicionActual.x + tHab / 2f;
                                    auxHab.tam = 1;
                                    listo = true;
                                }
                                break;
                        }

                    }
                    if (pisoActual != 0)
                    {
                        if (listaPisos[pisoActual - 1].escaleraEn >= auxHab.coordenadas.x && listaPisos[pisoActual - 1].escaleraEn < auxHab.coordenadas.x + auxHab.tam)
                        {
                            habitacion.GetComponent<RoomController>().ladderReceived = listaPisos[pisoActual - 1].escaleraEn - (int)auxHab.coordenadas.x;
                            //print ("La habitacion "+habitacion.GetComponent<RoomController> ().name+" recibe la escalera en "+habitacion.GetComponent<RoomController> ().ladderReceived+" posicion");
                        }

                    }

                    //METER AQUI EL CALCULO DEL PORCENTAJE DE APARICION DE ESCALERA EN BASE A LA DIFICULTAD



                    if (pisoActual < NumPisos - 1 && habitacion.GetComponent<RoomController>().ladderReceived == -1)
                    {
                        if (pisoActual == 0)
                        {
                            if (auxPiso.escaleraEn == -1 && espaciosRestantes == 0)
                            {
                                habitacion.GetComponent<RoomController>().tall = false;
                                habitacion.GetComponent<RoomController>().ladderPosition = Random.Range(1, auxHab.tam);
                                //print ("Coordenadas escalera piso "+ pisoActual +": " + (auxHab.coordenadas.x + habitacion.GetComponent<RoomController> ().ladderPosition - 1));
                                auxPiso.escaleraEn = (int)(auxHab.coordenadas.x + habitacion.GetComponent<RoomController>().ladderPosition - 1);
                            }

                            auxHab.tall = habitacion.GetComponent<RoomController>().tall;

                            if (auxPiso.escaleraEn == -1 && !auxHab.tall)
                            {
                                if (Random.Range(0, 3) == 1)
                                {
                                    habitacion.GetComponent<RoomController>().ladderPosition = Random.Range(1, auxHab.tam);
                                    //print ("Coordenadas escalera piso "+ pisoActual +": " + (auxHab.coordenadas.x + habitacion.GetComponent<RoomController> ().ladderPosition - 1));
                                    auxPiso.escaleraEn = (int)(auxHab.coordenadas.x + habitacion.GetComponent<RoomController>().ladderPosition - 1);
                                }
                            }
                        }
                        else
                        {
                            if (listaPisos[pisoActual - 1].escaleraEn >= auxHab.coordenadas.x && listaPisos[pisoActual - 1].escaleraEn < auxHab.coordenadas.x + auxHab.tam)
                            {

                            }
                            else
                            {
                                if (auxPiso.escaleraEn == -1 && espaciosRestantes == 0)
                                {
                                    habitacion.GetComponent<RoomController>().tall = false;
                                    habitacion.GetComponent<RoomController>().ladderPosition = Random.Range(1, auxHab.tam);
                                    //print ("Coordenadas escalera piso "+ pisoActual +": " + (auxHab.coordenadas.x + habitacion.GetComponent<RoomController> ().ladderPosition - 1));
                                    auxPiso.escaleraEn = (int)(auxHab.coordenadas.x + habitacion.GetComponent<RoomController>().ladderPosition - 1);
                                }

                                auxHab.tall = habitacion.GetComponent<RoomController>().tall;

                                if (auxPiso.escaleraEn == -1 && !auxHab.tall)
                                {
                                    if (Random.Range(0, 3) == 1)
                                    {
                                        habitacion.GetComponent<RoomController>().ladderPosition = Random.Range(1, auxHab.tam);
                                        //print ("Coordenadas escalera piso "+ pisoActual +": " + (auxHab.coordenadas.x + habitacion.GetComponent<RoomController> ().ladderPosition - 1));
                                        auxPiso.escaleraEn = (int)(auxHab.coordenadas.x + habitacion.GetComponent<RoomController>().ladderPosition - 1);
                                    }
                                }
                            }
                        }
                    }

                    habitacion.GetComponent<RoomController>().tipoHabitacion = RoomController.tipo.HabitacionPrincipal;

                    if (idRoom == 0 && pisoActual == 0)
                        habitacion.GetComponent<RoomController>().playerSpawn = true;
                    else
                        habitacion.GetComponent<RoomController>().playerSpawn = false;

                    //rellenarPeredes (habitacion, habActual, coordenadasActuales);

                    if (pisoActual == NumPisos - 1)
                        habitacion.GetComponent<RoomController>().hasTecho = true;
                    else
                        habitacion.GetComponent<RoomController>().hasTecho = false;

                    if (pisoActual == NumPisos - 2 && auxHab.tall)
                        habitacion.GetComponent<RoomController>().hasTecho = true;

                    habitacion.GetComponent<RoomController>().estilo = estilo;


                    //print ("He hecho "+cont + "iteraciones");

                    //print ("--- Soy la habitacion "+idRoom+" con dificultad -> "+aux+" ---");

                    habitacion.GetComponent<RoomController>().dificultad = dificultadParaHabitacion();
                    habitacion.GetComponent<RoomController>().mapGenController = this.gameObject;
                    habitacion.transform.SetParent(this.transform);
                    roomList.Add(habitacion);
                    cantHabitacionesTotal++;
                    idRoom++;
                    habActual++;

                    auxPiso.habitaciones.Add(auxHab);


                }
                else
                {

                    int restante = ((int)listaPisos[pisoActual - 1].habitaciones[getIndexHabitacionPorCoordenada(pisoActual - 1, coordenadasActuales)].coordenadas.x + listaPisos[pisoActual - 1].habitaciones[getIndexHabitacionPorCoordenada(pisoActual - 1, coordenadasActuales)].tam) - coordenadasActuales;
                    posicionActual.x = posicionActual.x + (tHab / 2f * restante);
                    //print ("Estoy en (" + pisoActual + "," + coordenadasActuales + ") y me sobra " + restante);
                    espaciosRestantes = espaciosRestantes - restante;

                }
            }
            //66.3f
            posicionActual.y = 66.5f * (pisoActual + 1);

            if (auxPiso.escaleraEn == -1 && pisoActual != NumPisos - 1)
                print("PROBLEMAS EN EL PISO " + pisoActual);

            listaPisos.Add(auxPiso);
        }


    }

    private void contruccionMatrizPisos(int pisoObjetivo)
    {

        if (pisoObjetivo == -1)
        {
            for (int piso = 0; piso < listaPisos.Count; piso++)
            {
                for (int habitacion = 0; habitacion < listaPisos[piso].habitaciones.Count; habitacion++)
                {
                    for (int i = 0; i < listaPisos[piso].habitaciones[habitacion].tam; i++)
                    {
                        matrizHabitaciones[piso, (int)listaPisos[piso].habitaciones[habitacion].coordenadas.x + i] = listaPisos[piso].habitaciones[habitacion].id;
                        if (listaPisos[piso].habitaciones[habitacion].tall)
                            matrizHabitaciones[piso + 1, (int)listaPisos[piso].habitaciones[habitacion].coordenadas.x + i] = listaPisos[piso].habitaciones[habitacion].id;
                    }
                }
            }
        }
        else
        {
            for (int habitacion = 0; habitacion < listaPisos[pisoObjetivo].habitaciones.Count; habitacion++)
            {
                for (int i = 0; i < listaPisos[pisoObjetivo].habitaciones[habitacion].tam; i++)
                {
                    matrizHabitaciones[pisoObjetivo, (int)listaPisos[pisoObjetivo].habitaciones[habitacion].coordenadas.x + i] = listaPisos[pisoObjetivo].habitaciones[habitacion].id;
                    if (listaPisos[pisoObjetivo].habitaciones[habitacion].tall)
                        matrizHabitaciones[pisoObjetivo + 1, (int)listaPisos[pisoObjetivo].habitaciones[habitacion].coordenadas.x + i] = listaPisos[pisoObjetivo].habitaciones[habitacion].id;
                }
            }
        }

    }

    public int dificultadParaHabitacion()
    {

        int aux = 0;
        int cont = 0;
        int total = 0;

        for (int i = 0; i < probDificultad.Count; i++)
        {
            total += probDificultad[i];
        }

        //print ("Total es "+total);

        if (total != 0)
        {
            while (aux == 0)
            {
                int it = Random.Range(0, probDificultad.Count);
                //print ("La prob de " +it+" es "+probDificultad [it]);
                if (probDificultad[it] > 0)
                {
                    aux = it + 1;
                    probDificultad[it] = probDificultad[it] - 1;
                }
            }
        }
        else
        {
            if (dificultad > 6)
                aux = 6;
            else
                aux = dificultad;
        }
        return aux;
    }

    public void lecDif()
    {

        for (int i = 0; i < probDificultad.Count; i++)
        {
            print(probDificultad[i]);

        }
    }

    public estructuraHabitacion getHabitacionPorID(int id)
    {
        estructuraHabitacion aux = new estructuraHabitacion();
        aux.id = -1;

        for (int i = 0; i < listaPisos.Count; i++)
        {
            for (int j = 0; j < listaPisos[i].habitaciones.Count; i++)
            {
                if (listaPisos[i].habitaciones[j].id == id)
                {
                    aux = listaPisos[i].habitaciones[j];
                    return aux;
                }
            }
        }
        return aux;
    }

    public GameObject getRoomObjectByID(int id)
    {
        GameObject aux = null;

        for (int pisoActual = 0; pisoActual < NumPisos; pisoActual++)
        {
            for (int habActual = 0; habActual < listaPisos[pisoActual].habitaciones.Count; habActual++)
            {
                if (listaPisos[pisoActual].habitaciones[habActual].id == id)
                    aux = listaPisos[pisoActual].habitaciones[habActual].habitacion;

            }
        }

        return aux;

    }



    public void rellenarParedes()
    {

        for (int pisoActual = 0; pisoActual < NumPisos; pisoActual++)
        {
            for (int habActual = 0; habActual < listaPisos[pisoActual].habitaciones.Count; habActual++)
            {

                GameObject habitacion = listaPisos[pisoActual].habitaciones[habActual].habitacion;
                Vector2 coordenadas = listaPisos[pisoActual].habitaciones[habActual].coordenadas;
                int id = listaPisos[pisoActual].habitaciones[habActual].id;
                int tam = listaPisos[pisoActual].habitaciones[habActual].tam;
                bool tall = listaPisos[pisoActual].habitaciones[habActual].tall;

                habitacion.GetComponent<RoomController>().setlistaLaterales(0, RoomController.tiposParedes.puerta);
                habitacion.GetComponent<RoomController>().setlistaLaterales(1, RoomController.tiposParedes.puerta);

                if (habActual != 0) //Por lo general la puerta izquierda se omite
                    habitacion.GetComponent<RoomController>().setlistaLaterales(0, RoomController.tiposParedes.nada);

                if (coordenadas.x + tam == NumHabitaciones * 2)// si es la ultima habitacion, a la derecha tiene pared
                    habitacion.GetComponent<RoomController>().setlistaLaterales(1, RoomController.tiposParedes.pared);

                if (pisoActual != 0)
                {

                    if (coordenadas.x == 0)
                        habitacion.GetComponent<RoomController>().setlistaLaterales(0, RoomController.tiposParedes.pared);

                    if (coordenadas.x + tam < NumHabitaciones * 2 && matrizHabitaciones[pisoActual, (int)(coordenadas.x + tam)] < id)
                        habitacion.GetComponent<RoomController>().setlistaLaterales(1, RoomController.tiposParedes.trampilla);

                    if (coordenadas.x > 0 && matrizHabitaciones[pisoActual, (int)(coordenadas.x - 1)] == matrizHabitaciones[pisoActual - 1, (int)(coordenadas.x - 1)])
                        habitacion.GetComponent<RoomController>().setlistaLaterales(0, RoomController.tiposParedes.nada);

                }

                if (tall)
                {//Si es una habitacion alta

                    if (coordenadas.x == 0)
                    { // Si está en la primera posicion, pared 
                        habitacion.GetComponent<RoomController>().setlistaLaterales(2, RoomController.tiposParedes.pared);
                    }

                    if (coordenadas.x + tam == NumHabitaciones * 2) // Si acaba en la ultima posicion, pared
                        habitacion.GetComponent<RoomController>().setlistaLaterales(3, RoomController.tiposParedes.pared);

                    /*
					if(coordenadas.x != 0 && matrizHabitaciones[pisoActual, (int) (coordenadas.x -1)] != matrizHabitaciones[pisoActual+1, (int) (coordenadas.x -1)])
						habitacion.GetComponent<RoomController> ().setlistaLaterales (2, RoomController.tiposParedes.trampilla);
					*/
                    /*if (coordenadas.x != 0 && matrizHabitaciones [pisoActual, (int)(coordenadas.x - 1)] == matrizHabitaciones [pisoActual + 1, (int)(coordenadas.x - 1)])
						habitacion.GetComponent<RoomController> ().setlistaLaterales (2, RoomController.tiposParedes.pared);
*/
                    if (coordenadas.x + tam < NumHabitaciones * 2 && matrizHabitaciones[pisoActual, (int)(coordenadas.x + tam)] != matrizHabitaciones[pisoActual + 1, (int)(coordenadas.x + tam)])
                        habitacion.GetComponent<RoomController>().setlistaLaterales(3, RoomController.tiposParedes.trampilla);

                    if (coordenadas.x + tam < NumHabitaciones * 2 && matrizHabitaciones[pisoActual, (int)(coordenadas.x + tam)] == matrizHabitaciones[pisoActual + 1, (int)(coordenadas.x + tam)])
                        habitacion.GetComponent<RoomController>().setlistaLaterales(3, RoomController.tiposParedes.pared);

                }
            }
        }

        for (int pisoActual = 0; pisoActual < NumPisos; pisoActual++)
        {
            for (int habActual = 0; habActual < listaPisos[pisoActual].habitaciones.Count; habActual++)
            {
                GameObject habitacion = listaPisos[pisoActual].habitaciones[habActual].habitacion;

                if (habitacion.GetComponent<RoomController>().listaLaterales[1] == RoomController.tiposParedes.puerta)
                {
                    listaPisos[pisoActual].habitaciones[habActual].conectaCon.Add(listaPisos[pisoActual].habitaciones[habActual + 1]);
                    listaPisos[pisoActual].habitaciones[habActual + 1].conectaCon.Add(listaPisos[pisoActual].habitaciones[habActual]);
                }

                if (habitacion.GetComponent<RoomController>().ladderPosition != 0)
                {
                    //print("La habitacion " + listaPisos[pisoActual].habitaciones[habActual].id + " conecta con " + (listaPisos[pisoActual + 1].habitaciones[getIndexHabitacionPorCoordenada(pisoActual + 1, listaPisos[pisoActual].escaleraEn)].id));
                    listaPisos[pisoActual].habitaciones[habActual].conectaCon.Add(listaPisos[pisoActual + 1].habitaciones[getIndexHabitacionPorCoordenada(pisoActual + 1, listaPisos[pisoActual].escaleraEn)]);
                    listaPisos[pisoActual + 1].habitaciones[getIndexHabitacionPorCoordenada(pisoActual + 1, listaPisos[pisoActual].escaleraEn)].conectaCon.Add(listaPisos[pisoActual].habitaciones[habActual]);
                }


            }
        }

    }

    private int posicionNextRoom(int piso)
    {

        if (piso < NumPisos)
        {
            for (int i = 0; i < NumHabitaciones * 2; i++)
            {
                if (matrizHabitaciones[piso, i] == -1)
                    return i;
            }
        }
        print("No hay mas espacio en el piso " + piso);
        return -1;

    }

    private int tamRoomMaxDisponible(int piso)
    {
        int aux = 0;
        bool count = false;
        for (int i = 0; i < NumHabitaciones * 2; i++)
        {

            if (matrizHabitaciones[piso, i] == -1 && aux == 0)
                count = true;

            if (count && matrizHabitaciones[piso, i] == -1)
                aux++;

            if (count && matrizHabitaciones[piso, i] != -1)
                count = false;
        }

        return aux;
    }

    void Update()
    {

    }

    public void calculoCondicionVictoria()
    {

        switch (Random.Range(0, 2))
        {
            case 0:
                //print ((int)condicionesVictoria.conseguirDocumentos);
                break;
            case 1:
                //print ((int)condicionesVictoria.desactivarTrampas);
                break;

        }
    }

    public int getIndexHabitacionPorCoordenada(int piso, int coordenada)
    {
        for (int i = 0; i < listaPisos[piso].habitaciones.Count; i++)
        {
            if (listaPisos[piso].habitaciones[i].coordenadas.x <= coordenada && coordenada < listaPisos[piso].habitaciones[i].coordenadas.x + listaPisos[piso].habitaciones[i].tam)
                return i;
        }
        return -1;
    }

    public int getIndexHabitacionCompletaPorCoordenada(int piso, int coordenada)
    {
        for (int i = 0; i < listaPisos[piso].habitaciones.Count; i++)
        {
            if (piso > 0 && getIndexHabitacionPorCoordenada(piso, coordenada) == -1 && getIndexHabitacionPorCoordenada(piso - 1, coordenada) != -1 && listaPisos[piso - 1].habitaciones[getIndexHabitacionPorCoordenada(piso - 1, coordenada)].tall)
                return getIndexHabitacionPorCoordenada(piso - 1, coordenada);
        }
        return -1;
    }

    public bool puedoConstruir(int piso, int coordenada)
    {
        if (piso > 0)
        {
            if (getIndexHabitacionPorCoordenada(piso - 1, coordenada) != -1 && !listaPisos[piso - 1].habitaciones[getIndexHabitacionPorCoordenada(piso - 1, coordenada)].tall)
            {
                //print ("Estoy en " + piso + " - " + coordenada + " y puedo contruir encima de " + (piso - 1) + " - " + coordenada);
                return true;
            }
        }
        if (piso > 1)
        {
            if (getIndexHabitacionPorCoordenada(piso - 2, coordenada) != -1 && listaPisos[piso - 2].habitaciones[getIndexHabitacionPorCoordenada(piso - 2, coordenada)].tall)
            {
                //print ("Estoy en " + piso + " - " + coordenada + " y puedo contruir encima de " + (piso - 2) + " - " + coordenada);
                return true;
            }
        }
        if (piso == 0)
            return true;
        //print ("Estoy en " + piso + " - " + coordenada + " y NO puedo contruir encima de " + (piso - 1) + " - " + coordenada);
        return false;
    }

    private void lecturaDificultad()
    {

        FileInfo theSourceFile = null;
        StreamReader reader = null;
        string text = " "; // assigned to allow first line to be read below

        theSourceFile = new FileInfo("tablaDificultad.txt");
        reader = theSourceFile.OpenText();
        bool listo = false;
        string[] strArr;
        while (!listo)
        {
            text = reader.ReadLine();
            strArr = text.Split(' ');
            if (int.Parse(strArr[0]) == dificultad)
            {
                for (int i = 1; i < strArr.Length; i++)
                {

                    float aux = int.Parse(strArr[i]) * NumHabitaciones * NumPisos / 100f;
                    probDificultad.Add(Mathf.RoundToInt(aux));
                    //print ("El valor es " +Mathf.RoundToInt(aux));
                }
                listo = true;
            }
        }


    }

}
