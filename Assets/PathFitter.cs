using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.IO;

[ExecuteInEditMode]
public class PathFitter : MonoBehaviour
{
    public List<Vector3> path;
    private int propertyHash;

    private List<GameObject> models = new();

    public Vector3 modelScale = new Vector3(1.0f, 1.0f, 1.0f);
    public Color cubeColor = Color.white;

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
            // Create a GameObject
            GameObject model = CreateModel();
            model.transform.parent = transform;
            model.transform.localScale = modelScale;
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