using System;
using System.IO;
using DiamondListCreator.Models;
using Newtonsoft.Json;

namespace DiamondListCreator.Services
{
    static class StretchedCanvasSettingsService
    {
        private static readonly string jsonPath = Path.Combine(Environment.CurrentDirectory, "Config", Properties.Settings.Default.IsIPFPrinting ? "stretched_canvases.json" : "stretched_canvases_uv.json");

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
