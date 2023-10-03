﻿using System.Collections.Generic;
using System.ComponentModel;
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
        private bool _initialized;
        private List<TweenPlayableBehaviour> _tweenBehaviours = new();
        private List<TweenSetupMixerBehaviour> _childSetups = new();

        private void Initialize(PlayableGraph graph)
        {
            if (_initialized) return;
            _initialized = true;
            
            // キャッシュしつつ初期化
            var director = (PlayableDirector)graph.GetResolver();
            var parameterHolder = director.GetComponent<TweenParameterHolder>();
            var parameter = parameterHolder != null ? parameterHolder.GetParameter() : null;
            
            var playables = new List<Playable>();
            TweenTimelineUtility.GetAllPlayables(graph, playables, includeChildDirector: false);

            for (int i = 0; i < playables.Count; i++)
            {
                var playable = playables[i];
                if (TweenTimelineUtility.TryGetBehaviour<TweenPlayableBehaviour>(playable, out var tweenBehaviour))
                {
                    _tweenBehaviours.Add(tweenBehaviour);
                    tweenBehaviour.ApplyOverrides(parameter);
                }
                else if (TweenTimelineUtility.TryGetBehaviour<DirectorControlPlayable>(playable, out var control))
                {
                    var childGraph = control.director.playableGraph;
                    var childSetup = TweenTimelineUtility.FindBehaviour<TweenSetupMixerBehaviour>(childGraph);
                    if (childSetup != null)
                    {
                        // 子のSetupビヘイビアがあれば、配下のPlayableは子が管理
                        _childSetups.Add(childSetup);
                        childSetup.Initialize(childGraph);
                    }
                    else
                    {
                        // 子にSetupビヘイビアがなければ、このビヘイビアが管理
                        TweenTimelineUtility.GetAllPlayables(childGraph, playables, includeChildDirector: false);
                    }
                }
            }
        }
        
        public override void OnGraphStart(Playable playable)
        {
            if (_initialized) return;
            Initialize(playable.GetGraph());
            
            var director = (PlayableDirector)playable.GetGraph().GetResolver();
            
            var parameterHolder = director.GetComponent<TweenParameterHolder>();
            var parameter = parameterHolder != null ? parameterHolder.GetParameter() : null;

            foreach (var setup in _childSetups)
            {
                setup.ApplyRootParameter(parameter);
            }
        }
        
        // TODO: 中間層のparameterは反映されない？

        private void ApplyRootParameter(TweenParameter parameter)
        {
            foreach (var behaviour in _tweenBehaviours)
            {
                behaviour.ApplyOverrides(parameter);
            }

            foreach (var child in _childSetups)
            {
                child.ApplyRootParameter(parameter);
            }
        }
    }
}