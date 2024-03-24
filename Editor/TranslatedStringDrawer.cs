using UnityEditor;
using UnityEngine;

namespace com.victorafael.translation
{
    [CustomPropertyDrawer(typeof(TranslatedString))]
    public class TranslatedStringDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            return (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing) * 2;
        }
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            // position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
            EditorGUI.BeginProperty(position, label, property);

            SerializedProperty key = property.FindPropertyRelative("key");
            Rect fullLineRect = new Rect(position);

            fullLineRect.y += fullLineRect.height / 2;
            fullLineRect.height = EditorGUIUtility.singleLineHeight;

            position.height = EditorGUIUtility.singleLineHeight;


            position.width -= 25;
            EditorGUI.PropertyField(position, key, label, true);
            position.x = position.xMax + 5;
            position.width = 20;

            // EditorGUILayout.PropertyField(key, label, GUILayout.MinWidth(0));
            if (GUI.Button(position, "..."))
            {
                TranslationEditorWindow.ShowWindow(selectedKey =>
                {
                    key.stringValue = selectedKey;
                    key.serializedObject.ApplyModifiedProperties();
                });
            }

            EditorGUI.HelpBox(fullLineRect, TranslationManager.GetTranslation(key.stringValue), MessageType.None);

            EditorGUI.EndProperty();
        }
    }
}
