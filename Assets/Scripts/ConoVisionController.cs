using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConoVisionController : MonoBehaviour
{

    public bool detectoPlayer;
    private ScriptCamara camara;
    public bool detectando;
    private float detectionTime;
    private float finishTime;


    // Use this for initialization
    void Start()
    {
        camara = transform.GetComponentInParent<ScriptCamara>();
        detectoPlayer = false;
        detectando = false;
    }

    public void setDetectionTime(float time)
    {
        if (time > 0)
            detectionTime = time;
    }

    // Update is called once per frame
    void Update()
    {
        if (detectando && Time.time > finishTime)
            detectoPlayer = true;
        else
            detectoPlayer = false;

        if(camara.aturdido && detectando)
            finishTime = Time.time + detectionTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        detection(other);
    }

    private void OnTriggerStay(Collider other)
    {
        detection(other);
    }

    public void detection(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!other.GetComponent<PlayerController>().hidden && !detectando)
            {
                detectando = true;
                finishTime = Time.time + detectionTime;
            }
            else if (other.GetComponent<PlayerController>().hidden)
                detectando = false;
            if (detectoPlayer)
                other.GetComponent<PlayerController>().startDetection();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            detectando = false;
    }

    public bool jugadorDetectado()
    {
        return detectoPlayer;
    }
}
