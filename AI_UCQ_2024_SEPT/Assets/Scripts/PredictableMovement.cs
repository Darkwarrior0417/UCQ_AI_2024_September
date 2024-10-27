using System.Drawing;
using UnityEngine;

public class PredictableMovement : SimpleMovement
{
    [SerializeField]
    private GameObject[] PatrolPoints;

    private int CurrentPatrolPoint = 0;

    [SerializeField]
    private float PatrolPointToleranceRadius;

    [SerializeField]
    private float DetectionRadius = 10.0f;  // Radio para detectar al jugador y activar el estado de alerta

    protected SimpleMovement ObjectToEvade = null;

    [SerializeField]
    private float speed = 5.0f; // Definir speed aquí si no está en SimpleMovement

    new void Start()
    {
        base.Start(); // Asegúrate de llamar al Start de SimpleMovement
        ObjectToEvade = FindObjectOfType<SimpleMovement>();
        if (ObjectToEvade == null)
        {
            Debug.LogError("ERROR: no object to evade.");
        }
        else if (ObjectToEvade == this)
        {
            Debug.LogError("ERROR: evading itself.");
        }
        else
        {
            Debug.Log("Found object to evade.");
        }
    }

    void Update()
    {
        Vector3 PosToTarget = gameObject.transform.forward;

        if (currentEnemyState == EnemyState.Idle)
        {
            // Detectar al jugador y cambiar a estado de alerta si está dentro del radio de detección
            float distanceToPlayer = Vector3.Distance(transform.position, ObjectToEvade.transform.position);
            if (distanceToPlayer <= DetectionRadius)
            {
                currentEnemyState = EnemyState.Alert;
                Debug.Log("Jugador detectado, cambiando a estado de alerta.");
            }
            else
            {
                // Patrullar entre los puntos de patrullaje si no está en alerta
                if (Utilities.Utility.IsInsideRadius(PatrolPoints[CurrentPatrolPoint].transform.position, transform.position, PatrolPointToleranceRadius))
                {
                    CurrentPatrolPoint++;
                    if (CurrentPatrolPoint >= PatrolPoints.Length)
                    {
                        CurrentPatrolPoint = 0;
                    }
                }
                PosToTarget = PuntaMenosCola(PatrolPoints[CurrentPatrolPoint].transform.position, transform.position);
            }
        }
        else if (currentEnemyState == EnemyState.Alert)
        {
            // Evadir al ObjectToEvade si estamos en estado de alerta
            PosToTarget = Evade(ObjectToEvade.transform.position, ObjectToEvade.Velocity); // Cambia 'velocity' por 'Velocity'
        }

        // Movimiento hacia el objetivo calculado
        MoveTowardsTarget(PosToTarget);
    }

    private void MoveTowardsTarget(Vector3 targetDirection)
    {
        transform.position += targetDirection.normalized * speed * Time.deltaTime;
        transform.forward = targetDirection.normalized;
    }

    private Vector3 Evade(Vector3 targetPosition, Vector3 targetVelocity)
    {
        // Implementa la lógica de evasión aquí. Por ejemplo:
        Vector3 directionToTarget = transform.position - targetPosition;
        float distanceToTarget = directionToTarget.magnitude;

        // Calcula la dirección de evasión (ajusta la lógica según sea necesario)
        if (distanceToTarget > 0)
        {
            return directionToTarget / distanceToTarget * speed; // Ajusta la velocidad
        }
        return Vector3.zero; // Si estás en la misma posición, no te muevas
    }
}
