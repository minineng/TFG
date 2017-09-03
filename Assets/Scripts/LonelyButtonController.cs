using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LonelyButtonController : MonoBehaviour {

    public enum tipoBoton
    {
        dPadUp,
        dPadDown,
        dPadBothSides,
        dPadRight,
        dPadleft,
        leftJoy,
        rightJoy,
        LT,
        RT,
        LB,
        RB,
        A,
        B,
        X,
        Y,
        Start,
        Back,
    }

    private Image iconoBoton;
    private Text textoBoton;
    public tipoBoton tipo;
    public string descripcion;

    // Use this for initialization
    void Start () {
        textoBoton = transform.Find("Texto").GetComponent<Text>();
        iconoBoton = transform.Find("Icono").GetComponent<Image>();

        switch (tipo){
            case tipoBoton.A:
                iconoBoton.sprite = Resources.Load("Images/AButtonIcon", typeof(Sprite)) as Sprite;
                break;
            case tipoBoton.B:
                iconoBoton.sprite = Resources.Load("Images/BButtonIcon", typeof(Sprite)) as Sprite;
                break;
            case tipoBoton.X:
                iconoBoton.sprite = Resources.Load("Images/XButtonIcon", typeof(Sprite)) as Sprite;
                break;
            case tipoBoton.Y:
                iconoBoton.sprite = Resources.Load("Images/YButtonIcon", typeof(Sprite)) as Sprite;
                break;
            case tipoBoton.Start:
                iconoBoton.sprite = Resources.Load("Images/StartButtonIcon", typeof(Sprite)) as Sprite;
                break;
            case tipoBoton.Back:
                iconoBoton.sprite = Resources.Load("Images/BackButtonIcon", typeof(Sprite)) as Sprite;
                break;
            case tipoBoton.LB:
                iconoBoton.sprite = Resources.Load("Images/LBButtonIcon", typeof(Sprite)) as Sprite;
                break;
            case tipoBoton.RB:
                iconoBoton.sprite = Resources.Load("Images/RBButtonIcon", typeof(Sprite)) as Sprite;
                break;
            case tipoBoton.RT:
                iconoBoton.sprite = Resources.Load("Images/RTButtonIcon", typeof(Sprite)) as Sprite;
                break;
            case tipoBoton.LT:
                iconoBoton.sprite = Resources.Load("Images/LTButtonIcon", typeof(Sprite)) as Sprite;
                break;
            case tipoBoton.leftJoy:
                iconoBoton.sprite = Resources.Load("Images/LeftjoyIcon", typeof(Sprite)) as Sprite;
                break;
            case tipoBoton.rightJoy:
                iconoBoton.sprite = Resources.Load("Images/RightjoyIcon", typeof(Sprite)) as Sprite;
                break;
            case tipoBoton.dPadDown:
                iconoBoton.sprite = Resources.Load("Images/DPadDownIcon", typeof(Sprite)) as Sprite;
                break;
            case tipoBoton.dPadleft:
                iconoBoton.sprite = Resources.Load("Images/DPadleftIcon", typeof(Sprite)) as Sprite;
                break;
            case tipoBoton.dPadRight:
                iconoBoton.sprite = Resources.Load("Images/DPadRightIcon", typeof(Sprite)) as Sprite;
                break;
            case tipoBoton.dPadBothSides:
                iconoBoton.sprite = Resources.Load("Images/DPadBothSidesIcon", typeof(Sprite)) as Sprite;
                break;
            case tipoBoton.dPadUp:
                iconoBoton.sprite = Resources.Load("Images/DPadUpIcon", typeof(Sprite)) as Sprite;
                break;
        }
        textoBoton.text = descripcion;
    }

    public void setText(string texto)
    {
        descripcion = texto;
    }
	
}
