using DiamondListCreator.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiamondListCreator.Services.ConsumablesCreators
{
    public class StickerCreator
    {
        public StickerCreator()
        {

        }

        /// <summary>
        /// Appending sticker on page on specified position
        /// </summary>
        /// <returns>Page with appended sticker on specified position</returns>
        public Bitmap AppendStickerOnPage(DiamondSettings diamond, int row, int column)
        {
            return null;
        }

        /// <summary>
        /// Appending only 3 sticker on specified row, even if the number of list items is larger
        /// </summary>
        /// <returns>Page with appended 3 stickers on specified row</returns>
        public Bitmap AppendStickersRowOnPage(List<DiamondSettings> diamonds, int row)
        {
            return null;
        }
    }
}
