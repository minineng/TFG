using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjetoAtaque : ClaseObjeto {

	protected float velocidadActivacion;
    public bool setAlarm;
    public float timeAturdido;
    public bool aturdido;
    
    public void desactivar(float time)
    {
        timeAturdido = time + Time.time;
        aturdido = true;
    }

	void ObjetoActivado (){}

}
