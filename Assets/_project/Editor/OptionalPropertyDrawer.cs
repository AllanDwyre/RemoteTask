using _project.scripts.Utils;
using UnityEditor;
using UnityEngine;

namespace _project.Editor
{
    [CustomPropertyDrawer(typeof(Optional<>))]
    public class OptionalPropertyDrawer: PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var valueProperty = property.FindPropertyRelative("value");
            var enableProperty = property.FindPropertyRelative("enable");

            position.width -= 24;
            EditorGUI.BeginDisabledGroup(!enableProperty.boolValue);
            EditorGUI.PropertyField(position, valueProperty, label, true);
            EditorGUI.EndDisabledGroup();
            
            position.x += position.width + 24;
            position.width = position.height = EditorGUI.GetPropertyHeight(enableProperty);
            EditorGUI.PropertyField(position, valueProperty, label, true);
            position.x -= position.width;
            EditorGUI.PropertyField(position, enableProperty, GUIContent.none);

        }
    }
}