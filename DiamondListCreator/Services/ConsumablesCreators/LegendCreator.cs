﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using DiamondListCreator.Models;

namespace DiamondListCreator.Services.ConsumablesCreators
{
    public class LegendCreator : IDisposable
    {
        private readonly PrivateFontCollection pfc;

        public LegendCreator()
        {
            pfc = FontCollectionService.InitCustomFont(Properties.Resources.VanishingSizeName_Regular);
        }

        public void Dispose()
        {
            pfc.Dispose();
        }

        public Bitmap[] CreateEnglish(DiamondSettings diamond)
        {
            return CreateLegend(diamond, CreateLegendTemplate(diamond, true));
        }

        public Bitmap[] CreateUkrainian(DiamondSettings diamond)
        {
            return CreateLegend(diamond, CreateLegendTemplate(diamond, false));
        }

        /// <summary>
        /// Creating Legend pages for diamond
        /// </summary>
        /// <returns>Array of Bitmaps with created legends</returns>
        private Bitmap[] CreateLegend(DiamondSettings diamond, Bitmap legendPage)
        {
            Bitmap[] legends;

            string legendPath = Path.Combine(diamond.Path, "Легенда, лист 3.png");
            if (File.Exists(legendPath))
            {
                legends = new Bitmap[2];

                using (Bitmap legend3Bitmap = new Bitmap(legendPath))
                {
                    legends[1] = AppendColumnOfLegend(new Bitmap(legendPage), GraphicsService.CutRectangleFromBitmap(legend3Bitmap, 250, 540, 1100, 2450), 95, 775);
                }
            }
            else
            {
                legends = new Bitmap[1];
            }

            legendPath = Path.Combine(diamond.Path, "Легенда, лист 1.png");
            using (Bitmap legend1Bitmap = File.Exists(legendPath)
                ? new Bitmap(legendPath)
                : new Bitmap(diamond.Path + "/Легенда.png"))
            {
                legends[0] = AppendColumnOfLegend(new Bitmap(legendPage), GraphicsService.CutRectangleFromBitmap(legend1Bitmap, 260, 780, 1100, 2450), 95, 782);
            }

            legendPath = Path.Combine(diamond.Path, "Легенда, лист 2.png");
            if (File.Exists(legendPath))
            {
                using (Bitmap legend2Bitmap = new Bitmap(legendPath))
                {
                    legends[0] = AppendColumnOfLegend(legends[0], GraphicsService.CutRectangleFromBitmap(legend2Bitmap, 250, 540, 1100, 2700), 1335, 542);
                }
            }

            return legends;
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
        /// Creating legend template with thumbnail, diamond name and size text
        /// </summary>
        /// <returns>Bitmap with legend template</returns>
        private Bitmap CreateLegendTemplate(DiamondSettings diamond, bool isEnglish)
        {
            Bitmap legendTemplate;
            
            if (isEnglish)
            {
                legendTemplate = new Bitmap(Properties.Resources.LegendaTemplate_English);
            }
            else
            {
                using (var ms = new MemoryStream(Properties.Resources.LegendaTemplate_discount))
                {
                    legendTemplate = new Bitmap(ms);
                }
            }

            Bitmap resultBitmap = new Bitmap(legendTemplate.Width, legendTemplate.Height, PixelFormat.Format32bppArgb);

            using (Graphics graph = GraphicsService.GetGraphFromImage(resultBitmap))
            {
                graph.DrawImage(legendTemplate, 0, 0);

                // Append thumbnail
                using (Bitmap OverlayBitmap = new Bitmap(Path.Combine(diamond.Path, "Вид вышивки.png")))
                {
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
                }

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
                string diamondSize = (isEnglish ? "LEGEND FOR THE SHEME IN THE SIZE" : "ЛЕГЕНДА ДЛЯ СXЕМИ РОЗМІРОМ") + $" {diamond.Width}x{diamond.Height}см*";
                font = new Font(pfc.Families[0], 65);
                graph.DrawString(diamondSize, font, drawBrush, 90, 445);
            }

            legendTemplate.Dispose();

            return resultBitmap;
        }
    }
}
