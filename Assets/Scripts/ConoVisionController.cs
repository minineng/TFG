using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConoVisionController : MonoBehaviour
{

    public bool detectoPlayer;
    public bool detectando;
    private float detectionTime;
    private float finishTime;


    // Use this for initialization
    void Start()
    {
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
        {
            detectoPlayer = true;
        }
        else
        {
            detectoPlayer = false;
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !other.GetComponent<PlayerController>().hidden && !detectando)
        {
            detectando = true;
            finishTime = Time.time + detectionTime;
            print("Te empiezo a pillar");
        }
        else if (other.tag == "Player" && other.GetComponent<PlayerController>().hidden)
            detectando = false;
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player" && !other.GetComponent<PlayerController>().hidden && !detectando)
        {
            detectando = true;
            finishTime = Time.time + detectionTime;
            print("Te empiezo a pillar");
        }
        else if (other.tag == "Player" && other.GetComponent<PlayerController>().hidden)
            detectando = false;

        if (other.tag == "Player" && detectoPlayer)
            other.GetComponent<PlayerController>().startDetection();
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
