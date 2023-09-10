using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface Distribution<T>
{
    T Sample();
}

public class IdentityDistribution<T> : Distribution<T>
{
    T val;
    public IdentityDistribution(T val) {
        this.val = val;
    }

    public T Sample()
    {
        return val;
    }
}

public class TriangularDistribution : Distribution<float>
{
    float lower;
    float upper;
    float mode; 
    float f_mode; // fraction of the way between lower and upper that the mode is

    public TriangularDistribution(float lower, float upper, float? mode = null)
    {
        if (mode == null) {
            mode = (upper + lower) / 2;
        }
        float fmode = (float)mode;

        this.lower = lower;
        this.upper = upper;
        this.mode = fmode;
        this.f_mode = (fmode - lower) / (upper - lower);
    }

    public float Sample() {
        float rand = UnityEngine.Random.value;

        if (rand < f_mode) { 
            return lower + Mathf.Sqrt(rand * (upper - lower) * (mode - lower));
        } 
        
        return upper - Mathf.Sqrt((1 - rand) * (upper - lower) * (upper - mode));
    }
}

public class DiscreteUniformDistribution<T> : Distribution<T>
{
    T[] buckets;
    int numBuckets;

    public DiscreteUniformDistribution(T[] buckets)
    {
        this.buckets = buckets;
        this.numBuckets = buckets.Length;
    }

    public T Sample() {
        int rand = UnityEngine.Random.Range(0, numBuckets);
        return buckets[rand];
    }

}