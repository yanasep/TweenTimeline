using System;
using UnityEngine;
using Yanasep;

namespace TweenTimeline
{
    public enum CountryCode
    {
        UK, US, JPN, GER
    }
    
    public class CountryDB : ScriptableObject
    {
        public static CountryDB Instance => _instance ??= Resources.Load<CountryDB>("CountryDB");
        private static CountryDB _instance;
        
        [Serializable]
        public class CountryData
        {
            [field: SerializeField] public Sprite CountryFlag { get; private set; }
            [field: SerializeField] public string CountryName { get; private set; }
        }

        [SerializeField, EnumIndex(typeof(CountryCode))] private CountryData[] _db;

        public CountryData Get(CountryCode code)
        {
            return _db[(int)code];
        }
    }
}