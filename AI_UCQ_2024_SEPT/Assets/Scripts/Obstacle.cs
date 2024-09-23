using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    [SerializeField] private float ObstacleForceToApply = 1.0f;

    // Como tip, podemos tomar en cuenta las dimensiones del collider/trigger para multiplicar esa ObstacleForceToApply

    // Queremos que cuando detecte un agente en su trigger, que lo empuje en otra direcci�n para que lo
    // desv�e ligeramente hacia otra direcci�n.

    // Si detectamos que viene un agente,
    // calculamos un vector con origen en la posici�n de este objeto, y cuyo fin es la posici�n de ese agente
    // y despu�s se lo aplicamos al agente como una fuerza que afecta su steering behavior.
    // (le podemos poner una variable para ajustar la cantidad de fuerza que se le puede aplicar al agente).

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
