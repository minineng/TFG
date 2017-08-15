using UnityEngine;
using System.Collections;

public class ScriptCamara : MonoBehaviour {

	public int level;
	// Use this for initialization
	void Start () {
		level = 1;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player") {
			print ("Detectado");
		}

	}

}
