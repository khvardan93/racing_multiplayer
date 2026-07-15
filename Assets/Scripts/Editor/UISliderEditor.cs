using UnityEditor;
using UnityEditor.UI;

namespace UI.Editor
{
    [CustomEditor(typeof(UISlider), true)]
    [CanEditMultipleObjects]
    public class UISliderEditor : SliderEditor
    {
        private SerializedProperty _duration;
        private SerializedProperty _ease;
        private SerializedProperty _text;
        private SerializedProperty _format;

        protected override void OnEnable()
        {
            base.OnEnable();
            _duration = serializedObject.FindProperty("_duration");
            _ease = serializedObject.FindProperty("_ease");
            _text = serializedObject.FindProperty("_text");
            _format = serializedObject.FindProperty("_format");
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            serializedObject.Update();
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Value Animation", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_duration);
            EditorGUILayout.PropertyField(_ease);
            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Text", EditorStyles.boldLabel);
            EditorGUILayout.PropertyField(_text);
            EditorGUILayout.PropertyField(_format);
            serializedObject.ApplyModifiedProperties();
        }
    }
}
