using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Edificio : MonoBehaviour
{

    public int NumPisos;
    public float initialTime;
    public int NumHabitaciones;
    public int cantTrampasHackeablesTotal;
    private int cantHabitacionesTotal;
    public GameObject room;
    public int dificultad;
    public GameObject player;
    public listaEstilos estilo;
    private List<int> probDificultad;
    private List<estructuraPisos> listaPisos;
    public List<condicionesVictoria> objetivosMision;
    public int[,] matrizHabitaciones;
    public float timeToForgetPlayer;
    public float tiempoNivel;
    public int NumeroTrampas; //Solo minas y cepos
    public bool pausado;
    private bool buildingSaved;
    private bool mapLoaded;

    private List<GameObject> roomList;

    public enum listaEstilos
    {
        oficina,
        casaCutre,
        casaNormal,
        casaPija
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
        public listaEstilos estiloHabitacion;
        public RoomController.tipo tipoHabitacion;
        public Vector2 coordenadas;
        public int tam;
        public bool tall;
        public int id;
        public int piso;
        public bool hasTecho;
        public int cameraPosition;
        public int nivel;
        public int cantSitiosOcultos;
        public ObjetoRecompensa.tipoRecompensa reward;
        public RoomController.tiposParedes[] listaLaterales;
        public List<GeneradorObjetos.trapStruct> listaTrampas;
    }

    public enum condicionesVictoria
    {
        conseguirDocumentos = 1,
        conseguirPiezas = 2,
        desactivarTrampas = 3,
    }

    // Use this for initialization
    void Start()
    {
        pausado = false;
        buildingSaved = false;
        tiempoNivel = 0;
        cantTrampasHackeablesTotal = 0;
        timeToForgetPlayer = dificultad * 1.5f;
        cantHabitacionesTotal = 0;
        matrizHabitaciones = new int[NumPisos, NumHabitaciones * 2];
        matrixInitialator();

        if (!mapLoaded)
        {
            if (objetivosMision.Count == 0)
                objetivosMision = new List<condicionesVictoria>();
            //roomList = new List<GameObject>();
            listaPisos = new List<estructuraPisos>();
            probDificultad = new List<int>();
            lecturaDificultad();
            mapGeneration();
        }
        MapBuilder();

        if (!mapLoaded)
        {
            rellenarParedes();
            comprobacionEdificio();
        }
    }

    public void loadedMap(List<estructuraPisos> lista, condicionesVictoria condicion0, condicionesVictoria condicion1, int dif)
    {
        mapLoaded = true;
        objetivosMision = new List<condicionesVictoria>();
        objetivosMision.Add(condicion0);
        objetivosMision.Add(condicion1);
        listaPisos = lista;
        estilo = lista[0].habitaciones[0].estiloHabitacion;
        NumPisos = listaPisos.Count;
        room = Resources.Load("Prefabs/Room", typeof(GameObject)) as GameObject;
        dificultad = dif;
        int tamAcum = 0;
        for (int i = 0; i < lista[0].habitaciones.Count; i++)
            tamAcum += lista[0].habitaciones[i].tam;

        NumHabitaciones = (tamAcum/2);
    }

    private void comprobacionEdificio()
    {
        int pisoActual = 0;
        int habActual = 0;
        int cont = 0;

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
        /*
        string lista = "las habitaciones conectadas son: ";
        for (int i = 0; i < idConectados.Count; i++)
            lista += idConectados[i].id + ", ";
        print(lista);*/

        if (objetivosMision.Contains(condicionesVictoria.conseguirDocumentos))
        {
            idConectados[idConectados.Count - 1].habitacion.GetComponent<RoomController>().reward = ObjetoRecompensa.tipoRecompensa.documentos;
            setReward(ObjetoRecompensa.tipoRecompensa.documentos, idConectados.Count - 1);
        }

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
                        setReward(ObjetoRecompensa.tipoRecompensa.piezasSecretas, auxID);
                        done = true;
                    }
                    else
                        auxID++;
                }
            }
        }
    }

    public static listaEstilos getEstiloRandom()
    {
        switch (Random.Range(0, 2))
        { // Se elige el estilo entre los que hay
            case 0:
                return listaEstilos.casaNormal;
            case 1:
                return listaEstilos.oficina;
            default:
                return listaEstilos.casaNormal;
        }
    }

    private void matrixInitialator()
    {
        matrizHabitaciones.Initialize();
        for (int i = 0; i < NumPisos; i++)
            for (int j = 0; j < NumHabitaciones * 2; j++)
                matrizHabitaciones[i, j] = -1;
    }

    private void printListaHab()
    {
        for (int i = 0; i < NumPisos; i++)
        {
            string linea = "Piso " + i + "(";
            for (int j = 0; j < NumHabitaciones * 2; j++)
                linea += matrizHabitaciones[i, j] + ", ";
            linea += ")";
            print(linea);
        }

    }

    public static condicionesVictoria getRandomCondicionVictoria()
    {
        condicionesVictoria aux = new condicionesVictoria();
        switch (Random.Range(0, 3))
        {
            case 0:
                aux = condicionesVictoria.conseguirDocumentos;
                break;
            case 1:
                aux = condicionesVictoria.conseguirPiezas;
                break;
            case 2:
                aux = condicionesVictoria.desactivarTrampas;
                break;
        }
        return aux;
    }

    private estructuraPisos generateFloor(int piso)
    {
        estructuraPisos auxPiso = new estructuraPisos();
        auxPiso.habitaciones = new List<estructuraHabitacion>();
        auxPiso.escaleraEn = -1;

        int espaciosRestantes = (int)espacioLibreEnPiso(piso);
        int idRoom = 0;
        if (piso != 0)
            idRoom = listaPisos[piso - 1].habitaciones[listaPisos[piso - 1].habitaciones.Count - 1].id + 1;

        int HabActual, maxEspDisponible;

        while (espaciosRestantes > 0)
        {
            estructuraHabitacion auxHab = new estructuraHabitacion();
            HabActual = (int)HabitacionMasGrandeDisponible(piso).x;
            maxEspDisponible = (int)HabitacionMasGrandeDisponible(piso).y;

            auxHab.id = idRoom;
            auxHab.estiloHabitacion = estilo;
            auxHab.tipoHabitacion = RoomController.tipo.HabitacionPrincipal;
            int contRandom = 0;

            if (idRoom == 0)
                auxHab.tipoHabitacion = RoomController.tipo.Entrada;

            auxHab.coordenadas = new Vector2(piso, HabActual);
            auxHab.tall = false;
            auxHab.conectaCon = new List<estructuraHabitacion>();
            auxHab.hasTecho = false;

            auxHab.listaLaterales = new RoomController.tiposParedes[4];
            for (int i = 0; i < 4; i++)
                auxHab.listaLaterales[i] = RoomController.tiposParedes.nada;

            if (maxEspDisponible > 3)
                contRandom += 3;
            else if (maxEspDisponible > 1)
                contRandom += 2;
            else if (maxEspDisponible == 1)
                contRandom += 1;

            int auxTam = Random.Range(1, (contRandom + 1));
            if (auxTam == 1 && auxHab.tipoHabitacion == RoomController.tipo.HabitacionPrincipal)
                auxHab.tipoHabitacion = Random.Range(0, 5) == 0 ? RoomController.tipo.Banyo : RoomController.tipo.HabitacionPrincipal;

            switch (auxHab.tipoHabitacion)
            {
                case RoomController.tipo.Entrada:
                    if (maxEspDisponible > 1)
                        auxTam = Random.Range(1, 3);
                    else
                        auxTam = 1;
                    break;
                case RoomController.tipo.Banyo:
                        auxTam = 1;
                    break;
            }

            switch (auxTam)
            {
                case 2:
                    espaciosRestantes = espaciosRestantes - 2;
                    auxHab.tam = 2;
                    break;
                case 3:
                    espaciosRestantes = espaciosRestantes - 4;
                    auxHab.tam = 4;
                    break;
                default:
                    espaciosRestantes--;
                    auxHab.tam = 1;
                    break;
            }

            if (piso < (NumPisos - 1))
                auxHab.tall = Random.Range(0, 3) == 1 ? true : false;

            if ((piso == NumPisos - 2 && auxHab.tall) || piso == (NumPisos - 1))
                auxHab.hasTecho = true;

            for (int i = HabActual; i < HabActual + auxHab.tam; i++)
            {
                matrizHabitaciones[piso, i] = auxHab.id;
                if (auxHab.tall)
                    matrizHabitaciones[piso + 1, i] = auxHab.id;

            }
            cantHabitacionesTotal++;
            auxPiso.habitaciones.Add(auxHab);
            idRoom++;
        }//Acabo de generar los pisos

        if (piso < (NumPisos - 1))
        {
            if (areAllTall(auxPiso, true) || areAllTall(auxPiso, false)) //Compruebo que no todos sean altos, ni todos sean bajos (al menos 1 de cada)
            {
                int habCambio = Random.Range(0, auxPiso.habitaciones.Count);
                estructuraHabitacion habTemp = auxPiso.habitaciones[habCambio];
                habTemp.tall = !habTemp.tall;
                if (piso == NumPisos - 2) {
                    if (habTemp.tall)
                        habTemp.hasTecho = true;
                    else
                        habTemp.hasTecho = false;
                }

                auxPiso.habitaciones[habCambio] = habTemp;

                for (int i = (int)habTemp.coordenadas.y; i < (int)habTemp.coordenadas.y + habTemp.tam; i++)
                {
                    matrizHabitaciones[piso, i] = habTemp.id;
                    if (habTemp.tall)
                        matrizHabitaciones[piso + 1, i] = habTemp.id;
                    else
                        matrizHabitaciones[piso + 1, i] = -1;

                }


            }

        }

        return auxPiso;
    }

    private int espacioLibreEnPiso(int piso)
    {
        int espacio = 0;
        for (int i = 0; i < NumHabitaciones * 2; i++)
        {
            if (matrizHabitaciones[piso, i] == -1)
            {
                espacio++;
            }
        }
        return espacio;
    }

    private Vector2 HabitacionMasGrandeDisponible(int piso)
    {
        int espacio = 0;
        int comienzo = -1;
        bool salir = false;
        for (int i = 0; i < NumHabitaciones * 2; i++)
        {
            if (comienzo != -1 && matrizHabitaciones[piso, i] != -1)
                salir = true;

            if (matrizHabitaciones[piso, i] == -1 && !salir)
            {
                if (comienzo == -1)
                    comienzo = i;
                espacio++;
            }
        }
        return new Vector2(comienzo, espacio);
    }

    private void deleteFloorFromMatriz(int piso)
    {
        for (int i = 0; i < NumHabitaciones * 2; i++)
        {
            matrizHabitaciones[piso, i] = -1;
        }
    }

    private void mapGeneration()
    {
        for (int pisoActual = 0; pisoActual < NumPisos; pisoActual++)
            listaPisos.Add(generateFloor(pisoActual));
    }

    private void MapBuilder()
    {
        float tHab = RoomController.T_HAB;
        float altHab = RoomController.ALT_HAB;
        Vector3 posicionActual = new Vector3(0, 0, 0);
        for (int pisoActual = 0; pisoActual < NumPisos; pisoActual++)
        {
            posicionActual.y = altHab * pisoActual;
            for (int habActual = 0; habActual < listaPisos[pisoActual].habitaciones.Count; habActual++)
            {
                estructuraHabitacion auxHab = listaPisos[pisoActual].habitaciones[habActual];
                posicionActual.x = tHab / 2 * auxHab.coordenadas.y;
                GameObject habitacion = Instantiate(room, new Vector3(posicionActual.x, posicionActual.y, posicionActual.z), Quaternion.identity, this.transform);
                habitacion.transform.Rotate(0, 180, 0);
                habitacion.GetComponent<RoomController>().edificio = transform.GetComponent<Edificio>();

                if (habActual < 10)
                    habitacion.transform.name = "Habitacion: " + pisoActual + "0" + habActual;
                else
                    habitacion.transform.name = "Habitacion: " + pisoActual + "" + habActual;

                RoomController auxRoom = habitacion.GetComponent<RoomController>();
                auxRoom.tall = auxHab.tall;
                auxRoom.estilo = auxHab.estiloHabitacion;
                auxRoom.tipoHabitacion = auxHab.tipoHabitacion;
                auxRoom.hasTecho = auxHab.hasTecho;
                auxHab.habitacion = habitacion;
                auxRoom.id = auxHab.id;
                auxRoom.piso = pisoActual;
                if (mapLoaded)
                {
                    auxRoom.listaLaterales = auxHab.listaLaterales;
                    auxRoom.listaTrampas = auxHab.listaTrampas;
                    auxRoom.reward = auxHab.reward;
                    auxRoom.dificultad = auxHab.nivel;
                }
                else
                {
                    auxRoom.dificultad = dificultadParaHabitacion();
                    auxHab.nivel = auxRoom.dificultad;
                }
                    
                switch (auxHab.tam)
                {
                    case 2:
                        auxRoom.tamanyo = RoomController.roomSize.medium;
                        break;
                    case 4:
                        auxRoom.tamanyo = RoomController.roomSize.large;
                        break;
                    default:
                        auxRoom.tamanyo = RoomController.roomSize.small;
                        break;
                }

                listaPisos[pisoActual].habitaciones[habActual] = auxHab;


            }
        }


    }

    public void setListaLaterales(RoomController.tiposParedes[] var, int idRoom)
    {
        Vector2 aux = getRoomPosbyID(idRoom);
        estructuraHabitacion habTemp = listaPisos[(int)aux.x].habitaciones[(int)aux.y];
        habTemp.listaLaterales = var;
        listaPisos[(int)aux.x].habitaciones[(int)aux.y] = habTemp;
    }

    public void setReward(ObjetoRecompensa.tipoRecompensa var, int idRoom)
    {
        Vector2 aux = getRoomPosbyID(idRoom);
        estructuraHabitacion habTemp = listaPisos[(int)aux.x].habitaciones[(int)aux.y];
        habTemp.reward = var;
        listaPisos[(int)aux.x].habitaciones[(int)aux.y] = habTemp;
    }

    public void setCameraPosition(int cameraPos, int idRoom)
    {
        Vector2 aux = getRoomPosbyID(idRoom);
        estructuraHabitacion habTemp = listaPisos[(int)aux.x].habitaciones[(int)aux.y];
        habTemp.cameraPosition = cameraPos;
        listaPisos[(int)aux.x].habitaciones[(int)aux.y] = habTemp;
    }

    public void setCantSitiosOcultos(int cantSitios, int idRoom)
    {
        Vector2 aux = getRoomPosbyID(idRoom);
        estructuraHabitacion habTemp = listaPisos[(int)aux.x].habitaciones[(int)aux.y];
        habTemp.cantSitiosOcultos = cantSitios;
        listaPisos[(int)aux.x].habitaciones[(int)aux.y] = habTemp;
    }

    public void setListaTrampas(List<GeneradorObjetos.trapStruct> auxListaTemp ,int idRoom)
    {
        Vector2 aux = getRoomPosbyID(idRoom);
        estructuraHabitacion habTemp = listaPisos[(int)aux.x].habitaciones[(int)aux.y];
        habTemp.listaTrampas = auxListaTemp;
        listaPisos[(int)aux.x].habitaciones[(int)aux.y] = habTemp;
    }

    public Vector2 getRoomPosbyID(int idRoom)
    {
        Vector2 aux = new Vector2(-1, -1);

        for(int i = 0; i < NumPisos; i++)
        {
            for(int j = 0; j < listaPisos[i].habitaciones.Count; j++)
            {
                if (listaPisos[i].habitaciones[j].id == idRoom)
                {
                    return new Vector2(i, j);
                }
            }
        }
        return aux;
    }


    /*
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
                    if (pisoActual != NumPisos - 1)
                    {
                        habitacion.GetComponent<RoomController>().tall = Random.Range(0, 3) == 1 ? true : false;

                        if (espaciosRestantes == 0 && areAllTall(pisoActual, false))
                            habitacion.GetComponent<RoomController>().tall = true;
                        else if (espaciosRestantes == 0 && areAllTall(pisoActual, true))
                            habitacion.GetComponent<RoomController>().tall = false;
                    }
                    else
                        habitacion.GetComponent<RoomController>().tall = false;

                    auxHab.tall = habitacion.GetComponent<RoomController>().tall;

                    if (pisoActual != 0)
                    {
                        if (listaPisos[pisoActual - 1].escaleraEn >= auxHab.coordenadas.x && listaPisos[pisoActual - 1].escaleraEn < auxHab.coordenadas.x + auxHab.tam)
                        {
                            habitacion.GetComponent<RoomController>().ladderReceived = listaPisos[pisoActual - 1].escaleraEn - (int)auxHab.coordenadas.x;
                            //print ("La habitacion "+habitacion.GetComponent<RoomController> ().name+" recibe la escalera en "+habitacion.GetComponent<RoomController> ().ladderReceived+" posicion");
                        }

                    }

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
    }*/

    private bool areAllTall(estructuraPisos piso, bool tall)
    {
        if (piso.habitaciones.Count > 0)
        {
            int cont = 0;
            for (int i = 0; i < piso.habitaciones.Count; i++)
            {
                if (tall == piso.habitaciones[i].tall)
                    cont++;
            }

            if (cont == piso.habitaciones.Count)
                return true;
        }
        return false;
    }

    public void init(int dif, int pisos, int numHab, listaEstilos style, List<condicionesVictoria> condiciones)
    {
        dificultad = dif;
        NumPisos = pisos;
        room = Resources.Load("Prefabs/Room", typeof(GameObject)) as GameObject;
        objetivosMision = condiciones;
        NumHabitaciones = numHab;
        estilo = style;
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

                if (coordenadas.y + tam == NumHabitaciones * 2)// si es la ultima habitacion, a la derecha tiene pared
                    habitacion.GetComponent<RoomController>().setlistaLaterales(1, RoomController.tiposParedes.pared);

                if (pisoActual != 0)
                {

                    if (coordenadas.y == 0)
                        habitacion.GetComponent<RoomController>().setlistaLaterales(0, RoomController.tiposParedes.pared);

                    if (coordenadas.y + tam < NumHabitaciones * 2 && matrizHabitaciones[pisoActual, (int)(coordenadas.y + tam)] < id)
                        habitacion.GetComponent<RoomController>().setlistaLaterales(1, RoomController.tiposParedes.trampilla);

                    if (coordenadas.y > 0 && matrizHabitaciones[pisoActual, (int)(coordenadas.y - 1)] == matrizHabitaciones[pisoActual - 1, (int)(coordenadas.y - 1)])
                        habitacion.GetComponent<RoomController>().setlistaLaterales(0, RoomController.tiposParedes.nada);

                }

                if (tall)
                {//Si es una habitacion alta

                    if (coordenadas.y == 0)
                    { // Si está en la primera posicion, pared 
                        habitacion.GetComponent<RoomController>().setlistaLaterales(2, RoomController.tiposParedes.pared);
                    }

                    if (coordenadas.y + tam == NumHabitaciones * 2) // Si acaba en la ultima posicion, pared
                    {
                        habitacion.GetComponent<RoomController>().setlistaLaterales(3, RoomController.tiposParedes.pared);
                    }
                    if (coordenadas.y + tam < NumHabitaciones * 2 && matrizHabitaciones[pisoActual, (int)(coordenadas.y + tam)] != matrizHabitaciones[pisoActual + 1, (int)(coordenadas.y + tam)])
                        habitacion.GetComponent<RoomController>().setlistaLaterales(3, RoomController.tiposParedes.trampilla);

                    if (coordenadas.y + tam < NumHabitaciones * 2 && matrizHabitaciones[pisoActual, (int)(coordenadas.y + tam)] == matrizHabitaciones[pisoActual + 1, (int)(coordenadas.y + tam)])
                        habitacion.GetComponent<RoomController>().setlistaLaterales(3, RoomController.tiposParedes.pared);

                }
            }
        }

        for (int pisoActual = 0; pisoActual < NumPisos; pisoActual++)
        {
            for (int habActual = 0; habActual < listaPisos[pisoActual].habitaciones.Count; habActual++)
            {
                GameObject habitacion = listaPisos[pisoActual].habitaciones[habActual].habitacion;

               //print("Soy "+ listaPisos[pisoActual].habitaciones[habActual].id+" -> "+  habitacion.GetComponent<RoomController>().listaLaterales[1]);

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

    public void setInitialTime(float var) {
        initialTime = var;
    }

    private void prepareLevelsForSaving()
    {
        for(int pisoActual = 0; pisoActual < listaPisos.Count; pisoActual++)
        {
            for(int habActual = 0;habActual < listaPisos[pisoActual].habitaciones.Count; habActual++)
            {












            }





        }









    }

    public void setPausa(bool var)
    {
        pausado = var;
    }

    public bool getPausa()
    {
        return pausado;
    }

    void Update()
    {
        //print("Cant trampas -> "+cantTrampasHackeablesTotal);
        /*if(!buildingSaved)
            prepareLevelsForSaving();*/
        tiempoNivel = Time.time - initialTime;
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
        if (matrizHabitaciones[piso, coordenada] == -1)
            return true;
        else
            return false;
        /*


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
                //print ("Estoy en " + piso + " - " + coordenada + " y NO puedo contruir encima de " + (piso - 1) + " - " + coordenada);*/
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

    public List<estructuraPisos> getListaPisos()
    {
        return listaPisos;
    }
}
