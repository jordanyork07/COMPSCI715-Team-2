using System;
using static UnityEngine.UI.Image;
using System.Drawing;
using UnityEngine;

public class ModelPathFitter : PathFitter
{
	public ModelPathFitter()
	{
	}

	protected override GameObject CreateModel()
	{
        // Load the "SM_Env_DirtMount_01" prefab from the PolygonAdventure asset
        GameObject modelPrefab = Resources.Load<GameObject>("SM_Env_DirtMound_01");

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

