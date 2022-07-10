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
    }
}
