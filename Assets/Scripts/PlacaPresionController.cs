using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacaPresionController : ObjetoAtaque
{

    public BoxCollider colision;

    // Use this for initialization
    void Start()
    {
        colision = GetComponent<BoxCollider>();
        damage = 0;
        ruido = 20;

        if (level >= 2)
        {
            transform.GetChild(0).gameObject.SetActive(false);
            transform.GetChild(1).gameObject.SetActive(false);
        }

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            detectoJugador();
            anim.Play("Pisado");
        }

    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            detectoJugador();
            anim.Play("Levantado");
        }

    }

    private void detectoJugador()
    {
        print("Te detecto");
    }
}
