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
        Left,
        Right,
        Sprint,
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
        Random
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

    Verb chooseValidVerb(bool canJump, bool canSprint)
    {
        List<Verb> validVerbs = new List<Verb> {
            Verb.Left,
            Verb.Right
        };

        if (canJump)
        {
            validVerbs.Add(Verb.Jump);
            validVerbs.Add(Verb.DoubleJump);
            // hack to make jumping more likely
            validVerbs.Add(Verb.Jump);
            validVerbs.Add(Verb.DoubleJump);
            validVerbs.Add(Verb.Jump);
            validVerbs.Add(Verb.DoubleJump);
        }

        if (canSprint) {
            validVerbs.Add(Verb.Sprint);
        }

        Distribution<Verb> distribution = new DiscreteUniformDistribution<Verb>(validVerbs.ToArray());
        return distribution.Sample();
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

    List<Action> GenerateRandomRhythm(Density density, int length)
    {
        Debug.Log("Generating random rhythm density=" + density + ", length=" + length);

        var canJumpAfter = 1f;
        var canSprintAfter = 0f;

        // Chose spacing between beats
        float baseStep = actionStepMappings[density];
        Distribution<float> stepDistribution = new TriangularDistribution<float>(0.5f * baseStep, 1.5f * baseStep);

        List<Action> actions = new List<Action>();

        // Add initial move beat for entire duration
        // TODO: Allow pausing/waiting like in (Smith et al., 2009)
        actions.Add(new Action(Verb.Move, 0, length));

        float i = 0;
        while (i < length) {
            bool canJump = i > canJumpAfter;
            bool canSprint = i > canSprintAfter;
            
            Verb verb = chooseValidVerb(canJump, canSprint);
            float actionDuration = actionStep;


            switch (verb) {
                case Verb.Jump:
                case Verb.DoubleJump:
                    actionDuration = jumpLengths[(int)((UnityEngine.Random.value * 13) % 2)];
                    canJumpAfter = i + actionDuration;
                    break;
                case Verb.Sprint:
                    actionDuration = actionStep * 1.5f;
                    canSprintAfter = i + actionDuration;
                    break;
                default:
                    break;
            }
           
            actions.Add(new Action(verb, i, actionDuration));

            i += stepDistribution.Sample();
        }

        return actions;
    }

    List<Action> GenerateRegularRhythm(Density density, int length)
    {
        Debug.Log("Generating regular rhythm density=" + density + ", length=" + length);

        var canJumpAfter = 1f;
        var canSprintAfter = 0f;

        // Chose spacing between beats
        float actionStep = actionStepMappings[density];

        List<Action> actions = new List<Action>();

        // Add initial move beat for entire duration
        // TODO: Allow pausing/waiting like in (Smith et al., 2009)
        actions.Add(new Action(Verb.Move, 0, length));

        for (float i = 0; i < length; i += actionStep)
        {
            bool canJump = i > canJumpAfter;
            bool canSprint = i > canSprintAfter;
            
            Verb verb = chooseValidVerb(canJump, canSprint);
            float actionDuration = actionStep;


            switch (verb) {
                case Verb.Jump:
                case Verb.DoubleJump:
                    actionDuration = jumpLengths[(int)((UnityEngine.Random.value * 13) % 2)];
                    canJumpAfter = i + actionDuration;
                    break;
                case Verb.Sprint:
                    actionDuration = actionStep * 1.5f;
                    canSprintAfter = i + actionDuration;
                    break;
                default:
                    break;
            }
           
            actions.Add(new Action(verb, i, actionDuration));
        }

        return actions;

    }

    List<Action> GenerateSwingRhythm(Density density, int length)
    {
        Debug.Log("Generating swing rhythm density=" + density + ", length=" + length);

        var canJumpAfter = 1f;
        var canSprintAfter = 0f;

        // Chose spacing between beats
        float actionStep = actionStepMappings[density];
        float swingStep = actionStep * 0.125f; // 1/8th of a beat

        List<Action> actions = new List<Action>();

        // Add initial move beat for entire duration
        // TODO: Allow pausing/waiting like in (Smith et al., 2009)
        actions.Add(new Action(Verb.Move, 0, length));

        bool isSwingStep = false;
        float i = 0;
        while (i < length) {
            

            bool canJump = i > canJumpAfter;
            bool canSprint = i > canSprintAfter;
            
            Verb verb = chooseValidVerb(canJump, canSprint);
            float actionDuration = actionStep;


            switch (verb) {
                case Verb.Jump:
                case Verb.DoubleJump:
                    actionDuration = jumpLengths[(int)((UnityEngine.Random.value * 13) % 2)];
                    canJumpAfter = i + actionDuration;
                    break;
                case Verb.Sprint:
                    actionDuration = actionStep * 1.5f;
                    canSprintAfter = i + actionDuration;
                    break;
                default:
                    break;
            }
           
            actions.Add(new Action(verb, i, actionDuration));

            var stepDuration = isSwingStep ? swingStep : actionStep;
            i += stepDuration;
            isSwingStep = !isSwingStep;
        }

        return actions;
    }

    public List<Action> GenerateRhythm(Pattern type, Density density, int length)
    {
        List<Action> actions = type switch {
            Pattern.Regular => GenerateRegularRhythm(density, length),
            Pattern.Random => GenerateRandomRhythm(density, length),
            Pattern.Swing => GenerateSwingRhythm(density, length),
            _ => GenerateRegularRhythm(density, length),
        };
        RenderActions(actions);
        return actions;
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
