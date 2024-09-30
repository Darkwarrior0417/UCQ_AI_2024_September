using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class WanderBehavior : SimpleMovement
{

    // Qu� tan lejos debe de generar el punto de Wander al cual va a hacer seek.
    [SerializeField] private float WanderDistance = 3.0f;

    [SerializeField]
    private float ToleranceRadius = 3.0f;

    Vector3 CurrentWanderPoint = Vector3.zero;

    Vector3 GenerateWanderPoint()
    {
        // Primero, vamos a tomar una direcci�n aleatoria con respecto a nuestro personaje.
        // usando esa direcci�n, vamos a mover un punto en el espacio desde la posici�n de este personaje
        // y en esa direcci�n obtenida, por la cantidad de distancia (magnitud) que diga WanderDistance.

        // tomamos la direcci�n frontal del personaje,
        
        // y despu�s la rotamos una cantidad aleatoria de grados (desde 0 hasta 359.9f).
        float RandomDegrees = Random.value * 359.9f;
        Vector3 RotatedForward = Quaternion.AngleAxis(RandomDegrees, transform.up) * transform.forward;
        
        // Desde la posici�n de este personaje y en esa direcci�n obtenida, por la cantidad de distancia (magnitud) que diga WanderDistance.
        Vector3 WanderPosition = transform.position + RotatedForward * WanderDistance;

        return WanderPosition;
    }

    // Start is called before the first frame update
    void Start()
    {
        CurrentWanderPoint = GenerateWanderPoint();
    }

    // Update is called once per frame
    void Update()
    {
        if (Utilities.Utility.IsInsideRadius(CurrentWanderPoint, transform.position, ToleranceRadius))
        {
            CurrentWanderPoint = GenerateWanderPoint();
        }

        Vector3 OurPositionToWanderPoint = CurrentWanderPoint - transform.position ;

        Velocity += OurPositionToWanderPoint.normalized * MaxAcceleration * Time.deltaTime;

        // Queremos que lo m�s r�pido que pueda ir sea a MaxSpeed unidades por segundo. Sin importar qu� tan grande sea la
        // flecha de PosToTarget.
        // Como la magnitud y la direcci�n de un vector se pueden separar, �nicamente necesitamos limitar la magnitud para
        // que no sobrepase el valor de MaxSpeed.
        Velocity = Vector3.ClampMagnitude(Velocity, MaxSpeed);

        transform.position += Velocity * Time.deltaTime;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(CurrentWanderPoint, 2.0f);

    }
}
