using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class IngameMenuController : MonoBehaviour {

    public int estado;
    public GameObject CanvasPausa;
    public GameObject CanvasFinPartida;
    private Sprite tickSprite;
    private Sprite crossSprite;
    private PlayerController player;

    // Use this for initialization
    void Start () {
        estado = 0; // 0 - Jugando / 1 - Pausa / 2 - Fin partida
        CanvasPausa = transform.Find("CanvasPausa").gameObject;
        CanvasFinPartida = transform.Find("CanvasFinPartida").gameObject;
        tickSprite = Resources.Load("Images/TickImage", typeof(Sprite)) as Sprite;
        crossSprite = Resources.Load("Images/CrossImage", typeof(Sprite)) as Sprite;
        player = transform.parent.GetComponentInParent<PlayerController>();

    }
	
	// Update is called once per frame
	void Update () {
        switch (estado)
        {
            case 1:
                if (Input.GetButtonDown("Back"))
                    goToMainMenu();
                if (Input.GetButtonDown("ModoSigilo"))
                    restartLevel();
                break;
            case 2:
                if (Input.GetButtonDown("Back"))
                    goToMainMenu();
                if (Input.GetButtonDown("ModoSigilo"))
                    restartLevel();
                break;
        }


	}

    public void goToMainMenu()
    {      
        GameObject auxGO = Resources.Load("Prefabs/Main Menu", typeof(GameObject)) as GameObject;
        GameObject aux = Instantiate(auxGO);
        aux.GetComponent<MainMenuController>().Start();
        Destroy(player.Edificio.gameObject);
        /*GameObject obj = new GameObject();
        obj.name = "Menu Principal";
        obj.AddComponent<MapSwitcher>();*/
    }

    public void restartLevel()
    {
        print("Me rio por no llorar");
    }


    public void setDatosFinPartida(float vidaFinal, List<Edificio.condicionesVictoria> objetivosMision, bool primerObjetivo, bool segundoObjetivo, int trapCount, int trapCountTotal, float time, float puntos)
    {
        CanvasFinPartida.transform.Find("TimeText").GetComponent<Text>().text = formatTime(time);
        CanvasFinPartida.transform.Find("PointsText").GetComponent<Text>().text = "Ptos: "+puntos;
        CanvasFinPartida.transform.Find("SaludText").GetComponent<Text>().text = "Salud: " + vidaFinal+"%";
        CanvasFinPartida.transform.Find("TrapCountText").GetComponent<Text>().text = "Trampas " + (trapCountTotal - trapCount) + "/"+trapCountTotal;

        for(int i = 0; i < objetivosMision.Count; i++)
        {
            switch (objetivosMision[i])
            {
                case Edificio.condicionesVictoria.conseguirDocumentos:
                    if (i == 0)
                    {
                        Transform auxTrans = CanvasFinPartida.transform.Find("PrimerObjetivo");
                        auxTrans.GetComponent<Text>().text = "Documentos";
                        if (primerObjetivo)
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = tickSprite;
                        else
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = crossSprite;
                    }
                    else
                    {
                        Transform auxTrans = CanvasFinPartida.transform.Find("SegundoObjetivo");
                        auxTrans.GetComponent<Text>().text = "Documentos";
                        if (segundoObjetivo)
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = tickSprite;
                        else
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = crossSprite;
                    }
                    break;
                case Edificio.condicionesVictoria.conseguirPiezas:
                    if (i == 0)
                    {
                        Transform auxTrans = CanvasFinPartida.transform.Find("PrimerObjetivo");
                        auxTrans.GetComponent<Text>().text = "Piezas";
                        if (primerObjetivo)
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = tickSprite;
                        else
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = crossSprite;
                    }
                    else
                    {
                        Transform auxTrans = CanvasFinPartida.transform.Find("SegundoObjetivo");
                        auxTrans.GetComponent<Text>().text = "Piezas";
                        if (segundoObjetivo)
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = tickSprite;
                        else
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = crossSprite;
                    }
                    break;
                case Edificio.condicionesVictoria.desactivarTrampas:
                    if (i == 0)
                    {
                        Transform auxTrans = CanvasFinPartida.transform.Find("PrimerObjetivo");
                        auxTrans.GetComponent<Text>().text = "Trampas";
                        if (primerObjetivo)
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = tickSprite;
                        else
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = crossSprite;
                    }
                    else
                    {
                        Transform auxTrans = CanvasFinPartida.transform.Find("SegundoObjetivo");
                        auxTrans.GetComponent<Text>().text = "Trampas";
                        if (segundoObjetivo)
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = tickSprite;
                        else
                            auxTrans.GetChild(0).GetComponent<Image>().sprite = crossSprite;
                    }
                    break;
            }





        }
        

    }

    private string formatTime(float time)
    {
        int minutos, segundos;
        segundos = (int)time;
        minutos = segundos / 60;
        if (minutos > 0)
            segundos = segundos - (minutos * 60);

        string timeString = "Tiempo ";

        if (minutos < 10)
            timeString += "0" + minutos;
        else
            timeString += minutos;

        if (segundos < 10)
            timeString += ":0" + segundos;
        else
            timeString += ":" + segundos;

        return timeString;


    }

    public void setEstado(int var)
    {
        estado = var;
        switch (estado)
        {
            case 1:
                CanvasPausa.SetActive(true);
                CanvasFinPartida.SetActive(false);
                break;
            case 2:
                CanvasPausa.SetActive(false);
                CanvasFinPartida.SetActive(true);
                break;
            case 0:
                CanvasPausa.SetActive(false);
                CanvasFinPartida.SetActive(false);

                break;
        }



    }
}
