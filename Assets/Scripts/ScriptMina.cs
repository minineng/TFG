using UnityEngine;
using System.Collections;

public class ScriptMina : ObjetoAtaque {
	private SphereCollider collider;

	// Use this for initialization
	void Start () {
		collider = GetComponent<SphereCollider>();
		GetComponent<SphereCollider>().radius = level * 10;
		damage = (level + 1) * 20;
		if (damage > 80)
			damage = 75;
		habilitado = true;
		velocidadActivacion = 15;
	}
	
	// Update is called once per frame
	void Update () {
	}

	void OnTriggerStay(Collider other)
	{

		if (habilitado && other.tag == "Player" && other.GetComponent<CharacterController>().velocity.x > velocidadActivacion ){
			other.GetComponent<PlayerController> ().restarVida (damage);
			usar ();
			print ("BOOOM");
			Destroy (gameObject);
		}

	}

	void OnTriggerEnter(Collider other)
	{

		if (habilitado && other.tag == "Player" && other.GetComponent<CharacterController>().velocity.x > velocidadActivacion ){
			other.GetComponent<PlayerController> ().restarVida (damage);
			usar ();
			print ("BOOOM");
			Destroy (gameObject);
		}

	}

	void OnTriggerExit(Collider other)
	{

		if (habilitado && other.tag == "Player" && other.GetComponent<CharacterController>().velocity.x > velocidadActivacion ){
			other.GetComponent<PlayerController> ().restarVida (damage);
			usar ();
			print ("BOOOM");
			Destroy (gameObject);
		}

	}

	public void usar(){
		if (habilitado) {
			habilitado = false;
			print ("La mina deja de funcionar");
		}
	}

}
