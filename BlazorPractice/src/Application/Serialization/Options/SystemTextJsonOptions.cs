using BlazorPractice.Application.Interfaces.Serialization.Options;
using System.Text.Json;

namespace BlazorPractice.Application.Serialization.Options
{
    public class SystemTextJsonOptions : IJsonSerializerOptions
    {
        public JsonSerializerOptions JsonSerializerOptions { get; } = new();
    }
}