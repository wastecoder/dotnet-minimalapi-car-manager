using System.Text.Json.Serialization;

namespace CarManager.Domain.Enums;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AdmRole
{
    None = 0,
    Adm = 1,
    Editor = 2
}