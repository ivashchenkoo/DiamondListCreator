using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using DiamondListCreator.Models;
using DiamondListCreator.Services.ConsumablesCreators;

namespace DiamondListCreator.Services
{
    public class LegendsService : IDisposable
    {
        private readonly LegendCreator legendCreator;

        public LegendsService()
        {
            legendCreator = new LegendCreator();
        }

        public void Dispose()
        {
            legendCreator.Dispose();
        }

        /// <summary>
        /// Creates list with legends for specified diamond
        /// </summary>
        /// <returns>Array of bitmaps with created/retrieved legends</returns>
        public Bitmap[] CreateLegends(DiamondSettings diamond, string savedLegendsPath)
        {
            List<Bitmap> legends = new List<Bitmap>(CreateOrGetLegends(diamond, savedLegendsPath, false));

            if (diamond.IsEnglishVersion)
            {
                legends.AddRange(CreateOrGetLegends(diamond, savedLegendsPath, true));
            }

            return legends.ToArray();
        }

        /// <summary>
        /// Creates legends or finds them if they are already created
        /// </summary>
        /// <param name="diamond"></param>
        private Bitmap[] CreateOrGetLegends(DiamondSettings diamond, string savedLegendsPath, bool isEnglish)
        {
            string legendSavePath = Path.Combine(savedLegendsPath, $"{diamond.ShortName.Substring(0, 2)}000");
            string diamondName = diamond.Name + (isEnglish ? "E" : "");

            if (diamond.DiamondType == DiamondType.Standard && GetSavedLegends(diamondName, legendSavePath) is Bitmap[] savedLegends)
            {
                return savedLegends;
            }
            else
            {
                Bitmap[] legends = isEnglish ? legendCreator.CreateEnglish(diamond) : legendCreator.CreateUkrainian(diamond);

                if (diamond.DiamondType == DiamondType.Standard)
                {
                    FileService.SaveBitmapsInTif(legends, legendSavePath, diamondName);
                }

                return legends;
            }
        }

        /// <summary>
        /// Checking for already created legends in legends saving folder and get it if exist
        /// </summary>
        /// <param name="savedLegendsPath">Legends saving folder</param>
        /// <returns>Array of Legends Bitmaps if it exist, otherwise returns null</returns>
        private Bitmap[] GetSavedLegends(string diamondName, string legendSavePath)
        {
            Bitmap[] result;

            if (!Directory.Exists(legendSavePath))
            {
                Directory.CreateDirectory(legendSavePath);
            }

            if (File.Exists(Path.Combine(legendSavePath, $"{diamondName}.tif")))
            {
                if (File.Exists(Path.Combine(legendSavePath, $"{diamondName}_1.tif")))
                {
                    result = new Bitmap[2];
                    result[1] = new Bitmap(Path.Combine(legendSavePath, $"{diamondName}_1.tif"));
                }
                else
                {
                    result = new Bitmap[1];
                }

                result[0] = new Bitmap(Path.Combine(legendSavePath, $"{diamondName}.tif"));

                return result;
            }
            else
            {
                return null;
            }
        }
    }
}