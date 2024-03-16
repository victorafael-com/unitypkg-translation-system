using UnityEditor;
using UnityEngine;

namespace com.victorafael.translation
{
    [CustomPropertyDrawer(typeof(TranslatedString))]
    public class TranslatedStringDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            EditorGUI.BeginProperty(position, label, property);
            GUI.BeginGroup(position);
            EditorGUILayout.BeginHorizontal();

            SerializedProperty key = property.FindPropertyRelative("key");
            EditorGUILayout.PropertyField(key, GUILayout.MinWidth(0));
            if (GUILayout.Button("...", GUILayout.Width(20)))
            {
                TranslationEditorWindow.ShowWindow();
            }
            EditorGUILayout.EndHorizontal();
            GUI.EndGroup();



            EditorGUI.EndProperty();
        }
    }
}
