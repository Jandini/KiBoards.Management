using System.Text.Json.Serialization;

namespace KiBoards.Management
{
    public class KibanaSettingsChanges
    {
        [JsonPropertyName("theme:darkMode")]
        public bool? ThemeDarkMode { get; set; } = null;

        [JsonPropertyName("defaultRoute")]
        public string DefaultRoute { get; set; } = null;

    }
}