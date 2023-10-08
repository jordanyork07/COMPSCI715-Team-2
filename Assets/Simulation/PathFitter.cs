using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.IO;
using Random = UnityEngine.Random;

public class PathFitter : MonoBehaviour
{
    public List<Vector3> path = new();

    // public Vector3 modelScale = new Vector3(1.0f, 1.0f, 1.0f);
    public Color cubeColor = Color.white;
    
    public Vector2 minMaxScale = new Vector2(0.1f, 0.5f);
    public bool randomiseRotation = true;
    public bool transparent = false;
    public bool shouldDoCollision = true;
    
    public Material material;

    void Start()
    {   
    }

    void Update() 
    {
    }
    
    protected virtual GameObject CreateModel()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        var renderer = cube.GetComponent<Renderer>();
        renderer.material = material;
        renderer.material.color = cubeColor;
        return cube;
    }

    public List<GameObject> GetModels()
    {
        List<GameObject> models = new List<GameObject>();
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

            

            if (!shouldDoCollision && model.TryGetComponent<Collider>(out var component))
                DestroyImmediate(component);

            models.Add(model);
        }
        return models;
    }
}