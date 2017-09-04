using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptUsar : MonoBehaviour
{

    private BoxCollider col;
    private PlayerController player;

    // Use this for initialization
    void Start()
    {
        col = GetComponent<BoxCollider>();
        player = transform.parent.GetComponent<PlayerController>();

    }

    void detection(Collider other)
    {
        if (other != null)
        {
            switch (other.tag)
            {
                case "Mina":
                    if (Input.GetButtonDown("Use") && other.name == "ColisionMinaObjeto" && !other.transform.parent.GetComponent<ScriptMina>().activado)
                    {
                        player.successfulHack();
                        player.addPoints(other.transform.parent.GetComponent<ScriptMina>().getPuntos(true));
                        other.transform.parent.GetComponent<ScriptMina>().usar();
                    }
                    break;
                case "Cepo":
                    if (Input.GetButtonDown("Use") && !other.GetComponent<cepoController>().activado)
                    {
                        player.successfulHack();
                        other.GetComponent<cepoController>().usar();
                        player.addPoints(other.GetComponent<ObjetoAtaque>().getPuntos(true));

                    }
                    break;
                case "Puerta":
                    if (Input.GetButtonDown("Use"))
                    {
                        if (other.GetComponent<ScriptPuerta>().isExit)
                            player.endLevel();
                        else
                        {
                            other.GetComponent<ScriptPuerta>().usar();
                            //GetComponentInParent<ScriptPuerta> ().usar ();
                        }
                    }
                    break;
                case "Trampilla":
                    if (Input.GetButtonDown("Use"))
                    {
                        
                        other.GetComponent<ScriptTrampilla>().usar();
                    }
                    break;
            }


        }
    }

    void OnTriggerEnter(Collider other)
    {
        detection(other);
    }

    void OnTriggerStay(Collider other)
    {
        detection(other);
    }

}
