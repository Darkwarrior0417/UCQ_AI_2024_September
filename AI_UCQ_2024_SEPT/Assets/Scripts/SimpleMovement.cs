using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DebugConfigManager;

public enum WeaponType
{
    Weak,
    Normal,
    Hielo,
    Fuego,
    Trueno
}

public enum EnemyState
{
    Idle,
    Alert, // sospechoso/investigando/etc.
    Attack // ya te detectó y te ataca.
}

// On/Off son dos posibilidades únicamente
// true/false son dos posibilidades únicamente
// cliente/servidor
// para todas esas situaciones donde solo hay dos posibilidades, nos basta con un solo bit.
// cuántos bits tiene un booleano? 8 bits, que son un byte.
// 8 bits almacenan hasta 256 posibilidades (2^8)
// qué pasa con los otros 7 bits que no usamos para true/false?
// una posibilidad es meter otros true/false en los bits restantes.
// por ejemplo, el primer bit podría ser, OrigenDelMensaje (o cliente o servidor)
// otro podría ser: JugadorPremium (1 si sí es, 0 si no es)

public enum ExampleMessageBits
{
    ClientOrServer = 1, // [0 0 0 0 0 0 0 X ]
    PremiumUser = 2, // [0 0 0 0 0 0 X 0 ]
    EstáVolando = 4, // [0 0 0 0 0 X 0 0 ]
    OtraCosa = 8, // [0 0 0 0 X 0 0 0 ]
    OtraMas = 16,
}

public enum Layers
{
    Default = 1,
    TransparentFX = 2,
}

// Armas[5] = {Weak, Normal, Hielo, Fuego, Trueno}
// Armas[0]
// Armas[WeaponType.Normal]

public class SimpleMovement : MonoBehaviour
{
    [SerializeField]
    private int ContadorDeCuadros = 0;

    [SerializeField]
    protected float TiempoTranscurrido = 5;

    [SerializeField]
    protected float MaxSpeed = 5;

    // Queremos que nuestro agente tenga una posición en el espacio, y que tenga una velocidad actual a la que se está moviendo.
    // La variable que nos dice en qué posición en el espacio está el dueño de este script es: transform.position

    // Si ustedes quieren la posición de Otro gameObject que no sea el dueño de este script, también la accederían a través de 
    // transform.position, pero de ese gameObject en específico.
    // Por ejemplo, la posición del gameObject PiernaDerecha, tendrían que tener una referencia (una variable) a ese gameObject
    // y de ahí, acceder a la variable de posición así: PiernaDerecha.transform.position.

    // La velocidad actual a la que se está moviendo debe estar guardada en una variable. Es lo mismo que CurrentSpeed.
    public Vector3 Velocity = Vector3.zero;

    // Para manejar la aceleración, necesitamos otra variable, una que nos diga cuál es su máxima aceleración
    [SerializeField]
    protected float MaxAcceleration = 1.0f;

    // Qué tanto tiempo a futuro (o pasado, si es negativa) va a predecir el movimiento de su target.
    protected float PursuitTimePrediction = 1.0f;

    // Necesitamos saber la posición de la "cosa de interés" a la cual nos queremos acercar o alejar.
    public GameObject targetGameObject = null;

    [SerializeField]
    protected float ObstacleForceToApply = 1.0f;

    [SerializeField]
    private WeaponType myWeaponType;

    protected EnemyState currentEnemyState = EnemyState.Idle;

    protected Vector3 ExternalForces = Vector3.zero;

    [SerializeField] protected String ObstacleLayerName = "Obstacle";

    private Vector3 previousPosition; // Agregada para el cálculo de velocidad

    public void AddExternalForce(Vector3 ExternalForce)
    {
        ExternalForces += ExternalForce;
    }

    public Vector3 PuntaMenosCola(Vector3 Punta, Vector3 Cola)
    {
        float X = Punta.x - Cola.x;
        float Y = Punta.y - Cola.y;
        float Z = Punta.z - Cola.z;

        return new Vector3(X, Y, Z);
    }

    // Start is called before the first frame update
    // El orden de cuál Start se ejecuta primero puede variar de ejecución a ejecución.
    protected void Start()
    {
        Debug.Log("Se está ejecutando Start. " + gameObject.name);
        previousPosition = targetGameObject.transform.position; // Inicializa previousPosition
    }

    // Aquí, other va a ser el obstáculo, no el agente.
    void OnTriggerStay(Collider other)
    {
        // Si esta colisión es contra alguien que NO es un obstáculo (es decir, no está en la Layer de Obstacle),
        // entonces, no hagas nada.
        if (other.gameObject.layer != LayerMask.NameToLayer(ObstacleLayerName))
        {
            return;
        }

        // Si detectamos que un agente está dentro de nuestro radio/área de activación (en este caso es nuestro trigger),
        // calculamos un vector con origen en la posición de este objeto, y cuyo fin es la posición de ese agente
        // NOTA: Esta resta es hacia el CENTRO del agente, por lo que sí puede llegar a ser más grande que el radius del collider.
        Vector3 OriginToAgent = transform.position - other.transform.position;

        // Queremos que entre más cerca esté el agente de este obstáculo, más grande sea la fuerza que se aplica.
        // entre más chica sea la distancia entre estos dos objetos, con relación al radio del trigger, mayor 
        // será la fuerza aplicada.

        float distance = OriginToAgent.magnitude;

        SphereCollider collider = GetComponent<SphereCollider>();
        if (collider == null)
        {
            return;
        }

        float obstacleColliderRadius = collider.radius;

        float calculatedForce = ObstacleForceToApply * (1.0f - distance / obstacleColliderRadius);

        AddExternalForce(OriginToAgent.normalized * calculatedForce);
    }

    // Update is called once per frame
    void Update()
    {
        switch (myWeaponType)
        {
            case WeaponType.Weak:
                // Atacar con el arma weak
                break;
            case WeaponType.Normal:
                // atacar con el arma Normal.
                break;
        }

        // Verifica que el targetGameObject no sea nulo
        if (targetGameObject == null)
        {
            return; // Si es nulo, no hace nada
        }

        // Calcula la velocidad actual
        Vector3 currentVelocity = (targetGameObject.transform.position - previousPosition) / Time.deltaTime;
        previousPosition = targetGameObject.transform.position; // Actualiza previousPosition

        PursuitTimePrediction = CalculatePredictedTime(MaxSpeed, transform.position, targetGameObject.transform.position);

        // Primero predigo dónde va a estar mi objetivo
        Vector3 PredictedPosition =
            PredictPosition(targetGameObject.transform.position, currentVelocity, PursuitTimePrediction);

        // Hago seek hacia la posición predicha.
        Vector3 PosToTarget = PuntaMenosCola(PredictedPosition, transform.position); // SEEK

        PosToTarget += ExternalForces;

        Velocity += PosToTarget.normalized * MaxAcceleration * Time.deltaTime;

        // Limita la velocidad a MaxSpeed
        Velocity = Vector3.ClampMagnitude(Velocity, MaxSpeed);

        transform.position += Velocity * Time.deltaTime;

        // Resetea las fuerzas externas cada frame
        ExternalForces = Vector3.zero;
    }

    // Esta función predice a dónde se moverá un objeto cuya posición actual es InitialPosition, su velocidad actual es Velocity,
    // tras una cantidad de tiempo TimePrediction.
    protected Vector3 PredictPosition(Vector3 InitialPosition, Vector3 Velocity, float TimePrediction)
    {
        return InitialPosition + Velocity * TimePrediction;
    }

    protected float CalculatePredictedTime(float MaxSpeed, Vector3 InitialPosition, Vector3 TargetPosition)
    {
        float Distance = PuntaMenosCola(TargetPosition, InitialPosition).magnitude;
        return Distance / MaxSpeed;
    }

    protected Vector3 Pursuit(Vector3 TargetCurrentPosition, Vector3 TargetCurrentVelocity)
    {
        PursuitTimePrediction = CalculatePredictedTime(MaxSpeed, transform.position, TargetCurrentPosition);
        Vector3 PredictedPosition = PredictPosition(TargetCurrentPosition, TargetCurrentVelocity, PursuitTimePrediction);
        return PuntaMenosCola(PredictedPosition, transform.position); // SEEK
    }

    protected Vector3 Evade(Vector3 TargetCurrentPosition, Vector3 TargetCurrentVelocity)
    {
        return -Pursuit(TargetCurrentPosition, TargetCurrentVelocity);
    }

    void FixedUpdate()
    {

    }

    void OnDrawGizmos()
    {
        if (DebugGizmoManager.VelocityLines)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, transform.position + Velocity);
        }
        // Ahora vamos con la "flecha azul" que es la dirección y magnitud hacia nuestro objetivo (la posición de nuestro objetivo).
        if (DebugGizmoManager.DesiredVectors && targetGameObject != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + (targetGameObject.transform.position - transform.position));
        }
    }
}
