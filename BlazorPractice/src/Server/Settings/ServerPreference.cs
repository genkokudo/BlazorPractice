using BlazorPractice.Shared.Constants.Localization;
using BlazorPractice.Shared.Settings;
using System.Linq;

namespace BlazorPractice.Server.Settings
{
    public record ServerPreference : IPreference
    {
        public string LanguageCode { get; set; } = LocalizationConstants.SupportedLanguages.FirstOrDefault()?.Code ?? "en-US";

        //TODO - add server preferences
    }
}