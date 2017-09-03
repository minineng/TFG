using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenController : ObjetoUso
{
    private float endTime;
    private float speed;
    private float tiempoAturdimiento;

	// Use this for initialization
	void Start () {
        damage = 10;
        speed = 100;
        tiempoAturdimiento = 1.5f;

        lifeTime = 2.5f;
        endTime = Time.time + lifeTime;
	}
	
	// Update is called once per frame
	void Update () {
		if(Time.time> endTime)
        {
            //print("Soy un shuriken y me destruyo");
            Destroy(this.gameObject);
        }
	}

    public void setDirection(Vector3 direction)
    {
        Start();
        GetComponent<Rigidbody>().velocity = new Vector3(direction.x* speed, direction.y* speed, 0);


    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "RedLaser" ||  other.name == "ColisionMinaObjeto" || other.tag == "Cepo" || other.tag == "PlacaPresion" || other.tag == "Camara" )
        {
            
            if (other.name != "ColisionMinaObjeto" && other.name != "camara")
            {
                other.GetComponent<ObjetoAtaque>().desactivar(tiempoAturdimiento);
            }
            else if(other.name == "ColisionMinaObjeto")
            {
                other.GetComponentInParent<ScriptMina>().desactivar(tiempoAturdimiento);
            }
            else if (other.name == "camara")
            {
                print("hackeo la camara");
                other.transform.parent.GetComponentInParent<ScriptCamara>().desactivar(tiempoAturdimiento);
            }
            Destroy(this.gameObject);
        }
    }
}
