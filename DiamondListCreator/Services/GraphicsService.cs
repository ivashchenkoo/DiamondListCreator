using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;

namespace DiamondListCreator.Services
{
    public class GraphicsService
    {
        public static Graphics GetGraphFromImage(Bitmap image)
        {
            Graphics graph = Graphics.FromImage(image);
            graph.CompositingMode = CompositingMode.SourceOver;
            graph.SmoothingMode = SmoothingMode.HighQuality;
            graph.TextRenderingHint = TextRenderingHint.AntiAlias;
            graph.CompositingQuality = CompositingQuality.HighQuality;

            return graph;
        }

        /// <summary>
        /// Cutting rectangle from bitmap
        /// </summary>
        /// <param name="targetBitmap"></param>
        /// <param name="x">Initial coordinate along the x-axis</param>
        /// <param name="y">Initial coordinate along the y-axis</param>
        /// <param name="width">Rectangle width</param>
        /// <param name="height">Rectangle height</param>
        /// <returns>Cutted rectangle from bitmap</returns>
        public static Bitmap CutRectangleFromBitmap(Bitmap targetBitmap, int x, int y, int width, int height)
        {
            return targetBitmap.Clone(new Rectangle(x, y, width, height), targetBitmap.PixelFormat);
        }

        /// <summary>
        /// Removing the top border from a bitmap that has a specific color
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="borderColor"></param>
        /// <returns>Cutted bitmap</returns>
        public static Bitmap RemoveTopBorder(Bitmap bitmap, Color borderColor)
        {
            for (int i = 0; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    if (bitmap.GetPixel(j, i) != borderColor)
                    {
                        return bitmap.Clone(new Rectangle(0, i, bitmap.Width, bitmap.Height - i), bitmap.PixelFormat);
                    }
                }
            }

            return bitmap;
        }

        /// <summary>
        /// Removing the bottom border from a bitmap that has a specific color
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="borderColor"></param>
        /// <returns>Cutted bitmap</returns>
        public static Bitmap RemoveBottomBorder(Bitmap bitmap, Color borderColor)
        {
            for (int i = 1; i < bitmap.Height; i++)
            {
                for (int j = 0; j < bitmap.Width; j++)
                {
                    //Debug.WriteLine(bitmap.GetPixel(j, bitmap.Height - i) + "  " + bitmap.GetPixel(j, bitmap.Height - i).ToString());
                    if (bitmap.GetPixel(j, bitmap.Height - i) != borderColor)
                    {
                        return bitmap.Clone(new Rectangle(0, 0, bitmap.Width, bitmap.Height - i + 1), bitmap.PixelFormat);
                    }
                }
            }

            return bitmap;
        }

        /// <summary>
        /// Removing the left border from a bitmap that has a specific color
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="borderColor"></param>
        /// <returns>Cutted bitmap</returns>
        public static Bitmap RemoveLeftBorder(Bitmap bitmap, Color borderColor)
        {
            for (int i = 1; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    if (bitmap.GetPixel(bitmap.Width - i, j) != borderColor)
                    {
                        return bitmap.Clone(new Rectangle(0, 0, bitmap.Width - i + 1, bitmap.Height), bitmap.PixelFormat);
                    }
                }
            }

            return bitmap;
        }

        /// <summary>
        /// Removing the right border from a bitmap that has a specific color
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="borderColor"></param>
        /// <returns>Cutted bitmap</returns>
        public static Bitmap RemoveRightBorder(Bitmap bitmap, Color borderColor)
        {
            for (int i = 0; i < bitmap.Width; i++)
            {
                for (int j = 0; j < bitmap.Height; j++)
                {
                    if (bitmap.GetPixel(i, j) != borderColor)
                    {
                        return bitmap.Clone(new Rectangle(i, 0, bitmap.Width - i, bitmap.Height), bitmap.PixelFormat);
                    }
                }
            }

            return bitmap;
        }

        /// <summary>
        /// Removes all norders from a bitmap that has a specific color
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="borderColor"></param>
        /// <returns>A bitmap without borders</returns>
        public static Bitmap RemoveBorders(Bitmap bitmap, Color borderColor)
        {
            bitmap = RemoveLeftBorder(bitmap, borderColor);
            bitmap = RemoveTopBorder(bitmap, borderColor);
            bitmap = RemoveRightBorder(bitmap, borderColor);
            bitmap = RemoveBottomBorder(bitmap, borderColor);

            return bitmap;
        }

        /// <summary>
        /// Creates bitmap with a transparent background and with the passed text and the font size
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        public static Bitmap CreateTextBitmap(string text, FontFamily fontFamily, int fontSize, FontStyle fontStyle, Color color)
        {
            Bitmap textBitmap;
            
            using (Font font = new Font(fontFamily, fontSize, fontStyle))
            {
                textBitmap = CreateTextBitmap(text, font, color);
            }

            return textBitmap;
        }

        /// <summary>
        /// Creates bitmap with a transparent background and with the passed text and the font size
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        public static Bitmap CreateTextBitmap(string text, Font font, Color color)
        {
            Bitmap textBitmap;

            SizeF sizef = MeasureString(text, font);

            textBitmap = new Bitmap((int)sizef.Width, (int)sizef.Height);
            textBitmap.MakeTransparent();
            using (Graphics graph = GetGraphFromImage(textBitmap))
            {
                using (SolidBrush drawBrush = new SolidBrush(color))
                {
                    graph.DrawString(text, font, drawBrush, 0, 0);
                }
            }

            textBitmap = RemoveBorders(textBitmap, Color.FromArgb(0, 0, 0, 0));

            return textBitmap;
        }

        /// <summary>
        /// Creates bitmap with a transparent background and with the passed text and the font size
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        public static Bitmap CreateTextBitmapWithShadow(string text, FontFamily fontFamily, int fontSize, FontStyle fontStyle, Color color, Color shadowColor, Size shadowOffset)
        {
            Bitmap textBitmap;

            using (Font font = new Font(fontFamily, fontSize, fontStyle))
            {
                textBitmap = CreateTextBitmapWithShadow(text, font, color, shadowColor, shadowOffset);
            }

            return textBitmap;
        }

        /// <summary>
        /// Creates bitmap with a transparent background, shadow and with the passed text and the font size
        /// </summary>
        /// <param name="text"></param>
        /// <param name="fontSize"></param>
        /// <returns></returns>
        public static Bitmap CreateTextBitmapWithShadow(string text, Font font, Color color, Color shadowColor, Size shadowOffset)
        {
            Bitmap textBitmap;

            SizeF sizef = MeasureString(text, font);

            textBitmap = new Bitmap((int)sizef.Width, (int)sizef.Height);
            textBitmap.MakeTransparent();
            using (Graphics graph = GetGraphFromImage(textBitmap))
            {
                using (SolidBrush shadowBrush = new SolidBrush(shadowColor))
                {
                    graph.DrawString(text, font, shadowBrush, shadowOffset.Width, shadowOffset.Height);
                }
                using (SolidBrush drawBrush = new SolidBrush(color))
                {
                    graph.DrawString(text, font, drawBrush, 0, 0);
                }
            }

            textBitmap = RemoveBorders(textBitmap, Color.FromArgb(0, 0, 0, 0));

            return textBitmap;
        }

        /// <summary>
        /// Measures the size of the text with the font
        /// </summary>
        /// <param name="text"></param>
        /// <param name="font"></param>
        /// <returns></returns>
        public static SizeF MeasureString(string text, Font font)
        {
            SizeF result;
            using (Bitmap image = new Bitmap(1, 1))
            {
                using (Graphics graph = Graphics.FromImage(image))
                {
                    result = graph.MeasureString(text, font);
                }
            }

            return result;
        }
    }
}