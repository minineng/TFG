﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decoracion : ClaseObjeto {

	public bool derecha;

	// Use this for initialization
	void Start () {
		if (!derecha) {
			this.transform.Rotate (new Vector3 (0, 180, 0));
		}
	}
	
	// Update is called once per frame
	void Update () {
	}
}
