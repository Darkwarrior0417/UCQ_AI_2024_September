using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimerExample : MonoBehaviour
{
    // �C�mo hacemos una funci�n que nos diga que ya pas� X tiempo cada X tiempo?
    // por ejemplo, que cada segundo nos imprima un log que diga: pas� un segundo.

    // Las maneras rudimentarias, son dos
    // 1) es guardar y acumular el tiempo transcurrido en una variable.
    // �c�mo que el tiempo transcurrido?
    private float AccumulatedTime = 0.0f;

    [SerializeField]
    private float TimeInterval = 1.0f;

    // 2) La segunda manera rudimentaria consiste en usar un reloj de referencia, por ejemplo el reloj de la m�quina.
    // usando ese reloj, tomamos la fecha de este instante y la comparamos contra una fecha anterior que guardamos.
    // y despu�s checamos si la diferencia entre ellas es mayor o igual que el intervalo de tiempo que queremos.
    // Necesitamos una variable d�nde guardar la fecha de un instante.
    private float Date = 0.0f;


    // 3) el tercer m�todo, consiste en usar Coroutines de Unity.
    // busquen informaci�n sobre las corrutinas, son vida, son amor, les van a hacer la vida m�s f�cil.
    // pero si les dan problemas, pues tienen las dos maneras rudimentarias que vimos arriba para resolver lo del examen.
    // en cualquier caso, ya saben que pueden preguntar(me).


    // Start is called before the first frame update
    void Start()
    {
        // le decimos que guarde la fecha al inicio del juego.
        Date = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        // Time.deltaTime nos dice cu�nto tiempo transcurri� entre este cuadro/update y el anterior.
        // vamos acumulando el tiempo transcurrido que nos da time.deltatime en la variable AccumulatedTime.
        AccumulatedTime += Time.deltaTime;

        // checamos si el tiempo acumulado (AccumulatedTime) ya tiene un valor mayor o igual al de TimeInterval.
        if (AccumulatedTime >= TimeInterval)
        {
            // si s�, imprimimos un log que nos diga: pas� TimeInterval de tiempo.
            // Debug.Log("Pas� " + TimeInterval + " cantidad de tiempo. Porque el valor de AccumulatedTime es: " + AccumulatedTime);

            // �Qu� nos falta hacer aqu� para que esta condici�n solo se cumpla una vez cada TimeInterval tiempo?
            // nos falta reiniciar el tiempo acumulado para poder iniciar otra vez a medir ese intervalo de tiempo que queremos medir.
            AccumulatedTime = 0.0f;

            // otra alternativa es �nicamente restarle la cantidad contra la que se compar�.
            // AccumulatedTime -= TimeInterval;

            // cada una de estas opciones tiene sus pros y sus contras. Hay que saber evaluarlos seg�n nuestras necesidades.
        }

        // M�todo 2)
        // usando ese reloj, tomamos la fecha de este instante y la comparamos contra una fecha anterior que guardamos.
        // y despu�s checamos si la diferencia entre ellas es mayor o igual que el intervalo de tiempo que queremos.
        float CurrentDate = Time.time;
        if (CurrentDate - Date >= TimeInterval)
        {
            // Debug.Log("Pas� " + TimeInterval + " cantidad de tiempo medido por el reloj de la m�quina.");
            Date = CurrentDate; // hay que remplazar la fecha guardada por la nueva fecha en que ya se cumpli� este timer.
        }


    }
}
