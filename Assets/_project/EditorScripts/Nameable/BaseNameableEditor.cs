using UnityEditor;
using UnityEngine;

namespace _project.EditorScripts.Nameable
{
    public abstract class BaseNameableEditor<T> : UnityEditor.Editor where T : ScriptableObject
    {
        private T _target;
        protected abstract string PropertyName { get; }
        protected abstract Object AssetPreview { get; }
        private void OnEnable()
        {
            _target = (T)target;
        }

        public override void OnInspectorGUI()
        {
            // Start listening for changes to the serializedObject
            serializedObject.Update();

            // Draw default inspector
            DrawDefaultInspector();

            // Get the string field property
            SerializedProperty stringFieldProperty = serializedObject.FindProperty(PropertyName);

            // Check if the string field has changed
            if (stringFieldProperty != null && GUI.changed)
            {
                // Get the new value of the string field
                string newStringValue = stringFieldProperty.stringValue;

                // Get the current path of the ScriptableObject
                string assetPath = AssetDatabase.GetAssetPath(_target);

                // Generate the new file name based on the string field
                string newFileName = $"{newStringValue}.asset";

                // Rename the asset file
                AssetDatabase.RenameAsset(assetPath, newFileName);

                // Save the asset database
                AssetDatabase.SaveAssets();
            }

            // Apply changes to the serializedObject
            serializedObject.ApplyModifiedProperties();
            
            DrawAssetPreview();
        }

        private void DrawAssetPreview()
        {
            if (AssetPreview == null) return;

            Texture2D texture = UnityEditor.AssetPreview.GetAssetPreview(AssetPreview);
            
            if (!texture) return;
            GUILayout.Label("",GUILayout.Height(texture.height),GUILayout.Width(texture.width));
            GUI.DrawTexture(GUILayoutUtility.GetLastRect(),texture);
        }
    }
}