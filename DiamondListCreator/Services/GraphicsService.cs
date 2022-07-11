using System.Drawing;
using System.Drawing.Text;

namespace DiamondListCreator.Services
{
    public class GraphicsService
    {
        public static Graphics GetGraphFromImage(Bitmap image)
        {
            Graphics graph = Graphics.FromImage(image);
            graph.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            graph.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graph.TextRenderingHint = TextRenderingHint.AntiAlias;
            graph.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;

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
    }
}
