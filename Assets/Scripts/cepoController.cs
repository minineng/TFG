using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cepoController : ObjetoAtaque {

	public float tiempoAturdimiento;



	// Use this for initialization
	void Start () {
		activado = false;
		habilitado = true;
		ruido = 1;
		damage = level * 10;
	}
	
	// Update is called once per frame
	void Update () {
		if (!activado) {
			anim.Play ("Normal");
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player" && !activado && habilitado) {
			activado = true;
			anim.Play ("Activado");
			other.GetComponent<PlayerController> ().Aturdido(tiempoAturdimiento);
			other.GetComponent<PlayerController> ().restarVida (damage);
		}

	}

	public void usar(){
		habilitado = false;
		anim.Play ("Activado");
	}
		
}
