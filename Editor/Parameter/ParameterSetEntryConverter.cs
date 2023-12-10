using UnityEngine;

namespace TweenTimeline
{
    public static class ParameterSetEntryConverter
    {
        public static void SetDefaultValue(TweenParameterTrack.ParameterSetEntry data)
        {
            switch (data)
            {
                case TweenParameterTrack.ParameterSetEntry<Color> colorVal:
                    colorVal.Value = Color.white;
                    break;
            }
        }
        
        public static void TryConvert(TweenParameterTrack.ParameterSetEntry prevData, TweenParameterTrack.ParameterSetEntry newData)
        {
            switch (newData)
            {
                case TweenParameterTrack.ParameterSetEntry<int> intVal:
                    if (prevData is TweenParameterTrack.ParameterSetEntry<float> prevFloat)
                    {
                        intVal.Value = (int)prevFloat.Value;
                    }
                    break;
                case TweenParameterTrack.ParameterSetEntry<float> flo:
                    if (prevData is TweenParameterTrack.ParameterSetEntry<int> prevInt)
                    {
                        flo.Value = prevInt.Value;
                    }
                    break;
                case TweenParameterTrack.ParameterSetEntry<Vector2> vec2:
                    if (prevData is TweenParameterTrack.ParameterSetEntry<Vector3> prevVec3)
                    {
                        vec2.Value = prevVec3.Value;
                    }
                    break;
                case TweenParameterTrack.ParameterSetEntry<Vector3> vec3:
                    if (prevData is TweenParameterTrack.ParameterSetEntry<Vector2> prevVec2)
                    {
                        vec3.Value = prevVec2.Value;
                    }
                    break;
            }
        }
    }
}