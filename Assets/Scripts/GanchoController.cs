using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GanchoController : ObjetoUso{

    public float flyingTime;
    private Vector3 finalPosition;
    private Vector3 initialPosition;

    public void launch(Vector3 destino)
    {
        initialPosition = transform.position;
        finalPosition = destino;
        lifeTime = Time.time + flyingTime;

    }

	// Use this for initialization
	void Start () {
        flyingTime = 0.25f;

    }
	
	// Update is called once per frame
	void Update () {
        if (Time.time < lifeTime)
        {
            float percentage = (Time.time - (lifeTime - flyingTime)) / flyingTime;
            transform.position = Vector3.Lerp(initialPosition, finalPosition, percentage);
        }
	}

    public bool getReady()
    {
        if (Time.time < lifeTime)
            return false;
        else
            return true;
    }
}
