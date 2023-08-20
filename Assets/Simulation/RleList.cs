using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Simulation
{
    public class RleList
    {
        private List<Vector3> points = new();
        public bool IsRunning = false;
        public float MaxDistanceBetweenPoints = 1.0f;

        public List<Vector3> GetPoints()
        {
            return points;
        }

        public void StartAt(Vector3 point)
        {
            IsRunning = true;
            points.Add(point);
        }

        public void Put(Vector3 point)
        {
            if (IsRunning && points.Count > 0)
            {
                var lastPoint = points.Last();
                if ((lastPoint - point).magnitude > MaxDistanceBetweenPoints)
                {
                    // Calculate the number of intermediate points needed to satisfy the distance constraint
                    int numPoints = Mathf.CeilToInt((lastPoint - point).magnitude / MaxDistanceBetweenPoints);

                    // Calculate the step size for each intermediate point
                    float stepSize = 1.0f / (numPoints + 1);

                    // Interpolate between the two points to generate the intermediate points
                    for (int i = 1; i <= numPoints; i++)
                    {
                        Vector3 intermediatePoint = Vector3.Lerp(lastPoint, point, i * stepSize);
                        points.Add(intermediatePoint);
                    }
                }
                
                points.Add(point);
                return;
            }

            points.Add(point);
        }

        public void EndAt(Vector3 point)
        {
            Put(point);
            IsRunning = false;
        }
    }
}