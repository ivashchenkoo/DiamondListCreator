using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using DiamondListCreator.Models;

namespace DiamondListCreator.Services.ConsumablesCreators
{
    static class ColorsListCreator
    {
        /// <summary>
        /// Creates a list with diamond colors
        /// </summary>
        /// <returns>List of diamond colors</returns>
        public static List<DiamondColor> Create(DiamondSettings diamond)
        {
            if (File.Exists($"{diamond.Path}/Легенда.png"))
            {
                using (Bitmap legend = new Bitmap($"{diamond.Path}/Легенда.png"))
                {
                    return GetDiamondColorsFromShortLegend(legend);
                }
            }
            else
            {
                List<DiamondColor> diamondColors = new List<DiamondColor>();

                using (Bitmap legend = new Bitmap($"{diamond.Path}/Легенда, лист 1.png"))
                {
                    diamondColors.AddRange(GetDiamondColorsFromShortLegend(legend));
                }

                using (Bitmap legend = new Bitmap($"{diamond.Path}/Легенда, лист 2.png"))
                {
                    diamondColors.AddRange(GetDiamondColorsFromLegend(legend));
                }

                if (File.Exists($"{diamond.Path}/Легенда, лист 3.png"))
                {
                    using (Bitmap legend = new Bitmap($"{diamond.Path}/Легенда, лист 3.png"))
                    {
                        diamondColors.AddRange(GetDiamondColorsFromLegend(legend));
                    }
                }

                return diamondColors;
            }
        }

        /// <summary>
        /// Creates the DiamondColors list from not the first page of legends ("Легенда, лист 2" or "Легенда, лист 3")
        /// </summary>
        /// <param name="legend"></param>
        /// <returns></returns>
        private static List<DiamondColor> GetDiamondColorsFromLegend(Bitmap legend)
        {
            if (ParseString(LegendOcr(legend, new Rectangle(720, 550, 300, 2680)), LegendOcr(legend, new Rectangle(1140, 550, 300, 2680))) is List<DiamondColor> colors)
            {
                return colors;
            }
            else
            {
                return ParseString(LegendOcr(legend, new Rectangle(720, 550, 650, 2680)));
            }
        }

        /// <summary>
        /// Creates the DiamondColors list from the first page of legends ("Легенда" or "Легенда, лист 1")
        /// </summary>
        /// <param name="legend"></param>
        /// <returns></returns>
        private static List<DiamondColor> GetDiamondColorsFromShortLegend(Bitmap legend)
        {
            if (ParseString(LegendOcr(legend, new Rectangle(720, 770, 300, 2480)), LegendOcr(legend, new Rectangle(1140, 770, 300, 2480))) is List<DiamondColor> colors)
            {
                return colors;
            }
            else
            {
                return ParseString(LegendOcr(legend, new Rectangle(720, 770, 605, 2480)));
            }
        }

        /// <summary>
        /// Parses strings which has the form of a columns to DiamondColor list
        /// </summary>
        /// <param name="text"></param>
        /// <returns>List of DiamondColor objects or null if colums have different rows count</returns>
        private static List<DiamondColor> ParseString(string firstColumn, string secondColumn)
        {
            string[] firstColumnArr = firstColumn.Split('\n').Where(x => x.Length > 2).ToArray();
            string[] secondColumnArr = secondColumn.Split('\n').Where(x => x != string.Empty).ToArray();

            if (firstColumnArr.Length != secondColumnArr.Length)
            {
                return null;
            }

            List<DiamondColor> colors = new List<DiamondColor>();
            for (int i = 0; i < firstColumnArr.Length; i++)
            {
                colors.Add(new DiamondColor
                {
                    Name = firstColumnArr[i],
                    Quantity = Convert.ToInt32(secondColumnArr[i])
                });
            }

            return colors;
        }

        /// <summary>
        /// Parses string which has the form of a table, to DiamondColor list
        /// </summary>
        /// <param name="text"></param>
        /// <returns>List of DiamondColor objects</returns>
        private static List<DiamondColor> ParseString(string table)
        {
            string[] tableArr = table.Split('\n').Where(x => x.Length > 4).ToArray();

            List<DiamondColor> colors = new List<DiamondColor>();
            for (int i = 0; i < tableArr.Length; ++i)
            {
                string[] rowItems = tableArr[i].Split(' ');
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
        /// <returns>Recognized text</returns>
        private static string LegendOcr(Bitmap legend, Rectangle rectangle)
        {
            legend = GraphicsService.CutRectangleFromBitmap(legend, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
            return OcrService.GetTextFromImage(legend);
        }
    }
}
