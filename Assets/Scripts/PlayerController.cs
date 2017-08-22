using UnityEngine;
using System.Collections;
using System.Collections.Generic;
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
    
    bool facingRight;
    public bool cambioProfundidad;

    bool lanzado;
    float tiempoGancho;
    float tiempoVueloGancho;
    Vector3 intialPosition;
    Vector3 finalPosition;
    int contGancho;

    private int shurikenCount;
    private float shurikenRecoveryTime;
    private bool recoveringShuriken;
    private float TimeNextShuriken;


    public List<ObjetoRecompensa.tipoRecompensa> missionRewards;


    public float vida;
    public float maxVida;
    public float gravity;
    public float jumpSpeed;
    public int objetoEquipado;
    public float distMaximaMira;
    public bool pulsado;
    private bool usandoObjeto;
    private GameObject shurikenPrefab;
    private GameObject gancho;

    private Vector3 moveDirection;
    private bool modoSigilo;
    private GameObject canvas;
    private GameObject mira;


    public enum objetosUso
    {
        nada = 0,
        gancho = 1,
        shuriken = 2

    }



    //public ObjetoRecompensa.tipoRecompensa = 


    // Use this for initialization
    void Start()
    {
        gancho = transform.Find("Gadgets").Find("Gancho").gameObject;
        tiempoVueloGancho = 2;
        recoveringShuriken = false;
        usandoObjeto = false;
        lanzado = false;
        pulsado = false;
        shurikenRecoveryTime = 1.5f;
        objetoEquipado = 0;
        distMaximaMira = 75;
        estado = 0;
        rigidBody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        velocidad = 30;
        tiempoAturdido = 0;
        maxVida = 100;
        vida = maxVida;
        facingRight = true;
        cambioProfundidad = false;
        gravity = 20.0f;
        jumpSpeed = 8.0f;
        moveDirection = Vector3.zero;
        modoSigilo = false;
        shurikenCount = 2;

        missionRewards = new List<ObjetoRecompensa.tipoRecompensa>();
        shurikenPrefab = Resources.Load("Prefabs/Shuriken", typeof(GameObject)) as GameObject;

        canvas = transform.Find("Canvas").gameObject;
        mira = transform.Find("PuntoMira").gameObject;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetAxis("ChangeObject") > 0 && !pulsado && !usandoObjeto)
        {
            pulsado = true;
            objetoEquipado++;
            if (objetoEquipado > 2)
                objetoEquipado = 0;
        }
        if (Input.GetAxis("ChangeObject") < 0 && !pulsado && !usandoObjeto)
        {
            pulsado = true;
            objetoEquipado--;
            if (objetoEquipado < 0)
                objetoEquipado = 2;
        }

        if (Input.GetAxis("ChangeObject") == 0)
            pulsado = false;

        controlMira();
        usoObjeto();



        if (vida <= 0 && estado != -1) // muerte
            estado = -1;

        if (estado >= 0)
        { //vivo
            float moveH = Input.GetAxis("Horizontal");
            float moveV = Input.GetAxis("Vertical");

            if (controller.isGrounded)
            {
                if (cambioProfundidad)
                {
                    moveDirection = new Vector3(moveH, 0, moveV);

                }
                else
                {
                    moveDirection = new Vector3(moveH, 0, 0);
                }
                moveDirection *= velocidad;

            }

            moveDirection.y -= gravity * Time.deltaTime;
            controller.Move(moveDirection * Time.deltaTime);

            if (moveH < 0 && facingRight)
            {
                facingRight = false;
                swap();
            }
            if (moveH > 0 && !facingRight)
            {
                facingRight = true;
                swap();
            }


        }

        if (Input.GetButtonDown("Run") && !modoSigilo)
        {
            velocidad = 60;
            anim.speed = 2;
        }
        else if (Input.GetButtonUp("Run") && !modoSigilo)
        {
            velocidad = 30;
            anim.speed = 1;
        }

        if (controller.isGrounded && Input.GetButtonDown("Jump"))
        {
            //controller.Move( new Vector3(0,20f,0));

        }

        ganchoUso();

        if (Input.GetButtonDown("ModoSigilo") && !modoSigilo)
        {
            print("Modo sigilo: ON");
            modoSigilo = true;
            velocidad = 10;
            controller.height = 2.93f;
        }
        else if (Input.GetButtonDown("ModoSigilo") && modoSigilo)
        {
            print("Modo sigilo: OFF");
            modoSigilo = false;
            velocidad = 30;
            controller.height = 3.66f;
        }

        elementosUI();

        elementosAnim();




        switch (estado)
        {
            case -3://subiendo escalera
                print("ESTOY EN EL ESTADO 3");
                break;
            case -2://aturdido
                if (Time.time > tiempoAturdido)
                    estado = 0;
                break;
            case -1://muerto
                print("Has muerto");
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

    public void elementosUI()
    {
        canvas.transform.Find("CanvasElementosBasicos").Find("TextoVida").GetComponent<Text>().text = "Salud: " + vida;
        
        switch (objetoEquipado)
        {
            case 0:
                canvas.transform.Find("CanvasElementosBasicos").Find("ObjEquipado").GetComponent<Image>().sprite = Resources.Load("Images/NadaIcono", typeof(Sprite)) as Sprite;
                break;
            case 1:
                canvas.transform.Find("CanvasElementosBasicos").Find("ObjEquipado").GetComponent<Image>().sprite = Resources.Load("Images/GanchoIcono", typeof(Sprite)) as Sprite;
                break;
            case 2:
                if(shurikenCount==0)
                    canvas.transform.Find("CanvasElementosBasicos").Find("ObjEquipado").GetComponent<Image>().sprite = Resources.Load("Images/ShurikenInactivoIcono", typeof(Sprite)) as Sprite;
                else if(shurikenCount == 1)
                    canvas.transform.Find("CanvasElementosBasicos").Find("ObjEquipado").GetComponent<Image>().sprite = Resources.Load("Images/ShurikenIcono", typeof(Sprite)) as Sprite;
                else if (shurikenCount == 2)
                    canvas.transform.Find("CanvasElementosBasicos").Find("ObjEquipado").GetComponent<Image>().sprite = Resources.Load("Images/ShurikenDobleIcono", typeof(Sprite)) as Sprite;
                break;
        }

        RectTransform auxRect = canvas.transform.Find("CanvasElementosBasicos").Find("Barra de Vida").Find("BarraVida").GetComponent<RectTransform>();
        auxRect.sizeDelta = new Vector2( 160* (vida / maxVida), auxRect.rect.height);

        if(missionRewards.Contains(ObjetoRecompensa.tipoRecompensa.documentos))
            canvas.transform.Find("CanvasRecompensasMision").Find("Documentos").gameObject.SetActive(true);
        else
            canvas.transform.Find("CanvasRecompensasMision").Find("Documentos").gameObject.SetActive(false);

        if (missionRewards.Contains(ObjetoRecompensa.tipoRecompensa.piezasSecretas))
            canvas.transform.Find("CanvasRecompensasMision").Find("Piezas").gameObject.SetActive(true);
        else
            canvas.transform.Find("CanvasRecompensasMision").Find("Piezas").gameObject.SetActive(false);

        if (missionRewards.Contains(ObjetoRecompensa.tipoRecompensa.armas))
            canvas.transform.Find("CanvasRecompensasMision").Find("Armas").gameObject.SetActive(true);
        else
            canvas.transform.Find("CanvasRecompensasMision").Find("Armas").gameObject.SetActive(false);





    }

    private void elementosAnim()
    {
        anim.SetFloat("velocidad", Mathf.Abs(controller.velocity.x));
        anim.SetBool("sigilo", modoSigilo);

    }



    public void Aturdido(float tiempo)
    {
        estado = -2;
        tiempoAturdido = Time.time + tiempo;
        controller.Move(new Vector3(0, 0, 0));
    }

    public void restarVida(float damage)
    {
        vida -= damage;
        print("Has recibido " + damage + " de daño - Vida restante " + vida);

    }

    private void swap()
    {

        Vector3 posAux = this.transform.GetChild(1).transform.position;
        Quaternion rotAux = this.transform.GetChild(1).transform.rotation;

        this.transform.Rotate(new Vector3(0, 180, 0));

        this.transform.GetChild(1).transform.position = posAux;
        this.transform.GetChild(1).transform.rotation = rotAux;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "UsarAbajo" || other.name == "UsarArriba")
        {
            cambioProfundidad = true;
        }
    }

    void OnTriggerStay(Collider other)
    {

    }

    void OnTriggerExit(Collider other)
    {
        if (other.name == "UsarAbajo" || other.name == "UsarArriba")
        {
            cambioProfundidad = false;
        }
    }
    public void setEstado(int est)
    {
        estado = est;
    }

    private void controlMira()
    {
        if (objetoEquipado == 1)
        {
            mira.gameObject.SetActive(true);
            float MiraH = Input.GetAxis("ApuntarHorizontal");
            float MiraV = Input.GetAxis("ApuntarVertical");

            Vector3 auxPos = new Vector3(this.transform.position.x + MiraH * distMaximaMira, this.transform.position.y + 20 + MiraV * distMaximaMira, this.transform.position.z);

            transform.Find("Gadgets").transform.position = new Vector3(this.transform.position.x + MiraH , this.transform.position.y + 20 + MiraV , this.transform.position.z);



            mira.transform.position = auxPos;
            //GetComponent<LineRenderer> ().SetPosition (0, new Vector3(this.transform.position.x,this.transform.position.y+20, this.transform.position.z));
            //GetComponent<LineRenderer> ().SetPosition (1, mira.transform.position);
        }
        else
        {
            mira.gameObject.SetActive(false);
        }
    }

    private void usoObjeto()
    {
        if (shurikenCount < 2 && !recoveringShuriken)
        {
            TimeNextShuriken = Time.time + shurikenRecoveryTime;
            recoveringShuriken = true;
        }
        if(recoveringShuriken && TimeNextShuriken < Time.time)
        {
            print("Recupero 1 Shuriken");
            shurikenCount++;
            recoveringShuriken = false;
        }
        if (objetoEquipado == 1)
            gancho.SetActive(true);
        else
            gancho.SetActive(false);


        switch (objetoEquipado)
        {
            case 1:
                if (Input.GetButtonDown("UseObject") && mira.GetComponent<ScriptMira>().getConTrampilla())
                {
                    usandoObjeto = true;

                    mira.GetComponent<ScriptMira>().usarObjeto = true;
                    modoSigilo = true;
                    velocidad = 10;
                    controller.height = 2.93f;
                    gancho.transform.position = transform.Find("Gadgets").transform.position;
                    intialPosition = this.transform.position;
                    //finalPosition = mira.transform.position;
                    finalPosition = mira.GetComponent<ScriptMira>().getPosicionTrampilla();
                    gancho.GetComponent<GanchoController>().launch(finalPosition);
                    GetComponent<LineRenderer>().enabled = true;
                    GetComponent<LineRenderer>().SetPosition(0, new Vector3(transform.position.x, transform.position.y + 20, transform.position.z));
                    tiempoGancho = Time.time + tiempoVueloGancho;
                }
                break;
            case 2:
                if (Input.GetButtonDown("UseObject") && shurikenCount > 0)
                {
                    GameObject auxShuriken = Instantiate(shurikenPrefab, transform.Find("Gadgets").transform.position, Quaternion.identity);

                    if(Input.GetAxis("ApuntarHorizontal") > 0 || (Input.GetAxis("ApuntarHorizontal") == 0 && facingRight))
                        auxShuriken.GetComponent<ShurikenController>().setDirection(new Vector3(1, 0, 0));
                    else if(Input.GetAxis("ApuntarHorizontal") < 0 || (Input.GetAxis("ApuntarHorizontal") == 0 && !facingRight))
                        auxShuriken.GetComponent<ShurikenController>().setDirection(new Vector3(-1, 0, 0));
                    shurikenCount--;
                }
                break;
        }

    }

    /*
     * private void usoObjeto()
    {
        if (shurikenCount < 2 && !recoveringShuriken)
        {
            TimeNextShuriken = Time.time + shurikenRecoveryTime;
            recoveringShuriken = true;
        }
        if(recoveringShuriken && TimeNextShuriken < Time.time)
        {
            print("Recupero 1 Shuriken");
            shurikenCount++;
            recoveringShuriken = false;
        }
        if (objetoEquipado == 1)
            gancho.SetActive(true);
        else
            gancho.SetActive(false);


        switch (objetoEquipado)
        {
            case 1:
                if (Input.GetButtonDown("UseObject") && mira.GetComponent<ScriptUsoObjeto>().getConTrampilla())
                {
                    usandoObjeto = true;
                    //controller.Move( new Vector3(mira.transform.position.x,mira.transform.position.y,0));
                    mira.GetComponent<ScriptUsoObjeto>().usarObjeto = true;
                    modoSigilo = true;
                    velocidad = 10;
                    controller.height = 2.93f;
                    float fuerzaGancho = 70;
                    Vector3 newPosition = new Vector3(mira.transform.position.normalized.x * fuerzaGancho, mira.transform.position.normalized.y * fuerzaGancho, 0);
                    Debug.Log(newPosition);

                    this.GetComponent<CharacterController>().Move(newPosition);

                    //this.transform.position = mira.GetComponent<ScriptUsoObjeto>().positionTP;
                    //print("Ahora deberia surcar los cielos");
                }
                break;
            case 2:
                if (Input.GetButtonDown("UseObject") && shurikenCount > 0)
                {
                    GameObject auxShuriken = Instantiate(shurikenPrefab, transform.Find("Gadgets").transform.position, Quaternion.identity);

                    if(Input.GetAxis("ApuntarHorizontal") > 0 || (Input.GetAxis("ApuntarHorizontal") == 0 && facingRight))
                        auxShuriken.GetComponent<ShurikenController>().setDirection(new Vector3(1, 0, 0));
                    else if(Input.GetAxis("ApuntarHorizontal") < 0 || (Input.GetAxis("ApuntarHorizontal") == 0 && !facingRight))
                        auxShuriken.GetComponent<ShurikenController>().setDirection(new Vector3(-1, 0, 0));
                    shurikenCount--;
                }
                break;
        }

    }
     */

    private void ganchoUso()
    {
        if (usandoObjeto && objetoEquipado == 1 )
        {
            GetComponent<LineRenderer>().SetPosition(1, gancho.transform.position);
            if (gancho.GetComponent<GanchoController>().getReady() && Time.time< tiempoGancho)
            {
                gancho.transform.position = finalPosition;
                float percentage = (Time.time - (tiempoGancho - tiempoVueloGancho)) / tiempoVueloGancho; ;
                transform.position = Vector3.Lerp(intialPosition, finalPosition, percentage);
            }
            else if(gancho.GetComponent<GanchoController>().getReady() && Time.time > tiempoGancho)
            {
                usandoObjeto = false;
                gancho.transform.position = transform.Find("Gadgets").transform.position;
                GetComponent<LineRenderer>().enabled = false;
            }
        }


    }

    /**
     * private void gancho()
    {
        float duracionGancho = 0.5f;
        if (!usandoObjeto && Input.GetButton("Jump"))
        {
            Vector3 posIn = new Vector3(this.transform.position.x, this.transform.position.y + 20, this.transform.position.z);
            posFinGancho = mira.transform.position;
            tiempoGancho = Time.time;
            usandoObjeto = true;
            print("usandoObjeto es true");
            //print ("Lanzo el gancho en "+tiempoGancho+" hasta "+(tiempoGancho + duracionGancho)+" posFinal "+mira.transform.position.x+","+mira.transform.position.y);
        }
        else
        {
            if (tiempoGancho + duracionGancho > Time.time)
            {

                float x;
                if (transform.position.x < posFinGancho.x)
                {
                    x = (this.transform.position.x - posFinGancho.x) * (Time.time / (tiempoGancho + duracionGancho));
                }
                else
                {
                    x = (posFinGancho.x - this.transform.position.x) * (Time.time / (tiempoGancho + duracionGancho));
                }
                float y = (posFinGancho.y - transform.position.y) * (Time.time / (tiempoGancho + duracionGancho));
                //print((Time.time / (tiempoGancho + duracionGancho))+"% y el gancho va por "+x+","+y);
                GetComponent<LineRenderer>().SetPosition(0, new Vector3(this.transform.position.x, this.transform.position.y + 20, this.transform.position.z));

                if (transform.position.x > posFinGancho.x)
                    GetComponent<LineRenderer>().SetPosition(1, new Vector3(this.transform.position.x + x, this.transform.position.y + y, transform.position.z));
                else
                    GetComponent<LineRenderer>().SetPosition(1, new Vector3(this.transform.position.x - x, this.transform.position.y + y, transform.position.z));


            }
            else
            {
                print("usandoObjeto es falso");
                usandoObjeto = false;
                //print ("El gancho acaba su viaje");
            }


        }

    }
     **/

    public void addReward(ObjetoRecompensa.tipoRecompensa recompensa)
    {
        missionRewards.Add(recompensa);
    }


}