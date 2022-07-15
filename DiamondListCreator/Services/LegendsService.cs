using DiamondListCreator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using DiamondListCreator.Services.ConsumablesCreators;
using System.Drawing;

namespace DiamondListCreator.Services
{
    public class LegendsService
    {
        /// <summary>
        /// Creating Legends and saving them into pdf /Legends {Date.Now}.pdf
        /// </summary>
        /// <param name="diamonds">DiamondSettings list</param>
        /// <param name="paths">Path settings</param>
        public void CreateLegendsPdf(List<DiamondSettings> diamonds, PathSettings paths)
        {
            PdfDocumentService document = new PdfDocumentService(2480, 3507);

            LegendCreator legendCreator = new LegendCreator(FontCollectionService.InitCustomFont(Properties.Resources.VanishingSizeName_Regular));

            for (int i = 0; i < diamonds.Count; i++)
            {
                if (diamonds[i].DiamondType == DiamondType.Standard && GetSavedLegends(diamonds[i], paths.SavedLegendsPath) is Bitmap[] savedLegends)
                {
                    document.AddPagesReverse(savedLegends);
                }
                else
                {
                    Bitmap[] legends = legendCreator.CreateUkrainian(diamonds[i]);
                    document.AddPagesReverse(legends);

                    if (diamonds[i].DiamondType == DiamondType.Standard)
                    {
                        FileService.SaveBitmapsInTif(legends, paths.SavedLegendsPath, diamonds[i].Name);
                    }
                }

                if (diamonds[i].IsEnglishVersion)
                {
                    Bitmap[] legends = legendCreator.CreateEnglish(diamonds[i]);
                    document.AddPagesReverse(legends);
                }
            }

            document.Save($"{paths.FilesSavePath}/Legends {DateTime.Now: dd.MM.yyyy}.pdf");
        }

        /// <summary>
        /// Checking for already created legends in legends saving folder and get it if exist
        /// </summary>
        /// <param name="savedLegendsPath">Legends saving folder</param>
        /// <returns>Array of Legends Bitmaps if it exist, otherwise returns null</returns>
        private Bitmap[] GetSavedLegends(DiamondSettings diamond, string savedLegendsPath)
        {
            Bitmap[] result;
            string legendPath = $"{savedLegendsPath}/{diamond.ShortName.Substring(0, 2)}000";

            if (File.Exists($"{legendPath}/{diamond.Name}.tif"))
            {
                if (File.Exists($"{legendPath}/{diamond.Name}_1.tif"))
                {
                    result = new Bitmap[2];
                    result[1] = new Bitmap($"{legendPath}/{diamond.Name}_1.tif");
                }
                else
                {
                    result = new Bitmap[1];
                }

                result[0] = new Bitmap($"{legendPath}/{diamond.Name}.tif");

                return result;
            }
            else
            {
                return null;
            }
        }
    }
}