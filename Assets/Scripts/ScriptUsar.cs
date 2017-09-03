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

    // Update is called once per frame
    void Update()
    {
    }


    /*void OnCollisionSat(Collision collision)
	{
		print (collision.contacts.ToString());

		switch (collision.collider.tag) {
		case "Mina":
			if (Input.GetAxis ("Use") > 0)
				print ("Uso la mina");
			break;
		case "Cepo":
			if (Input.GetAxis ("Use") > 0)
				print ("Uso el cepo");
			break;


		}

	}*/




    void OnTriggerStay(Collider other)
    {
        if (other != null)
        {
            switch (other.tag)
            {
                case "Mina":
                    if (Input.GetButtonDown("Use") && other.name == "ColisionMinaObjeto" && !other.transform.parent.GetComponent<ScriptMina>().activado)
                    {
                        player.successfulHack();
                        print("Desactivo la mina");
                        player.addPoints(other.transform.parent.GetComponent<ScriptMina>().getPuntos(true));
                        other.transform.parent.GetComponent<ScriptMina>().usar();
                    }
                    break;
                case "Cepo":
                    if (Input.GetButtonDown("Use") && !other.GetComponent<cepoController>().activado)
                    {
                        player.successfulHack();
                        print("Desactivo cepo");
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
                            print("Uso la puerta");
                            other.GetComponent<ScriptPuerta>().usar();
                            //GetComponentInParent<ScriptPuerta> ().usar ();
                        }
                    }
                    break;
                case "Escaleras":
                    if (Input.GetButtonDown("Use"))
                    {

                        print("Uso la escalera");
                        //transform.GetComponentInParent<PlayerController> ().setEstado (-3);
                        //GetComponentInParent<ScriptPuerta> ().usar ();
                    }
                    break;
                case "Trampilla":
                    if (Input.GetButtonDown("Use"))
                    {
                        print("Uso la trampilla");
                        other.GetComponent<ScriptTrampilla>().usar();
                    }
                    break;
            }


        }

    }
}
