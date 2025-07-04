﻿using System;
using System.IO;
using DiamondListCreator.Models;
using Newtonsoft.Json;

namespace DiamondListCreator.Services
{
    static class PathSettingsService
    {
        private static readonly string jsonPath = Path.Combine(Environment.CurrentDirectory, "Config", "PathSettings.json");

        public static PathSettings ReadSettings()
        {
            string output = File.ReadAllText(jsonPath);
            return JsonConvert.DeserializeObject<PathSettings>(output);
        }

        public static void WriteSettings(PathSettings pathSettings)
        {
            string input = JsonConvert.SerializeObject(pathSettings, Formatting.Indented);
            File.WriteAllText(jsonPath, input);
        }
    }
}
