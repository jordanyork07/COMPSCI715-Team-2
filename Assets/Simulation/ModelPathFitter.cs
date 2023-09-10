using System;
using static UnityEngine.UI.Image;
using System.Drawing;
using UnityEngine;
using Random = UnityEngine.Random;

public class ModelPathFitter : PathFitter
{
	public ModelPathFitter()
	{
	}

	protected override GameObject CreateModel()
	{
        // Create an array of prefab names
        string[] prefabNames = { "SM_Env_DirtMound_01", "SM_Env_DirtMound_02", "SM_Env_DirtMound_03", "SM_Env_DirtMound_04", "SM_Env_DirtMound_05" };

        // Randomly select a prefab name from the array
        string selectedPrefabName = prefabNames[Random.Range(0, prefabNames.Length)];

        // Load the "SM_Env_DirtMount_01" prefab from the PolygonAdventure asset
        GameObject modelPrefab = Resources.Load<GameObject>(selectedPrefabName);

        if (modelPrefab != null)
        {
            // Instantiate the modelPrefab
            GameObject model = Instantiate(modelPrefab, Vector3.zero, Quaternion.identity);
            return model;
        }
        else
        {
            Debug.LogError("Model prefab not found! Make sure the path is correct.");
            return base.CreateModel();
        }
    }
}

