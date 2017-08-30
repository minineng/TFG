using UnityEngine;
using System.Collections;

public class ScriptCamara : ObjetoAtaque
{

    private ConoVisionController ConoVision;
    private Transform CuerpoCamara;
    public float maxAngleRotation;
    public float minAngleRotation;

    public bool movimiento;
    public bool apuntaDerecha;

    private float auxTime;
    private float finishTime;
    private float turningTime;
    private int angleSign;
    private float angleStep;
    private float currentAngle;
    private Vector3 rotCamaraInicial;


    // Use this for initialization
    void Start()
    {
        ConoVision = transform.Find("CamaraVision").Find("ConoVision").GetComponent<ConoVisionController>();
        CuerpoCamara = transform.Find("CamaraVision").transform;
        currentAngle = 0;
        angleSign = 1;
        angleStep = 0.2f;
        maxAngleRotation = 75;
        movimiento = true;
        setAlarm = false;
        auxTime = -1;

        if (apuntaDerecha)
            transform.Rotate(90, 180, 0);
        else
            transform.Rotate(90, 0, 0);

        switch (level)
        {
            case 1:
                velocidadActivacion = 2;
                turningTime = 2;
                break;
            case 2:
                velocidadActivacion = 1.8f;
                turningTime = 2;
                break;
            case 3:
                velocidadActivacion = 1.5f;
                turningTime = 2;
                break;
            case 4:
                velocidadActivacion = 1.2f;
                turningTime = 1.8f;
                break;
            case 5:
                velocidadActivacion = 1f;
                turningTime = 1.7f;
                break;
            case 6:
                velocidadActivacion = 0.8f;
                turningTime = 1.5f;
                break;

        }

        rotCamaraInicial = CuerpoCamara.eulerAngles;
        minAngleRotation = rotCamaraInicial.z;
        ConoVision.setDetectionTime(velocidadActivacion);
        finishTime = Time.time + turningTime;
    }

    /*
    void Update()
    {
        setAlarm = ConoVision.jugadorDetectado();

        if (!setAlarm && movimiento)
        {
            if (auxTime != -1)
            {
                finishTime = Time.time + auxTime;
                auxTime = -1;
            }

            float percentage;
            if (angleSign == -1)
                percentage = (finishTime - Time.time) / turningTime;
            else
                percentage = 1 - (finishTime - Time.time) / turningTime;

            currentAngle = Mathf.Lerp(minAngleRotation, maxAngleRotation, percentage);

            if (Time.time > finishTime)
            {
                finishTime = Time.time + turningTime;
                angleSign *= -1;
            }

            if(!apuntaDerecha)
                CuerpoCamara.rotation = new Quaternion(rotCamaraInicial.x, rotCamaraInicial.y, minAngleRotation + currentAngle, rotCamaraInicial.w);
            else
                CuerpoCamara.rotation = new Quaternion(rotCamaraInicial.x, minAngleRotation + currentAngle, rotCamaraInicial.z, rotCamaraInicial.w);
        }
        else
        {
            if(auxTime == -1)
                auxTime = finishTime - Time.time;
        }
    }*/

    void Update()
    {
        setAlarm = ConoVision.jugadorDetectado();

        if (!setAlarm && movimiento)
        {
            if (auxTime != -1)
            {
                finishTime = Time.time + auxTime;
                auxTime = -1;
            }

            float percentage;
            if (angleSign == -1)
                percentage = (finishTime - Time.time) / turningTime;
            else
                percentage = 1 - (finishTime - Time.time) / turningTime;

            currentAngle = Mathf.Lerp(minAngleRotation, maxAngleRotation, percentage);

            if (Time.time > finishTime)
            {
                finishTime = Time.time + turningTime;
                angleSign *= -1;
            }
            CuerpoCamara.eulerAngles = new Vector3(rotCamaraInicial.x, rotCamaraInicial.y, minAngleRotation + currentAngle);
        }
        else
        {
            if (auxTime == -1)
                auxTime = finishTime - Time.time;
        }
    }

}
