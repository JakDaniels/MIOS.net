namespace MIOS.net.Models
{
    public class TreeViewDto
    {
        public string Text { get; set; } = string.Empty;
        public string Html { get; set; } = string.Empty;
        public List<TreeViewDto>? Children { get; set; } = null;

    }
}