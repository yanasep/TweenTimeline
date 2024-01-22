using System.Linq;
using Common;
using UnityEditor;
using UnityEngine;

namespace Yanasep
{
    /// <summary>
    /// 複数のPropertyDrawerを同時に描画するためのAttributeのベースクラス
    /// </summary>
    /// <remarks>https://light11.hatenadiary.com/entry/2021/08/16/201543</remarks>
    public abstract class MultiPropertyAttribute : PropertyAttribute
    {
#if UNITY_EDITOR
        public MultiPropertyAttribute[] Attributes;
        public IMultiPropertyGUIAttribute[] PropertyDrawers;

        public virtual void OnPreGUI(Rect position, SerializedProperty property) { }

        public virtual void OnPostGUI(Rect position, SerializedProperty property, bool changed) { }

        /// <summary>アトリビュートのうち一つでもfalseだったらそのGUIは非表示になる</summary>
        public virtual bool IsVisible(SerializedProperty property)
        {
            return true;
        }
#endif
    }

    /// <summary>
    /// GUI描画をコントロールしたいMultiPropertyAttributeにはこれを付ける
    /// </summary>
    public interface IMultiPropertyGUIAttribute
    {
#if UNITY_EDITOR
        void OnGUI(Rect position, SerializedProperty property, GUIContent label);

        float GetPropertyHeight(SerializedProperty property, GUIContent label);
#endif
    }

#if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(MultiPropertyAttribute), true)]
    public class MultiPropertyAttributeDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attributes = GetAttributes();
            var propertyDrawers = GetPropertyDrawers();

            // 非表示の場合
            if (attributes.Any(attr => !attr.IsVisible(property)))
            {
                return;
            }

            // 前処理
            foreach (var attr in attributes)
            {
                attr.OnPreGUI(position, property);
            }

            // 描画
            using (var ccs = new EditorGUI.ChangeCheckScope())
            {
                if (propertyDrawers.Length == 0)
                {
                    // そのタイプのカスタムPropertyDrawerがあればそれを描画
                    var drawer = PropertyDrawerUtility.GetPropertyDrawerForField(PropertyDrawerUtility.GetFieldInfo(this));
                    if (drawer != null)
                    {
                        drawer.OnGUI(position, property, label);
                    }
                    else
                    {
                        EditorGUI.PropertyField(position, property, label);
                    }
                }
                else
                {
                    propertyDrawers.Last().OnGUI(position, property, label);
                }

                // 後処理
                foreach (var attr in attributes.Reverse())
                {
                    attr.OnPostGUI(position, property, ccs.changed);
                }
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            var attributes = GetAttributes();
            var propertyDrawers = GetPropertyDrawers();

            // 非表示の場合
            if (attributes.Any(attr => !attr.IsVisible(property)))
            {
                return -EditorGUIUtility.standardVerticalSpacing;
            }

            var height = propertyDrawers.Length == 0
                ? GetDefaultHeight()
                : propertyDrawers.Last().GetPropertyHeight(property, label);
            return height;
            
            float GetDefaultHeight()
            {
                var drawer = PropertyDrawerUtility.GetPropertyDrawerForField(PropertyDrawerUtility.GetFieldInfo(this));
                if (drawer != null)
                {
                    return drawer.GetPropertyHeight(property, label);
                }
                else
                {
                    return base.GetPropertyHeight(property, label);
                }
            }
        }

        private MultiPropertyAttribute[] GetAttributes()
        {
            var attr = (MultiPropertyAttribute)attribute;

            if (attr.Attributes == null)
            {
                attr.Attributes = fieldInfo
                    .GetCustomAttributes(typeof(MultiPropertyAttribute), false)
                    .Cast<MultiPropertyAttribute>()
                    .OrderBy(x => x.order)
                    .ToArray();
            }

            return attr.Attributes;
        }

        private IMultiPropertyGUIAttribute[] GetPropertyDrawers()
        {
            var attr = (MultiPropertyAttribute)attribute;

            if (attr.PropertyDrawers == null)
            {
                attr.PropertyDrawers = fieldInfo
                    .GetCustomAttributes(typeof(MultiPropertyAttribute), false)
                    .OfType<IMultiPropertyGUIAttribute>()
                    .OrderBy(x => ((MultiPropertyAttribute)x).order)
                    .ToArray();
            }

            return attr.PropertyDrawers;
        }
    }
#endif
}