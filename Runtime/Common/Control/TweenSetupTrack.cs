using System.Collections.Generic;
using System.ComponentModel;
using Common;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    /// <summary>
    /// Tweenセットアップトラック
    /// </summary>
    [DisplayName("Tween/Tween Setup Track")]
    [TrackClipType(typeof(TweenSetupClip))]
    public class TweenSetupTrack : TrackAsset
    {
        /// <inheritdoc/>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            return ScriptPlayable<TweenSetupMixerBehaviour>.Create(graph, inputCount);
        }
    }

    public class TweenSetupMixerBehaviour : PlayableBehaviour
    {
        private readonly List<RuntimeTweenParameterHolder> _runtimeParamHolders = new();
        
        public override void OnGraphStart(Playable playable)
        {
            var graph = playable.GetGraph();
            var director = (PlayableDirector)graph.GetResolver();
            var parameterHolder = director.GetComponent<TweenParameterHolder>();
            var parameter = parameterHolder == null ? new TweenParameter() : parameterHolder.GetParameter().Clone();
            // 親から受け継いだParameterを反映
            if (director.TryGetComponent<RuntimeTweenParameterHolder>(out var runtimeHolder))
            {
                parameter.OverwriteFrom(runtimeHolder.Parameter);
                runtimeHolder.Parameter = parameter;
            }
            else
            {
                runtimeHolder = director.gameObject.AddComponent<RuntimeTweenParameterHolder>();
                _runtimeParamHolders.Add(runtimeHolder);
                runtimeHolder.Parameter = parameter;
            }

            var playables = new List<Playable>();
            TweenTimelineUtility.GetAllPlayables(graph, playables);
            foreach (var p in playables)
            {
                if (TweenTimelineUtility.TryGetBehaviour<DirectorControlPlayable>(p, out var control))
                {
                    var holder = control.director.gameObject.GetOrAddComponent<RuntimeTweenParameterHolder>();
                    holder.Parameter = parameter.Clone();
                    _runtimeParamHolders.Add(holder);
                }
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            DestroyRuntimeParameters();
        }

        private void DestroyRuntimeParameters()
        {
            foreach (var holder in _runtimeParamHolders)
            {
                if (holder != null)
                {
                    if (Application.isPlaying) Object.Destroy(holder);
                    else Object.DestroyImmediate(holder);
                }
            }

            _runtimeParamHolders.Clear();   
        }
    }
}