﻿namespace BlazorPractice.Shared.Constants.Localization
{
    /// <summary>
    /// 対応言語の列挙
    /// </summary>
    public static class LocalizationConstants
    {
        public static readonly LanguageCode[] SupportedLanguages = {
            new LanguageCode
            {
                Code = "en-US",
                DisplayName= "English"
            },
            new LanguageCode
            {
                Code = "fr-FR",
                DisplayName = "French"
            },
            new LanguageCode
            {
                Code = "km_KH",
                DisplayName= "Khmer"
            },
            new LanguageCode
            {
                Code = "de-DE",
                DisplayName = "German"
            },
            new LanguageCode
            {
                Code = "es-ES",
                DisplayName = "Español"
            },
            new LanguageCode
            {
                Code = "ru-RU",
                DisplayName = "Русский"
            },
            new LanguageCode
            {
                Code = "sv-SE",
                DisplayName = "Swedish"
            },
            new LanguageCode
            {
                Code = "id-ID",
                DisplayName = "Indonesia"
            },
            new LanguageCode
            {
                Code = "it-IT",
                DisplayName = "Italian"
            }
        };
    }
}
