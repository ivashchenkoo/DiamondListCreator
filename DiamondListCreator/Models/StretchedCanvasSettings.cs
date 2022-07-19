using Newtonsoft.Json;

namespace DiamondListCreator.Models
{
    public class StretchedCanvasSettings
    {
        [JsonProperty("Розмір")]
        public string SizeName { get; set; }
    }
}
