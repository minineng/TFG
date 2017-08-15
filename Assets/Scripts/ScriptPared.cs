using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptPared : ClaseObjeto {

	// Use this for initialization
	void Start () {

	}

	public void generateRandom(bool aux){
		if (aux) {
			int rand = Random.Range (0, transform.childCount-1);
			transform.GetChild (rand).gameObject.SetActive (true);
		}
		else
			transform.GetChild (0).gameObject.SetActive (true);

	}

	public void generateTrampilla(){

		for (int i = 0; i < transform.childCount-1; i++) {
			transform.GetChild (i).gameObject.SetActive (false);
		}
		transform.GetChild (transform.childCount-1).gameObject.SetActive (true);

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
