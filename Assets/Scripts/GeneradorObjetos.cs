using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorObjetos : MonoBehaviour
{

    public int nivel;
    public RoomController.listaEstilos estilo;
    public tipo Seleccion;
    public bool done;

    public enum tipo
    {
        Puerta,
        Pared,
        Mina,
        Cepo,
        PlacaPresion,
        RedLaser,
        Recompensa,
        SitioOculto
    }

    public bool camara;

    // Use this for initialization
    void Start()
    {

        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);//oculto la esfera para situar el generador
        if (!done)
            generateObjects(Seleccion);

    }

    public GameObject generateObjects(tipo newObject)
    {
        Vector3 posicion = this.gameObject.transform.position;
        GameObject objeto = this.gameObject;

        switch (newObject)
        {
            case tipo.Puerta:
                {
                    GameObject aux = Resources.Load("Prefabs/Puerta") as GameObject;

                    objeto = Instantiate(aux, posicion, Quaternion.identity);
                    objeto.GetComponent<ScriptPuerta>().level = nivel;
                    switch (transform.name)
                    {
                        case "Gen ParteDer":
                            objeto.transform.name = "Puerta Derecha";
                            break;
                        case "Gen ParteIzq":
                            objeto.transform.name = "Puerta Izquierda";
                            break;
                    }
                    objeto.GetComponent<ScriptPuerta>().estilo = estilo;
                    objeto.transform.position = new Vector3(objeto.transform.position.x, objeto.transform.position.y + 31, objeto.transform.position.z + 3.15F);
                    break;
                }
            case tipo.Pared:
                {
                    GameObject aux = Resources.Load("Prefabs/Pared") as GameObject;
                    objeto = Instantiate(aux, posicion, Quaternion.identity);
                    objeto.GetComponent<ScriptPared>().level = nivel;
                    objeto.GetComponent<ScriptPared>().estilo = estilo;
                    objeto.transform.position = new Vector3(objeto.transform.position.x, objeto.transform.position.y + 30.4f, objeto.transform.position.z + 3.75F);
                    break;
                }
            case tipo.Cepo:
                {
                    GameObject aux = Resources.Load("Prefabs/Cepo") as GameObject;
                    objeto = Instantiate(aux, posicion, Quaternion.identity);
                    objeto.GetComponent<cepoController>().level = nivel;
                    break;
                }
            case tipo.Mina:
                {
                    GameObject aux = Resources.Load("Prefabs/Mina") as GameObject;
                    objeto = Instantiate(aux, posicion, Quaternion.identity);
                    objeto.GetComponent<ScriptMina>().level = nivel;
                    break;
                }
            case tipo.PlacaPresion:
                {
                    GameObject aux = Resources.Load("Prefabs/PlacaPresion") as GameObject;
                    objeto = Instantiate(aux, posicion, Quaternion.identity);
                    objeto.GetComponent<PlacaPresionController>().level = nivel;
                    break;
                }
            case tipo.RedLaser:
                {
                    GameObject aux = Resources.Load("Prefabs/RedLaser") as GameObject;
                    objeto = Instantiate(aux, posicion, Quaternion.identity);
                    objeto.GetComponent<RedLaserController>().level = nivel;
                    break;
                }
            case tipo.Recompensa:
                {
                    GameObject aux = Resources.Load("Prefabs/Recompensa") as GameObject;
                    objeto = Instantiate(aux, new Vector3(posicion.x, posicion.y+15, posicion.z), Quaternion.identity);
                    objeto.GetComponent<ObjetoRecompensa>().tipoObjeto = ObjetoRecompensa.tipoRecompensa.documentos;
                    break;
                }
            case tipo.SitioOculto:
                {
                    GameObject aux = Resources.Load("Prefabs/SitioOculto") as GameObject;
                    objeto = Instantiate(aux, new Vector3(posicion.x, posicion.y-3, posicion.z), Quaternion.identity);
                    objeto.GetComponent<SitioOcultoController>().estilo = estilo;
                    break;
                }
        }
        print("Genero un "+ newObject);
        objeto.transform.SetParent(transform.parent);
        done = true;
        return objeto;

    }


    // Update is called once per frame
    void Update()
    {
        //Destroy(this.gameObject);
    }

    public static tipo getRandomObject()
    {
        switch (Random.Range(0, 4))
        {
            case 0:
                return (tipo.Mina);
                break;
            case 1:
                return (tipo.Cepo);
                break;
            case 2:
                return (tipo.PlacaPresion);
                break;
            case 3:
                return (tipo.RedLaser);
                break;
            default:
                return (tipo.Mina);
                break;
        }
    }
}
