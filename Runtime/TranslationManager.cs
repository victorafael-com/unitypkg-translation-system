using System.Collections.Generic;
using UnityEngine;

namespace com.victorafael.translation
{
    public static class TranslationManager
    {
        private static Dictionary<string, string> translations = new Dictionary<string, string>();
        private const string defaultLanguage = "en";

        public static void SetLanguage(string key)
        {
            LoadTranslation(key);
        }

        private static void LoadTranslation(string language = null)
        {
            if (string.IsNullOrEmpty(language))
            {
                language = PlayerPrefs.GetString("SELECTED_LANGUAGE", defaultLanguage);
            }
            else
            {
                PlayerPrefs.SetString("SELECTED_LANGUAGE", language);
            }
            translations.Clear();
            TranslationData data = Resources.Load<TranslationData>("TranslationData");
            if (data != null)
            {
                translations = data.GetTranslations(language);
            }
            else
            {
                Debug.LogWarning("TranslationData not found in Resources, please create a TranslationData asset and place it in a Resources folder");
            }
        }
        public static string GetTranslation(string key)
        {
            if (translations == null)
            {
                LoadTranslation();
            }
            return key;
        }
    }
}