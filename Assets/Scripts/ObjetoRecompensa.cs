using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetoRecompensa : ClaseObjeto
{

    public BoxCollider collider;
    public tipoRecompensa tipoObjeto;
    public float rotationSpeed;
    public float heightMovement;
    private float minHeight;
    public float verticalSpeed;
    private float percentage;
    private int signPercentage;

    public enum tipoRecompensa
    {
        ninguno,
        documentos,
        piezasSecretas,
        armas
    };

    // Use this for initialization
    void Start()
    {
        collider = this.GetComponent<BoxCollider>();
        rotationSpeed = 1;
        percentage = 0;
        transform.Rotate(new Vector3(0, 0, 1), 90);
        signPercentage = 1;
        minHeight = transform.position.y;
        heightMovement = 1.5f;
        verticalSpeed = 0.02f;
        bool value = false;

        switch (tipoObjeto)
        {
            case tipoRecompensa.documentos:
                for (int i = 0; i < transform.childCount; i++)
                {
                    value = i == 0 ? true : false;
                    transform.GetChild(i).gameObject.SetActive(value);
                }
                break;
            case tipoRecompensa.armas:
                for (int i = 0; i < transform.childCount; i++)
                {
                    value = i == 1 ? true : false;
                    transform.GetChild(i).gameObject.SetActive(value);
                }
                break;
            case tipoRecompensa.piezasSecretas:
                for (int i = 0; i < transform.childCount; i++)
                {
                    value = i == 2 ? true : false;
                    transform.GetChild(i).gameObject.SetActive(value);
                }
                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(1, 0, 0), rotationSpeed);
        transform.position = new Vector3(transform.position.x, minHeight + percentage * heightMovement, transform.position.z);
        percentage = percentage + verticalSpeed * signPercentage;
        if (percentage > heightMovement || percentage < -heightMovement)
            signPercentage *= -1;
    }

    public void getTipo(int random)
    {
        int tipo;
        if (random == -1)
            tipo = Random.Range(0, 3);
        else
            tipo = random;

        switch (tipo)
        {
            case 0:
                tipoObjeto = tipoRecompensa.documentos;
                break;
            case 1:
                tipoObjeto = tipoRecompensa.piezasSecretas;
                break;
            case 2:
                tipoObjeto = tipoRecompensa.armas;
                break;
            default:
                tipoObjeto = tipoRecompensa.documentos;
                break;
        }


    }

    public void OnTriggerStay(Collider other)
    {

        if (other.tag == "Player")
        {
            other.GetComponent<PlayerController>().addReward(tipoObjeto);
            Destroy(this.gameObject);
        }
    }


}
