using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PathTracking
{
    public class PT_PathTracker : MonoBehaviour
    {
        [SerializeField]
        Transform origin; // Starting position
        [SerializeField]
        Transform target; // Position to reach

        public List<Vector3> path;

        [SerializeField]
        GameObject objectToMove;

        [SerializeField]
        PT_GridController gridControllerScript;

        bool isMoving = false;

        /// Update is called once per frame
        void Update()
        {
            if (origin && target && objectToMove)
            {
                if (!isMoving)
                {
                    // Generate path input...
                    if (Input.GetKeyDown(KeyCode.Space))
                    {
                        // Get object to move size and set it in the grid
                        MeshRenderer objectToMoveMeshRenderer;
                        if (objectToMove.TryGetComponent(out objectToMoveMeshRenderer))
                        {
                            gridControllerScript.GC_SetNodesBounds(objectToMoveMeshRenderer.bounds.size, objectToMove.transform.localScale);
                        }
                    }
                    if (Input.GetKeyUp(KeyCode.Space))
                    {
                        // Get path
                        path = PT_GetPath();
                        // Draw path
                        PT_DrawPath();
                    }

                    // Move object input...
                    if (Input.GetKeyUp(KeyCode.M))
                    {
                        // Enable movement only if there is a path
                        if (path.Count > 0)
                        {
                            isMoving = true;
                            nextNodeIndex = 0;
                        }
                    }
                }
                if (isMoving) MoveObjectTrougPath();
            }
            else
            {
                string errorMessage = "ERROR: \n";
                if (!origin)
                    errorMessage += "- Origin reference is missing! \n";
                if (!target)
                    errorMessage += "- Target reference is missing! \n";
                if (!objectToMove)
                    errorMessage += "- ObjectToMove reference is missing! \n";

                Debug.Log(errorMessage);
            }
        }

        /// <summary>
        /// PathTracker. Gets the used nodes in the path (From Origin to Target).
        /// </summary>
        /// <returns>A list with the positions of the visited nodes.</returns>
        List<Vector3> PT_GetPath()
        {
            List<Vector3> nodesPath = new List<Vector3>();
            nodesPath.Add(origin.position);
            foreach (Transform nodeTransform in gridControllerScript.GC_GetPath(origin.position, target.position))
            {
                nodesPath.Add(nodeTransform.position);
            }
            nodesPath.Add(target.position);
            return nodesPath;
        }

        /// <summary>
        /// PathTracker. Draw a red line (In Debug mode) between the nodes used in the path.
        /// </summary>
        void PT_DrawPath()
        {
            // Draw only when the path has two or more points
            for (int index = 0; index < path.Count - 1; index++)
            {
                Debug.DrawLine(path[index], path[index+1], Color.red, 5);
            }
        }

        int nextNodeIndex = 0;
        /// <summary>
        /// Method to move the object through the path.
        /// </summary>
        void MoveObjectTrougPath()
        {
            if (nextNodeIndex < path.Count)
            {
                objectToMove.transform.position = 
                    Vector3.MoveTowards(objectToMove.transform.position, path[nextNodeIndex], Time.deltaTime);

                if (objectToMove.transform.position == path[nextNodeIndex])
                {
                    nextNodeIndex++;
                }
            }
            else
            {
                isMoving = false;
            }
        }
    }
}
