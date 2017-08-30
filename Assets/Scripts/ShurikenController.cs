using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShurikenController : ObjetoUso
{
    private float endTime;
    private float speed;

	// Use this for initialization
	void Start () {
        damage = 10;
        speed = 100;
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
        if(other.tag == "Enemy")
        {
            print("Te quito vida");
        }
        /*else if(other.tag != "Player" && other.tag != "Mira" && other.tag != "Untagged" && other.tag != "Recompensa")
        {
            print("Choco con "+other.tag);
            Destroy(this.gameObject);
        }*/
    }
}
