using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MapSwitcher : MonoBehaviour {

	 public struct nivel{
		public int NumPisos;
		public int NumHabitaciones;
		public int dificultad;
		public RoomController.listaEstilos estilo;

	};
	private Button boton1, boton2, boton3 ;

	public List<nivel> ListaMapas;




	// Use this for initialization
	void Start () {
		ListaMapas = new List<nivel> ();

		for (int i = 0; i < 3; i++) {
			int aux = Random.Range (1, 8);
			genMapa (aux);
			transform.GetChild (i).GetComponent<Text> ().text = "Mapa "+i+": \nDificultad " + ListaMapas[i].dificultad + " \nPisos "+ ListaMapas[i].NumPisos + " \nHabitaciones " + ListaMapas[i].NumHabitaciones+" \nEstilo "+ListaMapas[i].estilo;
		}
		boton1 = transform.GetChild(0).GetComponent<Button>();
		boton2 = transform.GetChild(1).GetComponent<Button>();
		boton3 = transform.GetChild(2).GetComponent<Button>();

	}
	
	// Update is called once per frame
	void Update () {
		
		/*if (boton2.onClick)
			print ("boton 2");
		if (boton3.onClick)
			print ("boton 3");*/

	}

	private void genMapa(int dif){
		nivel prueba;
		prueba.dificultad = dif;

		switch (dif) {
		case 1:
			prueba.NumPisos = 1;
			prueba.NumHabitaciones = Random.Range (3, 6);
			break;
		case 2:
			prueba.NumPisos = Random.Range (1, 3);
			prueba.NumHabitaciones = Random.Range (4, 7);
			break;
		case 3:
			prueba.NumPisos = 2;
			prueba.NumHabitaciones = Random.Range (5, 8);
			break;
		case 4:
			prueba.NumPisos = Random.Range (2, 4);
			prueba.NumHabitaciones = 6;
			break;
		case 5:
			prueba.NumPisos = 3;
			prueba.NumHabitaciones = 6;
			break;
		case 6:
			prueba.NumPisos = Random.Range (3, 5);
			prueba.NumHabitaciones = 6;
			break;
		case 7:
			prueba.NumPisos = 4;
			prueba.NumHabitaciones = 6;
			break;
		default:
			prueba.NumPisos = 1;
			prueba.NumHabitaciones = Random.Range (3, 6);
			break;
		}
		prueba.estilo = RoomController.getEstiloRandom ();
		/*
		switch (Random.Range (0, 2)) { // Se elige el estilo entre los que hay
		case 0:
			prueba.estilo = RoomController.listaEstilos.casaNormal;
			break;
		case 1:
			prueba.estilo = RoomController.listaEstilos.oficina;
			break;
		default:
			prueba.estilo = RoomController.listaEstilos.casaNormal;
			break;
		}*/

		print ("La dificultad del mapa es " + dif + " y tiene "+ prueba.NumPisos + " pisos y " + prueba.NumHabitaciones+" habitaciones, con estilo "+prueba.estilo);
		ListaMapas.Add (prueba);
	}

	public void goToMap(int index){
		string aux = ("Mapa "+ index);
		print (aux);
		Scene escena = SceneManager.CreateScene (aux);
		SceneManager.UnloadSceneAsync (SceneManager.GetActiveScene().buildIndex);
		SceneManager.SetActiveScene (escena);
		GameObject obj = new GameObject ();
		obj.AddComponent<MapGenController> ();
		obj.GetComponent<MapGenController> ().init (ListaMapas [index].dificultad, ListaMapas [index].NumPisos, ListaMapas [index].NumHabitaciones, ListaMapas [index].estilo);

		//script = new MapGenController(ListaMapas [index].dificultad, ListaMapas [index].NumPisos, ListaMapas [index].NumHabitaciones, ListaMapas [index].estilo);
		//obj.AddComponent (script);
	}


		
}
