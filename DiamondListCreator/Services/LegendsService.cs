using DiamondListCreator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using DiamondListCreator.Services.ConsumablesCreators;
using System.Drawing.Imaging;
using Bitmap = System.Drawing.Bitmap;

namespace DiamondListCreator.Services
{
    public class LegendsService
    {
        /// <summary>
        /// Creating Legends and saving them into pdf /Legends2List {Date.Now}.pdf
        /// </summary>
        /// <param name="diamonds">DiamondSettings list</param>
        /// <param name="paths">Path settings</param>
        public void CreateLegendsPdf(List<DiamondSettings> diamonds, PathSettings paths)
        {
            MemoryStream stream = new MemoryStream();
            Rectangle pageSize = new Rectangle(0, 0, 2480, 3507);
            Document document = new Document(pageSize, 0, 0, 0, 0);
            _ = PdfWriter.GetInstance(document, stream);
            document.Open();

            LegendCreator legendCreator = new LegendCreator(FontCollectionService.InitCustomFont(Properties.Resources.VanishingSizeName_Regular));

            for (int i = 0; i < diamonds.Count; i++)
            {
                if (GetSavedLegends(diamonds[i], paths.SavedLegendsPath) is Bitmap[] savedLegends)
                {
                    AppendPagesToPdf(savedLegends, ref document);
                }
                else
                {
                    Bitmap[] legends = legendCreator.CreateUkrainian(diamonds[i]);
                    AppendPagesToPdf(legends, ref document);
                    FileService.SaveBitmapsInTif(legends, paths.SavedLegendsPath, diamonds[i].Name);

                    if (diamonds[i].IsEnglishVersion)
                    {
                        legends = legendCreator.CreateEnglish(diamonds[i]);
                        AppendPagesToPdf(legends, ref document);
                        FileService.SaveBitmapsInTif(legends, paths.SavedLegendsPath, diamonds[i].Name);
                    }
                }
            }

            document.Close();
            byte[] content = stream.ToArray();
            using (FileStream fs = File.Create(paths.FilesSavePath + "/Legends2List " + DateTime.Today.ToString().Substring(0, 10) + ".pdf"))
            {
                fs.Write(content, 0, content.Length);
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

        private void AppendPagesToPdf(Bitmap[] pages, ref Document document)
        {
            for (int j = pages.Length - 1; j >= 0; j--)
            {
                Image image = Image.GetInstance(pages[j], ImageFormat.Tiff);
                _ = document.Add(image);
            }
        }
    }
}