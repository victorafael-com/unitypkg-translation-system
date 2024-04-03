using System.Collections.Generic;
using UnityEngine;

namespace com.victorafael.translation
{
    public static class TranslationManager
    {
        private static Dictionary<string, string> translations = null;
        public static void SetLanguage(string key)
        {
            LoadTranslation(key);
        }

        private static void LoadTranslation(string language = null)
        {

            TranslationData data = Resources.Load<TranslationData>("TranslationData");
            if (data != null)
            {
                if (string.IsNullOrEmpty(language))
                {
                    language = PlayerPrefs.GetString("SELECTED_LANGUAGE", data.defaultLanguage);
                }
                else
                {
                    PlayerPrefs.SetString("SELECTED_LANGUAGE", language);
                }


                if (translations != null)
                    translations.Clear();
                translations = data.GetTranslations(language, translations);
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
            return translations.ContainsKey(key) ? translations[key] : $">{key}< Missing!";
        }
    }
}