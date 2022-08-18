using System;
using System.IO;
using DiamondListCreator.Models;
using Newtonsoft.Json;

namespace DiamondListCreator.Services
{
    static class CanvasSettingsService
    {
        private static readonly string jsonPath = Path.Combine(Environment.CurrentDirectory, "Config", "canvases.json");

        public static CanvasSettings[] ReadSettings()
        {
            string output = File.ReadAllText(jsonPath);
            return JsonConvert.DeserializeObject<CanvasSettings[]>(output);
        }

        public static void WriteSettings(CanvasSettings[] canvasesSettings)
        {
            string input = JsonConvert.SerializeObject(canvasesSettings, Formatting.Indented);
            File.WriteAllText(jsonPath, input);
        }
    }
}
