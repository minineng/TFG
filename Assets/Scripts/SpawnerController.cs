using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum characters{Player, Enemy}

public class SpawnerController : MonoBehaviour {

	public GameObject PlayerPrefab;
	public GameObject EnemyPrefab;
	public characters spawn;


	// Use this for initialization
	void Start () {
		this.gameObject.transform.GetChild (0).gameObject.SetActive (false);//oculto la esfera para situar el generador
		if (GetComponentInParent<RoomController> ().playerSpawn) {
			Vector3 posicion = gameObject.transform.position;
			switch (spawn) {
			case characters.Player:
				//print ("Saco un jugador");
				GameObject personaje = Instantiate (PlayerPrefab, posicion, Quaternion.identity);
                    personaje.transform.SetParent(transform.parent.parent);

                personaje.transform.Rotate (new Vector3 (0, 90, 0));
				break;
			case characters.Enemy:
				print ("Saco un enemigo");
				break;
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
