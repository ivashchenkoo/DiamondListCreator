namespace DiamondListCreator.Models
{
    public class DiamondSettings
    {
        public string Name { get; set; }
        /// <summary>
        /// Name without TWD etc.
        /// Example: 00367M+
        /// </summary>
        public string ShortName { get; set; }
        /// <summary>
        /// Path to diamond folder
        /// </summary>
        public string Path { get; set; }
        public bool IsEnglishVersion { get; set; } = false;
        public bool IsStretchedCanvas { get; set; } = false;
        public int Width { get; set; }
        public int Height { get; set; }
        public DiamondType DiamondType { get; set; }
    }

    public enum DiamondType
    {
        PoPhoto,
        Standart
    }
}
