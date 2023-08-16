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
    private List<GameObject> models = new();  // Updated variable name

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

    void AddModelsAtPoints()
    {
        foreach (var point in path)
        {
            // Load the "SM_Env_DirtMount_01" prefab from the PolygonAdventure asset
            GameObject modelPrefab = Resources.Load<GameObject>("SM_Env_DirtMound_01");

            if (modelPrefab != null)
            {
                // Instantiate the modelPrefab
                GameObject model = Instantiate(modelPrefab, point, Quaternion.identity);
                model.transform.parent = transform;
                models.Add(model);
            }
            else
            {
                Debug.LogError("Model prefab not found! Make sure the path is correct.");
            }
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