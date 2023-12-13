using System.Collections.Generic;
using UnityEngine;

namespace TweenTimeline
{
    /// <summary>
    /// タイムラインに渡すパラメータを保持するクラス
    /// </summary>
    public class TweenParameter
    {   
        private readonly Dictionary<uint, float> floats = new();
        private readonly Dictionary<uint, int> ints = new();
        private readonly Dictionary<uint, bool> bools = new();
        private readonly Dictionary<uint, Vector3> vector3s = new();
        private readonly Dictionary<uint, Vector2> vector2s = new();
        private readonly Dictionary<uint, Color> colors = new();
        
        private readonly Dictionary<string, uint> nameToIdDic = new();
        
        private void AddParameter<T>(Dictionary<uint, T> dic, uint parameterId, string parameterName, T value)
        {
            nameToIdDic.Add(parameterName, parameterId);
            dic.Add(parameterId, value);
        }

        private void SetParameter<T>(Dictionary<uint, T> dic, uint parameterId, T value)
        {
            dic[parameterId] = value;
        }

        private void SetParameter<T>(Dictionary<uint, T> dic, string parameterName, T value)
        {
            if (nameToIdDic.TryGetValue(parameterName, out var id))
            {
                SetParameter(dic, id, value);
            }
            else
            {
                Debug.LogWarning($"{parameterName} is not found.");
            }
        }

        private T GetParameterOrDefault<T>(Dictionary<uint, T> dic, string parameterName)
        {
            if (nameToIdDic.TryGetValue(parameterName, out var id))
            {
                return GetParameterOrDefault(dic, id);
            }
            else
            {
                Debug.LogWarning($"{parameterName} is not found.");
                return default;
            }
        }

        public void Clear()
        {
            floats.Clear();
            ints.Clear();
            bools.Clear();
            vector3s.Clear();
            vector2s.Clear();
            colors.Clear();
            nameToIdDic.Clear();
        }

        private T GetParameterOrDefault<T>(Dictionary<uint, T> dic, uint parameterId)
        {
            return dic.GetValueOrDefault(parameterId, default);
        }

        public void SetFloat(uint parameterId, float value) => SetParameter(floats, parameterId, value);
        public void SetFloat(string parameterName, float value) => SetParameter(floats, parameterName, value);
        public float GetFloat(uint parameterId) => GetParameterOrDefault(floats, parameterId);
        public float GetFloat(string parameterName) => GetParameterOrDefault(floats, parameterName);
        public void SetInt(uint parameterId, int value) => SetParameter(ints, parameterId, value);
        public void SetInt(string parameterName, int value) => SetParameter(ints, parameterName, value);
        public int GetInt(uint parameterId) => GetParameterOrDefault(ints, parameterId);
        public int GetInt(string parameterName) => GetParameterOrDefault(ints, parameterName);
        public void SetBool(uint parameterId, bool value) => SetParameter(bools, parameterId, value);
        public void SetBool(string parameterName, bool value) => SetParameter(bools, parameterName, value);
        public bool GetBool(uint parameterId) => GetParameterOrDefault(bools, parameterId);
        public bool GetBool(string parameterName) => GetParameterOrDefault(bools, parameterName);
        public void SetVector3(uint parameterId, Vector3 value) => SetParameter(vector3s, parameterId, value);
        public void SetVector3(string parameterName, Vector3 value) => SetParameter(vector3s, parameterName, value);
        public Vector3 GetVector3(uint parameterId) => GetParameterOrDefault(vector3s, parameterId);
        public Vector3 GetVector3(string parameterName) => GetParameterOrDefault(vector3s, parameterName);
        public void SetVector2(uint parameterId, Vector2 value) => SetParameter(vector2s, parameterId, value);
        public void SetVector2(string parameterName, Vector2 value) => SetParameter(vector2s, parameterName, value);
        public Vector2 GetVector2(uint parameterId) => GetParameterOrDefault(vector2s, parameterId);
        public Vector2 GetVector2(string parameterName) => GetParameterOrDefault(vector2s, parameterName);
        public void SetColor(uint parameterId, Color value) => SetParameter(colors, parameterId, value);
        public void SetColor(string parameterName, Color value) => SetParameter(colors, parameterName, value);
        public Color GetColor(uint parameterId) => GetParameterOrDefault(colors, parameterId);
        public Color GetColor(string parameterName) => GetParameterOrDefault(colors, parameterName);

        internal void AddParameter<T>(uint parameterId, string parameterName, T value)
        {
            AddParameter(GetParameterDic<T>(), parameterId, parameterName, value);
        }
        
        internal void SetParameter<T>(uint parameterId, T value)
        {
            SetParameter(GetParameterDic<T>(), parameterId, value);
        }

        private Dictionary<uint, T> GetParameterDic<T>()
        {
            if (typeof(T) == typeof(float))
            {
                return floats as Dictionary<uint, T>;
            }
            else if (typeof(T) == typeof(int))
            {
                return ints as Dictionary<uint, T>;
            }
            else if (typeof(T) == typeof(bool))
            {
                return bools as Dictionary<uint, T>;
            }
            else if (typeof(T) == typeof(Vector3))
            {
                return vector3s as Dictionary<uint, T>;
            }
            else if (typeof(T) == typeof(Vector2))
            {
                return vector2s as Dictionary<uint, T>;
            }
            else if (typeof(T) == typeof(Color))
            {
                return colors as Dictionary<uint, T>;
            }
            else
            {
                Debug.LogError($"Unsupported type: {typeof(T)}");
                return null;
            }
        }
    }
}
