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
    Attack // ya te detect� y te ataca.
}


// On/Off son dos posibilidades �nicamente
// true/false son dos posibilidades �nicamente
// cliente/servidor
// para todas esas situaciones donde solo hay dos posibilidades, nos basta con un solo bit.

// cu�ntos bits tiene un booleano? 8 bits, que son un byte.
// 8 bits almacenan hasta 256 posibilidades (2^8)
// qu� pasa con los otros 7 bits que no usamos para true/false?
// una posibilidad es meter otros true/false en los bits restantes.

// por ejemplo, el primer bit podr�a ser, OrigenDelMensaje (o cliente o servidor)
// otro podr�a ser: JugadorPremium (1 si s� es, 0 si no es)
// 

public enum ExampleMessageBits
{
    ClientOrServer = 1, // [0 0 0 0 0 0 0 X ]
    PremiumUser = 2, // [0 0 0 0 0 0 X 0 ]
    Est�Volando = 4, // [0 0 0 0 0 X 0 0 ]
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


// Desventajas: es m�s largo de escribir, puede parecer tedioso al inicio.

// Ventajas: 
// 1) Claridad. 0 es mucho menos claro para alguien que "WeaponType.Normal"
// 2) Exclusividad. No te deja asignar valores num�ricos directamente a una variable de tipo enum (a menos que hayas puesto la conversi�n
//  expl�cita, que casi nunca lo deber�an de hacer ni necesitar).
// 3) Poder separar claramente distintas opciones de ejecuci�n de un programa. (por ejemplo, lo del EnemyState mostrado arriba).

public class SimpleMovement : MonoBehaviour
{
    [SerializeField]
    private int ContadorDeCuadros = 0;

    [SerializeField]
    protected float TiempoTranscurrido = 5;

    [SerializeField]
    protected float MaxSpeed = 5;

    // Queremos que nuestro agente tenga una posici�n en el espacio, y que tenga una velocidad actual a la que se est� moviendo.
    // La variable que nos dice en qu� posici�n en el espacio est� el due�o de este script es: transform.position

    // Si ustedes quieren la posici�n de Otro gameObject que no sea el due�o de este script, tambi�n la acceder�an a trav�s de 
    // transform.position, pero de ese gameObject en espec�fico.
    // Por ejemplo, la posici�n del gameObject PiernaDerecha, tendr�an que tener una referencia (una variable) a ese gameObject
    // y de ah�, acceder a la variable de posici�n as�: PiernaDerecha.transform.position.
    

    // La velocidad actual a la que se est� moviendo debe estar guardada en una variable. Es lo mismo que CurrentSpeed.
    public Vector3 Velocity = Vector3.zero;

    // Para manejar la aceleraci�n, necesitamos otra variable, una que nos diga cu�l es su m�xima aceleraci�n
    [SerializeField]
    protected float MaxAcceleration = 1.0f;

    // Qu� tanto tiempo a futuro (o pasado, si es negativa) va a predecir el movimiento de su target.
    protected float PursuitTimePrediction = 1.0f;

    // Necesitamos saber la posici�n de la "cosa de inter�s" a la cual nos queremos acercar o alejar.
    public GameObject targetGameObject = null;

    [SerializeField]
    protected float ObstacleForceToApply = 1.0f;

    [SerializeField]
    private WeaponType myWeaponType;

    protected EnemyState currentEnemyState = EnemyState.Idle;

    protected Vector3 ExternalForces = Vector3.zero;

    [SerializeField] protected String ObstacleLayerName = "Obstacle";

    public void AddExternalForce(Vector3 ExternalForce)
    {
        ExternalForces += ExternalForce;
    }

    // Queremos poder preguntarle al DebugConfigManager si ciertas banderas de debug est�n activadas.
    // para ello, pues necesitamos tener una referencia al DebugConfigManager.
    // protected DebugConfigManager debugConfigManagerRef = null;

    //void Awake()
    //{
    //}

    public Vector3 PuntaMenosCola(Vector3 Punta, Vector3 Cola)
    {
        float X = Punta.x - Cola.x;
        float Y = Punta.y - Cola.y;
        float Z = Punta.z - Cola.z;

        return new Vector3(X, Y, Z);

        // return Punta - Cola; // es lo mismo pero ya con las bibliotecas de Unity.
    }


    // Start is called before the first frame update
    // El orden de cu�l Start se ejecuta primero puede variar de ejecuci�n a ejecuci�n.
    protected void Start()
    {
        Debug.Log("Se est� ejecutando Start. " + gameObject.name);

        // myWeaponType = 1;  // No te deja, para mantener esa exclusividad de los enum.

        // debugConfigManagerRef = GameObject.FindAnyObjectByType<DebugConfigManager>();
        return;
    }

    // Aqu�, other va a ser el obst�culo, no el agente.
    void OnTriggerStay(Collider other)
    {

        // Si esta colisi�n es contra alguien que NO es un obst�culo (es decir, no est� en la Layer de Obstacle),
        // entonces, no hagas nada.
        if (other.gameObject.layer != LayerMask.NameToLayer(ObstacleLayerName))
        {
            return;
        }

        // Si detectamos que un agente est� dentro de nuestro radio/�rea de activaci�n (en este caso es nuestro trigger),
        // calculamos un vector con origen en la posici�n de este objeto, y cuyo fin es la posici�n de ese agente
        // NOTA: Esta resta es hacia el CENTRO del agente, por lo que s� puede llegar a ser m�s grande que el radius del collider.
        Vector3 OriginToAgent = transform.position - other.transform.position;
        // y despu�s se lo aplicamos al agente como una fuerza que afecta su steering behavior.
        //SimpleMovement otherSimpleMovement = GetComponent<SimpleMovement>();
        // aqu� podemos usar "this" en vez de hacer el getComponent que hac�amos cuando est�bamos en el obst�culo.

        // Debug.Log("Entr� a OnTriggerStay de SimpleMovement con: " + other.gameObject.name);

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


        // Debug.Log("Update n�mero: " + ContadorDeCuadros);
        // ContadorDeCuadros++;
        // este movimiento basado en cu�ntos cuadros han transcurrido no es justo para la gente con menos poder de c�mputo
        // transform.position = new Vector3(ContadorDeCuadros, 0, -1);
        // Ahorita tenemos una velocidad de 1 unidad en el eje X por cada cuadro de ejecuci�n.
        // Qu� tal si hacemos que avance una unidad en X por cada segundo que transcurra?


        // modificando la posici�n (acumulando los cambios)
        // transform.position += new Vector3(1 * Time.deltaTime, 0, 0);


        // Cada cuadro hay que actualizar el vector que nos dice a d�nde perseguir a nuestro objetivo.
        // Vector3 PosToTarget = PuntaMenosCola(targetGameObject.transform.position, transform.position); // SEEK

        // Vector3 PosToTarget = -PuntaMenosCola(targetGameObject.transform.position, transform.position);  // FLEE


        // Hay que pedirle al targetGameObject que nos d� acceso a su Velocity, la cual est� en el script SimpleMovement
        // Cuando usen algo que le pertenezca a un objeto que pueda llegar a ser null, ch�quenlo.
        if (targetGameObject == null)
        {
            // lo importante aqu� es que hicimos que ya no truene.
            return; // OJO: ahorita, en este en espec�fico, ya no va a hacer nada, ni siquiera moverse, cuando sea null.
        }

        Vector3 currentVelocity = targetGameObject.GetComponent<SimpleMovement>().Velocity;

        PursuitTimePrediction = CalculatePredictedTime(MaxSpeed, transform.position, targetGameObject.transform.position);

        // Primero predigo d�nde va a estar mi objetivo
        Vector3 PredictedPosition =
            PredictPosition(targetGameObject.transform.position, currentVelocity, PursuitTimePrediction);

        // Hago seek hacia la posici�n predicha.
        Vector3 PosToTarget = PuntaMenosCola(PredictedPosition, transform.position); // SEEK

        PosToTarget += ExternalForces;


        Velocity += PosToTarget.normalized * MaxAcceleration * Time.deltaTime;

        // Queremos que lo m�s r�pido que pueda ir sea a MaxSpeed unidades por segundo. Sin importar qu� tan grande sea la
        // flecha de PosToTarget.
        // Como la magnitud y la direcci�n de un vector se pueden separar, �nicamente necesitamos limitar la magnitud para
        // que no sobrepase el valor de MaxSpeed.
        Velocity = Vector3.ClampMagnitude(Velocity, MaxSpeed);

        transform.position += Velocity * Time.deltaTime;


        // Hay que resetearlas cada frame, si no se van a seguir aplicando aunque ya no se las deban aplicar.
        // Hay que resetearla al final del cuadro, si no se le va a quitar antes de poder utilizarla.
        ExternalForces = Vector3.zero;

        // transform.position += 
    }

    // Esta funci�n predice a d�nde se mover� un objeto cuya posici�n actual es InitialPosition, su velocidad actual es Velocity,
    // tras una cantidad de tiempo TimePrediction.
    protected Vector3 PredictPosition(Vector3 InitialPosition, Vector3 Velocity, float TimePrediction)
    {
        // Con base en la Velocity dada vamos a calcular en qu� posici�n estar� nuestro objeto con posici�n InitialPosition,
        // tras una cantidad X de tiempo (TimePrediction).
        return InitialPosition + Velocity * TimePrediction;

        // nosotros empezamos
    }

    protected float CalculatePredictedTime(float MaxSpeed, Vector3 InitialPosition, Vector3 TargetPosition)
    {
        // Primero obtenemos la distancia entre InitialPosition y TargetPosition. Lo hacemos con un punta menos cola, 
        // y nos quedamos con la pura magnitud, porque solo queremos saber cu�nto distancia hay entre ellos, no en qu� direcci�n.
        float Distance = PuntaMenosCola(TargetPosition, InitialPosition).magnitude;

        // Luego, dividimos nuestra distancia obtenida entre nuestra velocidad m�xima.
        return Distance / MaxSpeed;
    }

    protected Vector3 Pursuit(Vector3 TargetCurrentPosition, Vector3 TargetCurrentVelocity)
    {
        PursuitTimePrediction = CalculatePredictedTime(MaxSpeed, transform.position, TargetCurrentPosition);

        // Primero predigo d�nde va a estar mi objetivo
        Vector3 PredictedPosition =
            PredictPosition(TargetCurrentPosition, TargetCurrentVelocity, PursuitTimePrediction);

        // Hago seek hacia la posici�n predicha.
        return PuntaMenosCola(PredictedPosition, transform.position); // SEEK
    }

    protected Vector3 Evade(Vector3 TargetCurrentPosition, Vector3 TargetCurrentVelocity)
    {
        return - Pursuit(TargetCurrentPosition, TargetCurrentVelocity);
    }

    void FixedUpdate()
    {

    }


    void OnDrawGizmos()
    {

        if(DebugGizmoManager.VelocityLines)
        {
            Gizmos.color = Color.yellow;
            // Velocity S� tiene direcci�n y magnitud (es un vector de 1 o m�s dimensiones),
            // mientras que Speed no, �nicamente es una magnitud (o sea, un solo valor flotante)
            // Primero, dibujamos la "flecha naranja" que es nuestra velocidad (Velocity) actual, partiendo desde nuestra posici�n actual.
            Gizmos.DrawLine(transform.position, transform.position + Velocity);
        }
        // Ahora vamos con la "flecha azul" que es la direcci�n y magnitud hacia nuestro objetivo (la posici�n de nuestro objetivo).
        if (DebugGizmoManager.DesiredVectors && targetGameObject != null)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(transform.position, transform.position + (targetGameObject.transform.position - transform.position));
        }

        if(targetGameObject != null) 
        { 
            // Vamos a dibujar la posici�n a futuro que est� prediciendo.
            Vector3 currentVelocity = targetGameObject.GetComponent<SimpleMovement>().Velocity;

            PursuitTimePrediction = CalculatePredictedTime(MaxSpeed, transform.position, targetGameObject.transform.position);

            // Primero predigo d�nde va a estar mi objetivo
            Vector3 PredictedPosition =
                PredictPosition(targetGameObject.transform.position, currentVelocity, PursuitTimePrediction);

            Gizmos.color = Color.red;
            Gizmos.DrawCube(PredictedPosition, Vector3.one);
        }
    }


    int RetornarInt()
    {
        return 0;
    }
}
