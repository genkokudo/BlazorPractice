
using BlazorPractice.Application.Interfaces.Serialization.Settings;
using Newtonsoft.Json;

namespace BlazorPractice.Application.Serialization.Settings
{
    public class NewtonsoftJsonSettings : IJsonSerializerSettings
    {
        public JsonSerializerSettings JsonSerializerSettings { get; } = new();
    }
}