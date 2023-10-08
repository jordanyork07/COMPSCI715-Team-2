using UnityEngine;
using System.Collections.Generic;
using static PlayerController;
using System;
using System.Linq;
using Simulation;
using static PlayerSimulator;

[RequireComponent(typeof(PlayerSimulator))]
[RequireComponent(typeof(PathFitter))]
[ExecuteInEditMode]
public class LevelManager : MonoBehaviour
{
    // public PlayerSimulator simulator;
    public bool simulate = true;
    public int numberOfLevels = 1;
    public int activeLevel = 0;
    public List<GameObject> levels = new(); 
    void Start()
    {
        var fitter = GetComponent<PathFitter>();
        // fitter.SetActive(false);
        GetComponent<PlayerSimulator>().pathFitter = fitter;
        SimulateLevels();

    }

    void Update() 
    {

    }

    
    private void Reset()
    {
        levels.ForEach((level) =>
        {
            DestroyImmediate(level);
        });
        levels = new List<GameObject>();
    }

    public void SimulateLevels()
    {   
        if (!simulate) return ;
        
        Reset();
        for (int i = 0; i < numberOfLevels; i++)
        {
            var simulator = GetComponent<PlayerSimulator>();
            simulator.StartSimulation();
            List<GameObject> levelModels = simulator.pathFitter.GetModels();
            
            if (levelModels.Count == 0)
            {
                Debug.LogError("No models found for simulation " + i);
            }
            
            GameObject level = new GameObject("Level " + i);
            level.transform.parent = transform;
            // level.transform.position = new Vector3(0, 0, 0);
            // level.transform.rotation = Quaternion.identity;
            // level.transform.localScale = new Vector3(0.1f, 0.1f, 0.1);

            foreach (var model in levelModels)
            {
                model.transform.parent = level.transform;
            }

            levels.Add(level); 

            if (i != activeLevel) {
                level.SetActive(false);
            }
        }
    }

    private void GoToLevel(int level)
    {
        levels[activeLevel].SetActive(false);
        activeLevel = level;
        levels[activeLevel].SetActive(true);
    }

    public void Restart()
    {
        GoToLevel(0);
    }
    
    public void NextSimulation()
    {
        GoToLevel((activeLevel + 1) % numberOfLevels);
    }

    
}