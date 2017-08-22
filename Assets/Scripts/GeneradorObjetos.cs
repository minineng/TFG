using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneradorObjetos : MonoBehaviour
{

    private GameObject minaPrefab;
    private GameObject camaraPrefab;
    private GameObject puertaPrefab;
    private GameObject cepoPrefab;
    private GameObject paredPrefab;
    private GameObject recompensaPrefab;

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
        Recompensa
    }

    public bool camara;



    // Use this for initialization
    void Start()
    {

        this.gameObject.transform.GetChild(0).gameObject.SetActive(false);//oculto la esfera para situar el generador

        if (minaPrefab == null)
            init();

        if (!done)
            generateObjects(Seleccion);

    }

    private void init()
    {
        done = false;
        minaPrefab = Resources.Load("Prefabs/Mina") as GameObject;
        cepoPrefab = Resources.Load("Prefabs/Cepo") as GameObject;
        camaraPrefab = Resources.Load("Prefabs/Camara") as GameObject;
        puertaPrefab = Resources.Load("Prefabs/Puerta") as GameObject;
        paredPrefab = Resources.Load("Prefabs/Pared") as GameObject;
        recompensaPrefab = Resources.Load("Prefabs/Recompensa") as GameObject;
    }

    public GameObject generateObjects(tipo newObject)
    {

        if (minaPrefab == null)
            init();

        Vector3 posicion = this.gameObject.transform.position;
        GameObject objeto = this.gameObject;

        switch (newObject)
        {
            case tipo.Puerta:
                {
                    objeto = Instantiate(puertaPrefab, posicion, Quaternion.identity);
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
                    objeto = Instantiate(paredPrefab, posicion, Quaternion.identity);
                    objeto.GetComponent<ScriptPared>().level = nivel;
                    objeto.GetComponent<ScriptPared>().estilo = estilo;
                    objeto.transform.position = new Vector3(objeto.transform.position.x, objeto.transform.position.y + 30.4f, objeto.transform.position.z + 3.75F);
                    break;
                }
            case tipo.Cepo:
                {
                    objeto = Instantiate(cepoPrefab, posicion, Quaternion.identity);
                    objeto.GetComponent<cepoController>().level = nivel;
                    break;
                }
            case tipo.Mina:
                {
                    objeto = Instantiate(minaPrefab, posicion, Quaternion.identity);
                    objeto.GetComponent<ScriptMina>().level = nivel;
                    break;
                }
            case tipo.Recompensa:
                {
                    objeto = Instantiate(recompensaPrefab, new Vector3(posicion.x, posicion.y+15, posicion.z), Quaternion.identity);
                    objeto.GetComponent<ObjetoRecompensa>().tipoObjeto = ObjetoRecompensa.tipoRecompensa.documentos;
                    break;
                }
        }
        objeto.transform.SetParent(transform.parent);
        done = true;
        return objeto;

    }


    // Update is called once per frame
    void Update()
    {

    }

    public static tipo getRandomObject()
    {

        switch (Random.Range(0, 2))
        {
            case 0:
                return (tipo.Mina);
                break;
            case 1:
                return (tipo.Cepo);
                break;
            default:
                return (tipo.Mina);
                break;
        }
    }
}
