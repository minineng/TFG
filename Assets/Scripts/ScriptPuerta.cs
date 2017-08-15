using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptPuerta : ClaseObjeto {

	public bool cerrado;


	// Use this for initialization
	void Start () {
		cerrado = true;
		//print ("El estilo de esta puerta es "+estilo);

		switch(estilo){
		case RoomController.listaEstilos.casaNormal:
			//print ("Puerta estilo japones");
			this.transform.GetChild (0).gameObject.SetActive (true);
			anim = this.transform.GetChild(0).GetComponent<Animator> ();
			break;
		case RoomController.listaEstilos.oficina:
			//print (this.gameObject.name + " nivel: " + level);

			int aux = 0;
			switch (level) {
			case 1:
				aux = 1;
				break;
			case 2:
				aux = Random.Range (1, 3);
				break;
			case 3:
				aux = Random.Range (1, 4);
				break;
			case 4:
				aux = Random.Range (2, 5);
				break;
			case 5:
				aux = Random.Range (3, 5);
				break;
			case 6:
				aux = 4;
				break;
			default:
				aux = 1;
				break;
			}
			level = aux;
			//this.transform.name = "Puerta nivel " + aux;


			this.transform.GetChild (level).gameObject.SetActive (true);
			anim = this.transform.GetChild(level).GetComponent<Animator> ();
			this.transform.GetChild (5).gameObject.SetActive (true);
			break;
		}


	}
	
	// Update is called once per frame
	void Update () {

	}
		

	public void usar(){
		if (cerrado) {
			anim.Play ("Abrir");
			cerrado = false;
		} else {
			anim.Play ("Cerrar");
			cerrado = true;
		}
	}
		

}
