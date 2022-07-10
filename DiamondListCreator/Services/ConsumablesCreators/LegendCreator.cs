using DiamondListCreator.Models;
using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;

namespace DiamondListCreator.Services.ConsumablesCreators
{
    public class LegendCreator
    {
        private readonly PrivateFontCollection pfc;

        public LegendCreator()
        {
            pfc = InitCustomFont();
        }

        ~LegendCreator()
        {
            pfc.Dispose();
        }

        /// <summary>
        /// Creating Legend pages for diamond
        /// </summary>
        /// <returns>Array of Bitmaps with Created Legends.</returns>
        /// <exception cref="If the required files to create legends are missing in the root folder of diamond"></exception>
        public Bitmap[] Create(DiamondSettings diamond, string savedLegendsPath)
        {
            if ((File.Exists(diamond.Path + "/Легенда, лист 1.png") && File.Exists(diamond.Path + "/Легенда, лист 2.png")) || File.Exists(diamond.Path + "/Легенда.png"))
            {
                if (GetSavedLegends(diamond, savedLegendsPath) is Bitmap[] savedLegends)
                {
                    return savedLegends;
                }

                using (Bitmap legendPage = CreateLegendTemplate(diamond))
                {
                    Bitmap[] legends;

                    if (File.Exists(diamond.Path + "/Легенда, лист 3.png"))
                    {
                        legends = new Bitmap[2];

                        using (Bitmap legend3Bitmap = new Bitmap(diamond.Path + "/Легенда, лист 3.png"))
                        {
                            legends[1] = AppendColumnOfLegend(new Bitmap(legendPage), GraphicsService.CutRectangleFromBitmap(legend3Bitmap, 250, 540, 1100, 2450), 95, 775);
                        }

                        if (diamond.DiamondType == DiamondType.Standart)
                        {
                            SaveBitmapInTif(legends[1], savedLegendsPath, diamond.Name + "_1");
                        }
                    }
                    else
                    {
                        legends = new Bitmap[1];
                    }

                    using (Bitmap legend1Bitmap = File.Exists(diamond.Path + "/Легенда, лист 1.png")
                        ? new Bitmap(diamond.Path + "/Легенда, лист 1.png")
                        : new Bitmap(diamond.Path + "/Легенда.png"))
                    {
                        legends[0] = AppendColumnOfLegend(new Bitmap(legendPage), GraphicsService.CutRectangleFromBitmap(legend1Bitmap, 260, 780, 1100, 2450), 95, 782);
                    }

                    using (Bitmap legend2Bitmap = new Bitmap(diamond.Path + "/Легенда, лист 2.png"))
                    {
                        legends[0] = AppendColumnOfLegend(legends[0], GraphicsService.CutRectangleFromBitmap(legend2Bitmap, 250, 540, 1100, 2700), 1335, 542);
                    }

                    if (diamond.DiamondType == DiamondType.Standart)
                    {
                        SaveBitmapInTif(legends[0], savedLegendsPath, diamond.Name);
                    }

                    return legends;
                }                    
            }
            else
            {
                throw new Exception("Не знайдений файл \"Легенда, лист 1(2).png\" або \"Легенда.png\" - " + diamond.Name);
            }
        }

        /// <summary>
        /// Appending legendBitmap on targetBitmap on coords x and y
        /// </summary>
        private Bitmap AppendColumnOfLegend(Bitmap targetBitmap, Bitmap legendBitmap, int x, int y)
        {
            using (Graphics graph = GraphicsService.GetGraphFromImage(targetBitmap))
            {
                graph.DrawImage(legendBitmap, x, y, legendBitmap.Width, legendBitmap.Height);
            }

            return targetBitmap;
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

        /// <summary>
        /// Creating legend template with thumbnail, diamond name and size text
        /// </summary>
        /// <returns>Bitmap with legend template</returns>
        private Bitmap CreateLegendTemplate(DiamondSettings diamond)
        {
            Bitmap legendTemplate = new Bitmap(Properties.Resources.LegendaTemplate);
            Bitmap resultBitmap = new Bitmap(legendTemplate.Width, legendTemplate.Height, PixelFormat.Format32bppArgb);
            using (Graphics graph = GraphicsService.GetGraphFromImage(resultBitmap))
            {
                graph.DrawImage(legendTemplate, 0, 0);

                // Append thumbnail
                Bitmap OverlayBitmap = new Bitmap(diamond.Path + "/Вид вышивки.png");
                float OldWidth = OverlayBitmap.Width;
                float OldHeight = OverlayBitmap.Height;
                float NewWidth = 600f;
                float NewHeight = 450f;
                if (OldWidth > OldHeight)
                {
                    NewHeight = NewWidth / (OldWidth / OldHeight);
                }
                else
                {
                    NewWidth = NewHeight / (OldHeight / OldWidth);
                }
                float osX = 1800f + ((600f - NewWidth) / 2f);
                float osY = 50f + ((450f - NewHeight) / 2f);
                graph.DrawImage(OverlayBitmap, osX, osY, NewWidth, NewHeight);
                graph.DrawImage(OverlayBitmap, osX, osY, NewWidth, NewHeight);
                graph.DrawImage(OverlayBitmap, osX, osY, NewWidth, NewHeight);

                // Append diamond name
                Font font = new Font(pfc.Families[0], 165);
                SolidBrush drawBrush = new SolidBrush(Color.Black);
                SizeF sizef;
                if (diamond.Name.EndsWith("+"))
                {
                    sizef = graph.MeasureString(diamond.Name.Substring(0, diamond.Name.Length - 1), font);
                    SizeF plusSize = graph.MeasureString("+", new Font(pfc.Families[0], 110));
                    graph.DrawString(diamond.Name.Substring(0, diamond.Name.Length - 1), font, drawBrush, ((legendTemplate.Width - sizef.Width) / 2) + 105f - (plusSize.Width / 2f), 50f);
                    graph.DrawString("+", new Font(pfc.Families[0], 110), drawBrush, ((legendTemplate.Width - sizef.Width) / 2) + 35f + sizef.Width - (plusSize.Width / 2f), 90f);
                }
                else
                {
                    sizef = graph.MeasureString(diamond.Name, font);
                    graph.DrawString(diamond.Name, font, drawBrush, ((legendTemplate.Width - sizef.Width) / 2) + 70f, 50);
                }

                // Append size text
                string diamondSize = $"ЛЕГЕНДА ДЛЯ СХЕМИ РОЗМІРОМ {diamond.Width}x{diamond.Height}*";
                font = new Font(pfc.Families[0], 65);
                graph.DrawString(diamondSize, font, drawBrush, 90, 445);
            }

            return legendTemplate;
        }

        private void SaveBitmapInTif(Bitmap bitmap, string savingPath, string fileName)
        {
            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/tiff");
            Encoder myEncoder = Encoder.Compression;
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, (long)EncoderValue.CompressionNone);
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            myEncoderParameters.Param[0] = myEncoderParameter;
            bitmap.Save($"{savingPath}/{fileName}.tif", myImageCodecInfo, myEncoderParameters);
        }

        private ImageCodecInfo GetEncoderInfo(string mimeType)
        {
            int j;
            ImageCodecInfo[] encoders;
            encoders = ImageCodecInfo.GetImageEncoders();
            for (j = 0; j < encoders.Length; ++j)
            {
                if (encoders[j].MimeType == mimeType)
                {
                    return encoders[j];
                }
            }
            return null;
        }

        private PrivateFontCollection InitCustomFont()
        {
            PrivateFontCollection pfc = new PrivateFontCollection();
            int fontLength = Properties.Resources.VanishingSizeName_Regular.Length;
            byte[] fontdata = Properties.Resources.VanishingSizeName_Regular;
            IntPtr data = Marshal.AllocCoTaskMem(fontLength);
            Marshal.Copy(fontdata, 0, data, fontLength);
            pfc.AddMemoryFont(data, fontLength);

            return pfc;
        }
    }
}
