using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetoRecompensa : ClaseObjeto
{

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
        conjuntoPuntos
    };

    public struct rewardStruct
    {
        public tipoRecompensa tipo;
        public int RewardTrueLevel;
    };

    // Use this for initialization
    void Start()
    {
        rotationSpeed = 1;
        percentage = 0;
        transform.Rotate(new Vector3(1, 0, 0), 270);
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
                puntos = level * 250;
                break;
            case tipoRecompensa.piezasSecretas:
                for (int i = 0; i < transform.childCount; i++)
                {
                    value = i == 1 ? true : false;
                    transform.GetChild(i).gameObject.SetActive(value);
                    puntos = level * 350;
                }
                break;
            case tipoRecompensa.conjuntoPuntos:
                for (int i = 0; i < transform.childCount; i++)
                {
                    value = i == 2 ? true : false;
                    transform.GetChild(i).gameObject.SetActive(value);
                }

                int auxLevel = Random.Range(1, level + 1);
                print("Soy nivel "+level+ " y saco un "+auxLevel);
                int son;
                if (auxLevel < 3)
                {
                    puntos = 500;
                    son = 0;
                }
                else if (auxLevel >= 3 && auxLevel < 5)
                {
                    puntos = 1000;
                    son = 1;
                }
                else
                {
                    puntos = 2000;
                    son = 2;
                }
                for (int i = 0; i < transform.Find("Puntos").childCount; i++)
                {
                    value = i == son ? true : false;
                    transform.Find("Puntos").GetChild(i).gameObject.SetActive(value);
                }

                break;
        }

    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(new Vector3(0, 0, 1), rotationSpeed);
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
                tipoObjeto = tipoRecompensa.conjuntoPuntos;
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
            other.GetComponent<PlayerController>().addPoints(puntos);
            Destroy(this.gameObject);
        }
    }


}
