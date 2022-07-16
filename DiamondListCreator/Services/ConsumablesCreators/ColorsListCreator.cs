using DiamondListCreator.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace DiamondListCreator.Services.ConsumablesCreators
{
    public class ColorsListCreator
    {
        /// <summary>
        /// Creates a list with diamond colors
        /// </summary>
        /// <returns>List of diamond colors</returns>
        public List<DiamondColor> Create(DiamondSettings diamond)
        {
            List<DiamondColor> diamondColors = new List<DiamondColor>();

            if (File.Exists($"{diamond.Path}/Легенда.png"))
            {
                using (Bitmap legend = new Bitmap($"{diamond.Path}/Легенда.png"))
                {
                    diamondColors.AddRange(ParseString(LegendOcr(legend)));
                }
            }
            else
            {
                using (Bitmap legend = new Bitmap($"{diamond.Path}/Легенда, лист 1.png"))
                {
                    diamondColors.AddRange(ParseString(LegendOcr(legend)));
                }

                using (Bitmap legend = new Bitmap($"{diamond.Path}/Легенда, лист 2.png"))
                {
                    diamondColors.AddRange(ParseString(LegendOcr(legend)));
                }

                if (File.Exists($"{diamond.Path}/Легенда, лист 3.png"))
                {
                    using (Bitmap legend = new Bitmap($"{diamond.Path}/Легенда, лист 3.png"))
                    {
                        diamondColors.AddRange(ParseString(LegendOcr(legend)));
                    }
                }
            }

            return diamondColors;
        }

        /// <summary>
        /// Parses string which has the form of a table, to DiamondColor array
        /// </summary>
        /// <param name="text"></param>
        /// <returns>List of DiamondColor objects</returns>
        private List<DiamondColor> ParseString(string text)
        {
            string[] textRows = text.Split('\n');

            List<DiamondColor> colors = new List<DiamondColor>();

            for (int i = 0; i < textRows.Length; ++i)
            {
                string[] rowItems = textRows[i].Split(' ');
                if (rowItems.Length > 2)
                {
                    if (rowItems[0].Length == 2)
                    {
                        rowItems[0] += rowItems[1];
                        string diamsCount = "";
                        for (int j = 2; j < rowItems.Length; j++)
                        {
                            diamsCount += rowItems[j];
                        }
                        rowItems[1] = diamsCount;
                    }
                    else
                    {
                        for (int j = 2; j < rowItems.Length; j++)
                        {
                            rowItems[1] += rowItems[j];
                        }
                    }
                }

                if (rowItems.Length < 2)
                {
                    continue;
                }

                if (rowItems.Length == 2 && !int.TryParse(rowItems[0], out _))
                {
                    if (rowItems[0] == "anc")
                    {
                        rowItems[0] = "Blanc";
                    }
                    else if (rowItems[0] == "Ean")
                    {
                        rowItems[0] = "Ecru";
                    }
                }

                colors.Add(new DiamondColor
                {
                    Name = rowItems[0],
                    Quantity = Convert.ToInt32(rowItems[1])
                });
            }

            return colors;
        }

        /// <summary>
        /// Recognizes text on the legend bitmap
        /// </summary>
        /// <param name="legend"></param>
        /// <returns>Recognized text</returns>
        private string LegendOcr(Bitmap legend)
        {
            legend = GraphicsService.CutRectangleFromBitmap(legend, 720, 550, 605, 2695);
            OcrService ocrService = new OcrService();
            return ocrService.GetTextFromImage(legend);
        }
    }
}
