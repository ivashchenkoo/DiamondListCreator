using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DiamondListCreator.Models;

namespace DiamondListCreator.Services
{
    static class DiamondSettingsService
    {
        /// <summary>
        /// Converts passed string to the list with DiamondSettings.
        /// Checks diamonds pathes for issues and throws exception if issues are exist.
        /// Throws exception if the list has no items.
        /// </summary>
        /// <param name="diamondsListStr">String from the diamonds list text box</param>
        /// <param name="mainDiamondsDirectory">Path to the main diamonds directory</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static List<DiamondSettings> GetFromString(string diamondsListStr, string mainDiamondsDirectory)
        {
            List<DiamondSettings> diamonds = new List<DiamondSettings>();

            string failedDiamonds = "";

            string[] diamondsList = diamondsListStr.ToUpper().Replace((char)13, (char)32).Replace(" ", "").Replace('\n', (char)32).Replace("  ", " ").Split(' ');
            diamondsList = diamondsList.Where(x => x.Length > 3).ToArray();

            foreach (var item in diamondsList)
            {
                string sizeLetter = GetSizeLetter(item);
                bool isEnglish = sizeLetter.Contains("E");
                bool isStretched = sizeLetter.Contains("P");

                sizeLetter = sizeLetter.Replace("E", "").Replace("P", "");

                string name = GetDigitsFromString(item) + sizeLetter;

                DiamondSettings diamond = new DiamondSettings
                {
                    Name = $"TWD{name}",
                    ShortName = name,
                    SizeLetter = sizeLetter.Replace("+", ""),
                    IsEnglishVersion = isEnglish,
                    IsStretchedCanvas = isStretched
                };

                string path;
                if (name.StartsWith("0"))
                {
                    diamond.DiamondType = DiamondType.PoPhoto;
                    path = $"{mainDiamondsDirectory}/almaz_4u/{name}";
                }
                else
                {
                    diamond.DiamondType = DiamondType.Standard;
                    path = $"{mainDiamondsDirectory}/almaz/{name.Substring(0, 2)}000/{name.Replace("M", "").Replace("SL", "")}";
                }

                diamond.Path = path;

                if (!Directory.Exists(path))
                {
                    failedDiamonds += $"{name} - не знайдено\n";
                    continue;
                }
                else if (HasSubfolders(path))
                {
                    failedDiamonds += $"{name} - папка new\n";
                    continue;
                }

                if (sizeLetter.Contains("+"))
                {
                    AddCustomWidthAndHeight(ref diamond);
                }
                else
                {
                    diamond.Width = GetStandardWidth(sizeLetter);
                    diamond.Height = GetStandardHeight(sizeLetter);
                }

                diamonds.Add(diamond);
            }

            if (failedDiamonds != "")
            {
                throw new Exception(failedDiamonds);
            }
            if (diamonds.Count == 0)
            {
                throw new Exception("Не вірно введені номери алмазок");
            }

            return diamonds;
        }

        /// <summary>
        /// Checks if the directory has subfolders
        /// </summary>
        /// <param name="path"></param>
        /// <returns>True if the directory has subfolders, false if its not</returns>
        private static bool HasSubfolders(string path)
        {
            IEnumerable<string> subfolders = Directory.EnumerateDirectories(path);
            return subfolders != null && subfolders.Any();
        }

        /// <summary>
        /// Scans width and height from the diamond legend list and adds them to passed diamond object as ref
        /// </summary>
        /// <param name="diamond"></param>
        private static void AddCustomWidthAndHeight(ref DiamondSettings diamond)
        {
            using (Bitmap legend = File.Exists(diamond.Path + "/Легенда, лист 1.png")
                    ? new Bitmap(diamond.Path + "/Легенда, лист 1.png")
                    : new Bitmap(diamond.Path + "/Легенда.png"))
            {
                string ocrText = OcrService.GetTextFromImage(GraphicsService.CutRectangleFromBitmap(legend, 1190, 120, 450, 90)).Trim();

                ocrText = ocrText.ToLower().Replace("l", "1");
                string[] sizes = ocrText.Split('x');

                _ = int.TryParse(string.Join("", sizes[0].Where(c => char.IsDigit(c))), out int sizeWidth);
                _ = int.TryParse(string.Join("", sizes[1].Where(c => char.IsDigit(c))), out int sizeHeight);

                diamond.Width = sizeWidth;
                diamond.Height = sizeHeight;
            }
        }

        /// <summary>
        /// Gives standard width for passed size
        /// </summary>
        /// <param name="sizeLetter"></param>
        /// <returns></returns>
        private static int GetStandardWidth(string sizeLetter)
        {
            switch (sizeLetter)
            {
                case "S": return 20;
                case "M": return 30;
                case "L": return 40;
                case "XL": return 50;
                case "SL": return 40;
                default: return 0;
            }
        }

        /// <summary>
        /// Gives standard height for passed size
        /// </summary>
        /// <param name="sizeLetter"></param>
        /// <returns></returns>
        private static int GetStandardHeight(string sizeLetter)
        {
            switch (sizeLetter)
            {
                case "S": return 30;
                case "M": return 40;
                case "L": return 50;
                case "XL": return 60;
                case "SL": return 50;
                default: return 0;
            }
        }

        /// <summary>
        /// Removes chars, leaving only numbers
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static string GetDigitsFromString(string text)
        {
            return Regex.Match(text, @"\d+").Value;
        }

        /// <summary>
        /// Cuts size letters from diamond name
        /// </summary>
        /// <param name="name"></param>
        /// <returns>All size letters as string</returns>
        private static string GetSizeLetter(string name)
        {
            string size = "";

            for (int i = name.Length - 1; i >= 0; i--)
            {
                if (int.TryParse(name.Substring(i, 1), out _))
                {
                    if (i == name.Length - 1)
                    {
                        if (name[1] == 1)
                        {
                            size = "SL";
                        }
                        else
                        {
                            size = "M";
                        }
                        break;
                    }
                    else
                    {
                        size = name.Substring(i + 1, name.Length - i - 1);
                        break;
                    }
                }
            }

            return size;
        }
    }
}
