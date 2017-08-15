using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

	public float velocidad;
	public float maxSpeed;
	Rigidbody rigidBody;
	CharacterController controller;
	BoxCollider colliderUsar;
    public Animator anim;
    int estado;
	//0 normal / 1 Aturdido
	float tiempoAturdido;
	float tiempoGancho;
	bool facingRight;
	bool cambioProfundidad;
	bool lanzado;
	Vector3 posFinGancho;
	int contGancho;




	public float vida;
	public float gravity;
	public float jumpSpeed;
	public int objetoEquipado;
	public float distMaximaMira;
	public bool pulsado;

	private Vector3 moveDirection;
	private bool modoSigilo;
	private GameObject canvas;
	private GameObject mira;


	public enum objetosUso{
		nada = 0,
		gancho = 1

	}


	// Use this for initialization
	void Start ()
	{
		lanzado = false;
		pulsado = false;
		objetoEquipado = 0;
		distMaximaMira = 75;
		estado = 0;
		rigidBody = GetComponent<Rigidbody> ();
        anim = GetComponent<Animator>();
		controller = GetComponent<CharacterController> ();
		velocidad = 30;
		tiempoAturdido = 0;
		vida = 100;
		facingRight = true;
		cambioProfundidad = false;
		gravity = 20.0f;
		jumpSpeed = 8.0f;
		moveDirection = Vector3.zero;
		modoSigilo = false;

		for (int i = 0; i < transform.childCount; i++) {
			switch (transform.GetChild (i).name) {
			case "Canvas":
				canvas = transform.GetChild (i).gameObject;
				break;
			case "PuntoMira":
				mira = transform.GetChild (i).gameObject;
				break;
			}

		}


	}

	// Update is called once per frame
	void Update ()
	{
		if (Input.GetAxis ("ChangeObject") > 0 && !pulsado) {
			pulsado = true;
			print ("FUNSIONA");
			objetoEquipado++;
			if (objetoEquipado > 1)
				objetoEquipado = 0;
		}

		if (Input.GetAxis ("ChangeObject") == 0)
			pulsado = false;

		controlMira ();
		usoObjeto ();

		float moveH = Input.GetAxis ("Horizontal");
		float moveV = Input.GetAxis ("Vertical");

		if (vida <= 0 && estado != -1) // muerte
			estado = -1;

		if (estado >= 0) { //vivo
			if (controller.isGrounded) {
				if (cambioProfundidad) {
					moveDirection = new Vector3 (moveH, 0, moveV);

				} else {
					moveDirection = new Vector3 (moveH, 0, 0);
				}
				moveDirection *= velocidad;

			}

			moveDirection.y -= gravity * Time.deltaTime;
			controller.Move (moveDirection * Time.deltaTime);

			if (moveH < 0 && facingRight) {
				facingRight = false;
				swap ();
			}
			if (moveH > 0 && !facingRight) {
				facingRight = true;
				swap ();
			}


		}

		if (Input.GetButtonDown ("Run") && !modoSigilo) {
			velocidad = 60;
			anim.speed = 2;
		} else if (Input.GetButtonUp ("Run") && !modoSigilo) {
			velocidad = 30;
			anim.speed = 1;
		}

		if (controller.isGrounded && Input.GetButtonDown ("Jump")) {
			//controller.Move( new Vector3(0,20f,0));

		}

		gancho();

		if (Input.GetButtonDown ("ModoSigilo") && !modoSigilo) {
			print ("Modo sigilo: ON");
			modoSigilo = true;
			velocidad = 10;
			controller.height = 2.93f;
		}
		else if (Input.GetButtonDown ("ModoSigilo") && modoSigilo) {
			print ("Modo sigilo: OFF");
			modoSigilo = false;
			velocidad = 30;
			controller.height = 3.66f;
		}

		elementosUI ();

		elementosAnim ();



			
		switch (estado) {	
		case -3://subiendo escalera
			print ("ESTOY EN EL ESTADO 3");
			break;
		case -2://aturdido
			if (Time.time > tiempoAturdido)
				estado = 0;
			break;
		case -1://muerto
			print ("Has muerto");
			break;
		case 0://normal

			break;
		/*case 1://modo sigilo
			if (Input.GetButtonDown ("ModoSigilo")) {
				estado = 0;
				print ("Modo sigilo: OFF");
				velocidad = 30;
			}
			break;*/




		}
	}

	public void elementosUI(){
		canvas.transform.GetChild(0).GetComponent<Text> ().text = "Salud: " + vida;
		switch (objetoEquipado) {
		case 0:
			canvas.transform.GetChild (1).GetComponent<Text> ().text = "Equipado: " + objetosUso.nada;
			break;
		case 1:
			canvas.transform.GetChild (1).GetComponent<Text> ().text = "Equipado: " + objetosUso.gancho;
			break;
		}
		canvas.transform.GetChild (2).transform.right = new Vector3 (7000, 0, 0);



	}

	private void usoObjeto(){


		switch (objetoEquipado) {
		case 1:
			if(Input.GetButtonDown("UseObject") && mira.GetComponent<ScriptUsoObjeto>().getConTrampilla()){
				//controller.Move( new Vector3(mira.transform.position.x,mira.transform.position.y,0));
				mira.GetComponent<ScriptUsoObjeto> ().usarObjeto = true;
				modoSigilo = true;
				velocidad = 10;
				controller.height = 2.93f;
				this.transform.position = mira.GetComponent<ScriptUsoObjeto>().positionTP;
				print ("Ahora deberia surcar los cielos");
			}
			break;
		}

	}

	private void elementosAnim(){
		anim.SetFloat ("velocidad", Mathf.Abs(controller.velocity.x));
		anim.SetBool ("sigilo", modoSigilo);

	}



	public void Aturdido (float tiempo)
	{
		estado = -2;
		tiempoAturdido = Time.time + tiempo;
		controller.Move (new Vector3 (0, 0, 0));
	}

	public void restarVida (float damage)
	{
		vida -= damage;
		print ("Has recibido " + damage + " de daño - Vida restante " + vida);

	}

	private void swap ()
	{

		Vector3 posAux = this.transform.GetChild (1).transform.position;
		Quaternion rotAux = this.transform.GetChild (1).transform.rotation;
	
		this.transform.Rotate (new Vector3 (0, 180, 0));

		this.transform.GetChild (1).transform.position = posAux;
		this.transform.GetChild (1).transform.rotation = rotAux;
	}

	void OnTriggerEnter (Collider other)
	{
		if (other.name == "UsarAbajo" || other.name == "UsarArriba") {
			cambioProfundidad = true;
		}
	}

	void OnTriggerStay (Collider other)
	{

	}

	void OnTriggerExit (Collider other)
	{
		if (other.name == "UsarAbajo" || other.name == "UsarArriba") {
			cambioProfundidad = false;
		}
	}
	public void setEstado(int est){
		estado = est;
	}

	private void controlMira(){

		float MiraH = Input.GetAxis ("ApuntarHorizontal");
		float MiraV = Input.GetAxis ("ApuntarVertical");

		Vector3 auxPos = new Vector3(this.transform.position.x + MiraH*distMaximaMira, this.transform.position.y+20+ MiraV*distMaximaMira, this.transform.position.z);

		mira.transform.position = auxPos;
		//GetComponent<LineRenderer> ().SetPosition (0, new Vector3(this.transform.position.x,this.transform.position.y+20, this.transform.position.z));
		//GetComponent<LineRenderer> ().SetPosition (1, mira.transform.position);
	}

	private void gancho(){
		float duracionGancho = 0.5f;
		if (!lanzado && Input.GetButton("Jump")) {
			Vector3 posIn = new Vector3 (this.transform.position.x, this.transform.position.y + 20, this.transform.position.z);
			posFinGancho = mira.transform.position;
			tiempoGancho = Time.time;
			lanzado = true;
			//print ("Lanzo el gancho en "+tiempoGancho+" hasta "+(tiempoGancho + duracionGancho)+" posFinal "+mira.transform.position.x+","+mira.transform.position.y);
		} else {
			if (tiempoGancho + duracionGancho > Time.time) {
				float x;
				if (transform.position.x < posFinGancho.x) {
					x = (this.transform.position.x - posFinGancho.x) * (Time.time / (tiempoGancho + duracionGancho));
				} else {
					x = (posFinGancho.x - this.transform.position.x) * (Time.time / (tiempoGancho + duracionGancho));
				}
				float y = (posFinGancho.y - transform.position.y) * (Time.time / (tiempoGancho + duracionGancho));
				//print((Time.time / (tiempoGancho + duracionGancho))+"% y el gancho va por "+x+","+y);
				GetComponent<LineRenderer> ().SetPosition (0, new Vector3 (this.transform.position.x, this.transform.position.y + 20, this.transform.position.z));

				if (transform.position.x >	 posFinGancho.x)
					GetComponent<LineRenderer> ().SetPosition (1, new Vector3 (this.transform.position.x+x, this.transform.position.y+y, transform.position.z));
				else
					GetComponent<LineRenderer> ().SetPosition (1, new Vector3 (this.transform.position.x-x, this.transform.position.y+y, transform.position.z));
				
				
			} else {
				//print ("El gancho acaba su viaje");
				lanzado = false;
			}


		}

	}


}