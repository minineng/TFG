using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedLaserController : ObjetoAtaque
{

    struct setLaser
    {
        public LineRenderer line;
        public Transform pos0;
        public Transform pos1;
    };

    List<setLaser> conjuntoLaser;

    private GameObject laseresMoviles;
    private BoxCollider colision;

    private Vector3 posinicial;

    private float minHeight;
    public float maxHeight;
    private float percentage;
    private int signPercentage;
    public float verticalSpeed;
    private float correcionColision;

    // Use this for initialization
    void Start()
    {
        aturdido = false;
        switch (level)
        {
            case 1:
                maxHeight = 7.15f;
                correcionColision = 3f;
                break;
            case 2:
                maxHeight = 6.9f;
                correcionColision = 4f;
                break;
            case 3:
                maxHeight = 6.7f;
                correcionColision = 5.5f;
                break;
            case 4:
                maxHeight = 6.4f;
                correcionColision = 6.8f;
                break;
            case 5:
                maxHeight = 6.2f;
                correcionColision = 8.8f;
                break;
            case 6:
                maxHeight = 5.9f;
                correcionColision = 11f;
                break;
            default:
                maxHeight = 5f;
                break;

        }
        damage = level * 5;
        puntos = level * 5;
        colision = GetComponent<BoxCollider>();

        conjuntoLaser = new List<setLaser>();

        laseresMoviles = transform.Find("ConjuntoLaseres").gameObject;
        GameObject aux = laseresMoviles.transform.GetChild(0).gameObject;

        if(verticalSpeed == 0)
        verticalSpeed = Random.Range(0.01f, 0.21f);
        minHeight = laseresMoviles.transform.position.y + 1f;
        percentage = 0;
        signPercentage = 1;

        for (int i = 1; i <= level; i++)
        {
            float dif = 3.33f;
            Instantiate(aux, new Vector3(aux.transform.position.x, aux.transform.position.y + (i * dif), aux.transform.position.z), Quaternion.identity, laseresMoviles.transform);
        }
        //print("Saco "+ transform.Find("ConjuntoLaseres").childCount+" hijos");

        float nuevoCentroY = laseresMoviles.transform.childCount * 1.4f;
        float nuevoTamY = laseresMoviles.transform.childCount * 3f;

        colision.center = new Vector3(colision.center.x, colision.center.y + nuevoCentroY, colision.center.z);
        colision.size = new Vector3(colision.size.x, colision.size.y + nuevoTamY, colision.size.z);

        for (int i = 0; i < laseresMoviles.transform.childCount; i++)
        {
            setLaser set = new setLaser();
            set.line = transform.GetChild(0).GetChild(i).GetComponent<LineRenderer>();
            set.pos0 = transform.GetChild(0).GetChild(i).GetChild(0).transform;
            set.pos1 = transform.GetChild(0).GetChild(i).GetChild(1).transform;
            conjuntoLaser.Add(set);
        }

        posinicial = transform.position;

    }

    public void setVerticalSpeed(float speed)
    {
        verticalSpeed = speed;
    }

    // Update is called once per frame
    void Update()
    {
        pausa = GetComponentInParent<RoomController>().edificio.pausado;

        if (aturdido && Time.time > timeAturdido)
            aturdido = false;

        if (!pausa && !aturdido)
        {

            laseresMoviles.transform.position = new Vector3(posinicial.x, minHeight + percentage * maxHeight, posinicial.z);
            percentage = percentage + verticalSpeed * signPercentage;
            if (percentage > maxHeight)
                signPercentage = -1;
            else if (laseresMoviles.transform.position.y < minHeight)
                signPercentage = 1;

            colision.center = new Vector3(colision.center.x, laseresMoviles.transform.localPosition.y + correcionColision, colision.center.z);
            updateLaser();
        }

        /*
        print("Soy de " + transform.parent.GetComponent<RoomController>().id + " y tengo " + conjuntoLaser.Count);

        if (conjuntoLaser == null)
            print("falla algo en " + transform.parent.GetComponent<RoomController>().id);

        for (int i = 0; i < conjuntoLaser.Count; i++)
        {
            conjuntoLaser[i].line.SetPosition(0, conjuntoLaser[i].pos0.position);
            conjuntoLaser[i].line.SetPosition(1, conjuntoLaser[i].pos1.position);
        }*/


    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !aturdido)
        {
            other.GetComponent<PlayerController>().restarVida(damage);
            if (level > 4)
                other.GetComponent<PlayerController>().startDetection();
            other.GetComponent<PlayerController>().subtractPoints(puntos);
            setAlarm = true;
        }
    }

    private void updateLaser()
    {
        for (int i = 0; i < laseresMoviles.transform.childCount; i++)
        {
            LineRenderer laser = transform.GetChild(0).GetChild(i).GetComponent<LineRenderer>();
            Vector3[] pos = new Vector3[2];
            pos[0] = laser.transform.GetChild(0).transform.position;
            pos[1] = laser.transform.GetChild(1).transform.position;
            laser.SetPositions(pos);
        }
    }

}
