using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
public class PathFitter : MonoBehaviour
{

    public List<Vector3> path;
    private int propertyHash;
    private List<GameObject> cubes = new();


    // Start is called before the first frame update
    void Start()
    {

    }

    void DestroyCubes()
    {
        // Remove any existing cubes
        cubes.ForEach((cube) =>
        {
            DestroyImmediate(cube);
        });

        // Clear the list record
        cubes.Clear();
    }

    void AddCubesAtPoints()
    {

        foreach (var point in path)
        {
            // Create a cube GameObject
            GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.parent = transform;
            cubes.Add(cube);

            // Set the position of the cube
            cube.transform.position = point;
            // You can also set other properties of the cube here if needed
            // For example, you can set the scale, color, etc.
        }
    }

    // Update is called once per frame
    void Update()
    {
        var hash = path.GetHashCode();

        // Get the hashcode of the all path elements
        path.ForEach((point) =>
        {
            hash += point.GetHashCode();
        });

        // Only recreate if something has changed
        if (hash != propertyHash)
        {
            propertyHash = hash;
            DestroyCubes();
            AddCubesAtPoints();
        }
    }
}
