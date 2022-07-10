using DiamondListCreator.Models;
using System;
using System.Collections.Generic;
using System.IO;
using iTextSharp.text;
using iTextSharp.text.pdf;
using DiamondListCreator.Services.ConsumablesCreators;
using System.Drawing.Imaging;

namespace DiamondListCreator.Services
{
    public class LegendsService
    {
        /// <summary>
        /// Creating Legends and saving them into pdf /Legends2List {Date.Now}.pfd
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
                System.Drawing.Bitmap[] legends = legendCreator.Create(diamonds[i], paths.SavedLegendsPath);

                for (int j = legends.Length - 1; j >= 0; j--)
                {
                    Image image = Image.GetInstance(legends[j], ImageFormat.Tiff);
                    _ = document.Add(image);
                }
            }

            document.Close();
            byte[] content = stream.ToArray();
            using (FileStream fs = File.Create(paths.FilesSavePath + "/Legends2List " + DateTime.Today.ToString().Substring(0, 10) + ".pdf"))
            {
                fs.Write(content, 0, content.Length);
            }
        }
    }
}