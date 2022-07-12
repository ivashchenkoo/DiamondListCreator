using DiamondListCreator.Models;
using DiamondListCreator.Services.ConsumablesCreators;
using System;
using System.Collections.Generic;
using System.Drawing;

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
            PdfDocumentService document = new PdfDocumentService(2480, 3507);

            StickerCreator stickerCreator = new StickerCreator(FontCollectionService.InitCustomFont(Properties.Resources.VanishingSizeName_Regular));
            Bitmap[] stickers = stickerCreator.CreateStickersPage(diamonds);
            
            document.AddPages(stickers);
            document.Save(paths.FilesSavePath + "/Stickers " + DateTime.Today.ToString().Substring(0, 10) + ".pdf");
        }
    }
}
