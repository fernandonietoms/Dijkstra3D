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
        float weight = 0;
        [SerializeField]
        Transform parentNode = null;
        [SerializeField]
        List<Transform> neighbourNode;
        [SerializeField]
        bool isWalkable = true;
        [SerializeField]
        bool isActiveOnlyInsideArea = false;
        [SerializeField]
        BoxCollider transformCollider;
        List<Collider> collidersInCollision = new List<Collider>();
        #endregion

        #region MonoBehaviour Methods
        private void Awake()
        {
            // Disable node if it will only be avaliable inside an Area collider (If it collides with one, it will be enabled)
            if (isActiveOnlyInsideArea)
            {
                GN_SetWalkable(false);
            }
        }

        private void Update()
        {
            if (isActiveOnlyInsideArea)
            {
                bool showNode = false;
                foreach (Collider collider in collidersInCollision)
                {
                    if (collider.bounds.Contains(transformCollider.bounds.max) &&
                        collider.bounds.Contains(transformCollider.bounds.min))
                    {
                        showNode = true;
                    }
                }

                if (showNode)
                    GN_SetWalkable(true);
                else
                    GN_SetWalkable(false);
            }
        }

        #region Collisions
        private void OnCollisionEnter(Collision collision)
        {
            // Any overlapped node by an obstacke will be disabled
            if (collision.gameObject.tag.Contains("Obstacle"))
            {
                GN_SetWalkable(false);
            }

            // Enable the node if the collided object is an Area and will be active only inside it
            if (isActiveOnlyInsideArea && collision.gameObject.tag.Contains("Area"))
            {
                collidersInCollision.Add(collision.collider);
            }
        }

        private void OnCollisionExit(Collision collision)
        {
            // Enable the node if it is reachable again
            if (collision.gameObject.tag.Contains("Obstacle"))
            {
                GN_SetWalkable(true);
            }

            // Disable the node if the collided object is an Area and will be active only inside it
            if (isActiveOnlyInsideArea && collision.gameObject.tag.Contains("Area"))
            {
                collidersInCollision.Remove(collision.collider);
            }
        }
        #endregion
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
            this.GetComponent<MeshRenderer>().enabled = value;
        }

        /// <summary>
        /// GridNode. Adding neighbour node object.
        /// </summary>
        /// <param name="node">Node transform</param>
        public void GN_AddNeighbourNode(Transform node)
        {
            this.neighbourNode.Add(node);
        }

        /// <summary>
        /// GridNode. Change the bounds of the BoxCollider
        /// </summary>
        /// <param name="x">X Size Value</param>
        /// <param name="y">Y Size Value</param>
        /// <param name="z">Z Size  Value</param>
        public void GN_SetNodeColliderDimension(float x, float y, float z)
        {
            BoxCollider boxCollider;
            if (this.TryGetComponent(out boxCollider))
            {
                // The size is divided by the node scale to get the real bound size of the wanted object
                boxCollider.size = new Vector3(
                    x / this.transform.localScale.x,
                    y / this.transform.localScale.y,
                    z / this.transform.localScale.z
                  );
            }
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
