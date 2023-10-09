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
    
    public bool shouldHaveEndPlatform = true;
    public LevelManager levelManager;
    public GameObject player;
    
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
        var until = shouldHaveEndPlatform ? path.Count - 1 : path.Count;
        for (int i = 0; i < until; i++)
        {
            var point = path[i];
            
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

        if (shouldHaveEndPlatform)
        {
            var lastPoint = path.Last();
            
            // Randomness
            var rotation = Quaternion.identity;
            var scale = Random.Range(minMaxScale.x, minMaxScale.y);

            if (randomiseRotation)
                rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            
            // Create a GameObject
            GameObject modelPrefab = Resources.Load<GameObject>("Final_Platform");
            GameObject model = Instantiate(modelPrefab, Vector3.zero, Quaternion.identity);
            model.transform.parent = transform;
            model.transform.localScale = new Vector3(scale, scale, scale);
            model.transform.rotation = rotation;
            model.transform.position = lastPoint + new Vector3(0, 1, 0);

            var teleporter = model.GetComponent<PlayerTeleporterLevel>();
            teleporter.player = player;
            teleporter.manager = levelManager;

            models.Add(model);
        }
        
        return models;
    }
}