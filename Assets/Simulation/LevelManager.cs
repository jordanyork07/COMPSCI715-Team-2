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

    public bool useOrder = false;
    private int orderedLevelIndex = 0;
    public List<int> order = new();

    void Start()
    {
        var fitter = GetComponent<PathFitter>();
        GetComponent<PlayerSimulator>().pathFitter = fitter;
        InitializeLevels();
        SimulateLevels();
        Restart();
    }

    void InitializeLevels() 
    {
        // Remove extra levels
        if (levels.Count > numberOfLevels)
        {
            foreach (var level in levels.GetRange(numberOfLevels, levels.Count - numberOfLevels))
            {
                DestroyImmediate(level);
            }
            
            levels = levels.GetRange(0, numberOfLevels);
        }

        // setup empty levels        
        for (int i = 0; i < numberOfLevels; i++)
        {   
            if (i < levels.Count) continue;

            GameObject level = new GameObject("Level " + i);
            level.transform.parent = transform;
            levels.Add(level); 
            
            if (i != activeLevel) {
                level.SetActive(false);
            }
        }
    }

    void Update() 
    {
        if (useOrder && order.Count == numberOfLevels)
        
        {
            if (orderedLevelIndex >= order.Count) orderedLevelIndex = 0;
            if (order[orderedLevelIndex] != activeLevel) GoToLevel(order[orderedLevelIndex]);
        }
        // Calling InitializeLevels causes funk
    }

    
    private void Reset()
    {
        levels.ForEach((level) =>
        {
            DestroyImmediate(level);
        });
        levels.Clear();
        InitializeLevels();
    }

    public void SimulateLevels()
    {   
        if (!simulate) return ;
        Reset();
        Restart();

        for (int i = 0; i < numberOfLevels; i++)
        {
            var simulator = GetComponent<PlayerSimulator>();
            simulator.StartSimulation();
            List<GameObject> levelModels = simulator.pathFitter.GetModels();
            
            if (levelModels.Count == 0)
            {
                Debug.LogError("No models found for simulation " + i);
            }
            
            GameObject level = levels[i];
            level.transform.parent = transform;
            // level.transform.position = new Vector3(0, 0, 0);
            // level.transform.rotation = Quaternion.identity;
            // level.transform.localScale = new Vector3(0.1f, 0.1f, 0.1);

            foreach (var model in levelModels)
            {
                model.transform.parent = level.transform;
            }            
        }
    }

    private void GoToLevel(int level)
    {
        if (level < numberOfLevels) {

            levels[activeLevel].SetActive(false);
        }
        activeLevel = level;
        levels[activeLevel].SetActive(true);
    }

    public void Restart()
    {
        if (useOrder && order.Count == numberOfLevels)
        {
            GoToLevel(order[0]);
            orderedLevelIndex = 0;  
        }
        else 
        {
            GoToLevel(0);
        }
    }
    
    public void NextSimulation()
    {   
        if (useOrder && orderedLevelIndex < order.Count && order.Count == numberOfLevels)
        {
            GoToLevel(order[orderedLevelIndex++]);
        }
        else 
        {
            GoToLevel((activeLevel + 1) % numberOfLevels);
        }
    }

    
}