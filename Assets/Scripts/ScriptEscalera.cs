using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptEscalera : Decoracion {

	// Use this for initialization
	void Start () {
        if (!derecha)
        {
            this.transform.Rotate(new Vector3(0, 180, 0));
        }
        else
            transform.Find("Posamanos").localScale = new Vector3(-1, 1, -1);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void Usar(){

	}
}
