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
        PT_GridController gridControllerScript;

        // Update is called once per frame
        void Update()
        {
            if (origin && target)
            {
                if (Input.GetKeyUp(KeyCode.Space))
                {
                    path.Clear();
                    path.Add(origin.position);

                    foreach (Transform nodeTransform in gridControllerScript.GC_GetPath(origin.position, target.position))
                    {
                        path.Add(nodeTransform.position);
                    }

                    path.Add(target.position);
                    DrawPath();
                }
            }
            else
            {
                string errorMessage = "ERROR: \n";
                if (!origin)
                    errorMessage += "- Origin reference is missing! \n";
                if (!target)
                    errorMessage += "- Target reference is missing! \n";

                Debug.Log(errorMessage);
            }
        }

        void DrawPath()
        {
            // Draw only when the path has two or more points
            for (int index = 0; index < path.Count - 1; index++)
            {
                Debug.DrawLine(path[index], path[index+1], Color.red, 5);
            }
        }
    }
}
