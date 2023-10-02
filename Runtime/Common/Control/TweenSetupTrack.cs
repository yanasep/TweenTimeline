using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.UI;
using Yanasep;

namespace TweenTimeline
{
    /// <summary>
    /// Tweenセットアップトラック
    /// </summary>
    [DisplayName("Tween/Tween Setup Track")]
    // [TrackBindingType(typeof(Graphic))]
    [TrackClipType(typeof(TweenSetupClip))]
    public class TweenSetupTrack : TrackAsset
    {
        /// <inheritdoc/>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var director = (PlayableDirector)graph.GetResolver();

            var parameterHolder = director.GetComponent<TweenParameterHolder>();
            var parameter = parameterHolder != null ? parameterHolder.GetParameter() : new TweenParameter();

            return ScriptPlayable<TweenSetupMixerBehaviour>.Create(graph, inputCount);
        }
    }

    [Serializable]
    public class TweenSetupMixerBehaviour : PlayableBehaviour
    {
        private bool initialized;
        
        public override void OnPlayableCreate(Playable playable)
        {
            // TODO : TweenMixerBehaviourでもよさそう
            if (initialized) return;
            if (!playable.IsValid())
            {
                return;
            }

            var inputs = new List<Playable>();
            GetAllPlayables(playable.GetGraph(), inputs);

            for (int i = 0; i < inputs.Count; i++)
            {
                Playable input = inputs[i];
                if (TryGetBehaviour<TweenBehaviour>(input, out var tweenBehaviour))
                {
                    // TODO: Parameter反映
                    Debug.Log($"tweenBehaviour : {tweenBehaviour}");
                }
                else if (TryGetBehaviour<DirectorControlPlayable>(input, out var controlPlayable))
                {
                    var graph = controlPlayable.director.playableGraph;
                    if (graph.IsValid())
                    {
                        GetAllPlayables(graph, inputs);
                    }
                }
            }
        }

        private bool TryGetBehaviour<T>(Playable playable, out T behaviour) where T : PlayableBehaviour, new()
        {
            Debug.Log($"playable.GetPlayableType() : {playable.GetPlayableType()}");
            if (playable.GetPlayableType() == typeof(T))
            {
                behaviour = ((ScriptPlayable<T>)playable).GetBehaviour();
                return true;
            }

            behaviour = null;
            return false;
        }

        public static void GetAllPlayables(PlayableGraph playableGraph, List<Playable> results)
        {
            if (!playableGraph.IsValid())
            {
                return;
            }

            int outputCount = playableGraph.GetOutputCount();
            for (int i = 0; i < outputCount; i++)
            {
                PlayableOutput output = playableGraph.GetOutput(i);

                if (!output.IsOutputValid() || !output.IsPlayableOutputOfType<ScriptPlayableOutput>())
                {
                    continue;
                }

                int sourceOutputPort = output.GetSourceOutputPort();
                Playable playable = output.GetSourcePlayable().GetInput(sourceOutputPort);

                GetAllPlayables(playable, results);
            }
        }

        public static void GetAllPlayables(Playable playable, List<Playable> results)
        {
            if (!playable.IsValid())
            {
                return;
            }

            int inputCount = playable.GetInputCount();
            for (int i = 0; i < inputCount; i++)
            {
                Playable input = playable.GetInput(i);

                if (input.GetInputCount() > 0)
                {
                    GetAllPlayables(input, results);
                }
                else
                {
                    if (!results.Contains(input))
                    {
                        results.Add(input);
                    }
                }
            }
        }
    }
}
