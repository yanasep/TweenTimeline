using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    [Serializable]
    public class Vector2Flags : FlagGroup
    {
        public bool X = true;
        public bool Y = true;

        public Vector2 Apply(Vector2 origin, Vector2 newValue)
        {
            if (X) origin.x = newValue.x;
            if (Y) origin.y = newValue.y;
            return origin;
        }
    }
    
    [Serializable]
    public class Vector3Flags : FlagGroup
    {
        public bool X = true;
        public bool Y = true;
        public bool Z = true;

        public Vector3 Apply(Vector3 origin, Vector3 newValue)
        {
            if (X) origin.x = newValue.x;
            if (Y) origin.y = newValue.y;
            if (Z) origin.z = newValue.z;
            return origin;
        }
    }
    
    [Serializable]
    public class RGBAFlags : FlagGroup
    {
        public bool R = true;
        public bool G = true;
        public bool B = true;
        public bool A = true;

        public Color Apply(Color origin, Color newValue)
        {
            if (R) origin.r = newValue.r;
            if (G) origin.g = newValue.g;
            if (B) origin.b = newValue.b;
            if (A) origin.a = newValue.a;
            return origin;
        }
    }
}