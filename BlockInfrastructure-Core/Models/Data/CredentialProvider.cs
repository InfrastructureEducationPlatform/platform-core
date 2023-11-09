using System.Text.Json.Serialization;

namespace BlockInfrastructure.Core.Models.Data;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum CredentialProvider
{
    Google,
    Self
}