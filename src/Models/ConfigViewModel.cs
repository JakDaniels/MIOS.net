using IniFileParser.Model;

namespace MIOS.net.Models
{
    public class ConfigViewModel
    {
        /// <summary>
        /// A JSON representation of the INI file
        /// </summary>
        public string? JsonConfig { get; set; }

        /// <summary>
        /// A DTO representation of the INI file
        /// </summary>
        public ConfigDto Config { get; set; } = new ConfigDto();


    }
}