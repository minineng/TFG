using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cepoController : ObjetoAtaque
{

    public float tiempoAturdimiento;



    // Use this for initialization
    void Start()
    {
        aturdido = false;
        activado = false;
        habilitado = true;
        ruido = 1;
        damage = level * 7.5f;
        puntos = level * 50;
    }

    private void Update()
    {
        pausa = GetComponentInParent<RoomController>().edificio.pausado;

        if (aturdido && Time.time > timeAturdido)
            aturdido = false;

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player" && !activado && habilitado && !aturdido && !pausa)
        {
            activado = true;
            anim.Play("Activado");
            other.GetComponent<PlayerController>().Aturdido(tiempoAturdimiento);
            other.GetComponent<PlayerController>().restarVida(damage);
            other.GetComponent<PlayerController>().subtractPoints(puntos);
            other.GetComponent<PlayerController>().successfulHack();

        }

    }

    public void usar()
    {

        activado = true;
        anim.Play("Activado");
    }

}
