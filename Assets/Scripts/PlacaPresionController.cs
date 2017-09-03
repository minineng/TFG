using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacaPresionController : ObjetoAtaque
{

    public BoxCollider colision;
    private float tiempoHastaAlarma;
    private bool pulsando;

    // Use this for initialization
    void Start()
    {
        activado = false;
        pulsando = false;
        velocidadActivacion = 2.4f / (float)level;
        colision = GetComponent<BoxCollider>();
        damage = 0;
        ruido = 20;

    }

    private void OnTriggerEnter(Collider other)
    {
        detection(other);
    }

    private void OnTriggerStay(Collider other)
    {
        detection(other);
    }

    private void detection(Collider other)
    {
        if (other.tag == "Player")
        {
            if (!activado)
            {
                tiempoHastaAlarma = velocidadActivacion + Time.time;
                activado = true;
                anim.Play("Pisado");
            }
            else
            {
                if (tiempoHastaAlarma < Time.time && !pulsando)
                {
                    pulsando = true;
                    other.GetComponent<PlayerController>().startDetection();
                }
            }
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            activado = false;
            pulsando = false;
            anim.Play("Levantado");
        }

    }

}
