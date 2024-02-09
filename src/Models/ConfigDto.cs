namespace MIOS.net.Models
{
    public class ConfigDto
    {
        public List<string> Comments { get; set; } = new List<string>();
        public Dictionary<string, SectionDto> Sections { get; set; } = new Dictionary<string, SectionDto>();
    }

    public class SectionDto
    {
        public string NiceName { get; set; } = string.Empty;
        public List<string> Comments { get; set; } = new List<string>();
        public Dictionary<string, KeyDto> Keys { get; set; } = new Dictionary<string, KeyDto>();
    }

    public class KeyDto
    {
        public string NiceName { get; set; } = string.Empty;
        public List<string> Comments { get; set; } = new List<string>();
        public string Value { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public List<string> Enum { get; set; } = new List<string>();
        public bool Active { get; set; } = true;
    }
}