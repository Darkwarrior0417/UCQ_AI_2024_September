using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    [SerializeField] private float ObstacleForceToApply = 1.0f;

    // Como tip, podemos tomar en cuenta las dimensiones del collider/trigger para multiplicar esa ObstacleForceToApply

    // Queremos que cuando detecte un agente en su trigger, que lo empuje en otra direcci�n para que lo
    // desv�e ligeramente hacia otra direcci�n.

    void OnTriggerStay(Collider other)
    {

        // Si detectamos que un agente est� dentro de nuestro radio/�rea de activaci�n (en este caso es nuestro trigger),
        // calculamos un vector con origen en la posici�n de este objeto, y cuyo fin es la posici�n de ese agente
        // NOTA: Esta resta es hacia el CENTRO del agente, por lo que s� puede llegar a ser m�s grande que el radius del collider.
        Vector3 OriginToAgent = other.transform.position - transform.position;
        // y despu�s se lo aplicamos al agente como una fuerza que afecta su steering behavior.
        SimpleMovement otherSimpleMovement = other.gameObject.GetComponent<SimpleMovement>();

        if (otherSimpleMovement == null)
        {
            // entonces no le podemos asignar fuerzas ni nada, porque es null.
            return;
        }
        else
        {
            // Queremos que entre m�s cerca est� el agente de este obst�culo, m�s grande sea la fuerza que se aplica.
            // entre m�s chica sea la distancia entre estos dos objetos, con relaci�n al radio del trigger, mayor 
            // ser� la fuerza aplicada.

            float distance = OriginToAgent.magnitude;

            SphereCollider collider = GetComponent<SphereCollider>();
            if (collider == null)
            {
                return;
            }

            // collider.radius nos da el radio en espacio local, sin embargo, nosotros lo necesitamos en espacio de mundo
            // es decir, escalado por las escalas de sus padres en la Jerarqu�a de la escena. 
            float obstacleColliderRadius = collider.radius; // * transform.lossyScale.y;

            float calculatedForce = ObstacleForceToApply * (1.0f - distance / obstacleColliderRadius);

            otherSimpleMovement.AddExternalForce(OriginToAgent.normalized * calculatedForce);
        }


        // (le podemos poner una variable para ajustar la cantidad de fuerza que se le puede aplicar al agente).

    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
