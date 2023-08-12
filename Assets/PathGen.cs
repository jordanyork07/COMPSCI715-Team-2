using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathGen : MonoBehaviour
{
    [Serializable]
    enum Verb
    {
        Move,
        Jump
    }

    static float JUMP_FREQUENCY = 0.8f;

    [Serializable]
    record Action
    {
        public Verb verb;
        public float startTime;
        public float duration;

        public Action(Verb verb, float startTime, float duration)
        {
            this.verb = verb;
            this.startTime = startTime;
            this.duration = duration;
        }
    }

    enum Pattern
    {
        Regular,
        Swing,
        Random
    }

    enum Density
    {
        Low,
        Medium,
        High
    }

    // TODO: Decide on jump variations
    // Short, Long
    private static float[] jumpLengths = { 1.0f, 2.4f };

    Verb chooseRandomVerb()
    {
        int rand = UnityEngine.Random.Range(0, 2);
        if (rand == 1)
        {
            return Verb.Move;
        }
        else
        {
            return Verb.Jump;
        }
    }

    void Dump(object obj)
    {
        var str = JsonUtility.ToJson(obj);
        Debug.Log(str);
    }

    void GenerateRegularRhythm(Density density, int length)
    {
        Debug.Log("Generating regular rhythm density=" + density + ", length=" + length);

        var lastJumpStartTime = 0;
        var lastJumpDuration = 0;

        // Chose spacing between beats
        float actionStep = density switch
        {
            Density.Low => length / 3.0f,
            Density.Medium => length / 5.0f,
            Density.High => length / 10.0f,
            _ => length / 3.0f
        };

        List<Action> actions = new List<Action>();

        // Add initial move beat for entire duration
        // TODO: Allow pausing/waiting like in (Smith et al., 2009)
        actions.Add(new Action(Verb.Move, 0, length));

        for (float i = 0; i < length; i += actionStep)
        {
            if (lastJumpStartTime + lastJumpDuration > i)
            {
                // If last jump is still happening, skip this beat
                continue;
            }
            if (UnityEngine.Random.value < JUMP_FREQUENCY)
            {
                // Add jump beat
                actions.Add(new Action(Verb.Jump, (float)i, jumpLengths[(int)((UnityEngine.Random.value * 13) % 2)]));
            }

        }

        foreach (var action in actions)
        {
            Dump(action);
        }
    }

    void GenerateRhythm(Pattern type, Density density, int length)
    {
        switch (type)
        {
            case Pattern.Regular:
                GenerateRegularRhythm(density, length);
                break;
            case Pattern.Swing:
                Debug.Log("Generating swing rhythm");
                break;
            case Pattern.Random:
                Debug.Log("Generating random rhythm");
                break;
            default:
                GenerateRegularRhythm(density, length);
                break;
        };
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateRhythm(Pattern.Regular, Density.Low, 20);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
