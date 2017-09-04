using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class RoomController : MonoBehaviour
{
    public int dificultad;
    public int id;
    public int piso;
    //public bool hasLadder;
    public bool hasTecho;
    public bool tall;
    public bool jugadorEnRoom;
    public Edificio edificio;
    public ObjetoRecompensa.tipoRecompensa reward;

    public List<GeneradorObjetos.trapStruct> listaTrampas;
    public int numeroElementos;
    public float tHab;
    public bool canHaveLadder;
    public int ladderPosition;
    public int ladderReceived;
    public int cameraPosition;
    public int cantSitiosOcultos;

    public GameObject generadorPrefab;
    public GameObject generadorDecoradoPrefab;
    public GameObject paredlateralPrefab;
    public GameObject paredIzquierda;
    public GameObject paredDerecha;

    public tiposParedes[] listaLaterales;

    private Vector3 limiteIzquierdo;
    private Vector3 limiteDerecho;
    public GameObject mapGenController;
    private float altTecho;

    private GameObject suelo;
    private GameObject techo;
    private GameObject spawner;

    public roomSize tamanyo;
    public Edificio.listaEstilos estilo;
    public tipo tipoHabitacion;
    public bool alarmTriggered;

    private List<GameObject> generadoresDecorado;


    public static float T_HAB = 202f;
    public static float ALT_HAB = 66.5f;

    public enum tiposParedes
    {
        puerta,
        pared,
        trampilla,
        nada
    }

    public enum roomSize
    {
        small,
        medium,
        large
    }

    public enum tipo
    {
        Banyo,
        Entrada,
        HabitacionPrincipal,
        Despacho
    }

    public enum IDhijos
    {
        DecoradoIzq,
        DecoradoDer,
        Suelo,
        ParedIzq,
        ParedDer,
        Techo
    }

    public void Start()
    {
        cantSitiosOcultos = 0;
        jugadorEnRoom = false;
        alarmTriggered = false;
        altTecho = ALT_HAB;
        limiteIzquierdo = new Vector3();
        limiteDerecho = new Vector3();
        generadoresDecorado = new List<GameObject>();

        suelo = transform.Find("ParedSuelo").gameObject;
        techo = transform.Find("Techo").gameObject;
        spawner = transform.Find("Spawner").gameObject;

        if (listaLaterales.Length == 0)
            initListaLaterales();


        if(listaTrampas == null)
            listaTrampas = new List<GeneradorObjetos.trapStruct>();

        if (tipoHabitacion == tipo.Entrada && listaTrampas.Count == 0)
        {
            GeneradorObjetos.trapStruct auxElement = new GeneradorObjetos.trapStruct();
            auxElement.tipo = GeneradorObjetos.tipo.RedLaser;
            auxElement.level = dificultad;
            auxElement.verticalSpeed = getRandomVerticalSpeed();
            listaTrampas.Add(auxElement);
        }

        techo.SetActive(hasTecho);

        if (tipoHabitacion == tipo.Entrada)
            spawner.SetActive(true);
        else
            spawner.SetActive(false);

        rellenarSala();
    }

    public float getRandomVerticalSpeed()
    {
        return Random.Range(0.01f, 0.21f);
    }

    private void initListaLaterales()
    {
        listaLaterales = new tiposParedes[4];
        for (int i = 0; i < 4; i++)
        {
            listaLaterales[i] = tiposParedes.nada;
        }
    }

    private void rellenarSala()
    {

        tHab = T_HAB;
        limiteIzquierdo = new Vector3(transform.position.x - 94.2f, transform.position.y - 23f, transform.position.z - 16.8f);

        switch (tamanyo)
        {
            case roomSize.small:

                for (int i = 0; i < suelo.transform.childCount; i++)
                {
                    string name = suelo.transform.GetChild(i).name;

                    if (name == "SueloSmall" || name == "ParedSmall")
                        suelo.transform.GetChild(i).gameObject.SetActive(true);
                    else
                        suelo.transform.GetChild(i).gameObject.SetActive(false);

                    if (ladderReceived == 1 && name == "SueloSmallEsc")
                    {
                        suelo.transform.GetChild(i).gameObject.SetActive(true);
                        suelo.transform.GetChild(i - 1).gameObject.SetActive(false);
                    }
                }

                if (!tall)
                    suelo.transform.Find("ParedSmall").transform.localScale = new Vector3(suelo.transform.Find("ParedSmall").transform.localScale.x, suelo.transform.Find("ParedSmall").transform.localScale.y, 1.105f);
                //suelo.transform.localScale = new Vector3(0.5f, 1f, 1f);

                suelo.transform.position = new Vector3(suelo.transform.position.x - 50.5f, suelo.transform.position.y, suelo.transform.position.z);
                numeroElementos = 1;
                tHab /= 2;
                canHaveLadder = true;

                for (int i = 0; i < 3; i++) {
                    GetComponents<BoxCollider> () [i].size = new Vector3 (GetComponents<BoxCollider> () [i].size.x / 2 - 12, GetComponents<BoxCollider> () [i].size.y, GetComponents<BoxCollider> () [i].size.z);
                    GetComponents<BoxCollider> () [i].center = new Vector3 (GetComponents<BoxCollider> () [i].center.x + 50, GetComponents<BoxCollider> () [i].center.y, GetComponents<BoxCollider> () [i].center.z);
                }

                if (hasTecho)
                {
                    techo.transform.localScale = new Vector3(techo.transform.localScale.x / 2, techo.transform.localScale.y, techo.transform.localScale.z);
                    techo.transform.position = new Vector3(techo.transform.position.x - 50.8f, techo.transform.position.y, techo.transform.position.z);
                }
                break;
            case roomSize.medium:
                numeroElementos = 2;
                suelo.transform.position = new Vector3(suelo.transform.position.x - 0.5f, suelo.transform.position.y, suelo.transform.position.z);
                canHaveLadder = true;
                break;
            case roomSize.large:
                canHaveLadder = true;
                numeroElementos = 4;
                tHab *= 2;

                suelo.transform.position = new Vector3(suelo.transform.position.x - 0.5f, suelo.transform.position.y, suelo.transform.position.z);

                string hijoEsc = "Suelo";
                if (ladderReceived == 2)
                    hijoEsc = "SueloIzq";
                else if (ladderReceived == 3)
                    hijoEsc = "SueloDer";

                GameObject suelo1 = Instantiate(suelo.transform.Find(hijoEsc).gameObject, suelo.transform);
                suelo1.transform.name = "Suelo 2";
                suelo1.SetActive(true);
                suelo1.transform.position = new Vector3((suelo.transform.GetChild(0).position.x + tHab / 2), suelo.transform.GetChild(0).position.y, suelo.transform.GetChild(0).position.z);

                GameObject pared1 = Instantiate(suelo.transform.Find("Pared").gameObject, suelo.transform);
                pared1.name = "Pared 2";
                pared1.transform.position = new Vector3((suelo.transform.GetChild(3).position.x + tHab / 2), suelo.transform.GetChild(3).position.y, suelo.transform.GetChild(3).position.z);

                for (int i = 0; i < 3; i++)
                {
                    GetComponents<BoxCollider>()[i].size = new Vector3(GetComponents<BoxCollider>()[i].size.x * 2 - 12, GetComponents<BoxCollider>()[i].size.y, GetComponents<BoxCollider>()[i].size.z);
                    GetComponents<BoxCollider>()[i].center = new Vector3(GetComponents<BoxCollider>()[i].center.x - 100, GetComponents<BoxCollider>()[i].center.y, GetComponents<BoxCollider>()[i].center.z);
                }

                if (tall)
                {
                    pared1.transform.localScale = new Vector3(pared1.transform.localScale.x, pared1.transform.localScale.y * 2, pared1.transform.localScale.z);
                    pared1.transform.position = new Vector3(pared1.transform.position.x, pared1.transform.position.y + 34, pared1.transform.position.z);
                }

                if (hasTecho)
                {
                    GameObject techo2 = Instantiate(techo, this.transform);
                    techo2.transform.name = "Techo 2";
                    techo2.transform.position = new Vector3(techo.transform.position.x + T_HAB, techo.transform.position.y, techo.transform.position.z);
                    if (tall)
                        techo2.transform.position = new Vector3(techo.transform.position.x + T_HAB, techo.transform.position.y + altTecho, techo.transform.position.z);

                }

                break;
        }

        if (ladderReceived != -1)
        {
            string childIN, childOUT;
            childIN = childOUT = "";

            switch (ladderReceived)
            {
                case 0:
                    if (tamanyo == roomSize.small)
                    {
                        childIN = "SueloSmallEsc";
                        childOUT = "SueloSmall";
                    }
                    else
                    {
                        childIN = "SueloIzq";
                        childOUT = "Suelo";
                    }
                    break;
                case 1:
                    childIN = "SueloDer";
                    childOUT = "Suelo";
                    break;
            }

            //print ("Deberia cambiar la "+id+" " +childOUT+" por "+childIN);

            for (int contHijos = 0; contHijos < suelo.transform.childCount; contHijos++)
            {
                if (suelo.transform.GetChild(contHijos).name == childIN)
                    suelo.transform.GetChild(contHijos).gameObject.SetActive(true);
                if (suelo.transform.GetChild(contHijos).name == childOUT)
                    suelo.transform.GetChild(contHijos).gameObject.SetActive(false);

            }
        }

        disposicionDecorados();
        disposicionGeneradoresPuertas();
        disposicionGeneradoresObjetos();
        disposicionGeneradoresSitiosOcultos();
        disposicionCamaras();

    }

    public void addCantTrampasHackeablesTotal()
    {
        edificio.cantTrampasHackeablesTotal++;
    }

    private void disposicionGeneradoresObjetos()
    {

        int aux = 0;
        int auxCamera = 0;
        switch (dificultad)
        {
            case 1:
                aux = Random.Range(0, 2);
                auxCamera = Random.Range(0, 2);
                break;
            case 2:
                aux = 1;
                auxCamera = Random.Range(0, 2);
                break;
            case 3:
                aux = Random.Range(1, 3);
                auxCamera = Random.Range(0, 3);
                break;
            case 4:
                aux = 2;
                auxCamera = Random.Range(0, 3);
                break;
            case 5:
                aux = Random.Range(2, 4);
                auxCamera = Random.Range(1, 3);
                break;
            case 6:
                aux = 3;
                auxCamera = Random.Range(1, 3);
                break;
        }

        if (auxCamera == 1)
            cameraPosition = Random.Range(auxCamera, 3);
        else if (auxCamera == 2)
            cameraPosition = 3;

        if (tipoHabitacion == tipo.Entrada || tipoHabitacion == tipo.Banyo )
            cameraPosition = 0;

        edificio.setCameraPosition(cameraPosition, this.id);

        aux++;

        if (tipoHabitacion == tipo.Banyo)
        {
            aux = 0;
            if (reward == ObjetoRecompensa.tipoRecompensa.ninguno)
            {
                int auxPuntos = Random.Range(0, 3);
                if(auxPuntos == 0)
                {
                    reward = ObjetoRecompensa.tipoRecompensa.conjuntoPuntos;
                    edificio.setReward(reward, this.id);
                }
            }
        }

        if (reward == ObjetoRecompensa.tipoRecompensa.ninguno)
        {

            if (listaTrampas.Count != 0)
                aux = listaTrampas.Count;
            else
                aux--;

            if (aux > 0)
            {
                bool generoNuevas = true;

                if (listaTrampas.Count != 0)
                    generoNuevas = false;

                Vector3 position;
                for (int i = 0; i < aux; i++)
                {

                    position = limiteIzquierdo;
                    position.x = position.x + (i + 1) * (tHab / (aux + 1));

                    //print (position.x + " + " + i +" * (" +tHab+" / "+aux+")");

                    GameObject objeto = Instantiate(generadorPrefab, position, Quaternion.identity);
                    objeto.GetComponent<GeneradorObjetos>().nivel = dificultad;
                    objeto.GetComponent<GeneradorObjetos>().estilo = estilo;
                    GameObject auxObj;
                    if (generoNuevas)
                    {
                        GeneradorObjetos.trapStruct auxItem = new GeneradorObjetos.trapStruct();

                        auxItem.tipo = GeneradorObjetos.getRandomObject();
                        auxItem.level = dificultad;
                        auxItem.verticalSpeed = 0;
                        auxObj = objeto.GetComponent<GeneradorObjetos>().generateObjects(auxItem.tipo);

                        if (auxItem.tipo == GeneradorObjetos.tipo.RedLaser)
                        {
                            auxItem.verticalSpeed = getRandomVerticalSpeed();
                            auxObj.GetComponent<RedLaserController>().setVerticalSpeed(auxItem.verticalSpeed);
                        }

                        listaTrampas.Add(auxItem);

                    }
                    else
                    {
                        if (listaTrampas[i].tipo == GeneradorObjetos.tipo.RedLaser)
                        {
                            auxObj = objeto.GetComponent<GeneradorObjetos>().generateObjects(listaTrampas[i].tipo);
                            auxObj.GetComponent<RedLaserController>().setVerticalSpeed(listaTrampas[i].verticalSpeed);
                        }
                        else
                        {
                            auxObj = objeto.GetComponent<GeneradorObjetos>().generateObjects(listaTrampas[i].tipo);
                        }

                    }

                    objeto.transform.SetParent(this.transform);
                    auxObj.transform.SetParent(this.transform);

                }
                edificio.setListaTrampas(listaTrampas, this.id);
            }
        }
        else
        {
            Vector3 position;
            position = limiteIzquierdo;
            position.x = position.x + (tHab / 2);

            //print (position.x + " + " + i +" * (" +tHab+" / "+aux+")");

            GameObject objeto = Instantiate(generadorPrefab, position, Quaternion.identity);
            objeto.GetComponent<GeneradorObjetos>().nivel = dificultad;
            objeto.GetComponent<GeneradorObjetos>().estilo = estilo;

            GameObject auxObj = objeto.GetComponent<GeneradorObjetos>().generateObjects(GeneradorObjetos.tipo.Recompensa);
            auxObj.GetComponent<ObjetoRecompensa>().tipoObjeto = reward;
            objeto.transform.SetParent(this.transform);
            auxObj.transform.SetParent(this.transform);
            edificio.setListaTrampas(listaTrampas, this.id);
        }

    }

    private void disposicionCamaras()
    {
        if (cameraPosition > 0)
        {
            GameObject cameraPrefab = Resources.Load("Prefabs/Camara", typeof(GameObject)) as GameObject;

            float corIzq, corDer;

            corIzq = corDer = 0;

            if (listaLaterales[0] == tiposParedes.nada)
                corIzq = -5;
            if (listaLaterales[1] == tiposParedes.nada)
                corDer = 4;

            Vector3 posIzq = new Vector3(limiteIzquierdo.x + 4 + corIzq, limiteIzquierdo.y + 57.5f, limiteIzquierdo.z);
            Vector3 posDer = new Vector3(limiteIzquierdo.x + tHab - 10 + corDer, limiteIzquierdo.y + 57.5f, limiteIzquierdo.z);

            switch (cameraPosition)
            {
                case 1:
                    {
                        GameObject cameraObject = Instantiate(cameraPrefab, posIzq, Quaternion.identity, this.transform);
                        cameraObject.name = "Camara Izq";
                        cameraObject.GetComponent<ScriptCamara>().apuntaDerecha = true;
                        cameraObject.GetComponent<ScriptCamara>().level = dificultad;
                        cameraObject.GetComponent<ScriptCamara>().estilo = estilo;
                    }
                    break;
                case 2:
                    {

                        GameObject cameraObject = Instantiate(cameraPrefab, posDer, Quaternion.identity, this.transform);
                        cameraObject.name = "Camara Der";
                        cameraObject.GetComponent<ScriptCamara>().apuntaDerecha = false;
                        cameraObject.GetComponent<ScriptCamara>().level = dificultad;
                        cameraObject.GetComponent<ScriptCamara>().estilo = estilo;
                    }
                    break;
                case 3:
                    {
                        GameObject cameraObject1 = Instantiate(cameraPrefab, posIzq, Quaternion.identity, this.transform);
                        cameraObject1.name = "Camara Izq";
                        cameraObject1.GetComponent<ScriptCamara>().apuntaDerecha = true;
                        cameraObject1.GetComponent<ScriptCamara>().level = dificultad;
                        cameraObject1.GetComponent<ScriptCamara>().estilo = estilo;

                        GameObject cameraObject2 = Instantiate(cameraPrefab, posDer, Quaternion.identity, this.transform);
                        cameraObject2.name = "Camara Der";
                        cameraObject2.GetComponent<ScriptCamara>().apuntaDerecha = false;
                        cameraObject2.GetComponent<ScriptCamara>().level = dificultad;
                        cameraObject2.GetComponent<ScriptCamara>().estilo = estilo;
                    }
                    break;
            }
        }
    }

    private void disposicionGeneradoresSitiosOcultos()
    {

        float percentage = 50 - (dificultad - 1) * 10;
        if (percentage < 20)
            percentage = 15;

        int auxRand = Random.Range(0, 100);

        if (auxRand <= percentage)
        {
            //print("Salgo con " + auxRand);
            int auxRandElement = Random.Range(1, numeroElementos + 1);


            //print("Elementos " + auxRandElement);

            Vector3 position;


            float auxDiv = 0;
            switch (tamanyo)
            {
                case roomSize.small:
                    auxDiv = (tHab / (numeroElementos));
                    break;
                case roomSize.medium:
                    auxDiv = (tHab / (numeroElementos));
                    break;
                case roomSize.large:
                    auxDiv = (tHab / (numeroElementos));
                    break;
            }
            List<int> elements = new List<int>();

            while (elements.Count < auxRandElement)
            {
                int auxPos = Random.Range(1, numeroElementos + 1);

                if (!elements.Contains(auxPos))
                {
                    position = limiteIzquierdo;
                    position.x = position.x + (auxPos * auxDiv);
                    position.z = -64.04f;

                    //print("Pongo el "+ (elements.Count+1) + " en la posicion "+auxPos);

                    GameObject objeto = Instantiate(generadorPrefab, position, Quaternion.identity);
                    objeto.transform.position = new Vector3(position.x - 40, position.y, position.z);
                    objeto.GetComponent<GeneradorObjetos>().nivel = dificultad;
                    objeto.GetComponent<GeneradorObjetos>().estilo = estilo;

                    GameObject auxObj = objeto.GetComponent<GeneradorObjetos>().generateObjects(GeneradorObjetos.tipo.SitioOculto);
                    auxObj.transform.name = "Sitio Oculto " + (elements.Count);
                    objeto.transform.SetParent(this.transform);
                    auxObj.transform.SetParent(this.transform);
                    cantSitiosOcultos++;
                    elements.Add(auxPos);
                }
            }
        }
        edificio.setCantSitiosOcultos(cantSitiosOcultos, this.id);

    }

    private void disposicionDecorados()
    {

        float posicionDecorados = 39.6f;
        Vector3 position;

        for (int i = 1; i <= numeroElementos; i++)
        { //For que genera los decorados y las luces
            position = limiteIzquierdo;
            position.x = position.x + i * (tHab / numeroElementos);

            GameObject lightGameObject = new GameObject("Luz" + i);
            Light lightComp = lightGameObject.AddComponent<Light>();
            lightGameObject.transform.position = new Vector3(position.x - 40, position.y + 51, position.z);
            lightGameObject.GetComponent<Light>().range = 100;
            lightGameObject.GetComponent<Light>().intensity = 4;
            lightGameObject.transform.SetParent(this.transform);

            if (i - 1 != ladderReceived)
            {
                GameObject auxDeco = Instantiate(generadorDecoradoPrefab, this.transform);
                auxDeco.name = "Generador Decorado " + (i);
                auxDeco.transform.position = new Vector3(position.x - 40, position.y, posicionDecorados);
                auxDeco.GetComponent<GeneradorDecorado>().estilo = estilo;
                if (ladderPosition == i)
                    auxDeco.GetComponent<GeneradorDecorado>().escalera = true;

                if (ladderPosition != i)
                    auxDeco.GetComponent<GeneradorDecorado>().seleccion = tipoHabitacion;

                switch (tipoHabitacion)
                {
                    case tipo.HabitacionPrincipal:

                        if (tamanyo == roomSize.medium && i == 1) // pongo los generadores de decorados orientados en la direccion correcta
                            auxDeco.GetComponent<GeneradorDecorado>().derecha = false;
                        else if (tamanyo == roomSize.medium && i == 2)
                            auxDeco.GetComponent<GeneradorDecorado>().derecha = true;

                        if (tamanyo == roomSize.large && i < 3)
                            auxDeco.GetComponent<GeneradorDecorado>().derecha = false;
                        else if (tamanyo == roomSize.large && i > 2)
                            auxDeco.GetComponent<GeneradorDecorado>().derecha = true;

                        generadoresDecorado.Add(auxDeco);
                        break;
                    case tipo.Entrada:

                        if (i != numeroElementos && !auxDeco.GetComponent<GeneradorDecorado>().escalera)
                            auxDeco.GetComponent<GeneradorDecorado>().apagado = true;
                        else
                            auxDeco.GetComponent<GeneradorDecorado>().derecha = true;

                        generadoresDecorado.Add(auxDeco);

                        break;
                }
            }

        }

    }

    private void disposicionGeneradoresPuertas()
    {

        Vector3 position;
        position = limiteIzquierdo;

        paredIzquierda = Instantiate(generadorPrefab, this.transform);
        paredIzquierda.transform.position = new Vector3(position.x, position.y, position.z);
        paredIzquierda.transform.name = "Gen ParteIzq";

        switch (listaLaterales[0])
        {
            case tiposParedes.puerta:
                paredIzquierda.GetComponent<GeneradorObjetos>().generateObjects(GeneradorObjetos.tipo.Puerta);
                break;
            case tiposParedes.pared:
                {
                    GameObject auxObj = paredIzquierda.GetComponent<GeneradorObjetos>().generateObjects(GeneradorObjetos.tipo.Pared);
                    auxObj.GetComponent<ScriptPared>().generateRandom(true);
                }
                break;
            case tiposParedes.trampilla:
                {
                    GameObject auxObj = paredIzquierda.GetComponent<GeneradorObjetos>().generateObjects(GeneradorObjetos.tipo.Pared);
                    auxObj.GetComponent<ScriptPared>().generateTrampilla();
                }
                break;
        }

        paredDerecha = Instantiate(generadorPrefab, this.transform);
        paredDerecha.transform.position = new Vector3(position.x + tHab - 6f, position.y, position.z);
        paredDerecha.transform.name = "Gen ParteDer";

        switch (listaLaterales[1])
        {
            case tiposParedes.puerta:
                paredDerecha.GetComponent<GeneradorObjetos>().generateObjects(GeneradorObjetos.tipo.Puerta);
                break;
            case tiposParedes.pared:
                {
                    GameObject auxObj = paredDerecha.GetComponent<GeneradorObjetos>().generateObjects(GeneradorObjetos.tipo.Pared);
                    auxObj.GetComponent<ScriptPared>().generateRandom(true);
                }
                break;
            case tiposParedes.trampilla:
                {
                    GameObject auxObj = paredDerecha.GetComponent<GeneradorObjetos>().generateObjects(GeneradorObjetos.tipo.Pared);
                    auxObj.GetComponent<ScriptPared>().generateTrampilla();
                }
                break;
        }

        if (tall)
        {
            if (tamanyo != roomSize.small)
            {
                suelo.transform.GetChild(3).transform.localScale = new Vector3(suelo.transform.GetChild(3).transform.localScale.x, suelo.transform.GetChild(3).transform.localScale.y * 2, suelo.transform.GetChild(3).transform.localScale.z);
                suelo.transform.GetChild(3).transform.position = new Vector3(suelo.transform.GetChild(3).transform.position.x, suelo.transform.GetChild(3).transform.position.y + 34, suelo.transform.GetChild(3).transform.position.z);
            }
            else
            {
                suelo.transform.GetChild(6).transform.localScale = new Vector3(suelo.transform.GetChild(6).transform.localScale.x, suelo.transform.GetChild(6).transform.localScale.y, suelo.transform.GetChild(6).transform.localScale.z * 2.08f);
                suelo.transform.GetChild(6).transform.position = new Vector3(suelo.transform.GetChild(6).transform.position.x, suelo.transform.GetChild(6).transform.position.y + 32.5f, suelo.transform.GetChild(6).transform.position.z);
            }

            GameObject paredSupIzquierda = Instantiate(paredlateralPrefab, this.transform);
            paredSupIzquierda.transform.localScale = new Vector3(1, 1.08f, 1.045f);
            paredSupIzquierda.transform.position = new Vector3(limiteIzquierdo.x, limiteIzquierdo.y + 95.2f, limiteIzquierdo.z + 3.6f);
            paredSupIzquierda.transform.name = "Pared Superior Izquierda";

            GameObject paredSupDerecha = Instantiate(paredlateralPrefab, this.transform);
            paredSupDerecha.transform.localScale = new Vector3(1, 1.08f, 1.045f);
            paredSupDerecha.transform.position = new Vector3(position.x + tHab - 6f, position.y + 95.2f, position.z + 3.6f);
            paredSupDerecha.transform.name = "Pared Superior Derecha";

            switch (listaLaterales[2])
            {
                case tiposParedes.pared:
                    paredSupIzquierda.GetComponent<ScriptPared>().generateRandom(false);
                    break;
                case tiposParedes.trampilla:
                    paredSupIzquierda.GetComponent<ScriptPared>().generateTrampilla();
                    break;
            }

            switch (listaLaterales[3])
            {
                case tiposParedes.pared:
                    paredSupDerecha.GetComponent<ScriptPared>().generateRandom(false);
                    break;
                case tiposParedes.trampilla:
                    paredSupDerecha.GetComponent<ScriptPared>().generateTrampilla();
                    break;
            }

            if (hasTecho)
            {
                techo.transform.position = new Vector3(techo.transform.position.x, techo.transform.position.y + altTecho, techo.transform.position.z);
            }
        }
        edificio.setListaLaterales(listaLaterales, this.id);
    }

    /*
	private void rellenarSalas ()
	{

		float posicionDecorados = 39.6f;
		tHab = T_HAB;
		limiteIzquierdo = new Vector3 (transform.position.x -94.2f, transform.position.y -23f, transform.position.z -16.8f);


		switch (tamanyo) {
		case roomSize.small:
			suelo.transform.localScale = new Vector3 (0.5f, 1f, 1f);
			suelo.transform.position = new Vector3 (suelo.transform.position.x - 50, suelo.transform.position.y, suelo.transform.position.z);
			numeroElementos = 1;
			tHab /= 2;
			canHaveLadder = false;

			for (int i = 0; i < 2; i++) {
				GetComponents<BoxCollider> () [i].size = new Vector3 (GetComponents<BoxCollider> () [i].size.x / 2 - 12, GetComponents<BoxCollider> () [i].size.y, GetComponents<BoxCollider> () [i].size.z);
				GetComponents<BoxCollider> () [i].center = new Vector3 (GetComponents<BoxCollider> () [i].center.x + 50, GetComponents<BoxCollider> () [i].center.y, GetComponents<BoxCollider> () [i].center.z);
			}

			if (hasTecho) {
				techo.transform.localScale = new Vector3 (techo.transform.localScale.x / 2, techo.transform.localScale.y, techo.transform.localScale.z);
				techo.transform.position = new Vector3 (techo.transform.position.x - 50.8f, techo.transform.position.y, techo.transform.position.z);
			}



			break;
		case roomSize.medium:
			numeroElementos = 2;
			canHaveLadder = true;
			break;
		case roomSize.large:
			canHaveLadder = false;
			numeroElementos = 4;
			tHab *= 2;

			GameObject suelo1 = Instantiate (suelo.transform.GetChild (0).gameObject, suelo.transform);
			suelo1.transform.position = new Vector3 ((suelo.transform.GetChild (0).position.x + tHab / 2), suelo.transform.GetChild (0).position.y, suelo.transform.GetChild (0).position.z);

			GameObject pared1 = Instantiate (suelo.transform.GetChild (3).gameObject, suelo.transform);
			pared1.transform.position = new Vector3 ((suelo.transform.GetChild (3).position.x + tHab / 2), suelo.transform.GetChild (3).position.y, suelo.transform.GetChild (3).position.z);

			for (int i = 0; i < 2; i++) {
				GetComponents<BoxCollider> () [i].size = new Vector3 (GetComponents<BoxCollider> () [i].size.x * 2 - 12, GetComponents<BoxCollider> () [i].size.y, GetComponents<BoxCollider> () [i].size.z);
				GetComponents<BoxCollider> () [i].center = new Vector3 (GetComponents<BoxCollider> () [i].center.x - 100, GetComponents<BoxCollider> () [i].center.y, GetComponents<BoxCollider> () [i].center.z);
			}



			if (tall) {
				pared1.transform.localScale = new Vector3 (pared1.transform.localScale.x, pared1.transform.localScale.y * 2, pared1.transform.localScale.z);
				pared1.transform.position = new Vector3 (pared1.transform.position.x, pared1.transform.position.y + 34, pared1.transform.position.z);
			}

			if (hasTecho) {
				GameObject techo2 = Instantiate (techo, this.transform);
				techo2.transform.name = "Techo 2";
				techo2.transform.position = new Vector3 (techo.transform.position.x + T_HAB, techo.transform.position.y, techo.transform.position.z);
				if (tall)
					techo2.transform.position = new Vector3 (techo.transform.position.x + T_HAB, techo.transform.position.y + altTecho, techo.transform.position.z);

			}

			break;
		}

		Vector3 position;

		for (int i = 1; i <= numeroElementos; i++) { //For que genera los decorados y las luces
			position = limiteIzquierdo;
			position.x = position.x + i * (tHab / numeroElementos);

			GameObject auxDeco = Instantiate (generadorDecoradoPrefab, this.transform);
			auxDeco.name = "Generador Decorado " + (i);
			auxDeco.transform.position = new Vector3 (position.x - 40, position.y, posicionDecorados);
			auxDeco.GetComponent<GeneradorDecorado> ().estilo = estilo;
			auxDeco.GetComponent<GeneradorDecorado> ().seleccion = tipoHabitacion;

			GameObject lightGameObject = new GameObject ("Luz" + i);
			Light lightComp = lightGameObject.AddComponent<Light> ();
			lightGameObject.transform.position = new Vector3 (position.x - 40, position.y + 51, position.z);
			lightGameObject.GetComponent<Light> ().range = 100;
			lightGameObject.GetComponent<Light> ().intensity = 4;

			lightGameObject.transform.SetParent (this.transform);

			if (tamanyo == roomSize.medium && i == 1) // pongo los generadores de decorados orientados en la direccion correcta
				auxDeco.GetComponent<GeneradorDecorado> ().derecha = false;
			else if (tamanyo == roomSize.medium && i == 2)
				auxDeco.GetComponent<GeneradorDecorado> ().derecha = true;

			if (tamanyo == roomSize.large && i < 3)
				auxDeco.GetComponent<GeneradorDecorado> ().derecha = false;
			else if (tamanyo == roomSize.large && i > 2)
				auxDeco.GetComponent<GeneradorDecorado> ().derecha = true;

			generadoresDecorado.Add (auxDeco);

		}

		//print ("hay "+generadoresDecorado.Count+ " generadores en esta habitacion");

		position = limiteIzquierdo;

		paredIzquierda = Instantiate (generadorPrefab, this.transform);
		paredIzquierda.transform.position = new Vector3 (position.x , position.y, position.z);
		paredIzquierda.transform.name = "Gen ParteIzq";
		if (listaTrampillas [0]) {
			GameObject auxObj = paredIzquierda.GetComponent<GeneradorObjetos> ().generateObjects (GeneradorObjetos.tipo.Pared);
			auxObj.GetComponent<ScriptPared> ().generateTrampilla ();
		}
		else
			paredIzquierda.GetComponent<GeneradorObjetos> ().generateObjects(GeneradorObjetos.tipo.Puerta);


		paredDerecha = Instantiate (generadorPrefab, this.transform);
		paredDerecha.transform.position = new Vector3 (position.x + tHab - 6f, position.y, position.z);
		paredDerecha.transform.name = "Gen ParteDer";
		if (listaTrampillas [1]) {
			GameObject auxObj = paredDerecha.GetComponent<GeneradorObjetos> ().generateObjects (GeneradorObjetos.tipo.Pared);
			auxObj.GetComponent<ScriptPared> ().generateTrampilla ();
		}
		else
			paredDerecha.GetComponent<GeneradorObjetos> ().generateObjects(GeneradorObjetos.tipo.Puerta);

		if (tall) {
			suelo.transform.GetChild (3).transform.localScale = new Vector3 (suelo.transform.GetChild (3).transform.localScale.x, suelo.transform.GetChild (3).transform.localScale.y * 2, suelo.transform.GetChild (3).transform.localScale.z);
			suelo.transform.GetChild (3).transform.position = new Vector3 (suelo.transform.GetChild (3).transform.position.x, suelo.transform.GetChild (3).transform.position.y + 34, suelo.transform.GetChild (3).transform.position.z);

			if (puertaIz) {
				GameObject paredSupIzquierda = Instantiate (paredlateralPrefab, this.transform);
				paredSupIzquierda.transform.localScale = new Vector3 (1, 1.08f, 1.045f);
				paredSupIzquierda.transform.position = new Vector3 (limiteIzquierdo.x, limiteIzquierdo.y + 95.2f, limiteIzquierdo.z + 3.6f);
				paredSupIzquierda.transform.name = "Pared Superior Izquierda";
				if (listaTrampillas [2])
					paredSupIzquierda.GetComponent<ScriptPared> ().generateTrampilla ();
				else
					paredSupIzquierda.GetComponent<ScriptPared> ().generateRandom (false);
			}

			if (puertaDer) {
				GameObject paredSupDerecha = Instantiate (paredlateralPrefab, this.transform);
				paredSupDerecha.transform.localScale = new Vector3 (1, 1.08f, 1.045f);
				paredSupDerecha.transform.position = new Vector3 (position.x + tHab - 6f, position.y + 95.2f, position.z + 3.6f);
				paredSupDerecha.transform.name = "Pared Superior Derecha";
				if (listaTrampillas [3])
					paredSupDerecha.GetComponent<ScriptPared> ().generateTrampilla ();
				else
					paredSupDerecha.GetComponent<ScriptPared> ().generateRandom (false);
			}

			if (hasTecho) {
				techo.transform.position = new Vector3 (techo.transform.position.x, techo.transform.position.y + altTecho, techo.transform.position.z);
			}
		}
	}*/

    public void setlistaLaterales(int elem, tiposParedes tipoP)
    {
        if (listaLaterales.Length == 0)
            initListaLaterales();
        listaLaterales[elem] = tipoP;
    }


    // Update is called once per frame
    void Update()
    {

    }

    public static int getIDChild(IDhijos id)
    {
        switch (id)
        {
            case IDhijos.DecoradoIzq: //Decorado izquierda
                return 0;
            case IDhijos.DecoradoDer: //Decorado Derecha
                return 1;
            case IDhijos.Suelo: // suelo
                return 2;
            case IDhijos.ParedIzq://Puerta/pared izquierda
                return 3;
            case IDhijos.ParedDer://Puerta/pared derecha
                return 4;
            case IDhijos.Techo://Puerta/pared derecha
                return 5;
        }
        return -1;
    }

    public GameObject getElement(int element)
    {
        print("Doy comprobacion paredes");
        switch (element)
        {
            case 1:
                if (paredDerecha == null)
                    print("Pared derecha es nulo");
                return paredDerecha;
                break;
            case 0:
                if (paredIzquierda == null)
                    print("Pared izquierda es nulo");
                return paredIzquierda;
                break;
            default:
                return null;
                break;

        }



    }

    public bool hasLadder()
    {

        for (int i = 0; i < generadoresDecorado.Count; i++)
        {
            if (generadoresDecorado[i].GetComponent<GeneradorDecorado>().escalera)
                return true;
        }
        return false;
    }

    public List<GameObject> getGeneradoresDecorado()
    {
        return generadoresDecorado;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
            jugadorEnRoom = true;
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")
            jugadorEnRoom = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            jugadorEnRoom = false;
    }



}
