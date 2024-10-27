using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreadthFirstSearch : MonoBehaviour
{
    // Método para ejecutar el BFS
    public bool BFS(Node start, Node goal, out List<Node> path)
    {
        path = new List<Node>();
        Queue<Node> queue = new Queue<Node>();
        HashSet<Node> visited = new HashSet<Node>();

        queue.Enqueue(start);
        visited.Add(start);
        start.Parent = start; // Asigna el padre para el nodo de inicio

        while (queue.Count > 0)
        {
            Node current = queue.Dequeue();
            Debug.Log("Visitando nodo: " + current.Name);

            if (current == goal)
            {
                path = Backtrack(current);
                return true; // Se encontró el camino
            }

            // Obtener vecinos usando EdgeSet en lugar de Edges en Node
            List<Node> neighbors = GetNeighbors(current);

            foreach (Node neighbor in neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    visited.Add(neighbor);
                    neighbor.Parent = current; // Establece el padre
                    queue.Enqueue(neighbor);
                }
            }
        }

        return false; // No se encontró camino
    }

    // Método para realizar el retroceso y obtener el camino
    private List<Node> Backtrack(Node goal)
    {
        List<Node> path = new List<Node>();
        Node current = goal;

        while (current != current.Parent)
        {
            path.Add(current);
            current = current.Parent;
        }
        path.Add(current); // Agrega el nodo de inicio

        path.Reverse(); // Invertir el camino para que esté en el orden correcto
        return path;
    }

    // Método para obtener vecinos de un nodo
    private List<Node> GetNeighbors(Node inNode)
    {
        List<Node> neighbors = new List<Node>();

        foreach (Edge edge in FindObjectOfType<Graph>().EdgeSet)
        {
            if (edge.A == inNode)
            {
                neighbors.Add(edge.B);
            }
            else if (edge.B == inNode)
            {
                neighbors.Add(edge.A);
            }
        }

        return neighbors;
    }


    public HashSet<Edge> EdgeSet = new HashSet<Edge>();
    void Start()
    {
        // Caso 1: Exitoso (Hay camino)
        Debug.Log("Caso 1: Exitoso");
        Node H = new Node("H");
        Node E = new Node("E");
        Node A = new Node("A");
        Node B = new Node("B");
        Node C = new Node("C");
        Node D = new Node("D");
        Node F = new Node("F");

        // Ahora queremos declarar las aristas.
        Edge EdgeHE = new Edge("HE", H, E);
        Edge EdgeEA = new Edge("EA", E, A);
        Edge EdgeEF = new Edge("EF", E, F);
        Edge EdgeAB = new Edge("AB", A, B);
        Edge EdgeBC = new Edge("BC", B, C);
        Edge EdgeBD = new Edge("BD", B, D);

        EdgeSet.Add(EdgeHE);
        EdgeSet.Add(EdgeEA);
        EdgeSet.Add(EdgeEF);
        EdgeSet.Add(EdgeAB);
        EdgeSet.Add(EdgeBC);
        EdgeSet.Add(EdgeBD);

        List<Node> path;
        bool found = BFS(H, D, out path);

        if (found)
        {
            Debug.Log("¡Se encontró un camino! Camino:");
            foreach (Node node in path)
            {
                Debug.Log(node.Name);
            }
        }
        else
        {
            Debug.Log("No se encontró camino.");
        }

        // Caso 2: Fallido (No hay camino)

        Debug.Log("----------------------------------------");
        Debug.Log("Caso 2: Fallido");
        Node G = new Node("G");

        Edge Edge2HE = new Edge("HE", H, E);
        Edge Edge2EF = new Edge("EF", E, F);
        Edge Edge2EG = new Edge("EA", E, G);

        EdgeSet.Add(Edge2HE);
        EdgeSet.Add(Edge2EF);
        EdgeSet.Add(Edge2EG);
        // D queda desconectado

        found = BFS(H, D, out path);

        if (found)
        {
            Debug.Log("¡Se encontró un camino! Camino:");
            foreach (Node node in path)
            {
                Debug.Log(node.Name);
            }
        }
        else
        {
            Debug.Log("No se encontró camino.");
        }
    }

}