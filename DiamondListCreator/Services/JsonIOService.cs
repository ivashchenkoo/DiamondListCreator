using System.IO;
using Newtonsoft.Json;

namespace DiamondListCreator.Services
{
    static class JsonIOService
    {
        public static T Read<T>(string path)
        {
            string output = File.ReadAllText(path);
            return JsonConvert.DeserializeObject<T>(output);
        }

        public static void Write<T>(T pathSettings, string path)
        {
            string input = JsonConvert.SerializeObject(pathSettings, Formatting.Indented);
            File.WriteAllText(path, input);
        }
    }
}
