using System;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using DiamondListCreator.Models;

namespace DiamondListCreator.Services.ConsumablesCreators
{
    public class StickerCreator : IDisposable
    {
        private readonly PrivateFontCollection pfc;

        public StickerCreator()
        {
            pfc = FontCollectionService.InitCustomFont(Properties.Resources.VanishingSizeName_Regular);
        }

        public void Dispose()
        {
            pfc.Dispose();
        }

        /// <summary>
        /// Appending sticker on page on specified position
        /// </summary>
        /// <returns>Page with appended sticker on specified position</returns>
        public Bitmap AppendStickerOnPage(Bitmap page, DiamondSettings diamond, int row, int column)
        {
            using (Graphics graph = GraphicsService.GetGraphFromImage(page))
            {
                int osX = 0;
                int osY = 0;

                // Thumbnail
                using (Bitmap OverlayBitmap = new Bitmap(Path.Combine(diamond.Path, "Вид вышивки.png")))
                {
                    if (OverlayBitmap.Height > OverlayBitmap.Width)
                    {
                        OverlayBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    }
                    float OldWidth = OverlayBitmap.Width;
                    float OldHeight = OverlayBitmap.Height;
                    float appendedImageWidth = 808f;
                    float appendedImageHeight = 640f;
                    float newHeight = OldHeight / OldWidth < 0.6 ? appendedImageWidth / (OldWidth / OldHeight) : appendedImageHeight;
                    osX = 822 * column;
                    osY = 820 * row;
                    graph.DrawImage(OverlayBitmap, 14 + osX, 6 + osY + (appendedImageHeight - newHeight), appendedImageWidth, newHeight);
                    graph.DrawImage(OverlayBitmap, 14 + osX, 6 + osY + (appendedImageHeight - newHeight), appendedImageWidth, newHeight);
                    graph.DrawImage(OverlayBitmap, 14 + osX, 6 + osY + (appendedImageHeight - newHeight), appendedImageWidth, newHeight);
                }


                // Name
                using (SolidBrush drawBrush = new SolidBrush(Color.Black))
                {
                    using (Font font = new Font(pfc.Families[0], 120))
                    {
                        SizeF sizef;
                        if (diamond.Name.EndsWith("+"))
                        {
                            sizef = graph.MeasureString(diamond.Name.Substring(0, diamond.Name.Length - 1), font);
                            SizeF plusSize = graph.MeasureString("+", new Font(pfc.Families[0], 90));
                            graph.DrawString(diamond.Name.Substring(0, diamond.Name.Length - 1), font, drawBrush, (((page.Width / 3) - sizef.Width) / 2) + osX - (plusSize.Width / 3f) + 30f, 650 + ((200 - sizef.Height) / 2) + osY);
                            graph.DrawString("+", new Font(pfc.Families[0], 90), drawBrush, (((page.Width / 3) - sizef.Width) / 2) + osX + sizef.Width - plusSize.Width + 60f, 650 + ((200 - plusSize.Height) / 2) + osY);
                        }
                        else
                        {
                            sizef = graph.MeasureString(diamond.Name, font);
                            graph.DrawString(diamond.Name, font, drawBrush, (((page.Width / 3) - sizef.Width) / 2) + osX, 650 + ((200 - sizef.Height) / 2) + osY);
                        }
                    }

                    // Lines
                    using (Pen pen = new Pen(drawBrush, 14))
                    {
                        graph.DrawLine(pen, osX + 7, osY, osX + 7, 650 + osY);
                        graph.DrawLine(pen, (822 * (column + 1)) + 7, osY, (822 * (column + 1)) + 7, 650 + osY);
                    }

                    using (Pen pen = new Pen(drawBrush, 10))
                    {
                        graph.DrawLine(pen, osX, 3 + osY, 822 * (column + 1), 3 + osY);
                        graph.DrawLine(pen, osX, 6 + 640 + osY, 822 * (column + 1), 6 + 640 + osY);

                        if (column == 0)
                        {
                            graph.DrawLine(pen, 0, 3 + (820 * (row + 1)), 2480, 3 + (820 * (row + 1)));
                        }
                    }
                }
            }

            return page;
        }
    }
}
