using UnityEngine;
using System.Collections;

public class Prueba : MonoBehaviour {

	public float velocidad;
	public float maxSpeed;
	Rigidbody rigidBody;

	// Use this for initialization
	void Start () {
		rigidBody = GetComponent<Rigidbody>();
		velocidad = 2;
		maxSpeed = 10f;
	}
	
	// Update is called once per frame
	void Update () {
		float move = Input.GetAxis("Horizontal");
		rigidBody.velocity = new Vector2(move * maxSpeed, 0.0f);
	}



}
