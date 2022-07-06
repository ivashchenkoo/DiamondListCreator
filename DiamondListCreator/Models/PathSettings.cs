using Newtonsoft.Json;

namespace DiamondListCreator.Models
{
    public class PathSettings
    {
        [JsonProperty("Шлях до збереження файлів пдф")]
        public string FilesSavePath { get; set; }

        [JsonProperty("Шлях до папки з алмазками")]
        public string DiamondsFolderPath  { get; set; }

        [JsonProperty("Шлях до файлу обліку")]
        public string AccountingExcelFilePath  { get; set; }

        [JsonProperty("Шлях до збережених легенд")]
        public string SavedLegendsPath  { get; set; }

        [JsonProperty("Шлях до збереження холстів")]
        public string CanvasesSavePath  { get; set; }

        [JsonProperty("Шлях до збережених холстів")]
        public string SavedCanvasesPath  { get; set; }
    }
}
