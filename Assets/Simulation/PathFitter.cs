using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using System.IO;
using Random = UnityEngine.Random;

[ExecuteInEditMode]
public class PathFitter : MonoBehaviour
{
    public List<Vector3> path;
    private int propertyHash;

    private List<GameObject> models = new();

    // public Vector3 modelScale = new Vector3(1.0f, 1.0f, 1.0f);
    public Color cubeColor = Color.white;
    
    public Vector2 minMaxScale = new Vector2(1f, 1f);
    public Vector2 minMaxVerticalRange = new Vector2(1f, 1f);
    public bool randomiseRotation = true;
    public float opacity = 1.0f;
    public bool shouldDoCollision = true;
    public bool animateIn = false;
    public AnimationCurve AnimationCurve;

    private List<Animatable> _animatables = new();

    private record Animatable
    {
        public GameObject target;
        public Vector3 start;
        public Vector3 end;
        public float frameTime;
        public float finalTime;

        public Animatable(GameObject target, Vector3 start, Vector3 end, float frameTime, float finalTime)
        {
            this.target = target;
            this.start = start;
            this.end = end;
            this.finalTime = finalTime;
            this.frameTime = frameTime;
        }
    }

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

    protected virtual GameObject CreateModel()
    {
        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.GetComponent<Renderer>().material.color = cubeColor;
        return cube;
    }

    void AddModelsAtPoints()
    {
        foreach (var point in path)
        {
            // Randomness
            var rotation = Quaternion.identity;
            var scale = Random.Range(minMaxScale.x, minMaxScale.y);
            var vert = Random.Range(minMaxVerticalRange.x, minMaxVerticalRange.y);

            if (randomiseRotation)
                rotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);
            
            // Create a GameObject
            GameObject model = CreateModel();
            model.transform.parent = transform;
            model.transform.localScale = new Vector3(scale, scale, scale);
            model.transform.rotation = rotation;
            model.transform.position = point + new Vector3(0f, vert, 0f);

            if (!shouldDoCollision && model.TryGetComponent<Collider>(out var component))
                DestroyImmediate(component);

            models.Add(model);

            if (animateIn)
            {
                var position = model.transform.position;
                var start = position - new Vector3(0, 10f, 0);
                _animatables.Add(new Animatable(model, start, position, 0, 1.0f));
                model.transform.position = start;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        foreach (var anim in _animatables.ToArray())
        {
            anim.frameTime += Time.deltaTime;
            var t = Math.Clamp(anim.frameTime / anim.finalTime, 0f, 1f);

            var easing = AnimationCurve.Evaluate(t);
            anim.target.transform.position = Vector3.Slerp(anim.start, anim.end, easing);
            
            if (anim.frameTime >= anim.finalTime)
                _animatables.Remove(anim);
        }
        
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