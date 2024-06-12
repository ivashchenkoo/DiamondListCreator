using Newtonsoft.Json;

namespace DiamondListCreator.Models
{
    public class ZipWeightSettings
    {
        [JsonProperty("small_zip")]
        public double SmallZipWeight { get; set; }

        [JsonProperty("big_zip")]
        public double BigZipWeight { get; set; }
    }
}
