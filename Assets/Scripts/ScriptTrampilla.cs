using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptTrampilla : ClaseObjeto {

	public bool cerrado;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
		cerrado = true;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void usar(){
		if (cerrado) { // abro la trampilla
			anim.Play("Abrir");
			transform.GetChild(0).GetComponent<BoxCollider> ().enabled = false;
			cerrado = false;
			print ("La abro");
		} else { // cierro la trampilla
			anim.Play("Cerrar");
			transform.GetChild(0).GetComponent<BoxCollider> ().enabled = true;
			cerrado = true;
			print ("La cierro");
		}
	}
}
