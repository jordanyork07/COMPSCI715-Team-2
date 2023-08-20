using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.IO;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class PathFitter : MonoBehaviour
{
    public List<Vector3> path;
    private int propertyHash;

    private List<GameObject> models = new();

    // public Vector3 modelScale = new Vector3(1.0f, 1.0f, 1.0f);
    public Color cubeColor = Color.white;
    
    public Vector2 minMaxScale = new Vector2(1f, 1f);
    public bool randomiseRotation = true;
    public float opacity = 1.0f;

    // Start is called before the first frame update
    void Start()
    {

    }

    void DestroyModels()
    {
        // Remove any existing models
        models.ForEach((model) =>
        {
            DestroyImmediate(model);
        });

        // Clear the list record
        models.Clear();
    }

    protected virtual GameObject CreateModel()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.GetComponent<Renderer>().material.color = cubeColor;
        return cube;
    }

    void AddModelsAtPoints()
    {
        foreach (var point in path)
        {
            // Randomness
            var rotation = Quaternion.identity;
            var scale = Random.Range(minMaxScale.x, minMaxScale.y);

            if (randomiseRotation)
                rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            
            // Create a GameObject
            GameObject model = CreateModel();
            model.transform.parent = transform;
            model.transform.localScale = new Vector3(scale, scale, scale);
            model.transform.rotation = rotation;
            model.transform.position = point;
            models.Add(model);            
        }
    }

    // Update is called once per frame
    void Update()
    {
        var hash = path.GetHashCode();

        // Get the hashcode of all path elements
        path.ForEach((point) =>
        {
            hash += point.GetHashCode();
        });

        // Only recreate if something has changed
        if (hash != propertyHash)
        {
            propertyHash = hash;
            DestroyModels();
            AddModelsAtPoints();
        }
    }
}