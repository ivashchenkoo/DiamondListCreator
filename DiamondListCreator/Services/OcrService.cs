﻿using System.Drawing;
using Patagames.Ocr;

namespace DiamondListCreator.Services
{
    static class OcrService
    {
        /// <summary>
        /// Recognizes text from bitmap.
        /// The defaul language - English and the default filter - "0123456789BlancErux"
        /// </summary>
        public static string GetTextFromImage(Bitmap image)
        {
            return GetTextFromImage(image, Patagames.Ocr.Enums.Languages.English, "0123456789BlancErux");
        }

        /// <summary>
        /// Recognizes text from bitmap. Can pass the nessesary language and the chars whitelist.
        /// </summary>
        public static string GetTextFromImage(Bitmap image, Patagames.Ocr.Enums.Languages language, string filter)
        {
            using (OcrApi ocrApi = OcrApi.Create())
            {
                ocrApi.Init(language);
                ocrApi.SetVariable("tessedit_char_whitelist", filter);

                return ocrApi.GetTextFromImage(image);
            }
        }
    }
}
