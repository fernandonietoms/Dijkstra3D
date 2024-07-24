/*
 * This code is based on the N3evin´s repository: Dijkstra-Algorithm-Unity
 * URL: https://github.com/N3evin/Dijkstra-Algorithm-Unity
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathTracking
{
    /// <summary>
    /// Script Component added to each node of the grid to set its connections and weight.
    /// </summary>
    public class PT_GridNode : MonoBehaviour
    {
        #region Variables
        [SerializeField]
        float weight = int.MaxValue;
        [SerializeField]
        Transform parentNode = null;
        [SerializeField]
        List<Transform> neighbourNode;
        [SerializeField]
        bool isWalkable = true;
        #endregion

        #region MonoBehaviour Methods
        // Use this for initialization
        void Start()
        {
            GN_ResetNode();
        }
        #endregion
        #region Set Data
        /// <summary>
        /// GridNode. Reset all the values in the nodes.
        /// </summary>
        public void GN_ResetNode()
        {
            weight = int.MaxValue;
            parentNode = null;
        }

        /// <summary>
        /// GridNode. Set the parent node.
        /// </summary>
        /// <param name="node">Set the node for parent node.</param>
        public void GN_SetParentNode(Transform node)
        {
            this.parentNode = node;
        }

        /// <summary>.
        /// GridNode. Set the weight value
        /// </summary>
        /// <param name="value">weight value</param>
        public void GN_SetWeight(float value)
        {
            this.weight = value;
        }

        /// <summary>
        /// GridNode. Set is node is walkable.
        /// </summary>
        /// <param name="value">boolean</param>
        public void GN_SetWalkable(bool value)
        {
            this.isWalkable = value;
        }

        /// <summary>
        /// GridNode. Adding neighbour node object.
        /// </summary>
        /// <param name="node">Node transform</param>
        public void GN_AddNeighbourNode(Transform node)
        {
            this.neighbourNode.Add(node);
        }
        #endregion
        #region Get Data
        /// <summary>
        /// GridNode. Get neighbour node.
        /// </summary>
        /// <returns>All the nodes.</returns>
        public List<Transform> GN_GetNeighbourNode()
        {
            List<Transform> result = this.neighbourNode;
            return result;
        }

        /// <summary>
        /// GridNode. Get weight
        /// </summary>
        /// <returns>get weight in float.</returns>
        public float GN_GetWeight()
        {
            float result = this.weight;
            return result;

        }

        /// <summary>
        /// GridNode. Get the parent Node.
        /// </summary>
        /// <returns>Return the parent node.</returns>
        public Transform GN_GetParentNode()
        {
            Transform result = this.parentNode;
            return result;
        }


        /// <summary>
        /// GridNode. Get if the node is walkable.
        /// </summary>
        /// <returns>boolean</returns>
        public bool GN_IsWalkable()
        {
            bool result = isWalkable;
            return result;
        }
        #endregion
    }
}
