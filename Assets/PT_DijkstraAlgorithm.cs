/*
 * This code is based on the N3evin´s repository: Dijkstra-Algorithm-Unity
 * URL: https://github.com/N3evin/Dijkstra-Algorithm-Unity
 */

using PathTracking;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class PT_DijkstraAlgorithm : MonoBehaviour
{
    Transform[] nodes;

    /// <summary>
    /// DijkstraAlgorithm. Finding the shortest path between two points in the grid nodes.
    /// </summary>
    /// <param name="start">The start point</param>
    /// <param name="end">The end point</param>
    /// <param name="nodeList">List of Nodes in the grid</param>
    /// <returns>A List of the nodes (Transform) for the shortest path</returns>
    public List<Transform> DA_FindShortestPath(Transform start, Transform end, List<Transform> nodeList)
    {
        nodes = nodeList.ToArray();

        List<Transform> result = new List<Transform>();
        Transform node = DA_StartAlgorithm(start, end);

        // While there's still previous node, we will continue.
        while (node != null)
        {
            result.Add(node);
            PT_GridNode currentNode = node.GetComponent<PT_GridNode>();
            node = currentNode.GN_GetParentNode();
        }

        // Reverse the list so that it will be from start to end.
        result.Reverse();
        return result;
    }

    /// <summary>
    /// DijkstraAlgorithm. Dijkstra Algorithm to find the shortest path
    /// </summary>
    /// <param name="start">The start point in grid</param>
    /// <param name="end">The end point in grid</param>
    /// <returns>The end node</returns>
    private Transform DA_StartAlgorithm(Transform start, Transform end)
    {
        double startTime = Time.realtimeSinceStartup;

        // Nodes that are unexplored
        List<Transform> unexplored = new List<Transform>();

        // We add all the nodes we found into unexplored.
        foreach (Transform obj in nodes)
        {
            PT_GridNode node = obj.GetComponent<PT_GridNode>();
            if (node.GN_IsWalkable())
            {
                node.GN_ResetNode();
                unexplored.Add(obj.transform);
            }
        }

        // Set the starting node weight to 0;
        PT_GridNode startNode = start.GetComponent<PT_GridNode>();
        startNode.GN_SetWeight(0);

        while (unexplored.Count > 0)
        {
            // Sort the explored by their weight in ascending order.
            unexplored.Sort((x, y) => x.GetComponent<PT_GridNode>().GN_GetWeight().CompareTo(y.GetComponent<PT_GridNode>().GN_GetWeight()));

            // Get the lowest weight in unexplored.
            Transform current = unexplored[0];

            // Note: This is used for games, as we just want to reduce compuation, better way will be implementing A*
            /*
            // If we reach the end node, we will stop.
            if(current == end)
            {   
                return end;
            }*/

            //Remove the node, since we are exploring it now.
            unexplored.Remove(current);

            PT_GridNode currentNode = current.GetComponent<PT_GridNode>();
            List<Transform> neighbours = currentNode.GN_GetNeighbourNode();
            foreach (Transform neighbourNode in neighbours)
            {
                PT_GridNode node = neighbourNode.GetComponent<PT_GridNode>();

                // We want to avoid those that had been explored and is not walkable.
                if (unexplored.Contains(neighbourNode) && node.GN_IsWalkable())
                {
                    // Get the distance of the object.
                    float distance = Vector3.Distance(neighbourNode.position, current.position);
                    distance = currentNode.GN_GetWeight() + distance;

                    // If the added distance is less than the current weight.
                    if (distance < node.GN_GetWeight())
                    {
                        // We update the new distance as weight and update the new path now.
                        node.GN_SetWeight(distance);
                        node.GN_SetParentNode(current);
                    }
                }
            }

        }

        double endTime = (Time.realtimeSinceStartup - startTime);
        print("Compute time: " + endTime);

        print("Path completed!");

        return end;
    }

}
