using System;
using System.IO;
using DiamondListCreator.Models;
using Newtonsoft.Json;

namespace DiamondListCreator.Services
{
    public class StretchedCanvasSettingsService
    {
        private static readonly string jsonPath = Environment.CurrentDirectory + "\\Config\\stretched_canvases.json";

        public static StretchedCanvasSettings[] ReadSettings()
        {
            string output = File.ReadAllText(jsonPath);
            return JsonConvert.DeserializeObject<StretchedCanvasSettings[]>(output);
        }

        public static void WriteSettings(StretchedCanvasSettings[] canvasesSettings)
        {
            string input = JsonConvert.SerializeObject(canvasesSettings, Formatting.Indented);
            File.WriteAllText(jsonPath, input);
        }
    }
}
