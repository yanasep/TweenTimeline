using System;
using System.Linq;
using UnityEngine;
using UnityEngine.Timeline;
using Yanasep;
using Object = UnityEngine.Object;

namespace TweenTimeline
{
    [Serializable]
    public class TimelineBindingPathDictionary
    {
        [SerializeField] private SerializableDictionary<TimelineAsset, SerializableDictionary<TrackAsset, string>> _dic = new();
        
        public void ClearNull()
        {
            foreach (var asset in _dic.Keys.ToList())
            {
                if (asset == null) _dic.Remove(asset);
            }
        }

        public void Clear()
        {
            _dic.Clear();
        }

        public void ClearTimeline(TimelineAsset timelineAsset)
        {
            if (_dic.TryGetValue(timelineAsset, out var pathDic))
            {
                pathDic.Clear();
            }
        }

        public void AddPath(TimelineAsset timelineAsset, TrackAsset track, string bindingPath)
        {
            if (!_dic.TryGetValue(timelineAsset, out var pathDic))
            {
                pathDic = new();
                _dic[timelineAsset] = pathDic;
            }

            pathDic[track] = bindingPath;
        }

        public Object Resolve(TimelineAsset timelineAsset, TrackAsset track, Transform root)
        {
            if (!_dic.TryGetValue(timelineAsset, out var pathDic))
            {
                return null;
            }

            if (!pathDic.TryGetValue(track, out var path))
            {
                return null;
            }

            return root.Find(path);
        }
    }
}