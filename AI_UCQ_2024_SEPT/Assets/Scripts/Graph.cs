using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;



public class Node
{
    public Node(string inName)
    {
        Name = inName;
        Parent = null;
    }

    public string Name = "";  // es string por pura claridad, idealmente se usan ints para diferenciar objetos.

    public Node Parent;  // referencia al nodo padre de este nodo en el �rbol que se genera durante un Pathfinding.
}

public class Edge
{
    public Edge(string inName, Node inA, Node inB, float inWeight = 1.0f)
    {
        Name = inName;
        A = inA;
        B = inB;
        Weight = inWeight;
    }

    public string Name = ""; // es string por pura claridad, las aristas normalmente no necesitan un nombre.
    public Node A;
    public Node B;
    public float Weight = 1.0f;

    // EdgeA == EdgeB
    // Si son punteros/referencias pues nom�s comparan la direcci�n de memoria y ya.
    // PERO SI NO, ustedes tendr�an que comparar una o m�s cosas.
    // Por ejemplo podr�amos checar EdgeA.A == EdgeB.A && EdgeA.B == EdgeB.B && EdgeA.Weight == EdgeB.Weight

    // Un hash te da un solo n�mero que representa a ese objeto.

    // Vector3 A == Vector3 B?
    // A.x == B.x && A.y == B.y && A.z == B.z

}


public class Graph : MonoBehaviour
{

    // Podr�amos guardarlos en un array.
    // Podr�amos guardarlos en un List, Set
    // Dictionary, Queue, Stack, DynamicArray, Heap

    // Array:
    // Ventajas: super r�pido de acceder de manera secuencial. Te da el espacio de memoria completo.
    // int [10]Array
    // Desventajas: Te da el espacio de memoria completo (lo vayas a usar o no, lo que puede llevar a desperdicios).
    // desventajas: su tama�o (capacidad de almacenamiento) es totalmente est�tico.
    // desventajas: poner y quitar elementos que hagan que cambie el tama�o del array es MUY lento.


    // �Qu� es un "Set" en estructuras de datos / programaci�n?
    // Un set es una estructura de datos que no permite repetidos
    // espec�ficamente en nuestros grafos, no vamos a querer ni nodos ni aristas repetidas.


    protected HashSet<Node> NodeSet = new HashSet<Node>();
    protected HashSet<Edge> EdgeSet = new HashSet<Edge>();


    // Start is called before the first frame update
    void Start()
    {
        // Vamos a llenar nuestros sets de nodos y aristas.
        // Comenzamos creando todos los nodos, porque las aristas necesitan que ya existan los nodos.
        Node NodeA = new Node("A");
        Node NodeB = new Node("B");
        Node NodeC = new Node("C");
        Node NodeD = new Node("D");
        Node NodeE = new Node("E");
        Node NodeF = new Node("F");
        Node NodeG = new Node("G");
        Node NodeH = new Node("H");

        NodeSet.Add(NodeA);
        NodeSet.Add(NodeB);
        NodeSet.Add(NodeC);
        NodeSet.Add(NodeD);
        NodeSet.Add(NodeE);
        NodeSet.Add(NodeF);
        NodeSet.Add(NodeG);
        NodeSet.Add(NodeH);

        // Ahora queremos declarar las aristas.
        Edge EdgeAB = new Edge("AB", NodeA, NodeB);
        Edge EdgeAE = new Edge("AE", NodeA, NodeE);
        Edge EdgeBC = new Edge("BC", NodeB, NodeC);
        Edge EdgeBD = new Edge("BD", NodeB, NodeD);
        Edge EdgeEF = new Edge("EF", NodeE, NodeF);
        Edge EdgeEG = new Edge("EG", NodeE, NodeG);
        Edge EdgeEH = new Edge("EH", NodeE, NodeH);

        EdgeSet.Add(EdgeAB);
        EdgeSet.Add(EdgeAE);
        EdgeSet.Add(EdgeBC);
        EdgeSet.Add(EdgeBD);
        EdgeSet.Add(EdgeEF);
        EdgeSet.Add(EdgeEG);
        EdgeSet.Add(EdgeEH);

        if (RecursiveDFS(NodeA, NodeH))
        {
            Debug.Log("S� hay camino del nodo: " + NodeA.Name + " hacia el nodo: " + NodeH.Name);
        }
        else
        {
            Debug.Log("No hay camino del nodo: " + NodeA.Name + " hacia el nodo: " + NodeH.Name);
        }

        // FuncionRecursiva(0);  // comentada para que no truene ahorita.
    }

    // Vamos a implementar el algoritmo de depth-first search (DFS) usando la pila de llamadas,
    // de manera recursiva.
    // �Qu� pregunta nos va a responder o qu� resultado nos va a dar?
    // Pues nos dice si hay un camino desde un Nodo Origen hasta un nodo Destino (de un grafo)
    // y si s� hay un camino, nos dice cu�l fue. Esto del camino tiene un truco interesante.
    bool RecursiveDFS(Node Origin, Node Goal)
    {
        // Para evitar que alguien se vuelva padre del nodo ra�z de todo el �rbol
        // hacemos que el nodo ra�z del �rbol sea su propio padre.
        if (Origin.Parent == null)
        {
            // si esto se cumple, entonces este nodo es la ra�z del �rbol.
            Origin.Parent = Origin;
        }

        // La condici�n de terminaci�n de esta funci�n recursiva es 
        // "el nodo en el que estoy actualmente (Origin) es la meta (Goal)"
        if (Origin == Goal)
            return true;

        // Desde el nodo donde estamos ahorita, checamos cu�les son nuestros vecinos.
        // Los vecinos de este nodo son los que comparten una arista con �l.
        // lo que podemos hacer es revisar todas las aristas y obtener las que hagan referencia a este nodo.
        // 1) Checar todas las aristas.
        foreach (Edge currentEdge in EdgeSet)
        {
            bool Result = false;
            Node Neighbor = null;
            // Vamos a checar si la arista en cuesti�n hace referencia a este nodo "Origin"
            // Checamos su A y su B. Y tenemos que checar que el vecino NO tenga padre, para que Origin 
            // se convierta en su padre.
            if (currentEdge.A == Origin && currentEdge.B.Parent == null)
            {
                // entonces s� hace referencia a este nodo. Lo que hacemos es meter al Nodo vecino.
                // Si encontramos un vecino, primero le decimos que el nodo Origin actual es
                // su nodo padre, y despu�s mandamos a llamar la funci�n de nuevo, pero 
                // usando a este vecino como el nuevo origen.
                currentEdge.B.Parent = Origin;
                Neighbor = currentEdge.B;
            }
            else if (currentEdge.B == Origin && currentEdge.A.Parent == null)
            {
                // entonces s� hace referencia a este nodo. Lo que hacemos es meter al Nodo vecino.
                // Si encontramos un vecino, primero le decimos que el nodo Origin actual es
                // su nodo padre, y despu�s mandamos a llamar la funci�n de nuevo, pero 
                // usando a este vecino como el nuevo origen.
                currentEdge.A.Parent = Origin;
                Neighbor = currentEdge.A;
            }

            // Necesitamos esta comprobaci�n por si no entra ni al if ni al if else de arriba.
            if(Neighbor != null)
                Result = RecursiveDFS(Neighbor, Goal);


            if (Result == true)
            {
                // Si este nodo fue parte del camino al Goal, le decimos que imprima su nombre.
                Debug.Log("El nodo: " + Origin.Name + " fue parte del camino a la meta.");

                // entonces el hijo de este nodo ya encontr� el camino
                // y eso hace una reacci�n en cadena de "Pap�, s� encontr� el camino".
                return true;
            }
        }

        // si ya acab� el ciclo de intentar visitar a sus vecinos, se regresa a la funci�n del nodo
        // que es su padre.
        return false;
    }


    // Funciones recursivas VS funciones iterativas.

    // las funciones recursivas son funciones que se mandan a llamar a s� mismas.
    void FuncionRecursiva(int Counter)
    {
        Debug.Log("Hola n�mero: " + Counter);
        if (Counter == 10)
            return;
        FuncionRecursiva(Counter+1);
    }

    // MyArray [0, 1, 2, 3, 4...]

    // MyStack [0]
    // [1, 0]
    // 2, 1, 0
    // 3, 2, 1, 0
    // Ahora vamoas a sacar elementos
    // sacas el 3, que es el �ltimo que metiste, y te quedar�a:
    // 2, 1, 0
    // 1, 0, 
    // 0
    // Last in, First out
    // solo puedes sacar el �ltimo elemento que metiste.

    // Update is called once per frame
    void Update()
    {
        
    }
}
