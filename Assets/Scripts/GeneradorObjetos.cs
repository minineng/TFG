using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorObjetos : MonoBehaviour
{

	public GameObject minaPrefab;
	public GameObject camaraPrefab;
	public GameObject puertaPrefab;
	public GameObject cepoPrefab;
	public GameObject paredPrefab;
	public int nivel;
	public RoomController.listaEstilos estilo;
	public tipo Seleccion;

	public enum tipo
	{
		Puerta,
		Pared,
		Mina,
		Cepo,
        Recompensa
	}

	public bool camara;
		


	// Use this for initialization
	void Start ()
	{

		this.gameObject.transform.GetChild (0).gameObject.SetActive (false);//oculto la esfera para situar el generador
		//nivel = transform.parent.gameObject.GetComponent<RoomController> ().dificultad;
		//estilo = transform.parent.gameObject.GetComponent<RoomController> ().estilo;


		/*
		 * 		Vector3 posicion = this.gameObject.transform.position;
		nivel = transform.parent.gameObject.GetComponent<RoomController> ().dificultad;
		estilo = transform.parent.gameObject.GetComponent<RoomController> ().estilo;

		//print (estilo);


		GameObject objeto = this.gameObject;
		switch (Seleccion) {
		case tipo.Puerta:
			{
				objeto = Instantiate (puertaPrefab, posicion, Quaternion.identity);
				objeto.GetComponent<ScriptPuerta> ().level = nivel;
				switch (transform.name) {
				case "Gen ParteDer":
					objeto.transform.name = "Puerta Derecha";
					break;
				case "Gen ParteIzq":
					objeto.transform.name = "Puerta Izquierda";
					break;
				}
				objeto.GetComponent<ScriptPuerta> ().estilo = estilo;
				objeto.transform.position = new Vector3 (objeto.transform.position.x, objeto.transform.position.y + 31, objeto.transform.position.z + 3.15F);
				break;
			}
		case tipo.Pared:
			{
				objeto = Instantiate (paredPrefab, posicion, Quaternion.identity);
				objeto.GetComponent<ScriptPared> ().level = nivel;
				objeto.GetComponent<ScriptPared> ().estilo = estilo;
				objeto.GetComponent<ScriptPared> ().generateRandom (true);
				objeto.transform.position = new Vector3 (objeto.transform.position.x, objeto.transform.position.y + 30.4f, objeto.transform.position.z + 3.75F);
				break;
			}
		case tipo.Cepo:
			{
				objeto = Instantiate (cepoPrefab, posicion, Quaternion.identity);
				objeto.GetComponent<cepoController> ().level = nivel;
				break;
			}
		case tipo.Mina:
			{
				objeto = Instantiate (minaPrefab, posicion, Quaternion.identity);
				objeto.GetComponent<ScriptMina> ().level = nivel;
				break;
			}
		}
		objeto.transform.SetParent (transform.parent);*/
	}

	public GameObject generateObjects(tipo newObject){
		Vector3 posicion = this.gameObject.transform.position;
		GameObject objeto = this.gameObject;

		switch (newObject) {
		case tipo.Puerta:
			{
				objeto = Instantiate (puertaPrefab, posicion, Quaternion.identity);
				objeto.GetComponent<ScriptPuerta> ().level = nivel;
				switch (transform.name) {
				case "Gen ParteDer":
					objeto.transform.name = "Puerta Derecha";
					break;
				case "Gen ParteIzq":
					objeto.transform.name = "Puerta Izquierda";
					break;
				}
				objeto.GetComponent<ScriptPuerta> ().estilo = estilo;
				objeto.transform.position = new Vector3 (objeto.transform.position.x, objeto.transform.position.y + 31, objeto.transform.position.z + 3.15F);
				break;
			}
		case tipo.Pared:
			{
				objeto = Instantiate (paredPrefab, posicion, Quaternion.identity);
				objeto.GetComponent<ScriptPared> ().level = nivel;
				objeto.GetComponent<ScriptPared> ().estilo = estilo;
				objeto.transform.position = new Vector3 (objeto.transform.position.x, objeto.transform.position.y + 30.4f, objeto.transform.position.z + 3.75F);
				break;
			}
		case tipo.Cepo:
			{
				objeto = Instantiate (cepoPrefab, posicion, Quaternion.identity);
				objeto.GetComponent<cepoController> ().level = nivel;
				break;
			}
		case tipo.Mina:
			{
				objeto = Instantiate (minaPrefab, posicion, Quaternion.identity);
				objeto.GetComponent<ScriptMina> ().level = nivel;
				break;
			}
        case tipo.Recompensa:
                {
                    objeto = Instantiate(minaPrefab, posicion, Quaternion.identity);
                    objeto.GetComponent<ScriptMina>().level = nivel;
                    break;
                }
        }
		objeto.transform.SetParent (transform.parent);

		return objeto;

	}
		
		
	// Update is called once per frame
	void Update ()
	{
			
	}

	public static tipo getRandomObject(){

		switch (Random.Range (0, 2)) {
		case 0:
			return(tipo.Mina);
			break;
		case 1:
			return(tipo.Cepo);
			break;
		default:
			return(tipo.Mina);
			break;
		}
	}
}
