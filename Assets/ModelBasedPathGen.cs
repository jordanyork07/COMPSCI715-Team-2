using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathGen;

[Serializable]
public record AgentState
{
    /*
        Represents the state of the agent
    */
    public Input currentInput;
    public Vector3 position;
    public Vector3 velocity; // x = left/right, y = up/down, z = forward/backward

    public AgentState(Input currentInput)
    {
        this.currentInput = currentInput;
    }

    public AgentState clone()
    {
        return new AgentState(this.currentInput.clone());
    }
}

[Serializable]
public record Input
{
    /*
        Represents a simulated user input
    */
    public int timestamp; // milliseconds

    public const bool forward = true; // assumed true for now
    public bool left;
    public bool right;

    public const bool jump = false; // assumed false for now

    public Input(int timestamp) 
    {
        this.timestamp = timestamp;
        
        // this.forward = true;

        this.left = false;
        this.right = false;

        // this.jump = false;
    }

    public Input clone()
    {
        return new Input(this.timestamp);
    }
}

public class MarkovAgent {
    /*
        Simulates an action
    */
    // Probabilities of behaviours
    // a > prob indicates not performing the associated action
    // a <= prob indicates performing the associated action
    private float actionProbability = 0.8f;
    private float continueInCurrentDirectionProbability = 0.75f;
    private const float leftProbability = 0.5f;

    // TODO: inject this
    private int generateNextTimestamp(int timestamp) {
        return timestamp + UnityEngine.Random.Range(500, 1500);
    }

    public Input simulate(AgentState state) {
        Input nextInput = new Input(
            this.generateNextTimestamp(state.currentInput.timestamp)
        );
        
        // Don't do anything particularly special
        // Assumes default action is to move forwards
        if (UnityEngine.Random.Range(0.0f, 1.0f) > this.actionProbability) {
            return nextInput;
        } 

        // Keep moving in the same direction
        if (UnityEngine.Random.Range(0.0f, 1.0f) <= this.continueInCurrentDirectionProbability) {
            if (state.currentInput.left) {
                nextInput.left = true;
            } else if (state.currentInput.right) {
                nextInput.right = true;
            }
            return nextInput;
        } 

        // Change direction
        if (UnityEngine.Random.Range(0.0f, 1.0f) > this.leftProbability) {
            nextInput.right = true;
        } else {
            nextInput.left = true;
        }
    }
}


public class ModelController {
    /*
        Evaluates states depending on model input
        and previous state

        TODO: update to re-evaluate at non-input intervals
    */

    // Assumes constant acceleration atm
    // does not normalise acceleration as a vector
    private float accellerationFactor = 0.25f;
    private float maxHorizontalVelocity = 1.0f;
    // private float maxForwardVelocity = 1.0f;
    
    // TODO: consider removing the previous input from the agent 
    // state
    public AgentState evaluate(AgentState state, Input input) {
        // TODO: convert to Time.deltaTime? 
        float deltaTime = float(input.timestamp - state.currentInput.timestamp) / 1000.0f;

        float forwardVelocity = 1.0f; // constant for now
        float horizontalVelocity = AgentState.velocity.x;

        bool accelerateLeft = input.left;
        bool accelerateRight = input.right;

        // Keep moving forward baby
        if (!accelerateLeft && !accelerateRight) {
            AgentState newState = state.clone();
            newState.velocity = Vector3.forward
            newState.currentInput = input;
            newState.position = state.position + newState.velocity * deltaTime;
            return newState;
        }

        if (accelerateLeft) {
            horizontalVelocity -= this.accellerationFactor * deltaTime;
        } else if (accelerateRight) {
            horizontalVelocity += this.accellerationFactor * deltaTime;
        }
        horizontalVelocity = Mathf.Clamp(horizontalVelocity, -this.maxHorizontalVelocity, this.maxHorizontalVelocity);
        
        AgentState newState = state.clone();
        newState.velocity = Vector3.forward + Vector3.right * horizontalVelocity;
        newState.currentInput = input;
        newState.position = state.position + newState.velocity * deltaTime;
        return newState;
    }   
}



public class ModelBasedPathGeneration : MonoBehaviour
{
    /*
    Generates a path based on a model of player input.
    */
    public UIRenderer uiRenderer;
    public int timelimit = 20000; // 20 seconds of play time 



}