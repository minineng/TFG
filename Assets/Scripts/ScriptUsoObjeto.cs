using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptUsoObjeto : MonoBehaviour {

	public bool conTrampilla;
	public bool usarObjeto;
	public Vector3 positionTP;

	// Use this for initialization
	void Start () {
		conTrampilla = false;
		usarObjeto = false;
		positionTP = new Vector3(0,0,0);
	}
	
	// Update is called once per frame
	void Update () {


		
	}

	public bool getConTrampilla(){
		return conTrampilla;
	}

	void OnTriggerStay(Collider other){
		switch (other.tag) {
		case "Trampilla":
			print ("Colisiono con la trampilla");
			if (usarObjeto && other.GetComponent<ScriptTrampilla> ().cerrado) {
				other.GetComponent<ScriptTrampilla> ().usar ();
				usarObjeto = false;
			}
			positionTP = other.transform.GetChild(0).transform.position;
			conTrampilla = true;
			break;
		default:
			conTrampilla = false;
			break;
		}
	}
}
