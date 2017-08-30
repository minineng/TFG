using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{


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
    public float tiempoVueloGancho;
    Vector3 intialPosition;
    Vector3 finalPosition;
    int contGancho;

    public bool enSitioOculto;
    public bool hidden;

    public enum estadosMovimiento
    {
        parado = 0,
        moviendose = 1,
        frenando = 2,
        cambiandoDeDireccion = 3,
    };

    private estadosMovimiento movimiento;
    public bool detected;
    private float detectedTime;

    private int shurikenCount;
    private float shurikenRecoveryTime;
    private bool recoveringShuriken;
    private float TimeNextShuriken;
    private bool rolling;
    public float rollingDistance;
    private float rollingTime;
    private float rollingActualTime;
    private float rollingInitialPosition;
    private float rollingFinalPosition;


    public List<ObjetoRecompensa.tipoRecompensa> missionRewards;
    public MapGenController mapGenControl;

    public float velocidad;
    public float multiplicadorBase;
    public float multiplicadorCorrer;
    public float multiplicadorSigilo;
    public float maxSpeed;

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
    private float modoSigiloHeight;
    private float normalHeight;
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
        detected = false;
        detectedTime = -1;
        movimiento = estadosMovimiento.parado;
        if (transform.parent != null)
        {
            transform.parent.GetComponent<MapGenController>().player = this.gameObject;
            mapGenControl = transform.parent.GetComponent<MapGenController>();
        }
        hidden = false;
        enSitioOculto = false;
        gancho = transform.Find("Gadgets").Find("Gancho").gameObject;
        tiempoVueloGancho = 0.5f;
        recoveringShuriken = false;
        usandoObjeto = false;
        lanzado = false;
        pulsado = false;
        rollingDistance = 10;
        rollingTime = 1.3f;
        modoSigiloHeight = 2.53f;
        normalHeight = 3.96f;
        shurikenRecoveryTime = 1.5f;
        objetoEquipado = 0;
        distMaximaMira = 75;
        estado = 0;
        rigidBody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        velocidad = 60;
        rolling = false;
        multiplicadorBase = 1;
        multiplicadorCorrer = 2;
        multiplicadorSigilo = 0.25f;
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

        if (vida <= 0 && estado != -1) // muerte
            estado = -1;

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
                updateControl();

                if (detected && detectedTime < Time.time)
                {
                    detectedTime = -1;
                    detected = false;
                }


                break;

        }
        elementosUI();
        elementosAnim();

    }
    public void startDetection()
    {
        detected = true;
        detectedTime = Time.time + mapGenControl.timeToForgetPlayer;
    }


    private void updateControl()
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
            moveDirection *= velocidad * multiplicadorBase;

            if ((moveH > 0 && controller.velocity.x > 0) || (moveH < 0 && controller.velocity.x < 0))
                movimiento = estadosMovimiento.moviendose;
            else if ((moveH > 0 && controller.velocity.x < 0) || (moveH < 0 && controller.velocity.x > 0))
            {
                movimiento = estadosMovimiento.cambiandoDeDireccion;
            }
            else if (moveH == 0 && controller.velocity.x != 0)
                movimiento = estadosMovimiento.frenando;
            else if (moveH == 0 && controller.velocity.x == 0)
                movimiento = estadosMovimiento.parado;

            //print("Estoy "+movimiento);

        }

        if (enSitioOculto && modoSigilo)
        {
            hidden = true;
            //print("ERES INVISIBLE");
        }
        else
            hidden = false;

        moveDirection.y -= gravity * Time.deltaTime;
        controller.Move(moveDirection * Time.deltaTime);

        if (!rolling)
        {

            if (moveH < 0 && facingRight)
            {
                //facingRight = false;
                swap();
            }
            if (moveH > 0 && !facingRight)
            {
                //facingRight = true;
                swap();
            }
        }

        if (Input.GetButtonDown("Run"))
        {
            if (!modoSigilo)
            {
                multiplicadorBase = multiplicadorCorrer;
                anim.speed = 1.5f;
            }
            else
            {
                if (!rolling)
                {
                    print("EMPIEZO");
                    rolling = true;
                    rollingInitialPosition = transform.position.x;



                    if (facingRight)
                        rollingFinalPosition = rollingInitialPosition + rollingDistance;
                    else
                        rollingFinalPosition = rollingInitialPosition - rollingDistance;

                    rollingActualTime = Time.time + rollingTime;
                    anim.Play("Roll");
                }
            }
        }
        else if (Input.GetButtonUp("Run"))
        {
            if (!modoSigilo)
            {
                multiplicadorBase = 1;
                anim.speed = 1;
            }
        }

        rollingController();

        if (controller.isGrounded && Input.GetButtonDown("Jump"))
            jump();

        ganchoUso();

        if (Input.GetButtonDown("ModoSigilo") && !modoSigilo)
        {
            print("Modo sigilo: ON");
            setModoSigilo(true);
        }
        else if (Input.GetButtonDown("ModoSigilo") && modoSigilo)
        {
            print("Modo sigilo: OFF");
            setModoSigilo(false);
        }


    }

    private void rollingController()
    {
        if (rolling)
        {

            float percentage = rollingTime - (rollingActualTime - Time.time);
            percentage *= 1.5f;
            //print(rollingActualTime +" - " + Time.time + " / " + rollingTime +" -> " +percentage);
            if (rollingActualTime > Time.time)
            {
                if (!facingRight)
                    percentage *= -1;
                controller.Move(new Vector3(percentage, 0, 0));
            }
            else
            {
                rolling = false;
            }
        }

    }

    public void setModoSigilo(bool var)
    {
        modoSigilo = var;
        if (var)
        {
            multiplicadorBase = multiplicadorSigilo;
            controller.height = modoSigiloHeight;
        }
        else
        {
            multiplicadorBase = 1;
            controller.height = normalHeight;
        }

    }

    public void elementosUI()
    {
        canvas.transform.Find("CanvasElementosBasicos").Find("TextoVida").GetComponent<Text>().text = "Velocidad " + controller.velocity;

        switch (objetoEquipado)
        {
            case 0:
                canvas.transform.Find("CanvasElementosBasicos").Find("ObjEquipado").GetComponent<Image>().sprite = Resources.Load("Images/NadaIcono", typeof(Sprite)) as Sprite;
                break;
            case 1:
                canvas.transform.Find("CanvasElementosBasicos").Find("ObjEquipado").GetComponent<Image>().sprite = Resources.Load("Images/GanchoIcono", typeof(Sprite)) as Sprite;
                break;
            case 2:
                if (shurikenCount == 0)
                    canvas.transform.Find("CanvasElementosBasicos").Find("ObjEquipado").GetComponent<Image>().sprite = Resources.Load("Images/ShurikenInactivoIcono", typeof(Sprite)) as Sprite;
                else if (shurikenCount == 1)
                    canvas.transform.Find("CanvasElementosBasicos").Find("ObjEquipado").GetComponent<Image>().sprite = Resources.Load("Images/ShurikenIcono", typeof(Sprite)) as Sprite;
                else if (shurikenCount == 2)
                    canvas.transform.Find("CanvasElementosBasicos").Find("ObjEquipado").GetComponent<Image>().sprite = Resources.Load("Images/ShurikenDobleIcono", typeof(Sprite)) as Sprite;
                break;
        }

        RectTransform auxRect = canvas.transform.Find("CanvasElementosBasicos").Find("Barra de Vida").Find("BarraVida").GetComponent<RectTransform>();
        auxRect.sizeDelta = new Vector2(160 * (vida / maxVida), auxRect.rect.height);

        if (missionRewards.Contains(ObjetoRecompensa.tipoRecompensa.documentos))
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

        canvas.transform.Find("DetectedText").gameObject.SetActive(detected);

    }

    private void elementosAnim()
    {
        int aux = 0;
        switch (movimiento)
        {
            case estadosMovimiento.parado:
                aux = 0;
                break;
            case estadosMovimiento.moviendose:
                aux = 1;
                break;
            case estadosMovimiento.frenando:
                aux = 2;
                break;
            case estadosMovimiento.cambiandoDeDireccion:
                aux = 3;
                break;
        }

        anim.SetInteger("EstadoMovimiento", aux);
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
        facingRight = !facingRight;

        Transform camera = transform.Find("Camera").transform;

        Vector3 cameraPosAux = camera.position;
        Quaternion cameraRotAux = camera.rotation;

        Vector3 miraPosAux = mira.transform.position;
        Quaternion miraRotAux = mira.transform.rotation;

        this.transform.Rotate(new Vector3(0, 180, 0));

        camera.position = cameraPosAux;
        camera.rotation = cameraRotAux;

        mira.transform.position = miraPosAux;
        mira.transform.rotation = miraRotAux;

    }

    void OnTriggerEnter(Collider other)
    {
        if (other.name == "UsarAbajo" || other.name == "UsarArriba")
        {
            cambioProfundidad = true;
        }

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
            float margen = 0.15f;

            if (Mathf.Abs(MiraH) < margen && Mathf.Abs(MiraV) < margen)
                mira.transform.GetChild(0).gameObject.SetActive(false);
            else
            {
                mira.transform.GetChild(0).gameObject.SetActive(true);
                Vector3 auxPos = new Vector3(this.transform.position.x + MiraH * distMaximaMira, this.transform.position.y + 20 + MiraV * distMaximaMira, this.transform.position.z);

                transform.Find("Gadgets").transform.position = new Vector3(this.transform.position.x + MiraH, this.transform.position.y + 20 + MiraV, this.transform.position.z);
                mira.transform.position = auxPos;
                //GetComponent<LineRenderer> ().SetPosition (0, new Vector3(this.transform.position.x,this.transform.position.y+20, this.transform.position.z));
                //GetComponent<LineRenderer> ().SetPosition (1, mira.transform.position);
            }
        }
        else
        {
            mira.gameObject.SetActive(false);
        }
    }

    private void jump()
    {
        controller.Move(new Vector3(0, jumpSpeed, 0));
    }

    public void endLevel()
    {

        if (missionRewards.Count > 0)
        {
            if (mapGenControl.objetivosMision.Count == missionRewards.Count)
                print("Acabo el nivel con todo");
            else
                print("Acabo el nivel con " + missionRewards.Count + " objetivo cumplido");
        }
        else
        {
            print("No hay ningun objetivo cumplido");
        }

    }


    private void usoObjeto()
    {
        if (shurikenCount < 2 && !recoveringShuriken)
        {
            TimeNextShuriken = Time.time + shurikenRecoveryTime;
            recoveringShuriken = true;
        }
        if (recoveringShuriken && TimeNextShuriken < Time.time)
        {
            print("Recupero 1 Shuriken");
            shurikenCount++;
            recoveringShuriken = false;
        }

        switch (objetoEquipado)
        {
            case 1:
                if (Input.GetButtonDown("UseObject") && mira.GetComponent<ScriptMira>().getConTrampilla())
                {
                    gancho.SetActive(true);
                    usandoObjeto = true;

                    mira.GetComponent<ScriptMira>().usarObjeto = true;
                    setModoSigilo(true);
                    gancho.transform.position = transform.Find("Gadgets").transform.position;
                    intialPosition = this.transform.position;
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

                    if (Input.GetAxis("ApuntarHorizontal") > 0 || (Input.GetAxis("ApuntarHorizontal") == 0 && facingRight))
                        auxShuriken.GetComponent<ShurikenController>().setDirection(new Vector3(1, 0, 0));
                    else if (Input.GetAxis("ApuntarHorizontal") < 0 || (Input.GetAxis("ApuntarHorizontal") == 0 && !facingRight))
                        auxShuriken.GetComponent<ShurikenController>().setDirection(new Vector3(-1, 0, 0));
                    shurikenCount--;
                }
                break;
        }

    }

    private void ganchoUso()
    {
        if (usandoObjeto && objetoEquipado == 1)
        {
            GetComponent<LineRenderer>().SetPosition(0, new Vector3(transform.position.x, transform.position.y + 20, transform.position.z));
            GetComponent<LineRenderer>().SetPosition(1, gancho.transform.position);
            if (gancho.GetComponent<GanchoController>().getReady() && Time.time < tiempoGancho)
            {
                gancho.transform.position = finalPosition;
                float percentage = (Time.time - (tiempoGancho - tiempoVueloGancho)) / tiempoVueloGancho; ;
                transform.position = Vector3.Lerp(intialPosition, finalPosition, percentage);
            }
            else if (gancho.GetComponent<GanchoController>().getReady() && Time.time > tiempoGancho)
            {
                usandoObjeto = false;
                gancho.transform.position = transform.Find("Gadgets").transform.position;
                gancho.SetActive(false);
                GetComponent<LineRenderer>().enabled = false;
            }
        }


    }

    public void addReward(ObjetoRecompensa.tipoRecompensa recompensa)
    {
        missionRewards.Add(recompensa);
    }


}