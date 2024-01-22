using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;

namespace Common
{
    public static class PropertyDrawerUtility
    {
        private static Dictionary<Type, PropertyDrawer> cache = new();

        private static FieldInfo propertyDrawerFieldInfo =>
            _propertyDrawerFieldInfo ??= typeof(PropertyDrawer)
                .GetField("m_FieldInfo", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        private static FieldInfo _propertyDrawerFieldInfo;
        
        private static MethodInfo getDrawerMethod;
        
        public static FieldInfo GetFieldInfo(PropertyDrawer drawer)
        {
            return (FieldInfo)propertyDrawerFieldInfo.GetValue(drawer);
        }
        
        public static PropertyDrawer GetPropertyDrawerForField(FieldInfo fieldInfo)
        {
            // Getting the field type this way assumes that the property instance is not a managed reference (with a SerializeReference attribute); if it was, it should be retrieved in a different way:
            Type fieldType = fieldInfo.FieldType;

            if (cache.TryGetValue(fieldType, out var propertyDrawer))
            {
                return propertyDrawer;
            }

            getDrawerMethod ??= Type.GetType("UnityEditor.ScriptAttributeUtility,UnityEditor")
                .GetMethod("GetDrawerTypeForType", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);

            var propertyDrawerType = (Type)getDrawerMethod.Invoke(null, new object[] { fieldType });

            if (typeof(PropertyDrawer).IsAssignableFrom(propertyDrawerType))
            {
                propertyDrawer = (PropertyDrawer)Activator.CreateInstance(propertyDrawerType);
            }

            if (propertyDrawer != null)
            {   
                propertyDrawerFieldInfo.SetValue(propertyDrawer, fieldInfo);
            }
            
            cache.Add(fieldType, propertyDrawer);

            return propertyDrawer;
        }
    }
}
