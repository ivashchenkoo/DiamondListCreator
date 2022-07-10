using DiamondListCreator.Models;
using DiamondListCreator.Services.ConsumablesCreators;
using iTextSharp.text;
using iTextSharp.text.pdf;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;
using Rectangle = iTextSharp.text.Rectangle;

namespace DiamondListCreator.Services
{
    public class StickersService
    {
        /// <summary>
        /// Creating Stickers and saving them into pdf /Stickers {Date.Now}.pdf
        /// </summary>
        /// <param name="diamonds">DiamondSettings list</param>
        /// <param name="paths">Path settings</param>
        public void CreateStickersPdf(List<DiamondSettings> diamonds, PathSettings paths)
        {
            MemoryStream stream = new MemoryStream();
            Rectangle pageSize = new Rectangle(0, 0, 2480, 3507);
            Document document = new Document(pageSize, 0, 0, 0, 0);
            _ = PdfWriter.GetInstance(document, stream);
            document.Open();

            StickerCreator stickerCreator = new StickerCreator(FontCollectionService.InitCustomFont(Properties.Resources.VanishingSizeName_Regular));
            System.Drawing.Bitmap[] stickers = stickerCreator.CreateStickersPage(diamonds);

            for (int j = stickers.Length - 1; j >= 0; j--)
            {
                Image image = Image.GetInstance(stickers[j], ImageFormat.Tiff);
                _ = document.Add(image);
            }

            document.Close();
            byte[] content = stream.ToArray();
            using (FileStream fs = File.Create(paths.FilesSavePath + "/Stickers " + DateTime.Today.ToString().Substring(0, 10) + ".pdf"))
            {
                fs.Write(content, 0, content.Length);
            }
        }
    }
}
