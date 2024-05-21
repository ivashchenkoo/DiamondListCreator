using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DiamondListCreator.Models;

namespace DiamondListCreator.Services
{
    static class ListStickersService
    {
        private static readonly double bigZipWage = 23.5;
        private static readonly double smallZipWage = 10.5;

        /// <summary>
        /// Creates pdf with diamonds colors list stickers
        /// </summary>
        /// <param name="diamondsColors"></param>
        /// <param name="savePath"></param>
        public static void CreateListStickersPdf(List<DiamondColor> diamondsColors, string savePath, string fileName)
        {
            List<DiamondColor> sortedColors = diamondsColors.Where(x => Regex.IsMatch(x.Name, @"^\d+$")).OrderByDescending(x => Convert.ToInt32(x.Name)).ToList();
            sortedColors.InsertRange(0, diamondsColors.Where(x => !Regex.IsMatch(x.Name, @"^\d+$")).OrderByDescending(x => x.Name).ToList());

            using (PdfDocumentService document = new PdfDocumentService(65, 40))
            {
                foreach (var color in sortedColors)
                {
                    double weight = color.Weight;
                    int bigZipCount = Convert.ToInt32(Math.Truncate(weight / bigZipWage));
                    int smallZipCount = 0;
                    weight %= bigZipWage;
                    if (weight > smallZipWage)
                    {
                        bigZipCount++;
                    }
                    else
                    {
                        smallZipCount++;
                    }

                    for (int i = 0; i < bigZipCount; i++)
                    {
                        _ = document.NewPage();
                        int fontSize = 24;
                        int offsetX;
                        int offsetY;
                        switch ((color.Name + "*").Length)
                        {
                            case 4:
                                offsetX = 8;
                                offsetY = 11;
                                fontSize = 24;
                                break;

                            case 5:
                                offsetX = 4;
                                offsetY = 12;
                                fontSize = 22;
                                break;

                            case 6:
                                offsetX = 3;
                                offsetY = 12;
                                fontSize = 20;
                                break;

                            default:
                                offsetX = 8;
                                offsetY = 11;
                                fontSize = 24;
                                break;
                        }

                        document.DrawText(color.Name + "*", fontSize, offsetX, offsetY);
                    }

                    for (int i = 0; i < smallZipCount; i++)
                    {
                        _ = document.NewPage();
                        int offsetX;
                        int offsetY;
                        int fontSize = 24;
                        switch (color.Name.Length)
                        {
                            case 3:
                                offsetX = 11;
                                offsetY = 11;
                                break;

                            case 4:
                                offsetX = 4;
                                offsetY = 11;
                                break;

                            default:
                                offsetX = 4;
                                offsetY = 11;
                                break;
                        }

                        document.DrawText(color.Name, fontSize, offsetX, offsetY);
                    }
                }

                document.Save(savePath, fileName); ;
            }
        }
    }
}
