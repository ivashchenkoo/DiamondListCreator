using DiamondListCreator.Models;
using System.Collections.Generic;
using System.IO;
using DiamondListCreator.Services.ConsumablesCreators;
using System.Drawing;

namespace DiamondListCreator.Services
{
    public class LegendsService
    {
        private readonly LegendCreator legendCreator;

        public LegendsService()
        {
            legendCreator = new LegendCreator(FontCollectionService.InitCustomFont(Properties.Resources.VanishingSizeName_Regular));
        }

        /// <summary>
        /// Creates legends or finds them if they are already created
        /// </summary>
        /// <param name="diamond"></param>
        /// <param name="paths">Path settings</param>
        public Bitmap[] CreateLegends(DiamondSettings diamond, PathSettings paths)
        {
            if (diamond.DiamondType == DiamondType.Standard && GetSavedLegends(diamond, paths.SavedLegendsPath) is Bitmap[] savedLegends)
            {
                return savedLegends;
            }
            else
            {
                List<Bitmap> legends = new List<Bitmap>(legendCreator.CreateUkrainian(diamond));

                if (diamond.DiamondType == DiamondType.Standard)
                {
                    if (Directory.Exists(paths.SavedLegendsPath))
                    {
                        FileService.SaveBitmapsInTif(legends.ToArray(), paths.SavedLegendsPath, diamond.Name);
                    }
                }

                if (diamond.IsEnglishVersion)
                {
                    legends.AddRange(legendCreator.CreateEnglish(diamond));
                }

                return legends.ToArray();
            }           
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