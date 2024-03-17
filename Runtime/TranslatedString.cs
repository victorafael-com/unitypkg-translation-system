using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace com.victorafael.translation
{
    [System.Serializable]
    public class TranslatedString
    {
        [SerializeField] string key;
        public string Value => TranslationManager.GetTranslation(key);
    }
}