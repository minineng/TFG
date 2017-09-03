using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour {

    private int estado;
    private Transform canvas;
    private GameObject MenuPrincipal;
    private GameObject SeleccionNivel;
    private GameObject Instrucciones;

    public struct nivel
    {
        public int NumPisos;
        public int NumHabitaciones;
        public int dificultad;
        public Edificio.listaEstilos estilo;
        public List<Edificio.condicionesVictoria> condicionesVictoria;
    };
    public List<nivel> ListaMapas;

    // Use this for initialization
    public void Start () {
        estado = 0;
        canvas = transform.Find("Canvas");
        MenuPrincipal = canvas.Find("MenuPrincipal").gameObject;
        SeleccionNivel = canvas.Find("SeleccionDeNivel").gameObject;
        Instrucciones = canvas.Find("Instrucciones").gameObject;

    }
	
	// Update is called once per frame
	void Update () {
        switch (estado)
        {
            case 0: //Menu principal
                if (Input.GetButtonUp("Jump"))
                    setEstado(1);
                else if (Input.GetButtonUp("Use"))
                    setEstado(2);
                else if (Input.GetButtonUp("Back"))
                    setEstado(3);
                break;
            case 1: //Seleccion de nivel
                if (Input.GetButtonUp("ModoSigilo"))
                    setEstado(0);
                else if (Input.GetButtonUp("Jump"))
                    goToMap(0);
                else if (Input.GetButtonUp("Use"))
                    goToMap(1);
                else if (Input.GetButtonUp("Y"))
                    goToMap(2);
                break;
            case 2: //Pagina Instrucciones
                if (Input.GetButtonUp("ModoSigilo"))
                    setEstado(0);
                break;
        }
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
                break;
            case 1: //Seleccion de nivel
                MenuPrincipal.SetActive(false);
                SeleccionNivel.SetActive(true);
                Instrucciones.SetActive(false);
                generateMaps();
                break;
            case 2: //Pagina Instrucciones
                MenuPrincipal.SetActive(false);
                SeleccionNivel.SetActive(false);
                Instrucciones.SetActive(true);
                break;
            case 3: //Salir del juego
                Application.Quit();
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
        string aux = "Mapa " + index + 1;
        //Scene escena = SceneManager.CreateScene(aux);
        /*SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene().buildIndex);
        SceneManager.SetActiveScene(escena);*/
        GameObject obj = new GameObject();
        obj.name = "Edificio";
        obj.AddComponent<Edificio>();
        obj.GetComponent<Edificio>().init(ListaMapas[index].dificultad, ListaMapas[index].NumPisos, ListaMapas[index].NumHabitaciones, ListaMapas[index].estilo, ListaMapas[index].condicionesVictoria);
        obj.GetComponent<Edificio>().setInitialTime(Time.time);
        Destroy(this.gameObject);
    }
}
