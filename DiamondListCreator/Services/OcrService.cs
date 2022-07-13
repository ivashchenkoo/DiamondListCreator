using System.Drawing;
using Patagames.Ocr;

namespace DiamondListCreator.Services
{
    public class OcrService
    {
        private static OcrApi ocrApi;

        public OcrService(Patagames.Ocr.Enums.Languages language)
        {
            ocrApi = OcrApi.Create();
            ocrApi.Init(language);
        }

        /// <summary>
        /// Creates an instance of OcrService class with initializing OcrApi with English language
        /// </summary>
        public OcrService()
        {
            ocrApi = OcrApi.Create();
            ocrApi.Init(Patagames.Ocr.Enums.Languages.English);
        }

        ~OcrService()
        {
            ocrApi.Dispose();
        }

        public string GetTextFromImage(Bitmap image)
        {
            return ocrApi.GetTextFromImage(image);
        }
    }
}
