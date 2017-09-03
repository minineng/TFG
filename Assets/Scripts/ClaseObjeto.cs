using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClaseObjeto : MonoBehaviour {

	public int level;
	public Edificio.listaEstilos estilo;
	public bool activado;
	public bool habilitado;
	public float damage;
	public float ruido;
    public bool oculta;
    public float puntos;
    public Animator anim;
    public bool pausa;

    // Use this for initialization

    public float getPuntos(bool ataque)
    {
        if (ataque)
            return puntos / 2;
        else
            return puntos;
    }

	public void usar(){}
}
