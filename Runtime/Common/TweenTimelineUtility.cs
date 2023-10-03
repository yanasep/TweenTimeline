using System.Collections.Generic;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace TweenTimeline
{
    /// <summary>
    /// TimelineのUtility
    /// </summary>
    public static class TweenTimelineUtility
    {
        /// <summary>
        /// Playableから型指定でビヘイビアを取得
        /// </summary>
        public static bool TryGetBehaviour<T>(Playable playable, out T behaviour) where T : PlayableBehaviour, new()
        {
            if (typeof(T).IsAssignableFrom(playable.GetPlayableType()))
            {
                behaviour = ((ScriptPlayable<T>)playable).GetBehaviour();
                return true;
            }

            behaviour = null;
            return false;
        }

        /// <summary>
        /// グラフのPlayableをすべて取得
        /// </summary>
        public static void GetAllPlayables(PlayableGraph playableGraph, List<Playable> results, bool includeChildDirector)
        {
            if (!playableGraph.IsValid()) return;

            int playableCount = playableGraph.GetRootPlayableCount();
            for (int i = 0; i < playableCount; i++)
            {
                var playable = playableGraph.GetRootPlayable(i);
                GetAllPlayables(playable, results, includeChildDirector);
            }
        }

        /// <summary>
        /// 配下のPlayableをすべて取得
        /// </summary>
        public static void GetAllPlayables(Playable playable, List<Playable> results, bool includeChildDirector)
        {
            if (!playable.IsValid()) return;

            int inputCount = playable.GetInputCount();
            for (int i = 0; i < inputCount; i++)
            {
                Playable input = playable.GetInput(i);
                if (!input.IsValid()) continue;
                
                if (!results.Contains(input))
                {
                    results.Add(input);
                }
                
                if (includeChildDirector && TryGetBehaviour<DirectorControlPlayable>(input, out var controlPlayable))
                {
                    var childGraph = controlPlayable.director.playableGraph;
                    if (childGraph.IsValid())
                    {
                        GetAllPlayables(childGraph, results, true);
                    }
                }

                if (input.GetInputCount() > 0)
                {
                    GetAllPlayables(input, results, includeChildDirector);
                }
            }
        }

        public static T FindBehaviour<T>(PlayableGraph playableGraph) where T : PlayableBehaviour, new()
        {
            if (!playableGraph.IsValid()) return null;

            int playableCount = playableGraph.GetRootPlayableCount();
            for (int i = 0; i < playableCount; i++)
            {
                var playable = playableGraph.GetRootPlayable(i);
                var behaviour = FindBehaviour<T>(playable);
                if (behaviour != null) return behaviour;
            }

            return null;
        }

        /// <summary>
        /// 配下のPlayableをすべて取得
        /// </summary>
        public static T FindBehaviour<T>(Playable playable) where T : PlayableBehaviour, new()
        {
            if (!playable.IsValid()) return null;

            int inputCount = playable.GetInputCount();
            for (int i = 0; i < inputCount; i++)
            {
                Playable input = playable.GetInput(i);
                if (!input.IsValid()) continue;
                
                if (TryGetBehaviour<T>(input, out var b)) return b;

                if (input.GetInputCount() > 0)
                {
                    var behaviour = FindBehaviour<T>(input);
                    if (behaviour != null) return behaviour;
                }
            }

            return null;
        }
    }
}