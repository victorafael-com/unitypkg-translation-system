using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace com.victorafael.translation
{

    public class TranslationEditorWindow : EditorWindow
    {
        private System.Action<string> onSelectString;
        private TranslationData data;
        private SerializedObject serializedObject;
        private SerializedProperty languages;
        private SerializedProperty entries;
        private string newLanguage = "";

        private TranslationData.TranslationEntry editingEntry;
        private int editingEntryIndex;

        private WindowState state = WindowState.Main;

        private Vector2 scrollPos = Vector2.zero;
        private Vector2 entryScrollPos = Vector2.zero;

        private GUIStyle _hoverStyle = null;

        #region enums
        enum WindowState
        {
            Main,
            EditLanguages,
            EditEntry,
            AddEntry
        }
        #endregion

        public static void ShowWindow(System.Action<string> onSelect = null)
        {
            TranslationEditorWindow window = GetWindow<TranslationEditorWindow>();
            window.titleContent = new GUIContent("Translation Editor");
            window.onSelectString = onSelect;
            window.LoadData();
            window.Show();
        }
        void LoadData()
        {
            data = Resources.Load<TranslationData>("TranslationData");
            if (data != null)
            {
                serializedObject = new SerializedObject(data);
                languages = serializedObject.FindProperty("languages");
                entries = serializedObject.FindProperty("entries");
            }
        }

        void CreateTranslationData()
        {
            data = ScriptableObject.CreateInstance<TranslationData>();
            AssetDatabase.CreateAsset(data, "Assets/Resources/TranslationData.asset");
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        void HorizontalLine()
        {
            GUI.color = new Color(1, 1, 1, 0.2f);
            GUI.DrawTexture(
                EditorGUILayout.GetControlRect(GUILayout.Height(1)),
                EditorGUIUtility.whiteTexture
            );
            GUI.color = Color.white;
        }

        private void DrawTitle(string title, bool closeButton)
        {
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(title, EditorStyles.boldLabel);
            if (closeButton && GUILayout.Button("close", GUILayout.Width(80)))
            {
                state = WindowState.Main;
            }
            EditorGUILayout.EndHorizontal();

            HorizontalLine();
            EditorGUILayout.Space();
            GUI.color = Color.white;

        }

        private void RemoveLanguage(int index)
        {
            if (EditorUtility.DisplayDialog("Remove Language", "Are you sure you want to remove this language?", "Yes", "No"))
            {
                languages.DeleteArrayElementAtIndex(index);

                var entries = serializedObject.FindProperty("entries");
                for (int i = 0; i < entries.arraySize; i++)
                {
                    entries.GetArrayElementAtIndex(i)
                        .FindPropertyRelative("values")
                        .DeleteArrayElementAtIndex(index);
                }
            }
        }

        private void HandleMultilineTextArea(SerializedProperty property, string controlName, string nextControlName)
        {
            Event e = Event.current;

            // Check if the current control has focus and if the Tab key is pressed
            if (GUI.GetNameOfFocusedControl() == controlName && e.type == EventType.KeyDown && e.keyCode == KeyCode.Tab)
            {
                // Prevent the Tab character from being added
                e.Use();

                // Move focus to the next control
                EditorGUI.FocusTextInControl(nextControlName);

                return;
            }

            // Set the name for the next control to manage focus
            GUI.SetNextControlName(controlName);

            // Draw the property field (which can be a multi-line text area)
            EditorGUILayout.PropertyField(property, new GUIContent(property.displayName), GUILayout.Height(60));
        }

        private void EditEntry(int index, TranslationData.TranslationEntry sourceEntry = null)
        {
            editingEntryIndex = index;
            if (sourceEntry != null)
                editingEntry = new TranslationData.TranslationEntry(sourceEntry);
            else
            {
                editingEntry = new TranslationData.TranslationEntry(languages.arraySize);
            }

            state = WindowState.EditEntry;
        }

        void DrawMain()
        {
            EditorGUILayout.BeginVertical();

            float columns = 1 + languages.arraySize;
            float optionOffset = 80;

            var columnWidth = Mathf.Floor((EditorGUIUtility.currentViewWidth - optionOffset) / columns);
            var columnSize = GUILayout.Width(columnWidth);

            //Draw transparent lines
            GUI.color = new Color(1, 1, 1, 0.2f);

            Rect lineRect = new Rect(0, 0, 1, Screen.height - 130);

            for (int i = 1; i <= columns; i++)
            {
                lineRect.x = columnWidth * i;
                GUI.DrawTexture(lineRect, EditorGUIUtility.whiteTexture);
            }
            GUI.color = Color.white;

            int buttonWidth = 30;
            var buttonSize = GUILayout.Width(buttonWidth);
            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Key", columnSize);
            for (int i = 0; i < languages.arraySize; i++)
            {
                GUILayout.Label(languages.GetArrayElementAtIndex(i).stringValue, columnSize);
            }
            if (GUILayout.Button("...", buttonSize))
            {
                state = WindowState.EditLanguages;
                newLanguage = "";
            }
            EditorGUILayout.EndHorizontal();
            HorizontalLine();

            scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
            for (var i = 0; i < data.entries.Length; i++)
            {
                var entry = data.entries[i];

                EditorGUILayout.BeginHorizontal();
                if (GUILayout.Button(entry.key, columnSize))
                {
                    onSelectString?.Invoke(entry.key);
                    Close();
                }
                for (int j = 0; j < entry.values.Length; j++)
                {
                    GUILayout.Label(entry.values[j], columnSize);
                }
                if (GUILayout.Button("Edit", buttonSize))
                {
                    state = WindowState.EditEntry;
                    editingEntryIndex = i;
                    EditEntry(i, entry);
                }
                EditorGUILayout.EndHorizontal();
            }
            EditorGUILayout.EndScrollView();

            if (GUILayout.Button("Add New Entry"))
            {
                state = WindowState.EditEntry;
                int pos = entries.arraySize;
                entries.arraySize++;
                EditEntry(pos);
            }

            EditorGUILayout.EndVertical();
        }
        void DrawEditLanguages()
        {
            DrawTitle("Edit Languages", true);

            for (int i = 0; i < languages.arraySize; i++)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.PropertyField(languages.GetArrayElementAtIndex(i), GUIContent.none);
                if (GUILayout.Button("X", GUILayout.Width(20)))
                {
                    RemoveLanguage(i);
                }
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.BeginHorizontal();
            newLanguage = EditorGUILayout.TextField("New Language", newLanguage);

            if (GUILayout.Button("add"))
            {
                languages.arraySize++;
                languages.GetArrayElementAtIndex(languages.arraySize - 1).stringValue = newLanguage;
                newLanguage = "";
            }
            EditorGUILayout.EndHorizontal();
        }
        void DrawEditEntry()
        {
            bool isEdit = state == WindowState.EditEntry;

            var entries = serializedObject.FindProperty("entries");

            if (editingEntry == null)
            {
                EditorGUILayout.HelpBox("No entry selected", MessageType.Warning);
                if (GUILayout.Button("Return to main screen"))
                {
                    state = WindowState.Main;
                }
                return;
            }

            DrawTitle(isEdit ? "Edit Entry" : "Add Entry", false);
            GUI.SetNextControlName("Key");
            editingEntry.key = EditorGUILayout.TextField(new GUIContent("Key"), editingEntry.key);
            if (editingEntry.values == null)
            {
                editingEntry.values = new string[languages.arraySize];
            }
            else if (editingEntry.values.Length != languages.arraySize)
            {
                var newValues = new string[languages.arraySize];
                for (int i = 0; i < Mathf.Min(newValues.Length, editingEntry.values.Length); i++)
                {
                    newValues[i] = editingEntry.values[i];
                }
                editingEntry.values = newValues;
            }
            for (int i = 0; i < languages.arraySize; i++)
            {
                var val = editingEntry.values[i];
                var lang = languages.GetArrayElementAtIndex(i).stringValue;

                Event e = Event.current;
                if (e.type == EventType.KeyDown && e.keyCode == KeyCode.Tab && GUI.GetNameOfFocusedControl() == lang)
                {
                    if (i < languages.arraySize - 1)
                        EditorGUI.FocusTextInControl(languages.GetArrayElementAtIndex(i + 1).stringValue);
                    else
                        GUI.FocusControl("save");

                    e.Use();
                }

                EditorGUILayout.LabelField(lang);
                GUI.SetNextControlName(lang);
                editingEntry.values[i] = EditorGUILayout.TextArea(val, GUILayout.MaxHeight(60));
            }

            #region Footer buttons
            EditorGUILayout.BeginHorizontal();
            var buttonWidth = GUILayout.Width(90);
            if (isEdit)
            {
                GUI.color = new Color(1, 0.6f, 0.6f);
                if (GUILayout.Button("delete", buttonWidth))
                {
                    serializedObject.FindProperty("entries").DeleteArrayElementAtIndex(editingEntryIndex);
                    state = WindowState.Main;
                }
            }
            GUILayout.FlexibleSpace();
            GUI.color = Color.white;
            if (GUILayout.Button("Cancel", buttonWidth))
            {
                if (!isEdit)
                {
                    entries.DeleteArrayElementAtIndex(editingEntryIndex);
                }
                state = WindowState.Main;
            }
            GUI.color = new Color(0.6f, 1, 0.6f);
            GUI.SetNextControlName("save");
            if (GUILayout.Button("Save", buttonWidth))
            {
                var saveEntry = entries.GetArrayElementAtIndex(editingEntryIndex);
                saveEntry.FindPropertyRelative("key").stringValue = editingEntry.key;

                var saveValues = saveEntry.FindPropertyRelative("values");

                if (saveValues.arraySize < languages.arraySize)
                {
                    saveValues.arraySize = languages.arraySize;
                }

                for (int i = 0; i < languages.arraySize; i++)
                {
                    saveValues.GetArrayElementAtIndex(i).stringValue = editingEntry.values[i];
                }
                state = WindowState.Main;
            }
            EditorGUILayout.EndHorizontal();
            #endregion
        }

        private void OnGUI()
        {
            if (data == null || serializedObject == null)
                LoadData();
            if (data == null || serializedObject == null)
            {
                EditorGUILayout.HelpBox("TranslationData not found in Resources, please create a TranslationData asset and place it in a Resources folder", MessageType.Warning);
                if (GUILayout.Button("Create TranslationData"))
                {
                    CreateTranslationData();
                }
                return;
            }

            EditorGUI.BeginChangeCheck();

            switch (state)
            {
                case WindowState.Main:
                    DrawMain();
                    break;
                case WindowState.EditLanguages:
                    DrawEditLanguages();
                    break;
                case WindowState.EditEntry:
                case WindowState.AddEntry:
                    DrawEditEntry();
                    break;
                default:
                    break;
            }
            if (EditorGUI.EndChangeCheck())
            {
                serializedObject.ApplyModifiedProperties();
            }
        }
    }

}