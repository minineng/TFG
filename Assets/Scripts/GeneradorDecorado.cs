using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorDecorado : MonoBehaviour
{

    public GameObject conjunto1prefab;
    public GameObject conjunto2prefab;
    public GameObject escaleraPrefeb;
    public bool apagado;
    public bool derecha;
    public Edificio.listaEstilos estilo;
    public RoomController.tipo seleccion;
    public bool escalera;

    // Use this for initialization
    void Start()
    {
        Vector3 posicion = this.gameObject.transform.position;
        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);//oculto la esfera para situar el generador
        estilo = this.GetComponentInParent<RoomController>().estilo;
        Vector3 correccion = new Vector3(0, 0, 0);
        GameObject set = this.gameObject;

        if (escalera)
        {
            set = Instantiate(escaleraPrefeb, posicion, Quaternion.identity);
            //GetComponentInParent<RoomController> ().hasLadder = true;
            set.transform.Rotate(new Vector3(0, -90, 0));
            if (derecha)
                correccion = new Vector3(-17.5f, 1f, -3);
            else
            {
                Vector3 corCol;
                corCol = new Vector3(0, 0, 53); // correcion de las colisiones
                correccion = new Vector3(4f, 1f, -2);
                set.transform.GetChild(2).transform.position = new Vector3(set.transform.GetChild(2).transform.position.x + corCol.x, set.transform.GetChild(2).transform.position.y + corCol.y, set.transform.GetChild(2).transform.position.z + corCol.z);
                set.transform.GetChild(3).transform.position = new Vector3(set.transform.GetChild(3).transform.position.x + corCol.x, set.transform.GetChild(3).transform.position.y + corCol.y, set.transform.GetChild(3).transform.position.z + corCol.z);
            }
            set.transform.position = new Vector3(set.transform.position.x + correccion.x, set.transform.position.y + correccion.y, set.transform.position.z + correccion.z);

        }
        else
        {
            if (!apagado)
            {
                switch (seleccion)
                {
                    case RoomController.tipo.Banyo:
                        switch (estilo)
                        {
                            case Edificio.listaEstilos.casaNormal:// estilo de prueba
                                {
                                    GameObject conjunto = Resources.Load("Prefabs/ConjuntoCasaBanyo", typeof(GameObject)) as GameObject;

                                    set = Instantiate(conjunto, posicion, Quaternion.identity);
                                    set.transform.Rotate(new Vector3(0, -90, 0));
                                    correccion = new Vector3(-33, 15, -2.6f);
                                }
                                break;
                            case Edificio.listaEstilos.oficina:
                                {
                                    GameObject conjunto = Resources.Load("Prefabs/ConjuntoCasaBanyo", typeof(GameObject)) as GameObject;

                                    set = Instantiate(conjunto, posicion, Quaternion.identity);
                                    set.transform.Rotate(new Vector3(0, -90, 0));
                                    correccion = new Vector3(-45, 15, -2.6f);
                                }
                                break;
                        }
                        break;
                    case RoomController.tipo.Entrada:
                        switch (estilo)
                        {
                            case Edificio.listaEstilos.casaNormal:// estilo de prueba
                                {
                                    GameObject conjunto = Resources.Load("Prefabs/ConjuntoCasaEntrada", typeof(GameObject)) as GameObject;

                                    set = Instantiate(conjunto, posicion, Quaternion.identity);
                                    set.transform.Rotate(new Vector3(0, -90, 0));
                                    correccion = new Vector3(-33, 15, -2.6f);
                                }
                                break;
                            case Edificio.listaEstilos.oficina:
                                {
                                    GameObject conjunto = Resources.Load("Prefabs/ConjuntoOficinaEntrada", typeof(GameObject)) as GameObject;

                                    set = Instantiate(conjunto, posicion, Quaternion.identity);
                                    set.transform.Rotate(new Vector3(0, -90, 0));
                                    correccion = new Vector3(-45, 15, -2.6f);
                                }
                                break;
                        }
                        break;
                    case RoomController.tipo.HabitacionPrincipal:
                        switch (estilo)
                        {
                            case Edificio.listaEstilos.casaNormal:// estilo de prueba
                                {
                                    set = Instantiate(conjunto1prefab, posicion, Quaternion.identity);
                                    set.transform.Rotate(new Vector3(0, 90, 0));
                                    if (derecha)
                                        correccion = new Vector3(-8, 15f, -5.4f);
                                    else
                                        correccion = new Vector3(-19, 15f, -5.4f);
                                }
                                break;
                            case Edificio.listaEstilos.oficina:
                                {
                                    set = Instantiate(conjunto2prefab, posicion, Quaternion.identity);
                                    set.transform.Rotate(new Vector3(0, 90, 0));
                                    if (derecha)
                                        correccion = new Vector3(16, 15, -16);
                                    else
                                        correccion = new Vector3(-45, 15, -3.1f);
                                }
                                break;
                        }
                        break;
                    case RoomController.tipo.Despacho:

                        break;

                }
            }
        }
        if (!apagado || escalera)
        {
            set.GetComponent<Decoracion>().derecha = derecha;
            set.transform.position = new Vector3(set.transform.position.x + correccion.x, set.transform.position.y + correccion.y, set.transform.position.z + correccion.z);
            set.transform.SetParent(transform.parent);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
