namespace TweenTimeline
{
    public class ParameterType<T> { }
    
    public class ParameterTypeInt : ParameterType<int> { }
    public class ParameterTypeFloat : ParameterType<float> { }
    public class ParameterTypeBool : ParameterType<bool> { }
    public class ParameterTypeVector3 : ParameterType<UnityEngine.Vector3> { }
    public class ParameterTypeVector2 : ParameterType<UnityEngine.Vector2> { }
    public class ParameterTypeVectorColor : ParameterType<UnityEngine.Color> { }
}
