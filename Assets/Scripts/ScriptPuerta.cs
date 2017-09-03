using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScriptPuerta : ClaseObjeto
{

    public bool cerrado;
    private BoxCollider colisionPuerta;
    public bool isExit;
    public bool hasCamera;


    // Use this for initialization
    void Start()
    {
        hasCamera = true;
        cerrado = true;
        //print ("El estilo de esta puerta es "+estilo);
        transform.localScale = new Vector3(-1, 1, -1);


        if (transform.name == "Puerta Izquierda" && GetComponentInParent<RoomController>().id == 0)
            isExit = true;
        else
            isExit = false;

        if (hasCamera)
        {
           //Vector3 position = new Vector3(transform.position)



        }

        switch (estilo)
        {
            case Edificio.listaEstilos.casaNormal:
                //print ("Puerta estilo japones");
                this.transform.GetChild(0).gameObject.SetActive(true);
                anim = this.transform.GetChild(0).GetComponent<Animator>();
                break;
            case Edificio.listaEstilos.oficina:
                //print (this.gameObject.name + " nivel: " + level);

                int aux = 0;
                switch (level)
                {
                    case 1:
                        aux = 1;
                        break;
                    case 2:
                        aux = Random.Range(1, 3);
                        break;
                    case 3:
                        aux = Random.Range(1, 4);
                        break;
                    case 4:
                        aux = Random.Range(2, 5);
                        break;
                    case 5:
                        aux = Random.Range(3, 5);
                        break;
                    case 6:
                        aux = 4;
                        break;
                    default:
                        aux = 1;
                        break;
                }
                level = aux;
                //this.transform.name = "Puerta nivel " + aux;


                this.transform.GetChild(level).gameObject.SetActive(true);
                colisionPuerta = this.transform.GetChild(level).GetChild(1).GetComponent<BoxCollider>();
                anim = this.transform.GetChild(level).GetComponent<Animator>();
                this.transform.GetChild(5).gameObject.SetActive(true);
                break;
        }


    }

    // Update is called once per frame
    void Update()
    {

    }


    public void usar()
    {
        if (!isExit)
        {
            if (cerrado)
            {
                colisionPuerta.enabled = false;
                anim.Play("Abrir");
                cerrado = false;
            }
            else
            {
                colisionPuerta.enabled = true;
                anim.Play("Cerrar");
                cerrado = true;
            }
        }
    }


}
