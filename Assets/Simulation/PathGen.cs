using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        Jump
    }

    static float JUMP_FREQUENCY = 0.8f;

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
    private static float[] jumpLengths = { 0.8f, 1.6f, 2.4f, 3.2f };

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

    void GenerateRandomRhythm(Density density, int length)
    {
        Debug.Log("Generating regular rhythm density=" + density + ", length=" + length);

        var lastJumpStartTime = 0f;
        var lastJumpDuration = 0f;

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
                lastJumpStartTime = i;
                lastJumpDuration = jumpLengths[(int)((UnityEngine.Random.value * 13) % 2)];
                actions.Add(new Action(Verb.Jump, (float)lastJumpStartTime, lastJumpDuration));
            }

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

    List<Action> GenerateRegularRhythm(Density density, int length)
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

        if (uiRenderer)
        {
            var points = new List<Vector2>();
            foreach (var action in actions)
            {
                Dump(action);
                DrawNotch(points, action.startTime);
                DrawLine(points, action.startTime, action.duration);
            }

            uiRenderer.points = points;
        }

        return actions;
    }

    public List<Action> GenerateRhythm(Pattern type, Density density, int length)
    {
        return type switch
        {
            Pattern.Regular => GenerateRegularRhythm(density, length),
            Pattern.Swing => GenerateRegularRhythm(density, length),
            _ => GenerateRegularRhythm(density, length),
        };
    }

    public List<Action> GetRhythm()
    {
        var hash = pattern.GetHashCode() + density.GetHashCode() + length.GetHashCode();

        // Only recreate if something has changed
        if (hash != propertyHash)
        {
            propertyHash = hash;
            _actions = GenerateRhythm(pattern, density, length);
            uiRenderer.SetVerticesDirty();
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
