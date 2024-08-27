/*
 * This code is based on the N3evin´s repository: Dijkstra-Algorithm-Unity
 * URL: https://github.com/N3evin/Dijkstra-Algorithm-Unity
 * 
 * Modifications:
 * - 3D Grid generation [Height, Width & Depth]
 */

using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

namespace PathTracking
{
    /// <summary>
    /// Script to control the generation of nodes and its connections.
    /// </summary>
    public class PT_GridController : MonoBehaviour
    {
        #region Variables
        [SerializeField, Tooltip("If true, only the nodes inside a collider tagged as 'Area' will be enabled")]
        bool enableNodesOnlyInArea = false;
        [SerializeField, Tooltip("Number of rows in the grid."), Range(1, 25)] 
        int depth = 5;
        [SerializeField, Tooltip("Number of columns in the grid."), Range(1, 25)] 
        int width = 5;
        [SerializeField, Tooltip("Number of rows (depth) in the grid."), Range(1, 25)]
        int height = 5;
        [SerializeField, Tooltip("Distance between each node.")] 
        float padding = 3f;
        [SerializeField, Tooltip("Transform from all the nodes will be generated")]
        Transform originTransform;
        Vector3 origin;
        [SerializeField, Tooltip("Gameobject that will save all the generated nodes.")]
        Transform parentTransform;
        [SerializeField, Tooltip("Prefab to be used to generate the Nodes.")] 
        Transform nodePrefab;

        Transform[,,] grid;

        [SerializeField]
        PT_DijkstraAlgorithm dijkstraAlgorithmScript;
        #endregion

        #region MonoBehaviour Methods
        // Use this for initialization
        void Start()
        {
            GC_GenerateGrid();
            GC_SetNodesData();
        }
        #endregion
        #region Grid Generation
        /// <summary>
        /// GridController. Generate the grid with the node.
        /// </summary>
        private void GC_GenerateGrid()
        {
            // Set origin transform to be in the center of the grid by moving the origin reference
            origin.x = originTransform.position.x - (((width - 1) * padding) / 2);
            origin.y = originTransform.position.y - (((height - 1) * padding) / 2);
            origin.z = originTransform.position.z - (((depth - 1) * padding) / 2);

            // Set grid size
            grid = new Transform[depth, width, height];

            // Iterate the grid to create the nodes
            for (int rowDepth = 0; rowDepth < depth; rowDepth++)
            {
                for (int row = 0; row < width; row++)
                {
                    for (int column = 0; column < height; column++)
                    {
                        Vector3 nodePosition = new Vector3(
                            (row * padding) + origin.x,
                            (column * padding) + origin.y, 
                            (rowDepth * padding) + origin.z);
                        Transform node = Instantiate(nodePrefab, nodePosition, Quaternion.identity);
                        node.name = "Node [" + rowDepth + ", " + row + ", " + column + "]";
                        node.parent = parentTransform;
                        grid[rowDepth, row, column] = node;
                    }
                }
            }

        }

        /// <summary>
        /// GridController. Set the neighbours and weigth of each node.
        /// </summary>
        private void GC_SetNodesData()
        {
            // Iterate the grid...
            for (int rowDepth = 0; rowDepth < depth; rowDepth++)
            {
                for (int row = 0; row < width; row++)
                {
                    for (int column = 0; column < height; column++)
                    {
                        // Get the node script...
                        PT_GridNode gridNode;
                        if (grid[rowDepth, row, column].TryGetComponent(out gridNode))
                        {
                            // Set neighbours only if they have a node there (Based on the grid)
                            if (rowDepth - 1 >= 0)    gridNode.GN_AddNeighbourNode(grid[rowDepth - 1, row, column]); // Left
                            if (rowDepth + 1 < depth) gridNode.GN_AddNeighbourNode(grid[rowDepth + 1, row, column]); // Right
                            if (row - 1 >= 0)         gridNode.GN_AddNeighbourNode(grid[rowDepth, row - 1, column]); // Front
                            if (row + 1 < width)      gridNode.GN_AddNeighbourNode(grid[rowDepth, row + 1, column]); // Back
                            if (column - 1 >= 0)      gridNode.GN_AddNeighbourNode(grid[rowDepth, row, column - 1]); // Top
                            if (column + 1 < height)  gridNode.GN_AddNeighbourNode(grid[rowDepth, row, column + 1]); // Bottom

                            // Set weight of the node (The closer to the robot , the less weight it will have)
                            gridNode.GN_SetWeight(
                                (height - 1) - column + // The upper to the workspace-top it is, the less weight it will have
                                Vector3.Distance(gridNode.transform.position, originTransform.position) // The closer to the robot , the less weight it will have
                                );
                        }
                    }
                }
            }
        }

        /// <summary>
        /// GridController. Generate again the grid.
        /// </summary>
        public void GC_ReloadGrid()
        {
            // Destroy any existing node
            foreach(Transform node in parentTransform)
            {
                GameObject.Destroy(node.gameObject);
            }

            // Create new grid
            GC_GenerateGrid();
        }
        #endregion
        #region Path Generation
        /// <summary>
        /// Use the Dijkstra algorithm to get the closest path between two points.
        /// </summary>
        /// <param name="origin">Starting position</param>
        /// <param name="target">End position</param>
        /// <returns>The path nodes between both points</returns>
        public List<Transform> GC_GetPath(Vector3 origin, Vector3 target)
        {
            // GameObject of the closest node to the Origin position
            GameObject originNode = parentTransform.gameObject;
            float originNodeDistance = 0.0f;
            // GameObject of the closest node to the Target position
            GameObject targetNode = parentTransform.gameObject;
            float targetNodeDistance = 0.0f;
            // List of all the nodes in the grid
            List<Transform> nodesList = new List<Transform>();

            // Iterate the parent Transform that has all the nodes generated...
            foreach (Transform node in parentTransform)
            {
                // If the node is walkable...
                PT_GridNode gridNodeScript;
                if (node.TryGetComponent(out gridNodeScript))
                {
                    if (gridNodeScript.GN_IsWalkable())
                    {
                        // Get the closest node for each position...
                        // If they haven't been initialized, save the first node as their reference
                        if (originNode == parentTransform.gameObject) { originNode = node.gameObject; originNodeDistance = Vector3.Distance(originNode.transform.position, origin); }
                        if (targetNode == parentTransform.gameObject) { targetNode = node.gameObject; targetNodeDistance = Vector3.Distance(targetNode.transform.position, target); }
                        // Check if the node is closer to the last saved node as the closest node to the Origin position
                        if (Vector3.Distance(node.position, origin) < originNodeDistance)
                        {
                            // If it is closer, change the saved node to this
                            originNode = node.gameObject;
                            originNodeDistance = Vector3.Distance(node.position, origin);
                        }
                        // Check if the node is closer to the last saved node as the closest node to the Origin position
                        if (Vector3.Distance(node.position, target) < targetNodeDistance)
                        {
                            // If it is closer, change the saved node to this
                            targetNode = node.gameObject;
                            targetNodeDistance = Vector3.Distance(node.position, target);
                        }

                        // Add the node to the list
                        nodesList.Add(node);
                    }
                }
            }

            // Find the path and return the nodess
            return dijkstraAlgorithmScript.DA_FindShortestPath(originNode.transform, targetNode.transform, nodesList);
        }

        /// <summary>
        /// GridController. Change the bounds of the BoxCollider from each node in the grid
        /// </summary>
        /// <param name="size">Bounds size (As Vector3) to be set</param>
        /// <param name="scale">Scale of the GameObject to have as reference for scaling the bounds of the nodes.</param>
        public void GC_SetNodesBounds(Vector3 size, Vector3 scale)
        {
            foreach (Transform node in parentTransform)
            {
                PT_GridNode nodeScript;
                if (node.TryGetComponent(out nodeScript))
                {
                    // The size is multiplied by the scale so the node can recognize the real bound size based on the node scale
                    nodeScript.GN_SetNodeColliderDimension(size.x * scale.x, size.y * scale.y, size.z * scale.z);
                }
            }
        }
        #endregion
    }
}
