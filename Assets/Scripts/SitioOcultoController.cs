using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SitioOcultoController : ClaseObjeto {


	void Start () {

        int auxRandom = Random.Range(0, transform.childCount);
        oculta = true;

        switch(estilo)
        {
            case RoomController.listaEstilos.oficina:
                transform.GetChild(auxRandom).gameObject.SetActive(true);
                break;
            default:
                transform.GetChild(auxRandom).gameObject.SetActive(true);
                break;
        }



	}

	void Update () {
		
	}

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
            other.GetComponent<PlayerController>().enSitioOculto = false;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
            other.GetComponent<PlayerController>().enSitioOculto = true;
    }
}
