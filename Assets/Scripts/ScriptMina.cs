using UnityEngine;
using System.Collections;

public class ScriptMina : ObjetoAtaque {
	private SphereCollider collider;
    public Material MaterialMinaActivada;
    private Material MaterialBase;

	// Use this for initialization
	void Start () {
        aturdido = false;
		collider = GetComponent<SphereCollider>();
		GetComponent<SphereCollider>().radius = level * 7.5f;
        MaterialBase = GetComponent<MeshRenderer>().material;

        damage = (level + 1) * 20;
		if (damage > 80)
			damage = 75;

        puntos = level * 150;
		activado = false;
		velocidadActivacion = 20;
	}

    private void Update()
    {
        pausa = GetComponentInParent<RoomController>().edificio.pausado;

        if (!pausa)
        {
            if (aturdido && Time.time > timeAturdido)
                aturdido = false;
            if (aturdido || activado)
                GetComponent<MeshRenderer>().material = MaterialMinaActivada;
            else if (!activado && !aturdido)
                GetComponent<MeshRenderer>().material = MaterialBase;
        }
    }



    void OnTriggerStay(Collider other)
	{
        detection(other);
    }

	void OnTriggerEnter(Collider other)
	{
        detection(other);
    }

	void OnTriggerExit(Collider other)
	{
        detection(other);
    }

    void detection(Collider other)
    {
        if (!activado && GetComponentInParent<RoomController>().jugadorEnRoom && !aturdido && other.tag == "Player" && other.GetComponent<CharacterController>().velocity.x > velocidadActivacion && (( other.transform.position.y >= this.transform.position.y && !other.GetComponent<PlayerController>().getModoSigilo()) || (other.transform.position.y >= (this.transform.position.y -6.5f) && other.GetComponent<PlayerController>().getModoSigilo())))
        {
            print("Ibas a " + other.GetComponent<CharacterController>().velocity.x);
            other.GetComponent<PlayerController>().restarVida(damage);
            other.GetComponent<PlayerController>().successfulHack();
            other.GetComponent<PlayerController>().subtractPoints(puntos);
            usar();
            print("BOOOM");
            Destroy(gameObject);
        }
    }

	public void usar(){
		if (!activado) {
            GetComponent<MeshRenderer>().material = MaterialMinaActivada;
            activado = true;
		}
	}

}
