using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[CustomEditor(typeof(PathGen))]
public class PathGenEditor : Editor
{
    private PathGen pathGen;

    private void OnEnable()
    {
        pathGen = (PathGen)target;
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        if (GUILayout.Button("Generate"))
        {
            pathGen.MarkDirty();
        }
    }
}

public class PathGen : MonoBehaviour
{
    public UIRenderer uiRenderer;
    public Pattern pattern = Pattern.Regular;
    public Density density = Density.High;
    public int length = 20;

    private List<Action> _actions = new();

    [Serializable]
    public enum Verb
    {
        Move,
        Jump,
        DoubleJump

    }

    public float jumpFrequency = 0.8f;

    [Serializable]
    public record Action
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

    public enum Pattern
    {
        Regular,
        Swing,
        Random,
        Markov
    }

    public enum Density
    {
        Low,
        Medium,
        High
    }

    // TODO: Decide on jump variations
    // Short, Long
    public float[] jumpLengths = { 0.8f, 1.6f };
    public Dictionary<Density, float> actionStepMappings = new()
    {
        { Density.Low, 3.0f },
        { Density.Medium, 2.0f },
        { Density.High, 1.0f },
    };
    Distribution<float> jumpLengthDist = new DiscreteUniformDistribution<float>(new[] { 0.8f, 1.6f });

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

    void DrawLine(List<Vector2> points, float x, float width)
    {
        float y_pos = 0f;
        points.Add(new Vector2(x, y_pos));
        points.Add(new Vector2(x + width, y_pos));
    }

    void DrawNotch(List<Vector2> points, float x)
    {
        float y_pos = 0f;
        points.Add(new Vector2(x, y_pos));
        points.Add(new Vector2(x, y_pos + 1));
        points.Add(new Vector2(x, y_pos + 1));
        points.Add(new Vector2(x, y_pos));
    }

    void RenderActions(List<Action> actions)
    {
        if (!uiRenderer)
        {
            return;
        }

        var points = new List<Vector2>();
        foreach (var action in actions)
        {
            Dump(action);
            DrawNotch(points, action.startTime);
            DrawLine(points, action.startTime, action.duration);
        }

        uiRenderer.points = points;
    }

    List<Action> GenerateMarkovActions(int length) {
        Debug.Log("Generating markov actions length=" + length);

        Distribution<float> actionTimeDist = new TriangularDistribution(1f, 2f); // time between actions
        Distribution<float> jumpDurationDist = new DiscreteUniformDistribution<float>(jumpLengths); // TODO: make different from jumpLengthDist

        List<Action> actions = new List<Action>();
        actions.Add(new Action(Verb.Move, 0, length)); // initial move action
        // NOTE: not stopping movement atm

        float time = 0;
        while (time < length) {
            float actionTime = actionTimeDist.Sample();
            time += actionTime;

            if (UnityEngine.Random.value < jumpFrequency) {
                float jumpDuration = jumpDurationDist.Sample();
                time += jumpDuration;

                actions.Add(new Action(Verb.Jump, time, jumpDuration));
            }
        }

        RenderActions(actions);
        return actions;
    }

    List<Action> GenerateRandomRhythm(Density density, int length)
    {
        Debug.Log("Generating random rhythm density=" + density + ", length=" + length);

        var lastJumpStartTime = 0f;
        var lastJumpDuration = 1f; // No jumps in the first second >:(

        // Chose spacing between beats
        float actionStep = actionStepMappings[density];

        List<Action> actions = new List<Action>();

        // Add initial move beat for entire duration
        // TODO: Allow pausing/waiting like in (Smith et al., 2009)
        actions.Add(new Action(Verb.Move, 0, length));

        float i = 0;
        while (i < length)
        {
            var interval = Random.value * actionStep;
            i += interval;
            if (lastJumpStartTime + lastJumpDuration > i)
            {
                // If last jump is still happening, skip this beat
                continue;
            }
            if (UnityEngine.Random.value < jumpFrequency)
            {
                // Add jump beat
                lastJumpStartTime = i;
                lastJumpDuration = jumpLengths[(int)((UnityEngine.Random.value * 13) % 2)];
                float randomValue = UnityEngine.Random.value;

                if (randomValue < 0.5f)
                {
                    actions.Add(new Action(Verb.Jump, (float)lastJumpStartTime, lastJumpDuration));

                }
                else
                {
                    actions.Add(new Action(Verb.DoubleJump, (float)lastJumpStartTime, lastJumpDuration));

                }
            }
        }

        RenderActions(actions);
        return actions;
    }

    List<Action> GenerateRegularRhythm(Density density, int length)
    {
        Debug.Log("Generating regular rhythm density=" + density + ", length=" + length);

        var lastJumpStartTime = 0f;
        var lastJumpDuration = 1f; // No jumps in the first second >:(

        // Chose spacing between beats
        float actionStep = actionStepMappings[density];

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
            if (UnityEngine.Random.value < jumpFrequency)
            {
                // Add jump beat
                lastJumpStartTime = i;
                lastJumpDuration = jumpLengths[(int)((UnityEngine.Random.value * 13) % 2)];
                float randomValue = UnityEngine.Random.value;
                if(randomValue < 0.5f)
                {
                    actions.Add(new Action(Verb.Jump, (float)lastJumpStartTime, lastJumpDuration));

                }
                else
                {
                    actions.Add(new Action(Verb.DoubleJump, (float)lastJumpStartTime, lastJumpDuration));

                }
            }

        }

        RenderActions(actions);
        return actions;
    }

    public List<Action> GenerateRhythm(Pattern type, Density density, int length)
    {
        return type switch
        {
            Pattern.Regular => GenerateRegularRhythm(density, length),
            Pattern.Random => GenerateRandomRhythm(density, length),
            Pattern.Swing => GenerateRegularRhythm(density, length),
            Pattern.Markov => GenerateMarkovActions(length),
            _ => GenerateRegularRhythm(density, length),
        };
    }

    public int GetArrayHash(float[] array)
    {
        int hc = array.Length;
        foreach (int val in array)
        {
            hc = unchecked(hc * 314159 + val);
        }

        return hc;
    }

    public void MarkDirty()
    {
        _actions = GenerateRhythm(pattern, density, length);
        uiRenderer.SetVerticesDirty();
    }

    public List<Action> GetRhythm()
    {
        var hash = pattern.GetHashCode() + density.GetHashCode() + length.GetHashCode() + jumpFrequency.GetHashCode() + GetArrayHash(jumpLengths);

        // Only recreate if something has changed
        if (hash != propertyHash)
        {
            propertyHash = hash;
            MarkDirty();
        }

        return _actions;
    }

    private int propertyHash = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        GetRhythm();
    }
}
