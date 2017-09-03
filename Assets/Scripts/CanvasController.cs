using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasController : MonoBehaviour
{

    private RectTransform auxRect;
    private Sprite SpriteNada;
    private Sprite SpriteGancho;
    private Sprite[] SpriteShuriken;
    private Sprite tarjetaDocumentos;
    private Text DetectedText;
    private bool detected;
    private Sprite tarjetaTrampasRestantes;
    private Sprite tarjetaPiezas;
    public int cantShurikens;
    private Image SpriteObjetoEquipado;
    public int objetoEquipado;
    private bool hasDocuments;
    private bool hasSecretParts;
    private Image primeraRecompensa;
    private Image segundaRecompensa;
    private Text timeText;
    private Text puntosText;
    private Text trapsRemainingText;
    private PlayerController player;
    private int trapCount;
    private List<Edificio.condicionesVictoria> condicionesVictoria;
    private bool ready;



    // Use this for initialization
    void Start()
    {
        if (!ready)
            init();


    }
    public void init()
    {

        trapCount = -1;
        auxRect = transform.Find("CanvasElementosBasicos").Find("Barra de Vida").Find("BarraVida").GetComponent<RectTransform>();
        detected = false;
        DetectedText = transform.Find("DetectedText").GetComponent<Text>();

        tarjetaDocumentos = Resources.Load("Images/TarjetaDocumentos", typeof(Sprite)) as Sprite;
        tarjetaTrampasRestantes = Resources.Load("Images/TarjetaNumeroTrampas", typeof(Sprite)) as Sprite;
        tarjetaPiezas = Resources.Load("Images/TarjetaPiezas", typeof(Sprite)) as Sprite;
        primeraRecompensa = transform.Find("CanvasRecompensasMision").Find("Recompensa 1").GetComponent<Image>();
        segundaRecompensa = transform.Find("CanvasRecompensasMision").Find("Recompensa 2").GetComponent<Image>();
        objetoEquipado = 0;
        SpriteNada = Resources.Load("Images/NadaIcono", typeof(Sprite)) as Sprite;
        SpriteGancho = Resources.Load("Images/GanchoIcono", typeof(Sprite)) as Sprite;
        SpriteObjetoEquipado = transform.Find("CanvasElementosBasicos").Find("ObjEquipado").GetComponent<Image>();
        SpriteObjetoEquipado.sprite = SpriteNada;
        SpriteShuriken = new Sprite[3];
        SpriteShuriken[0] = Resources.Load("Images/ShurikenInactivoIcono", typeof(Sprite)) as Sprite;
        SpriteShuriken[1] = Resources.Load("Images/ShurikenIcono", typeof(Sprite)) as Sprite;
        SpriteShuriken[2] = Resources.Load("Images/ShurikenDobleIcono", typeof(Sprite)) as Sprite;
        cantShurikens = 2;
        timeText = transform.Find("CanvasRecompensasMision").Find("TimeText").GetComponent<Text>();
        puntosText = transform.Find("CanvasRecompensasMision").Find("PointsText").GetComponent<Text>();
        trapsRemainingText = primeraRecompensa.transform.Find("TrapsRemainingText").GetComponent<Text>();
        updatePoints(0);
        if (condicionesVictoria == null)
            condicionesVictoria = new List<Edificio.condicionesVictoria>();
        ready = true;
    }

    public void updateDetected(bool var)
    {
        if (var)
            DetectedText.gameObject.SetActive(true);
        else
            DetectedText.gameObject.SetActive(false);

    }

    public void addCondicionVictoria(List<Edificio.condicionesVictoria> condiciones)
    {
        if (!ready)
            init();

        condicionesVictoria = condiciones;
        if (condiciones.Contains(Edificio.condicionesVictoria.desactivarTrampas))
        {
            primeraRecompensa.sprite = tarjetaTrampasRestantes;
            primeraRecompensa.gameObject.SetActive(true);
            primeraRecompensa.transform.Find("TrapsRemainingText").gameObject.SetActive(true);

            trapCount = 0;
        }
    }

    public void addReward(ObjetoRecompensa.tipoRecompensa reward)
    {
        if (reward != ObjetoRecompensa.tipoRecompensa.conjuntoPuntos)
        {
            Sprite auxSprite = new Sprite();
            switch (reward)
            {
                case ObjetoRecompensa.tipoRecompensa.documentos:
                    auxSprite = tarjetaDocumentos;
                    break;
                case ObjetoRecompensa.tipoRecompensa.piezasSecretas:
                    auxSprite = tarjetaPiezas;
                    break;
            }

            if (!primeraRecompensa.gameObject.activeSelf)
            {

                primeraRecompensa.sprite = auxSprite;
                primeraRecompensa.gameObject.SetActive(true);
            }
            else
            {
                segundaRecompensa.sprite = auxSprite;
                segundaRecompensa.gameObject.SetActive(true);
            }
        }
    }

    public void updateCantShurikens(int number)
    {
        if (number != cantShurikens)
        {
            cantShurikens = number;
            changeObjetoEquipado(2);
        }
    }

    public void updateLifeBar(float percentage)
    {
        auxRect.sizeDelta = new Vector2(160 * (percentage), auxRect.rect.height);
    }

    public void changeObjetoEquipado(int objeto)
    {
        switch (objeto)
        {
            case 0:
                print("No llevo nada");
                SpriteObjetoEquipado.sprite = SpriteNada;
                break;
            case 1:
                print("LLevo el gancho");
                SpriteObjetoEquipado.sprite = SpriteGancho;
                break;
            case 2:
                print("LLevo el shuriken");
                SpriteObjetoEquipado.sprite = SpriteShuriken[cantShurikens];
                break;
        }
    }

    public void updatePoints(float points)
    {
        puntosText.text = points + " puntos";
    }

    public void updateTrapCount(int number)
    {
        trapsRemainingText.text = "" + number;
    }

    public void updateTime(float time)
    {
        int minutos, segundos;
        segundos = (int)time;
        minutos = segundos / 60;
        if (minutos > 0)
            segundos = segundos - (minutos * 60);

        string timeString = "Time ";

        if (minutos < 10)
            timeString += "0" + minutos;
        else
            timeString += minutos;

        if (segundos < 10)
            timeString += ":0" + segundos;
        else
            timeString += ":" + segundos;

        timeText.text = timeString;






    }
}
