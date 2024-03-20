using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace com.victorafael.translation
{
    [CreateAssetMenu(fileName = "TranslationData", menuName = "Translation System/Translation Data")]
    public class TranslationData : ScriptableObject
    {
        [System.Serializable]
        public class TranslationEntry
        {
            public string key;
            [Multiline]
            public string[] values;
            public TranslationEntry(int languages)
            {
                key = string.Empty;
                values = new string[languages];
            }

            public TranslationEntry(TranslationEntry source = null)
            {
                key = source.key;
                values = new string[source.values.Length];
                for (int i = 0; i < source.values.Length; i++)
                {
                    values[i] = source.values[i];
                }

            }
        }
        public string[] languages;

        public TranslationEntry[] entries;

        public Dictionary<string, string> GetTranslations(string language, Dictionary<string, string> translationDictionary = null)
        {
            if (translationDictionary == null)
            {
                translationDictionary = new Dictionary<string, string>();
            }

            int languageIndex = -1;
            for (int i = 0; i < languages.Length; i++)
            {
                if (languages[i].Equals(language))
                {
                    languageIndex = i;
                    break;
                }
            }

            if (languageIndex == -1)
            {
                Debug.LogError("Language not found: " + language);
                return translationDictionary;
            }

            foreach (var entry in entries)
            {
                if (entry.values.Length > languageIndex)
                {
                    translationDictionary.Add(entry.key, entry.values[languageIndex]);
                }
                else
                {
                    Debug.LogError("Language index out of range: " + language);
                }
            }

            return translationDictionary;
        }
    }
}