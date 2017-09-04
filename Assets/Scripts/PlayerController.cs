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
    bool levelFinished;

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
    private float rollingDistance;
    private float rollingStep;
    private float rollingTime;
    private float rollingActualTime;
    private float rollingInitialPosition;
    private float rollingFinalPosition;


    public List<ObjetoRecompensa.tipoRecompensa> missionRewards;
    public Edificio Edificio;

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
    //private GameObject canvas;
    private MainMenuController canvas;
    private GameObject mira;
    public float points;
    private float secDetected;
    private float pointsPerDetection;
    public int trapCount;
    public bool pausado;


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
        Time.timeScale = 1;
        gancho = transform.Find("Gadgets").Find("Gancho").gameObject;
        rigidBody = GetComponent<Rigidbody>();
        anim = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        shurikenPrefab = Resources.Load("Prefabs/Shuriken", typeof(GameObject)) as GameObject;
        canvas = transform.parent.GetComponentInParent<MainMenuController>();
        mira = transform.Find("PuntoMira").gameObject;

        missionRewards = new List<ObjetoRecompensa.tipoRecompensa>();

        levelFinished = false;
        points = 0;
        detected = false;
        detectedTime = -1;
        secDetected = 0;
        movimiento = estadosMovimiento.parado;
        if (transform.parent != null)
        {
            transform.parent.GetComponent<Edificio>().player = this.gameObject;
            Edificio = transform.parent.GetComponent<Edificio>();
            canvas.addCondicionVictoria(Edificio.objetivosMision);
            pointsPerDetection = Edificio.dificultad * 0.5f;

        }
        canvas.setPlayer(this.gameObject.GetComponent<PlayerController>());
        canvas.setEdificio(Edificio);
        trapCount = Edificio.cantTrampasHackeablesTotal;
        canvas.updateTrapCount(trapCount);
        hidden = false;
        enSitioOculto = false;

        tiempoVueloGancho = 0.5f;
        recoveringShuriken = false;
        usandoObjeto = false;
        lanzado = false;
        pulsado = false;
        rollingDistance = 10;
        rollingTime = 1f;
        rollingStep = 1;
        modoSigiloHeight = 2.95f;
        normalHeight = 3.61f;
        shurikenRecoveryTime = 10f;
        objetoEquipado = 0;
        distMaximaMira = 75;
        estado = 0;
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
    }

    // Update is called once per frame
    void Update()
    {
        if (!levelFinished)
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
                    endLevel();
                    break;
                case 0://normal
                    updateControl();

                    if (detected)
                    {

                        if (detectedTime < Time.time)
                        {
                            detectedTime = -1;
                            canvas.updateDetected(false);
                            detected = false;
                        }
                        else if (detectedTime > Time.time && secDetected < Time.time)
                        {
                            secDetected = Time.time + 1;
                            print("Salto");
                            subtractPoints(pointsPerDetection);
                        }

                    }

                    break;

            }
            elementosUI();
            elementosAnim();
        }
    }

    public void pausar()
    {
        if (!pausado)
        {
            pausado = true;
            canvas.setEstado(5);
            Time.timeScale = 0;
        }
        else
        {
            pausado = false;
            canvas.setEstado(4);
            Time.timeScale = 1;
        }

        Edificio.setPausa(pausado);
    }

    public void successfulHack()
    {
        trapCount--;
        canvas.updateTrapCount(trapCount);
    }

    public void startDetection()
    {
        if (!detected)
        {
            secDetected = Time.time + 1;
            detected = true;
            canvas.updateDetected(true);
            detectedTime = Time.time + Edificio.timeToForgetPlayer;
        }
    }

    private void updateControl()
    {
        if (Input.GetButtonDown("Start"))
        {
            pausar();
        }
        if (!pausado)
        {

            if (Input.GetAxis("ChangeObject") > 0 && !pulsado && !usandoObjeto)
            {
                pulsado = true;
                objetoEquipado++;
                if (objetoEquipado > 2)
                    objetoEquipado = 0;
                canvas.changeObjetoEquipado(objetoEquipado);
            }
            if (Input.GetAxis("ChangeObject") < 0 && !pulsado && !usandoObjeto)
            {
                pulsado = true;
                objetoEquipado--;
                if (objetoEquipado < 0)
                    objetoEquipado = 2;
                canvas.changeObjetoEquipado(objetoEquipado);
            }

            if (Input.GetAxis("ChangeObject") == 0)
                pulsado = false;

            controlMira();
            usoObjeto();

            float moveH = Input.GetAxis("Horizontal");
            float moveV = Input.GetAxis("Vertical");

            if (controller.isGrounded && !usandoObjeto)
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

            if (!rolling && !usandoObjeto)
            {

                if (moveH < 0 && facingRight)
                    swap();
                if (moveH > 0 && !facingRight)
                    swap();
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
                setModoSigilo(true);
            }
            else if (Input.GetButtonDown("ModoSigilo") && modoSigilo && !rolling)
            {
                setModoSigilo(false);
            }
        }

    }

    private void rollingController()
    {
        if (rolling)
        {

            if (rollingActualTime > Time.time)
            {
                float auxMov = rollingStep;
                if (!facingRight)
                    auxMov *= -1;
                controller.Move(new Vector3(auxMov, 0, 0));
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
            anim.Play("Crouch");
            multiplicadorBase = multiplicadorSigilo;
            controller.height = modoSigiloHeight;
        }
        else
        {
            if (controller.velocity.x != 0)
                anim.Play("Walk_Standing");

            multiplicadorBase = 1;
            controller.height = normalHeight;
        }

    }

    public bool getModoSigilo()
    {
        return modoSigilo;
    }

    public void elementosUI()
    {
        canvas.updateTime(Edificio.tiempoNivel);
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
        canvas.updateLifeBar(vida / maxVida);
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

    public void addPoints(float number)
    {
        points += number;
        canvas.updatePoints(points);
    }

    public void subtractPoints(float number)
    {
        points -= number;
        canvas.updatePoints(points);
    }

    private void controlMira()
    {
        if (objetoEquipado != 0)
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
        controller.Move(new Vector3(0, jumpSpeed * 2, 0));
    }

    public void endLevel()
    {
        levelFinished = true;
        pausado = true;
        Edificio.setPausa(true);
        Time.timeScale = 0;
        bool obj1, obj2;
        obj1 = obj2 = false;

        for (int i = 0; i < Edificio.objetivosMision.Count; i++)
        {
            switch (Edificio.objetivosMision[i])
            {
                case Edificio.condicionesVictoria.conseguirDocumentos:
                    if (missionRewards.Contains(ObjetoRecompensa.tipoRecompensa.documentos))
                    {
                        if (i == 0)
                            obj1 = true;
                        else
                            obj2 = true;
                    }
                    break;
                case Edificio.condicionesVictoria.conseguirPiezas:
                    if (missionRewards.Contains(ObjetoRecompensa.tipoRecompensa.piezasSecretas))
                    {
                        if (i == 0)
                            obj1 = true;
                        else
                            obj2 = true;
                    }
                    break;
                case Edificio.condicionesVictoria.desactivarTrampas:
                    if (trapCount == 0)
                    {
                        if (i == 0)
                            obj1 = true;
                        else
                            obj2 = true;
                    }
                    break;
            }
        }

        canvas.setDatosFinPartida(vida, Edificio.objetivosMision, obj1, obj2, trapCount, Edificio.cantTrampasHackeablesTotal, Edificio.tiempoNivel, points);
        canvas.setEstado(6);
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
            //print("Recupero 1 Shuriken");
            shurikenCount++;
            canvas.updateCantShurikens(shurikenCount);
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
                    float MiraH = Input.GetAxis("ApuntarHorizontal");
                    float MiraV = Input.GetAxis("ApuntarVertical");

                    auxShuriken.GetComponent<ShurikenController>().setDirection(new Vector3(MiraH, MiraV, 0));
                    shurikenCount--;
                    canvas.updateCantShurikens(shurikenCount);
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
                mira.GetComponent<ScriptMira>().usarObjeto = false;
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
        canvas.addReward(recompensa);
    }


}