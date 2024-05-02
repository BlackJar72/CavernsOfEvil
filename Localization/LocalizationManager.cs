using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace CevarnsOfEvil {


    public enum SupportedLanguage {
        EN,
        ZH,
        JP,
        KO
    }


    public class LocalizationManager : MonoBehaviour {

        public static SystemLanguage language;

        void Awake() {
            DontDestroyOnLoad(gameObject);
            language = Application.systemLanguage;
        }


        // Start is called before the first frame update
        void Start() {

        }


        public static string GetTranslation(string table, string key) {
            return LocalizationSettings.StringDatabase.GetLocalizedString(table, key);
        }


        public static string GetTranslation(string table, string key, params string[] args) {
            // Unity "smart strings" are dumb -- using string.Format(), as its so much simpler
            string str = LocalizationSettings.StringDatabase.GetLocalizedString(table, key);
            return string.Format(str, args);
        }


        public static void SetLanguage(Locale locale) {
            LocalizationSettings.SelectedLocale = locale;
        }


        public static void SetLanguage(SupportedLanguage language) {
            Locale locale;
            LocaleIdentifier loc;
            switch(language) {
                case SupportedLanguage.EN:
                    loc = new LocaleIdentifier("en");
                    break;
                case SupportedLanguage.ZH:
                    loc = new LocaleIdentifier("zh");
                    break;
                case SupportedLanguage.JP:
                    loc = new LocaleIdentifier("jp");
                    break;
                case SupportedLanguage.KO:
                    loc = new LocaleIdentifier("ko");
                    break;
                default:
                    loc = new LocaleIdentifier("en");
                    break;
            }
            locale = LocalizationSettings.AvailableLocales.GetLocale(loc);
            LocalizationSettings.SelectedLocale = locale;
        }


    }

}
