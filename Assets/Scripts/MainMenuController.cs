using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;

public class MainMenuController : MonoBehaviour
{
    //Menu principal
    private int estado;
    private GameObject MainCamera;
    private Transform canvas;
    private GameObject MenuPrincipal;
    private GameObject SeleccionNivel;
    private GameObject Instrucciones;
    private PlayerController player;
    private Edificio edificio;
    public struct nivel
    {
        public int NumPisos;
        public int NumHabitaciones;
        public int dificultad;
        public Edificio.listaEstilos estilo;
        public List<Edificio.condicionesVictoria> condicionesVictoria;
    };
    public List<nivel> ListaMapas;

    //Menu Pausa
    public GameObject CanvasPausa;
    public GameObject CanvasFinPartida;
    private Sprite tickSprite;
    private Sprite crossSprite;

    //UI Ingame
    private GameObject IngameUI;
    private List<Edificio.condicionesVictoria> condicionesVictoria;
    private RectTransform auxRect;
    private Sprite[] SpriteShuriken;
    private Sprite SpriteNada;
    private Sprite SpriteGancho;
    private Sprite tarjetaDocumentos;
    private Sprite tarjetaTrampasRestantes;
    private Sprite tarjetaPiezas;
    private Image primeraRecompensa;
    private Image segundaRecompensa;
    private Image SpriteObjetoEquipado;
    private Text DetectedText;
    private Text timeText;
    private Text puntosText;
    private Text trapsRemainingText;
    private bool detected;
    private bool hasDocuments;
    private bool hasSecretParts;
    private bool readyIngameUI;
    private int cantShurikens;
    private int objetoEquipado;
    private int trapCount;
    private float BarraVidaMaxSize;

    // Use this for initialization
    public void Start()
    {
        estado = 0;

        //Incializo menu principal
        canvas = transform.Find("Canvas");
        MenuPrincipal = canvas.Find("MenuPrincipal").gameObject;
        SeleccionNivel = canvas.Find("SeleccionDeNivel").gameObject;
        Instrucciones = canvas.Find("Instrucciones").gameObject;
        MainCamera = transform.Find("Camera").gameObject;

        //Inicializo menu pausa
        CanvasPausa = canvas.transform.Find("CanvasPausa").gameObject;
        CanvasFinPartida = canvas.transform.Find("CanvasFinPartida").gameObject;

        tickSprite = Resources.Load("Images/TickImage", typeof(Sprite)) as Sprite;
        crossSprite = Resources.Load("Images/CrossImage", typeof(Sprite)) as Sprite;

        //Inicializo menu Ingame

        IngameUI = canvas.Find("IngameUI").gameObject;
        auxRect = IngameUI.transform.Find("Barra de Vida").transform.Find("BarraVida").GetComponent<RectTransform>();
        BarraVidaMaxSize = auxRect.sizeDelta.x;
        DetectedText = IngameUI.transform.Find("DetectedText").GetComponent<Text>();
        primeraRecompensa = IngameUI.transform.Find("Recompensa 1").GetComponent<Image>();
        segundaRecompensa = IngameUI.transform.Find("Recompensa 2").GetComponent<Image>();
        SpriteObjetoEquipado = IngameUI.transform.Find("ObjEquipado").GetComponent<Image>();
        timeText = IngameUI.transform.Find("Fondo").transform.Find("TimeText").GetComponent<Text>();
        puntosText = IngameUI.transform.Find("Fondo").transform.Find("PointsText").GetComponent<Text>();
        trapsRemainingText = primeraRecompensa.transform.Find("TrapsRemainingText").GetComponent<Text>();

        tarjetaDocumentos = Resources.Load("Images/TarjetaDocumentos", typeof(Sprite)) as Sprite;
        tarjetaTrampasRestantes = Resources.Load("Images/TarjetaNumeroTrampas", typeof(Sprite)) as Sprite;
        tarjetaPiezas = Resources.Load("Images/TarjetaPiezas", typeof(Sprite)) as Sprite;
        SpriteNada = Resources.Load("Images/NadaIcono", typeof(Sprite)) as Sprite;
        SpriteGancho = Resources.Load("Images/GanchoIcono", typeof(Sprite)) as Sprite;
        SpriteShuriken = new Sprite[3];
        SpriteShuriken[0] = Resources.Load("Images/ShurikenInactivoIcono", typeof(Sprite)) as Sprite;
        SpriteShuriken[1] = Resources.Load("Images/ShurikenIcono", typeof(Sprite)) as Sprite;
        SpriteShuriken[2] = Resources.Load("Images/ShurikenDobleIcono", typeof(Sprite)) as Sprite;

    }
    // Update is called once per frame
    void Update()
    {
        switch (estado)
        {
            case 0: //Menu principal
                if (Input.GetButtonUp("Jump"))
                    setEstado(1);
                else if (Input.GetButtonUp("Use"))
                    setEstado(2);
                else if (Input.GetButtonUp("ModoSigilo"))
                    setEstado(3);
                break;
            case 1: //Seleccion de nivel
                if (Input.GetButtonUp("ModoSigilo"))
                    setEstado(0);

                if (Input.GetButtonDown("Run"))
                {
                    SeleccionNivel.transform.Find("Button0").GetComponent<LonelyButtonController>().ChangeValues(LonelyButtonController.tipoBoton.A, "Espacio 1");
                    SeleccionNivel.transform.Find("Button1").GetComponent<LonelyButtonController>().ChangeValues(LonelyButtonController.tipoBoton.X, "Espacio 2");
                    SeleccionNivel.transform.Find("Button2").GetComponent<LonelyButtonController>().ChangeValues(LonelyButtonController.tipoBoton.Y, "Espacio 3");
                }
                if (Input.GetButtonUp("Run"))
                {
                    SeleccionNivel.transform.Find("Button0").GetComponent<LonelyButtonController>().ChangeValues(LonelyButtonController.tipoBoton.A, "Nivel 1");
                    SeleccionNivel.transform.Find("Button1").GetComponent<LonelyButtonController>().ChangeValues(LonelyButtonController.tipoBoton.X, "Nivel 2");
                    SeleccionNivel.transform.Find("Button2").GetComponent<LonelyButtonController>().ChangeValues(LonelyButtonController.tipoBoton.Y, "Nivel 3");
                }

                if (Input.GetButton("Run"))
                {
                    if (Input.GetButtonUp("Jump"))
                        loadLevel(0);
                    if (Input.GetButtonUp("Use"))
                        loadLevel(1);
                    if (Input.GetButtonUp("Y"))
                        loadLevel(2);
                }
                else
                {
                    SeleccionNivel.transform.Find("Button0").GetComponent<LonelyButtonController>().ChangeValues(LonelyButtonController.tipoBoton.A, "Nivel 1");
                    SeleccionNivel.transform.Find("Button1").GetComponent<LonelyButtonController>().ChangeValues(LonelyButtonController.tipoBoton.X, "Nivel 2");
                    SeleccionNivel.transform.Find("Button2").GetComponent<LonelyButtonController>().ChangeValues(LonelyButtonController.tipoBoton.Y, "Nivel 3");
                    if (Input.GetButtonUp("Jump"))
                        goToMap(0);
                    else if (Input.GetButtonUp("Use"))
                        goToMap(1);
                    else if (Input.GetButtonUp("Y"))
                        goToMap(2);
                }
                break;
            case 2: //Pagina Instrucciones
                if (Input.GetButtonUp("ModoSigilo"))
                    setEstado(0);
                break;
            case 4: //Jugando
                break;
            case 5: //Pausa
                if (Input.GetButtonDown("Back"))
                    goToMainMenu();
                if (Input.GetButtonDown("ModoSigilo"))
                    restartLevel();
                break;
            case 6: //Pantalla fin de nivel
                if (Input.GetButtonDown("Back"))
                    goToMainMenu();
                if (Input.GetButtonDown("ModoSigilo"))
                    restartLevel();
                if (Input.GetButtonDown("Run"))
                {
                    CanvasFinPartida.transform.Find("Button0").GetComponent<LonelyButtonController>().ChangeValues(LonelyButtonController.tipoBoton.A, "Espacio 1");
                    CanvasFinPartida.transform.Find("Button1").GetComponent<LonelyButtonController>().ChangeValues(LonelyButtonController.tipoBoton.X, "Espacio 2");
                    CanvasFinPartida.transform.Find("Button2").gameObject.SetActive(true);
                }
                if (Input.GetButton("Run"))
                {
                    if (Input.GetButtonUp("Jump"))
                        saveLevel(0);
                    if (Input.GetButtonUp("Use"))
                        saveLevel(1);
                    if (Input.GetButtonUp("Y"))
                        saveLevel(2);
                }
                else
                {
                    CanvasFinPartida.transform.Find("Button0").GetComponent<LonelyButtonController>().ChangeValues(LonelyButtonController.tipoBoton.B, "Repetir mapa");
                    CanvasFinPartida.transform.Find("Button1").GetComponent<LonelyButtonController>().ChangeValues(LonelyButtonController.tipoBoton.Back, "Menu Principal");
                    CanvasFinPartida.transform.Find("Button2").gameObject.SetActive(false);
                }
                if (Input.GetButtonUp("Run"))
                {
                    CanvasFinPartida.transform.Find("Button0").GetComponent<LonelyButtonController>().ChangeValues(LonelyButtonController.tipoBoton.B, "Repetir mapa");
                    CanvasFinPartida.transform.Find("Button1").GetComponent<LonelyButtonController>().ChangeValues(LonelyButtonController.tipoBoton.Back, "Menu Principal");
                    CanvasFinPartida.transform.Find("Button2").gameObject.SetActive(false);
                }
                break;
        }
    }

    public void restartLevel()
    {
        saveLevel(3);
        loadLevel(3);
    }

    public void setEstado(int var)
    {
        estado = var;
        switch (estado)
        {
            case 0: //Menu principal
                MenuPrincipal.SetActive(true);
                SeleccionNivel.SetActive(false);
                Instrucciones.SetActive(false);
                IngameUI.SetActive(false);
                CanvasPausa.SetActive(false);
                CanvasFinPartida.SetActive(false);
                readyIngameUI = false;
                break;
            case 1: //Seleccion de nivel
                MenuPrincipal.SetActive(false);
                SeleccionNivel.SetActive(true);
                Instrucciones.SetActive(false);
                IngameUI.SetActive(false);
                CanvasPausa.SetActive(false);
                CanvasFinPartida.SetActive(false);
                generateMaps();
                break;
            case 2: //Pagina Instrucciones
                MenuPrincipal.SetActive(false);
                SeleccionNivel.SetActive(false);
                Instrucciones.SetActive(true);
                IngameUI.SetActive(false);
                CanvasPausa.SetActive(false);
                CanvasFinPartida.SetActive(false);
                break;
            case 3: //Salir del juego
                Application.Quit();
                break;
            case 4: //Jugando
                MenuPrincipal.SetActive(false);
                SeleccionNivel.SetActive(false);
                Instrucciones.SetActive(false);
                IngameUI.SetActive(true);
                CanvasPausa.SetActive(false);
                CanvasFinPartida.SetActive(false);
                if (!readyIngameUI)
                    initIngameUI();
                break;
            case 5: //Pausa
                MenuPrincipal.SetActive(false);
                SeleccionNivel.SetActive(false);
                Instrucciones.SetActive(false);
                IngameUI.SetActive(true);
                CanvasPausa.SetActive(true);
                CanvasFinPartida.SetActive(false);
                break;
            case 6: //Pantalla fin de nivel
                MenuPrincipal.SetActive(false);
                SeleccionNivel.SetActive(false);
                Instrucciones.SetActive(false);
                IngameUI.SetActive(true);
                CanvasPausa.SetActive(false);
                CanvasFinPartida.SetActive(true);
                edificio.getPausa();
                break;
        }
    }

    public void generateMaps()
    {
        ListaMapas = new List<nivel>();

        for (int i = 0; i < 3; i++)
        {
            int aux = Random.Range(1, 8);
            genMapa(aux);
            string texto = "Dificultad ";
            texto += ListaMapas[i].dificultad + " \nPisos " + ListaMapas[i].NumPisos + " \nHabitaciones " + ListaMapas[i].NumHabitaciones;
            texto += " \nEstilo " + ListaMapas[i].estilo + "\n---- Objetivos ----\n" + ListaMapas[i].condicionesVictoria[0] + " \n" + ListaMapas[i].condicionesVictoria[1];

            SeleccionNivel.transform.Find(("ConjuntoMapa " + i)).Find("DescripcionNivel").GetComponent<Text>().text = texto;
        }
    }

    private void genMapa(int dif)
    {
        nivel prueba;
        prueba.dificultad = dif;

        switch (dif)
        {
            case 1:
                prueba.NumPisos = 1;
                prueba.NumHabitaciones = Random.Range(3, 6);
                break;
            case 2:
                prueba.NumPisos = Random.Range(1, 3);
                prueba.NumHabitaciones = Random.Range(4, 7);
                break;
            case 3:
                prueba.NumPisos = 2;
                prueba.NumHabitaciones = Random.Range(5, 8);
                break;
            case 4:
                prueba.NumPisos = Random.Range(2, 4);
                prueba.NumHabitaciones = 6;
                break;
            case 5:
                prueba.NumPisos = 3;
                prueba.NumHabitaciones = 6;
                break;
            case 6:
                prueba.NumPisos = Random.Range(3, 5);
                prueba.NumHabitaciones = 6;
                break;
            case 7:
                prueba.NumPisos = 4;
                prueba.NumHabitaciones = 6;
                break;
            default:
                prueba.NumPisos = 1;
                prueba.NumHabitaciones = Random.Range(3, 6);
                break;
        }
        prueba.estilo = Edificio.getEstiloRandom();

        prueba.condicionesVictoria = new List<Edificio.condicionesVictoria>();
        prueba.condicionesVictoria.Add(Edificio.getRandomCondicionVictoria());
        while (prueba.condicionesVictoria.Count < 2)
        {
            Edificio.condicionesVictoria aux = Edificio.getRandomCondicionVictoria();
            if (!prueba.condicionesVictoria.Contains(aux))
                prueba.condicionesVictoria.Add(aux);
        }

        //print ("La dificultad del mapa es " + dif + " y tiene "+ prueba.NumPisos + " pisos y " + prueba.NumHabitaciones+" habitaciones, con estilo "+prueba.estilo);
        ListaMapas.Add(prueba);
    }

    public void goToMap(int index)
    {
        GameObject obj = new GameObject();
        obj.name = "Edificio";
        obj.transform.SetParent(this.transform);
        obj.AddComponent<Edificio>();
        obj.GetComponent<Edificio>().init(ListaMapas[index].dificultad, ListaMapas[index].NumPisos, ListaMapas[index].NumHabitaciones, ListaMapas[index].estilo, ListaMapas[index].condicionesVictoria);
        obj.GetComponent<Edificio>().setInitialTime(Time.time);
        enableCamera(false);
        setEstado(4);
    }

    public void setDatosFinPartida(float vidaFinal, List<Edificio.condicionesVictoria> objetivosMision, bool primerObjetivo, bool segundoObjetivo, int trapCount, int trapCountTotal, float time, float puntos)
    {
        CanvasFinPartida.transform.Find("TimeText").GetComponent<Text>().text = formatTime(time);
        CanvasFinPartida.transform.Find("PointsText").GetComponent<Text>().text = "Ptos: " + puntos;
        CanvasFinPartida.transform.Find("SaludText").GetComponent<Text>().text = "Salud: " + vidaFinal + "%";
        CanvasFinPartida.transform.Find("TrapCountText").GetComponent<Text>().text = "Trampas " + (trapCountTotal - trapCount) + "/" + trapCountTotal;

        for (int i = 0; i < objetivosMision.Count; i++)
        {
            switch (objetivosMision[i])
            {
                case Edificio.condicionesVictoria.conseguirDocumentos:
                    if (i == 0)
                    {
                        Transform auxTrans = CanvasFinPartida.transform.Find("PrimerObjetivo");
                        auxTrans.GetComponent<Text>().text = "Documentos";
                        if (primerObjetivo)
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = tickSprite;
                        else
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = crossSprite;
                    }
                    else
                    {
                        Transform auxTrans = CanvasFinPartida.transform.Find("SegundoObjetivo");
                        auxTrans.GetComponent<Text>().text = "Documentos";
                        if (segundoObjetivo)
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = tickSprite;
                        else
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = crossSprite;
                    }
                    break;
                case Edificio.condicionesVictoria.conseguirPiezas:
                    if (i == 0)
                    {
                        Transform auxTrans = CanvasFinPartida.transform.Find("PrimerObjetivo");
                        auxTrans.GetComponent<Text>().text = "Piezas";
                        if (primerObjetivo)
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = tickSprite;
                        else
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = crossSprite;
                    }
                    else
                    {
                        Transform auxTrans = CanvasFinPartida.transform.Find("SegundoObjetivo");
                        auxTrans.GetComponent<Text>().text = "Piezas";
                        if (segundoObjetivo)
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = tickSprite;
                        else
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = crossSprite;
                    }
                    break;
                case Edificio.condicionesVictoria.desactivarTrampas:
                    if (i == 0)
                    {
                        Transform auxTrans = CanvasFinPartida.transform.Find("PrimerObjetivo");
                        auxTrans.GetComponent<Text>().text = "Trampas";
                        if (primerObjetivo)
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = tickSprite;
                        else
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = crossSprite;
                    }
                    else
                    {
                        Transform auxTrans = CanvasFinPartida.transform.Find("SegundoObjetivo");
                        auxTrans.GetComponent<Text>().text = "Trampas";
                        if (segundoObjetivo)
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = tickSprite;
                        else
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = crossSprite;
                    }
                    break;
            }
        }
    }

    private string formatTime(float time)
    {
        int minutos, segundos;
        segundos = (int)time;
        minutos = segundos / 60;
        if (minutos > 0)
            segundos = segundos - (minutos * 60);

        string timeString = "Tiempo ";

        if (minutos < 10)
            timeString += "0" + minutos;
        else
            timeString += minutos;

        if (segundos < 10)
            timeString += ":0" + segundos;
        else
            timeString += ":" + segundos;

        return timeString;


    }

    public void goToMainMenu()
    {
        setEstado(0);
        enableCamera(true);
        Destroy(edificio.gameObject);
    }

    public void updateDetected(bool var)
    {
        if (var)
            DetectedText.gameObject.SetActive(true);
        else
            DetectedText.gameObject.SetActive(false);

    }

    public void addCondicionVictoria(List<Edificio.condicionesVictoria> condiciones)
    {
        if (!readyIngameUI)
            initIngameUI();

        condicionesVictoria = condiciones;
        if (condiciones.Contains(Edificio.condicionesVictoria.desactivarTrampas))
        {
            primeraRecompensa.sprite = tarjetaTrampasRestantes;
            primeraRecompensa.gameObject.SetActive(true);
            primeraRecompensa.transform.Find("TrapsRemainingText").gameObject.SetActive(true);

            trapCount = 0;
        }
    }

    public void addReward(ObjetoRecompensa.tipoRecompensa reward)
    {
        if (reward != ObjetoRecompensa.tipoRecompensa.conjuntoPuntos)
        {
            Sprite auxSprite = new Sprite();
            switch (reward)
            {
                case ObjetoRecompensa.tipoRecompensa.documentos:
                    auxSprite = tarjetaDocumentos;
                    break;
                case ObjetoRecompensa.tipoRecompensa.piezasSecretas:
                    auxSprite = tarjetaPiezas;
                    break;
            }

            if (!primeraRecompensa.gameObject.activeSelf)
            {

                primeraRecompensa.sprite = auxSprite;
                primeraRecompensa.gameObject.SetActive(true);
            }
            else
            {
                segundaRecompensa.sprite = auxSprite;
                segundaRecompensa.gameObject.SetActive(true);
            }
        }
    }

    public void restartRewards()
    {
        trapsRemainingText.gameObject.SetActive(false);
        primeraRecompensa.gameObject.SetActive(false);
        segundaRecompensa.gameObject.SetActive(false);
    }

    public void updateCantShurikens(int number)
    {
        if (number != cantShurikens)
        {
            cantShurikens = number;
            changeObjetoEquipado(2);
        }
    }

    public void updateLifeBar(float percentage)
    {
        auxRect.sizeDelta = new Vector2(BarraVidaMaxSize * (percentage), auxRect.rect.height);
    }

    public void changeObjetoEquipado(int objeto)
    {
        switch (objeto)
        {
            case 0:
                print("No llevo nada");
                SpriteObjetoEquipado.sprite = SpriteNada;
                break;
            case 1:
                print("LLevo el gancho");
                SpriteObjetoEquipado.sprite = SpriteGancho;
                break;
            case 2:
                //print("LLevo el shuriken");
                SpriteObjetoEquipado.sprite = SpriteShuriken[cantShurikens];
                break;
        }
    }

    public void updatePoints(float points)
    {
        puntosText.text = points + " puntos";
    }

    public void updateTrapCount(int number)
    {
        trapsRemainingText.text = "" + number;
    }

    public void enableCamera(bool var)
    {
        MainCamera.SetActive(var);
    }

    public void updateTime(float time)
    {
        timeText.text = formatTime(time);
    }

    public void initIngameUI()
    {
        updateLifeBar(1);
        restartRewards();
        trapCount = -1;
        detected = false;
        updateDetected(detected);
        objetoEquipado = 0;
        SpriteObjetoEquipado.sprite = SpriteNada;
        cantShurikens = 2;

        updatePoints(0);
        if (condicionesVictoria == null)
            condicionesVictoria = new List<Edificio.condicionesVictoria>();
        readyIngameUI = true;
    }

    public void setPlayer(PlayerController var)
    {
        player = var;
    }

    public void setEdificio(Edificio var)
    {
        edificio = var;
    }

    public void saveLevel(int hueco)
    {
        StreamWriter writer = new StreamWriter("Nivel" + hueco + ".txt");
        writer.WriteLine("dificultad " + edificio.dificultad);

        List<Edificio.estructuraPisos> auxLista = edificio.getListaPisos();

        for (int i = 0; i < edificio.objetivosMision.Count; i++)
        {
            switch (edificio.objetivosMision[i])
            {
                case Edificio.condicionesVictoria.conseguirDocumentos:
                    writer.WriteLine("objetivo"+i+" 0");
                    break;
                case Edificio.condicionesVictoria.conseguirPiezas:
                    writer.WriteLine("objetivo" + i + " 1");
                    break;
                case Edificio.condicionesVictoria.desactivarTrampas:
                    writer.WriteLine("objetivo" + i + " 2");
                    break;
            }
        }

        for(int pisoActual = 0;pisoActual< auxLista.Count; pisoActual++)
        {
            writer.WriteLine("NewFloor " + pisoActual);
            for(int habActual = 0;habActual< auxLista[pisoActual].habitaciones.Count; habActual++)
            {
                writer.WriteLine("NewRoom " + auxLista[pisoActual].habitaciones[habActual].id + " ------------");
                writer.WriteLine("piso " + auxLista[pisoActual].habitaciones[habActual].piso);
                string auxStr = auxLista[pisoActual].habitaciones[habActual].estiloHabitacion == Edificio.listaEstilos.oficina ? "estilo 0" : "estilo 1";
                writer.WriteLine(auxStr);
                switch (auxLista[pisoActual].habitaciones[habActual].tipoHabitacion)
                {
                    case RoomController.tipo.Entrada:
                        auxStr = "tipo 0";
                        break;
                    case RoomController.tipo.HabitacionPrincipal:
                        auxStr = "tipo 1";
                        break;
                    case RoomController.tipo.Banyo:
                        auxStr = "tipo 2";
                        break;
                }
                writer.WriteLine(auxStr);
                writer.WriteLine("nivel "+auxLista[pisoActual].habitaciones[habActual].nivel);
                writer.WriteLine("coordenadas "+auxLista[pisoActual].habitaciones[habActual].coordenadas.x+" "+ auxLista[pisoActual].habitaciones[habActual].coordenadas.y);
                writer.WriteLine("tam " + auxLista[pisoActual].habitaciones[habActual].tam);
                auxStr = auxLista[pisoActual].habitaciones[habActual].hasTecho ? "techo 1" : "techo 0";
                writer.WriteLine(auxStr);
                auxStr = auxLista[pisoActual].habitaciones[habActual].tall ? "tall 1" : "tall 0";
                writer.WriteLine(auxStr);
                writer.WriteLine("camara "+ auxLista[pisoActual].habitaciones[habActual].cameraPosition);
                writer.WriteLine("ocultos " + auxLista[pisoActual].habitaciones[habActual].cantSitiosOcultos);
                switch (auxLista[pisoActual].habitaciones[habActual].reward)
                {
                    case ObjetoRecompensa.tipoRecompensa.ninguno:
                        auxStr = "reward 0";
                        break;
                    case ObjetoRecompensa.tipoRecompensa.documentos:
                        auxStr = "reward 1";
                        break;
                    case ObjetoRecompensa.tipoRecompensa.piezasSecretas:
                        auxStr = "reward 2";
                        break;
                    case ObjetoRecompensa.tipoRecompensa.conjuntoPuntos:
                        auxStr = "reward 3";
                        break;
                }
                writer.WriteLine(auxStr);
                auxStr = "lateral";
                for (int i=0;i< 4; i++)
                {
                    switch (auxLista[pisoActual].habitaciones[habActual].listaLaterales[i])
                    {
                        case RoomController.tiposParedes.nada:
                            auxStr += " 0";
                            break;
                        case RoomController.tiposParedes.pared:
                            auxStr += " 1";
                            break;
                        case RoomController.tiposParedes.puerta:
                            auxStr += " 2";
                            break;
                        case RoomController.tiposParedes.trampilla:
                            auxStr += " 3";
                            break;
                    }
                }
                writer.WriteLine(auxStr);
                if (auxLista[pisoActual].habitaciones[habActual].listaTrampas != null)
                {
                    if (auxLista[pisoActual].habitaciones[habActual].listaTrampas.Count > 0)
                    {
                        for (int i = 0; i < auxLista[pisoActual].habitaciones[habActual].listaTrampas.Count; i++)
                        {
                            auxStr = "trampa" + i;
                            switch (auxLista[pisoActual].habitaciones[habActual].listaTrampas[i].tipo)
                            {
                                case GeneradorObjetos.tipo.Mina:
                                    auxStr += " 0 " + auxLista[pisoActual].habitaciones[habActual].listaTrampas[i].level;
                                    break;
                                case GeneradorObjetos.tipo.Cepo:
                                    auxStr += " 1 " + auxLista[pisoActual].habitaciones[habActual].listaTrampas[i].level;
                                    break;
                                case GeneradorObjetos.tipo.PlacaPresion:
                                    auxStr += " 2 " + auxLista[pisoActual].habitaciones[habActual].listaTrampas[i].level;
                                    break;
                                case GeneradorObjetos.tipo.RedLaser:
                                    auxStr += " 3 " + auxLista[pisoActual].habitaciones[habActual].listaTrampas[i].level + " " + auxLista[pisoActual].habitaciones[habActual].listaTrampas[i].verticalSpeed;
                                    break;
                            }
                        }
                        writer.WriteLine(auxStr);
                    }
                }
            }
        }
        writer.Close();

        goToMainMenu();
    }

    public void loadLevel(int hueco)
    {
        FileInfo theSourceFile = null;
        StreamReader reader = null;
        string text = " ";
        theSourceFile = new FileInfo("Nivel" + hueco + ".txt");
        bool listo = false;
        if (theSourceFile != null)
        {
            reader = theSourceFile.OpenText();
            string[] strArr;
            List<Edificio.estructuraPisos> listaPisos = new List<Edificio.estructuraPisos>();
            Edificio.estructuraPisos auxPiso = new Edificio.estructuraPisos();
            Edificio.estructuraHabitacion auxRoom = new Edificio.estructuraHabitacion();
            Edificio.condicionesVictoria condicion0 = new Edificio.condicionesVictoria();
            Edificio.condicionesVictoria condicion1 = new Edificio.condicionesVictoria();
            int dificultad = 1;

            while (!listo)
            {
                text = reader.ReadLine();
                if (text != null)
                {
                    strArr = text.Split(' ');
                    int pisoActual = 0;

                    switch (strArr[0])
                    {
                        case "dificultad":
                            dificultad = int.Parse(strArr[1]);
                            break;
                        case "objetivo0":
                            switch (int.Parse(strArr[1]))
                            {
                                case 0:
                                    condicion0 = Edificio.condicionesVictoria.conseguirDocumentos;
                                    break;
                                case 1:
                                    condicion0 = Edificio.condicionesVictoria.conseguirPiezas;
                                    break;
                                case 2:
                                    condicion0 = Edificio.condicionesVictoria.desactivarTrampas;
                                    break;
                            }
                            break;
                        case "objetivo1":
                            switch (int.Parse(strArr[1]))
                            {
                                case 0:
                                    condicion1 = Edificio.condicionesVictoria.conseguirDocumentos;
                                    break;
                                case 1:
                                    condicion1 = Edificio.condicionesVictoria.conseguirPiezas;
                                    break;
                                case 2:
                                    condicion1 = Edificio.condicionesVictoria.desactivarTrampas;
                                    break;
                            }
                            break;
                        case "NewFloor":
                            if(int.Parse(strArr[1]) != 0)
                            {
                                auxPiso.habitaciones.Add(auxRoom);
                                listaPisos.Add(auxPiso);
                                pisoActual++;
                            }
                            auxPiso = new Edificio.estructuraPisos();
                            auxPiso.habitaciones = new List<Edificio.estructuraHabitacion>();
                            break;
                        case "NewRoom":
                            if (int.Parse(strArr[1]) != 0)
                                auxPiso.habitaciones.Add(auxRoom);
                            auxRoom = new Edificio.estructuraHabitacion();
                            auxRoom.listaLaterales = new RoomController.tiposParedes[4];
                            auxRoom.listaTrampas = new List<GeneradorObjetos.trapStruct>();
                            auxRoom.conectaCon = new List<Edificio.estructuraHabitacion>();
                            auxRoom.id = int.Parse(strArr[1]);
                            break;
                        case "piso":
                            auxRoom.piso = pisoActual;
                            break;
                        case "estilo":
                            auxRoom.estiloHabitacion = int.Parse(strArr[1]) == 0 ? Edificio.listaEstilos.oficina : Edificio.listaEstilos.casaNormal;
                            break;
                        case "tipo":
                            switch (int.Parse(strArr[1]))
                            {
                                case 0:
                                    auxRoom.tipoHabitacion = RoomController.tipo.Entrada;
                                    break;
                                case 1:
                                    auxRoom.tipoHabitacion = RoomController.tipo.HabitacionPrincipal;
                                    break;
                                case 2:
                                    auxRoom.tipoHabitacion = RoomController.tipo.Banyo;
                                    break;
                            }
                            break;
                        case "nivel":
                            auxRoom.nivel = int.Parse(strArr[1]);
                            break;
                        case "coordenadas":
                            auxRoom.coordenadas = new Vector2(float.Parse(strArr[1]), float.Parse(strArr[2]));
                            break;
                        case "tam":
                            auxRoom.tam = int.Parse(strArr[1]);
                            break;
                        case "techo":
                            auxRoom.hasTecho = int.Parse(strArr[1]) == 1 ? true : false;
                            break;
                        case "tall":
                            auxRoom.tall = int.Parse(strArr[1]) == 1 ? true : false;
                            break;
                        case "camara":
                            auxRoom.cameraPosition = int.Parse(strArr[1]);
                            break;
                        case "ocultos":
                            auxRoom.cantSitiosOcultos = int.Parse(strArr[1]);
                            break;
                        case "reward":
                            switch (int.Parse(strArr[1]))
                            {
                                case 0:
                                    auxRoom.reward = ObjetoRecompensa.tipoRecompensa.ninguno;
                                    break;
                                case 1:
                                    auxRoom.reward = ObjetoRecompensa.tipoRecompensa.documentos;
                                    break;
                                case 2:
                                    auxRoom.reward = ObjetoRecompensa.tipoRecompensa.piezasSecretas;
                                    break;
                                case 3:
                                    auxRoom.reward = ObjetoRecompensa.tipoRecompensa.conjuntoPuntos;
                                    break;
                            }
                            break;
                        case "lateral":
                            for (int i = 1; i <= 4; i++)
                            {
                                switch (int.Parse(strArr[i]))
                                {
                                    case 0:
                                        auxRoom.listaLaterales[i - 1] = RoomController.tiposParedes.nada;
                                        break;
                                    case 1:
                                        auxRoom.listaLaterales[i - 1] = RoomController.tiposParedes.pared;
                                        break;
                                    case 2:
                                        auxRoom.listaLaterales[i - 1] = RoomController.tiposParedes.puerta;
                                        break;
                                    case 3:
                                        auxRoom.listaLaterales[i - 1] = RoomController.tiposParedes.trampilla;
                                        break;
                                }
                            }
                            break;
                        case "trampa0":
                            GeneradorObjetos.trapStruct auxStruct = new GeneradorObjetos.trapStruct();

                            if (int.Parse(strArr[1]) < 3)
                            {
                                if (int.Parse(strArr[1]) == 0)
                                    auxStruct.tipo = GeneradorObjetos.tipo.Mina;
                                else if (int.Parse(strArr[1]) == 1)
                                    auxStruct.tipo = GeneradorObjetos.tipo.Cepo;
                                else if (int.Parse(strArr[1]) == 2)
                                    auxStruct.tipo = GeneradorObjetos.tipo.PlacaPresion;
                                auxStruct.level = int.Parse(strArr[2]);
                            }
                            else //Es la red laser
                            {
                                auxStruct.tipo = GeneradorObjetos.tipo.RedLaser;
                                auxStruct.level = int.Parse(strArr[2]);
                                auxStruct.verticalSpeed = float.Parse(strArr[3]);
                            }
                            auxRoom.listaTrampas.Add(auxStruct);
                            break;
                        case "trampa1":
                            auxStruct = new GeneradorObjetos.trapStruct();

                            if (int.Parse(strArr[1]) < 3)
                            {
                                if (int.Parse(strArr[1]) == 0)
                                    auxStruct.tipo = GeneradorObjetos.tipo.Mina;
                                else if (int.Parse(strArr[1]) == 1)
                                    auxStruct.tipo = GeneradorObjetos.tipo.Cepo;
                                else if (int.Parse(strArr[1]) == 2)
                                    auxStruct.tipo = GeneradorObjetos.tipo.PlacaPresion;
                                auxStruct.level = int.Parse(strArr[2]);
                            }
                            else //Es la red laser
                            {
                                auxStruct.tipo = GeneradorObjetos.tipo.RedLaser;
                                auxStruct.level = int.Parse(strArr[2]);
                                auxStruct.verticalSpeed = float.Parse(strArr[3]);
                            }
                            auxRoom.listaTrampas.Add(auxStruct);
                            break;
                        case "trampa2":
                            auxStruct = new GeneradorObjetos.trapStruct();

                            if (int.Parse(strArr[1]) < 3)
                            {
                                if (int.Parse(strArr[1]) == 0)
                                    auxStruct.tipo = GeneradorObjetos.tipo.Mina;
                                else if (int.Parse(strArr[1]) == 1)
                                    auxStruct.tipo = GeneradorObjetos.tipo.Cepo;
                                else if (int.Parse(strArr[1]) == 2)
                                    auxStruct.tipo = GeneradorObjetos.tipo.PlacaPresion;
                                auxStruct.level = int.Parse(strArr[2]);
                            }
                            else //Es la red laser
                            {
                                auxStruct.tipo = GeneradorObjetos.tipo.RedLaser;
                                auxStruct.level = int.Parse(strArr[2]);
                                auxStruct.verticalSpeed = float.Parse(strArr[3]);
                            }
                            auxRoom.listaTrampas.Add(auxStruct);
                            break;
                    }

                }
                else
                {
                    auxPiso.habitaciones.Add(auxRoom);
                    listaPisos.Add(auxPiso);

                    listo = true;
                }
            }
            GameObject obj = new GameObject();
            obj.name = "Edificio";
            obj.transform.SetParent(this.transform);
            obj.AddComponent<Edificio>();
            obj.GetComponent<Edificio>().loadedMap(listaPisos, condicion0, condicion1, dificultad);
            obj.GetComponent<Edificio>().setInitialTime(Time.time);
            enableCamera(false);
            setEstado(4);
        }
        else
        {
            print("Error al leer Nivel" + hueco + ".txt");
        }
        reader.Close();
    }

}
