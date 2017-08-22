using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptMira : MonoBehaviour {

    public bool conTrampilla;
    public bool usarObjeto;
    public Vector3 positionTrampilla;

    void Start()
    {
        conTrampilla = false;
        usarObjeto = false;
        positionTrampilla = new Vector3(0, 0, 0);
    }

    public bool getConTrampilla()
    {
        return conTrampilla;
    }

    public Vector3 getPosicionTrampilla()
    {
        return positionTrampilla;
    }

    void OnTriggerStay(Collider other)
    {
        switch (other.tag)
        {
            case "Trampilla":
                //print ("Colisiono con la trampilla");
                if (usarObjeto && other.GetComponent<ScriptTrampilla>().cerrado)
                {
                    other.GetComponent<ScriptTrampilla>().usar();
                    usarObjeto = false;
                }
                positionTrampilla = new Vector3(other.transform.GetChild(0).transform.position.x, other.transform.GetChild(0).transform.position.y - 25, other.transform.GetChild(0).transform.position.z);
                conTrampilla = true;
                break;
            default:
                conTrampilla = false;
                break;
        }
    }
}
