using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Monetizr
{
    public static class Analytics
    {
        private static readonly Dictionary<SystemLanguage, string> CountryCodes = new Dictionary<SystemLanguage, string>
        {
            { SystemLanguage.Afrikaans, "af-ZA"},
            { SystemLanguage.Arabic, "ar-SA"},
            { SystemLanguage.Basque, "eu-ES"},
            { SystemLanguage.Belarusian, "be-BY"},
            { SystemLanguage.Bulgarian, "bg-BG"},
            { SystemLanguage.Catalan, "ca-ES"},
            { SystemLanguage.Chinese, "zh-CN"},
            { SystemLanguage.Czech, "cs-CZ"},
            { SystemLanguage.Danish, "da-DK"},
            { SystemLanguage.Dutch, "nl-NL"},
            { SystemLanguage.English, "en-US"},
            { SystemLanguage.Estonian, "et-EE"},
            { SystemLanguage.Faroese, "fo-FO"},
            { SystemLanguage.Finnish, "fi-FI"},
            { SystemLanguage.French, "fr-FR"},
            { SystemLanguage.German, "de-DE"},
            { SystemLanguage.Greek, "el-GR"},
            { SystemLanguage.Hebrew, "he-IL"},
            { SystemLanguage.Hungarian, "hu-HU"},
            { SystemLanguage.Icelandic, "is-IS"},
            { SystemLanguage.Indonesian, "id-ID"},
            { SystemLanguage.Italian, "it-IT"},
            { SystemLanguage.Japanese, "ja-JP"},
            { SystemLanguage.Korean, "ko-KR"},
            { SystemLanguage.Latvian, "lv-LV"},
            { SystemLanguage.Lithuanian, "lt-LT"},
            { SystemLanguage.Norwegian, "no-NO"},
            { SystemLanguage.Polish, "pl-PL"},
            { SystemLanguage.Portuguese, "pt-PT"},
            { SystemLanguage.Romanian, "ro-RO"},
            { SystemLanguage.Russian, "ru-RU"},
            { SystemLanguage.SerboCroatian, "sr-RS"}, //HR for Croatia
            { SystemLanguage.Slovak, "sk-SK"},
            { SystemLanguage.Slovenian, "sl-SI"},
            { SystemLanguage.Spanish, "es-ES"},
            { SystemLanguage.Swedish, "sv-SE"},
            { SystemLanguage.Thai, "th-TH"},
            { SystemLanguage.Turkish, "tr-TR"},
            { SystemLanguage.Ukrainian, "uk-UA"},
            { SystemLanguage.Vietnamese, "vi-VN"},
            { SystemLanguage.Unknown, "zz-US" }
        };


        /// <summary>
        /// Returns approximate country code of the language.
        /// </summary>
        /// <returns>Approximated country code.</returns>
        /// <param name="language">Language which should be converted to country code.</param>
        public static string ToCountryCode(this SystemLanguage language)
        {
            string result;
            if (CountryCodes.TryGetValue(language, out result))
            {
                return result;
            }
            else
            {
                return CountryCodes[SystemLanguage.Unknown];
            }
        }
    }
}
