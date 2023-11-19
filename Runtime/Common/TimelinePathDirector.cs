using Common;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Yanasep;
using Object = System.Object;

namespace TweenTimeline
{
    /// <summary>
    /// PlayableDirectorのBindingをPath文字列で保持するクラス (動的Bindingをやりやすく)
    /// PlayableDirectorと同じ階層につける
    /// </summary>
    public class TimelinePathDirector : MonoBehaviour, ISerializationCallbackReceiver
    {
        [SerializeField] private PlayableDirector _director;
        [SerializeField, ReadOnly] private TimelineBindingPathDictionary _bindingDic;
        // TODO: rootはTransformでなく文字列でやらないと、動的Bindingに使えなさそう。カスタムインスペクタでD&DでRoot文字列をセットしたい
        [SerializeField] private SerializableDictionary<TimelineAsset, Transform> _timelineRootPaths;
        
        public Object GetBinding(TimelineAsset timelineAsset, TrackAsset track)
        {
            if (!_timelineRootPaths.TryGetValue(timelineAsset, out var root))
            {
                root = transform;
            }

            return _bindingDic.Resolve(timelineAsset, track, root);
        }
        
        public void OnBeforeSerialize()
        {
            if (_director == null) return;
            
            _bindingDic ??= new();
            _bindingDic.ClearNull();
            
            var timelineAsset = _director.playableAsset as TimelineAsset;
            if (timelineAsset == null) return;

            _bindingDic.ClearTimeline(timelineAsset);

            foreach (var track in timelineAsset.GetOutputTracks())
            {
                var binding = _director.GetGenericBinding(track);
                if (binding == null) continue;
                var root = transform;
                if (_timelineRootPaths.TryGetValue(timelineAsset, out var rootPath))
                {
                    root = rootPath;
                }

                var trans = binding switch
                {
                    Component comp => comp.transform,
                    GameObject go => go.transform,
                    _ => null
                };
                if (trans == null) continue;
                _bindingDic.AddPath(timelineAsset, track, GetPath(trans, root));
            }
        }

        public void OnAfterDeserialize()
        {
        }
        
        public static string GetPath(Transform self, Transform root = null)
        {
            string path = self.name;

            if (self == root) return path;

            Transform parent = self.parent;

            while (parent != null && parent != root)
            {
                path = parent.name + "/" + path;
                parent = parent.parent;
            }

            return path;
        }
    }
}