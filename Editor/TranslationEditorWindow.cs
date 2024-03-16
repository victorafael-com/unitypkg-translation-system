using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace com.victorafael.translation
{

    public class TranslationEditorWindow : EditorWindow
    {
        public static void ShowWindow()
        {
            TranslationEditorWindow window = GetWindow<TranslationEditorWindow>();
            window.titleContent = new GUIContent("Translation Editor");
            window.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Translation system not implemented yet.");
        }
    }

}