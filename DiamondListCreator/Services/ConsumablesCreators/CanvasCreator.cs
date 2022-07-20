using DiamondListCreator.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.IO;
using System.Linq;

namespace DiamondListCreator.Services.ConsumablesCreators
{
    public class CanvasCreator
    {
        private readonly PrivateFontCollection pfc;
        private readonly List<CanvasSettings> canvasesSettings;

        public CanvasCreator(PrivateFontCollection pfc)
        {
            this.pfc = pfc;
            canvasesSettings = CanvasSettingsService.ReadSettings().ToList();
        }

        ~CanvasCreator()
        {
            pfc.Dispose();
        }

        /// <summary>
        /// Creates the standard canvas for the diamond
        /// </summary>
        /// <param name="diamond"></param>
        /// <returns>The canvas bitmap</returns>
        public Bitmap Create(DiamondSettings diamond)
        {
            int width, height;

            if (diamond.Height < diamond.Width)
            {
                width = diamond.Height;
                height = diamond.Width;
            }
            else
            {
                width = diamond.Width;
                height = diamond.Height;
            }

            // trying to find the canvas settings for these width and height
            CanvasSettings canvasSettings = GetCanvasSettings(width, height);
            // if didn't find, then create the canvas settings by aspect ratio from standard and save it to json
            if (canvasSettings == null)
            {
                canvasSettings = new CanvasSettings(GetCanvasSettings(diamond.SizeLetter));
                canvasSettings.SetSize(diamond.Width, diamond.Height);
                canvasesSettings.Add(canvasSettings);
                CanvasSettingsService.WriteSettings(canvasesSettings.ToArray());
            }

            return IsVertical(diamond.Path)
                ? CreateVerticalCanvas(CreateTemplate(canvasSettings, diamond.Path, diamond.Name), canvasSettings, diamond.Path, diamond.SizeLetter)
                : CreateHorizontalCanvas(CreateTemplate(canvasSettings, diamond.Path, diamond.Name), canvasSettings, diamond.Path, diamond.SizeLetter);
        }

        /// <summary>
        /// Checks if diamond is vertical
        /// </summary>
        /// <param name="diamondPath"></param>
        /// <returns>True if the diamond has vertical orientation, false if the diamond has horizontal orientation</returns>
        private bool IsVertical(string diamondPath)
        {
            using (Bitmap thumbnailBitmap = new Bitmap(diamondPath + "/Вид вышивки.png"))
            {
                return thumbnailBitmap.Height >= thumbnailBitmap.Width;
            }
        }

        private CanvasSettings GetCanvasSettings(int width, int height)
        {
            return canvasesSettings.FirstOrDefault(x => x.SizeWidth == width && x.SizeHeight == height);
        }

        private CanvasSettings GetCanvasSettings(string sizeLetter)
        {
            return canvasesSettings.FirstOrDefault(x => x.SizeName == sizeLetter);
        }

        /// <summary>
        /// Creates a canvas with horizontal orientation
        /// </summary>
        /// <param name="diamondPath">The diamond directory</param>
        /// <param name="diamondSize">The diamond size letter</param>
        /// <param name="canvasSettings">CanvasSetting for diamond</param>
        /// <param name="template">The created template for diamond</param>
        /// <returns>The canvas bitmap</returns>
        private Bitmap CreateHorizontalCanvas(Bitmap template, CanvasSettings canvasSettings, string diamondPath, string diamondSize)
        {
            template.RotateFlip(RotateFlipType.Rotate180FlipNone);

            using (Graphics graph = GraphicsService.GetGraphFromImage(template))
            {
                int OffsetXAfterRotation = canvasSettings.MarginRight - canvasSettings.MarginLeft;
                if (OffsetXAfterRotation != 0)
                {
                    using (Bitmap canvasCopy = new Bitmap(template))
                    {
                        graph.Clear(Color.White);
                        graph.DrawImage(canvasCopy, -OffsetXAfterRotation, 0);
                    }
                }

                // Start X coord for next element to draw
                int RightElementsMarginBottom = canvasSettings.PageHeight - canvasSettings.MarginBottom;

                // The side horizontal logo
                using (Bitmap logoBitmap = new Bitmap(Properties.Resources.diamonds_logo))
                {
                    logoBitmap.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    graph.DrawImage(logoBitmap, canvasSettings.PageWidth - canvasSettings.MarginRight + ((canvasSettings.MarginRight - canvasSettings.RightHorizontalLogoWidth) / 2), RightElementsMarginBottom - canvasSettings.RightHorizontalLogoHeight, canvasSettings.RightHorizontalLogoWidth, canvasSettings.RightHorizontalLogoHeight);
                }

                // Update X coord for next element to draw
                RightElementsMarginBottom -= canvasSettings.RightHorizontalLogoHeight;

                int thumbnailX;

                // Thumbnail
                using (Bitmap thumbnailBitmap = new Bitmap(diamondPath + "/Вид вышивки.png"))
                {
                    float thumbnailAspectRatio = thumbnailBitmap.Width / thumbnailBitmap.Height;
                    thumbnailBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);

                    using (Bitmap thumbnail = new Bitmap(canvasSettings.ThumbnailWidth + (canvasSettings.ThumbnailBorderThickness * 2), (int)(canvasSettings.ThumbnailWidth * thumbnailAspectRatio) + (canvasSettings.ThumbnailBorderThickness * 2), PixelFormat.Format32bppArgb))
                    {
                        using (Graphics graphThumbnail = GraphicsService.GetGraphFromImage(thumbnail))
                        {
                            graphThumbnail.Clear(Color.Black);
                            graphThumbnail.DrawImage(thumbnailBitmap, canvasSettings.ThumbnailBorderThickness, canvasSettings.ThumbnailBorderThickness, canvasSettings.ThumbnailWidth, thumbnail.Height - (canvasSettings.ThumbnailBorderThickness * 2));
                        }

                        graph.DrawImage(thumbnail, canvasSettings.PageWidth - canvasSettings.MarginRight + ((canvasSettings.MarginRight - thumbnail.Width) / 2), RightElementsMarginBottom - canvasSettings.Spacing - thumbnail.Height);

                        // Update X coord for next element to draw
                        RightElementsMarginBottom = RightElementsMarginBottom - canvasSettings.Spacing - thumbnail.Height;

                        thumbnailX = canvasSettings.PageWidth - canvasSettings.MarginRight + ((canvasSettings.MarginRight - thumbnail.Width) / 2);
                    }
                }

                using (Bitmap canvasShemeBitmap = new Bitmap(diamondPath + "/Схема для печати.png"))
                {
                    using (Bitmap legendBitmap = CreateHorizontalLegend(canvasShemeBitmap, canvasSettings, GetColorsCountFromLegend(diamondPath, diamondSize)))
                    {
                        // Update X coord for next element to draw
                        RightElementsMarginBottom = RightElementsMarginBottom - legendBitmap.Height - canvasSettings.Spacing;

                        if (canvasSettings.LegendAlignCenter)
                        {
                            graph.DrawImage(legendBitmap, canvasSettings.PageWidth - canvasSettings.MarginRight + ((canvasSettings.MarginRight - legendBitmap.Width) / 2), RightElementsMarginBottom);
                        }
                        else
                        {
                            graph.DrawImage(legendBitmap, thumbnailX, RightElementsMarginBottom);
                        }
                    }

                    int RightElementsMarginTop = canvasSettings.MarginTop;

                    // if size canvas is more then 100 cm height, then make a duplicate of the legend
                    if (canvasSettings.SizeWidth >= 100 && (canvasSettings.PageHeight - RightElementsMarginBottom - canvasSettings.MarginBottom - canvasSettings.Spacing) >= (canvasSettings.UrlsHeight + (canvasSettings.Spacing * 2)))
                    {
                        using (Bitmap legendCopy = GraphicsService.CutRectangleFromBitmap(template, canvasSettings.MarginLeft + canvasSettings.CanvasWidth, RightElementsMarginBottom, canvasSettings.MarginRight, canvasSettings.PageHeight - RightElementsMarginBottom + canvasSettings.Spacing - canvasSettings.MarginBottom))
                        {
                            RightElementsMarginTop += legendCopy.Height + canvasSettings.Spacing;
                            graph.DrawImage(legendCopy, canvasSettings.MarginLeft + canvasSettings.CanvasWidth, canvasSettings.MarginTop);
                        }
                    }
                    else
                    // otherwise draw side vertical logo
                    if ((RightElementsMarginBottom - canvasSettings.MarginTop - canvasSettings.RightVerticalLogoHeight) >= canvasSettings.Spacing)
                    {
                        using (Bitmap logoBitmap = new Bitmap(Properties.Resources.diamonds_logo))
                        {
                            logoBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            graph.DrawImage(logoBitmap, canvasSettings.PageWidth - canvasSettings.MarginRight + ((canvasSettings.MarginRight - canvasSettings.RightVerticalLogoWidth) / 2), canvasSettings.MarginTop, canvasSettings.RightVerticalLogoWidth, canvasSettings.RightVerticalLogoHeight);
                            RightElementsMarginTop += canvasSettings.RightVerticalLogoHeight + canvasSettings.Spacing;
                        }
                    }

                    // Urls
                    if (diamondSize != "S")
                    {
                        if ((RightElementsMarginBottom - RightElementsMarginTop - canvasSettings.UrlsHeight) / 2 >= canvasSettings.Spacing)
                        {
                            using (Bitmap urlsBitmap = new Bitmap(Properties.Resources.urls))
                            {
                                urlsBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                                graph.DrawImage(urlsBitmap, canvasSettings.PageWidth - canvasSettings.MarginRight + ((canvasSettings.MarginRight - canvasSettings.UrlsWidth) / 2), RightElementsMarginTop + ((RightElementsMarginBottom - RightElementsMarginTop - canvasSettings.UrlsHeight) / 2), canvasSettings.UrlsWidth, canvasSettings.UrlsHeight);
                            }
                        }
                    }

                    // The canvas
                    using (Bitmap canvasBitmap = GraphicsService.CutRectangleFromBitmap(canvasShemeBitmap, canvasSettings.CanvasFromShemeMarginLeft, canvasSettings.CanvasFromShemeMarginTop, canvasShemeBitmap.Width - 2 - (canvasSettings.CanvasFromShemeMarginLeft * 2), canvasSettings.CanvasFromShemeWidth))
                    {
                        canvasBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        graph.DrawImage(canvasBitmap, canvasSettings.MarginLeft, canvasSettings.MarginTop, canvasSettings.CanvasWidth, canvasSettings.CanvasHeight);
                    }
                }
            }

            template = DuplicateAndAddOffset(template, canvasSettings, diamondSize);

            return template;
        }

        /// <summary>
        /// Creates the legend for the horizontal canvas
        /// </summary>
        /// <param name="canvasSheme">The canvas sheme bitmap</param>
        /// <param name="canvasSettings">The canvas settigns</param>
        /// <param name="colorsCount">The count of diamond colors</param>
        /// <returns></returns>
        private Bitmap CreateHorizontalLegend(Bitmap canvasSheme, CanvasSettings canvasSettings, int colorsCount)
        {
            // Cutting out all the colors from the scheme in the array
            List<Bitmap> legendColors = new List<Bitmap>();
            int startingYPoint = canvasSettings.CanvasFromShemeMarginTop + canvasSettings.CanvasFromShemeWidth + canvasSettings.LegendsFromShemeHorizontalMarginTop + 3;
            for (int i = 1, row = 1, col = 0; i <= colorsCount; i++)
            {
                int MarginTopLegend = startingYPoint + ((48 + 11) * row);

                if (MarginTopLegend + 48 > canvasSheme.Height)
                {
                    col++;
                    row = 0;
                    MarginTopLegend = startingYPoint + ((48 + 11) * row);
                }

                int MarginLeftLegend = 63 + (738 * col);

                Bitmap legendColor = GraphicsService.CutRectangleFromBitmap(canvasSheme, MarginLeftLegend, MarginTopLegend, 250, 47);
                legendColors.Add(legendColor);

                row++;
            }

            // The making of legends
            int rows = canvasSettings.RowsCountInLegend;
            int cols = colorsCount / rows;
            if ((colorsCount + 1) % rows != 0)
            {
                cols++;
            }

            Bitmap legendBitmap = new Bitmap(cols * (250 + canvasSettings.PaddingInLegend), rows * 57);

            using (Graphics graphLegend = GraphicsService.GetGraphFromImage(legendBitmap))
            {
                graphLegend.Clear(Color.White);

                for (int i = 0, row = 1, col = 0; i < legendColors.Count; i++)
                {
                    int MarginTopLegend = 57 * row;

                    if (MarginTopLegend + 48 > legendBitmap.Height)
                    {
                        col++;
                        row = 0;
                        MarginTopLegend = 57 * row;
                    }

                    int MarginLeftLegend = (250 + canvasSettings.PaddingInLegend) * col;

                    graphLegend.DrawImage(legendColors[i], MarginLeftLegend, MarginTopLegend, 250, 47);

                    row++;
                }

                int cloneX = 120;
                int pasteX = 70;
                int offset = cloneX - pasteX;
                using (Bitmap legendCopy = new Bitmap(legendBitmap))
                {
                    for (int i = 0; i < cols; i++)
                    {
                        graphLegend.DrawImage(GraphicsService.CutRectangleFromBitmap(legendCopy, cloneX, 0, legendCopy.Width - cloneX, legendCopy.Height), pasteX, 0);

                        pasteX += 250 + canvasSettings.PaddingInLegend - offset;
                        cloneX += 250 + canvasSettings.PaddingInLegend;
                    }
                }

                using (Bitmap dmc = new Bitmap(Properties.Resources.dmc))
                {
                    float dmcAspectRatio = dmc.Width / dmc.Height;
                    graphLegend.DrawImage(dmc, 0, 0, 40 * dmcAspectRatio, 40);
                }

                legendBitmap = GraphicsService.CutRectangleFromBitmap(legendBitmap, 0, 0, legendBitmap.Width - (cols * offset) - canvasSettings.PaddingInLegend, legendBitmap.Height - 10);
            }

            legendBitmap = GraphicsService.RemoveLeftBorder(legendBitmap, Color.FromArgb(255, 255, 255, 255));
            legendBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);

            return legendBitmap;
        }

        /// <summary>
        /// Determines the number of diamond colors by scanning the number from the legend list
        /// </summary>
        /// <returns></returns>
        private int GetColorsCountFromLegend(string diamondPath, string diamondSize)
        {
            string ocrText;
            int colorsCount = 0;

            using (Bitmap legendPage = File.Exists(diamondPath + "/Легенда, лист 1.png")
                        ? new Bitmap(diamondPath + "/Легенда, лист 1.png")
                        : new Bitmap(diamondPath + "/Легенда.png"))
            {
                if (diamondSize != "S")
                {
                    ocrText = OcrService.GetTextFromImage(GraphicsService.CutRectangleFromBitmap(legendPage, 830, 225, 100, 70)).Trim();

                    if (!int.TryParse(ocrText, out colorsCount))
                    {
                        throw new Exception("Невдалося зісканувати з легенди кількість кольорів у алмазці");
                    }
                }
                else
                {
                    ocrText = OcrService.GetTextFromImage(GraphicsService.CutRectangleFromBitmap(legendPage, 770, 225, 100, 70)).Trim();
                    if (!int.TryParse(ocrText, out colorsCount))
                    {
                        ocrText = OcrService.GetTextFromImage(GraphicsService.CutRectangleFromBitmap(legendPage, 830, 225, 100, 70)).Trim();
                        if (!int.TryParse(ocrText, out colorsCount))
                        {
                            ocrText = OcrService.GetTextFromImage(GraphicsService.CutRectangleFromBitmap(legendPage, 790, 225, 100, 70)).Trim();
                            if (!int.TryParse(ocrText, out colorsCount))
                            {
                                throw new Exception("Невдалося зісканувати з легенди кількість кольорів у алмазці");
                            }
                        }
                    }
                }
            }

            return colorsCount;
        }

        /// <summary>
        /// Creates a canvas with vertical orientation
        /// </summary>
        /// <param name="diamondPath">The diamond directory</param>
        /// <param name="diamondSize">The diamond size letter</param>
        /// <param name="canvasSettings">CanvasSetting for diamond</param>
        /// <param name="template">The created template for diamond</param>
        /// <returns>The canvas bitmap</returns>
        private Bitmap CreateVerticalCanvas(Bitmap template, CanvasSettings canvasSettings, string diamondPath, string diamondSize)
        {
            // Start X coord for next element to draw
            int RightElementsMarginTop = canvasSettings.MarginTop;

            // End X coord for side elements
            int RightElementsMarginBottom = canvasSettings.MarginBottom;

            using (Graphics graph = GraphicsService.GetGraphFromImage(template))
            {
                // Side horizontal logo
                using (Bitmap logoBitmap = new Bitmap(Properties.Resources.diamonds_logo))
                {
                    graph.DrawImage(logoBitmap, canvasSettings.PageWidth - canvasSettings.MarginRight + ((canvasSettings.MarginRight - canvasSettings.RightHorizontalLogoWidth) / 2), RightElementsMarginTop, canvasSettings.RightHorizontalLogoWidth, canvasSettings.RightHorizontalLogoHeight);
                }

                // Update X coord for next element to draw
                RightElementsMarginTop += canvasSettings.RightHorizontalLogoHeight + canvasSettings.Spacing;

                int thumbnailWidth;

                // Thumbnail
                using (Bitmap thumbnailBitmap = new Bitmap(diamondPath + "/Вид вышивки.png"))
                {
                    float thumbnailAspectRatio = (float)thumbnailBitmap.Width / (float)thumbnailBitmap.Height;

                    using (Bitmap thumbnail = new Bitmap(canvasSettings.ThumbnailWidth + (canvasSettings.ThumbnailBorderThickness * 2), (int)(canvasSettings.ThumbnailWidth / thumbnailAspectRatio) + (canvasSettings.ThumbnailBorderThickness * 2), PixelFormat.Format32bppArgb))
                    {
                        using (Graphics graphThumbnail = GraphicsService.GetGraphFromImage(thumbnail))
                        {
                            graphThumbnail.Clear(Color.Black);
                            graphThumbnail.DrawImage(thumbnailBitmap, canvasSettings.ThumbnailBorderThickness, canvasSettings.ThumbnailBorderThickness, canvasSettings.ThumbnailWidth, thumbnail.Height - (canvasSettings.ThumbnailBorderThickness * 2));
                        }

                        graph.DrawImage(thumbnail, canvasSettings.PageWidth - canvasSettings.MarginRight + ((canvasSettings.MarginRight - thumbnail.Width) / 2), RightElementsMarginTop, thumbnail.Width, thumbnail.Height);

                        // Update X coord for next element to draw
                        RightElementsMarginTop += thumbnail.Height + canvasSettings.Spacing;

                        thumbnailWidth = thumbnail.Width;
                    }
                }

                using (Bitmap canvasShemeBitmap = new Bitmap(diamondPath + "/Схема для печати.png"))
                {
                    // Legend
                    Bitmap legendBitmap = GraphicsService.CutRectangleFromBitmap(canvasShemeBitmap, canvasSettings.LegendsFromShemeVerticalMarginLeft, canvasSettings.LegendsFromShemeVerticalMarginTop, canvasSettings.LegendsFromShemeVerticalWidth, canvasSettings.LegendsFromShemeVerticalHeight);

                    // Calculate legend height on canvas sheme
                    string ocrText = OcrService.GetTextFromImage(legendBitmap);
                    string[] ocrTextArr = ocrText.Split('\n');
                    int legendHeight = ocrTextArr.Length * canvasSettings.LegendsFromShemeRowHeight;

                    // Cut legend from sheme
                    legendBitmap = GraphicsService.CutRectangleFromBitmap(legendBitmap, 0, 0, legendBitmap.Width, legendHeight);
                    legendBitmap = GraphicsService.RemoveBottomBorder(legendBitmap, Color.FromArgb(255, 255, 255, 255));

                    legendHeight = legendBitmap.Height;
                    graph.DrawImage(legendBitmap, canvasSettings.PageWidth - canvasSettings.MarginRight + ((canvasSettings.MarginRight - legendBitmap.Width) / 2), RightElementsMarginTop, legendBitmap.Width, legendHeight);

                    graph.FillRectangle(new SolidBrush(Color.White), new Rectangle(canvasSettings.PageWidth - canvasSettings.MarginRight + ((canvasSettings.MarginRight - legendBitmap.Width) / 2) - 1, RightElementsMarginTop - 1, legendBitmap.Width, 45));

                    // Image "DMC"
                    using (Bitmap dmc = new Bitmap(Properties.Resources.dmc))
                    {
                        float dmcAspectRatio = dmc.Width / (float)dmc.Height;
                        graph.DrawImage(dmc, canvasSettings.PageWidth - canvasSettings.MarginRight + ((canvasSettings.MarginRight - thumbnailWidth) / 2), RightElementsMarginTop, 35 * dmcAspectRatio, 35);
                    }

                    // Update X coord for next element to draw
                    RightElementsMarginTop += legendHeight + canvasSettings.Spacing;

                    legendBitmap.Dispose();

                    // Main canvas image
                    using (Bitmap canvasBitmap = GraphicsService.CutRectangleFromBitmap(canvasShemeBitmap, canvasSettings.CanvasFromShemeMarginLeft, canvasSettings.CanvasFromShemeMarginTop, canvasSettings.CanvasFromShemeWidth, canvasShemeBitmap.Height - 2 - (canvasSettings.CanvasFromShemeMarginTop * 2)))
                    {
                        graph.DrawImage(canvasBitmap, canvasSettings.MarginLeft, canvasSettings.MarginTop, canvasSettings.CanvasWidth, canvasSettings.CanvasHeight);
                    }
                }

                // if size canvas is more then 100 cm height, then make a duplicate of the legend
                if (canvasSettings.SizeHeight >= 100 && (RightElementsMarginTop - canvasSettings.MarginTop - canvasSettings.Spacing) >= (canvasSettings.UrlsHeight + (canvasSettings.Spacing * 2)))
                {
                    using (Bitmap legendCopy = GraphicsService.CutRectangleFromBitmap(template, canvasSettings.MarginLeft + canvasSettings.CanvasWidth, canvasSettings.MarginTop, canvasSettings.MarginRight, RightElementsMarginTop - canvasSettings.MarginTop - canvasSettings.Spacing))
                    {
                        RightElementsMarginBottom = canvasSettings.PageHeight - legendCopy.Height - canvasSettings.MarginBottom;
                        graph.DrawImage(legendCopy, canvasSettings.MarginLeft + canvasSettings.CanvasWidth, RightElementsMarginBottom);
                    }
                }
                else
                // otherwise draw side vertical logo
                if ((canvasSettings.PageHeight - canvasSettings.MarginBottom - canvasSettings.RightVerticalLogoHeight - RightElementsMarginTop) >= canvasSettings.Spacing)
                {
                    using (Bitmap logoBitmap = new Bitmap(Properties.Resources.diamonds_logo))
                    {
                        logoBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        RightElementsMarginBottom = canvasSettings.PageHeight - canvasSettings.MarginBottom - canvasSettings.RightVerticalLogoHeight;
                        graph.DrawImage(logoBitmap, canvasSettings.PageWidth - canvasSettings.MarginRight + ((canvasSettings.MarginRight - canvasSettings.RightVerticalLogoWidth) / 2), RightElementsMarginBottom, canvasSettings.RightVerticalLogoWidth, canvasSettings.RightVerticalLogoHeight);
                    }
                }

                // Urls
                if (diamondSize != "S")
                {
                    if ((canvasSettings.PageHeight - RightElementsMarginTop - (canvasSettings.PageHeight - RightElementsMarginBottom) - canvasSettings.UrlsHeight) / 2 >= canvasSettings.Spacing)
                    {
                        using (Bitmap urlsBitmap = new Bitmap(Properties.Resources.urls))
                        {
                            urlsBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                            graph.DrawImage(urlsBitmap, canvasSettings.PageWidth - canvasSettings.MarginRight + ((canvasSettings.MarginRight - canvasSettings.UrlsWidth) / 2), RightElementsMarginTop + ((canvasSettings.PageHeight - RightElementsMarginTop - (canvasSettings.PageHeight - RightElementsMarginBottom) - canvasSettings.UrlsHeight) / 2), canvasSettings.UrlsWidth, canvasSettings.UrlsHeight);
                        }
                    }
                }
                else
                {
                    using (Bitmap instBitmap = new Bitmap(Properties.Resources.inst))
                    {
                        instBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        graph.DrawImage(instBitmap, (canvasSettings.MarginLeft - canvasSettings.UrlsWidth) / 2, canvasSettings.MarginTop, canvasSettings.UrlsWidth, canvasSettings.UrlsHeight);
                    }
                    using (Bitmap urlBitmap = new Bitmap(Properties.Resources.url))
                    {
                        urlBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        graph.DrawImage(urlBitmap, (canvasSettings.MarginLeft - canvasSettings.UrlsWidth) / 2, canvasSettings.MarginTop + canvasSettings.CanvasHeight - canvasSettings.UrlsHeight, canvasSettings.UrlsWidth, canvasSettings.UrlsHeight);
                    }
                }
            }

            template = DuplicateAndAddOffset(template, canvasSettings, diamondSize);

            return template;
        }

        /// <summary>
        /// Creates canvas template with instruction, size, name and logo
        /// </summary>
        /// <param name="diamondName">The diamond name</param>
        /// <param name="diamondPath">The diamond directory</param>
        /// <param name="canvasSettings">CanvasSetting for diamond</param>
        /// <returns>Created canvas template</returns>
        private Bitmap CreateTemplate(CanvasSettings canvasSettings, string diamondPath, string diamondName)
        {
            Bitmap template = new Bitmap(canvasSettings.PageWidth, canvasSettings.PageHeight, PixelFormat.Format32bppArgb);

            using (Graphics graph = GraphicsService.GetGraphFromImage(template))
            {
                graph.Clear(Color.White);

                // Instruction
                using (Bitmap instructionBitmap = new Bitmap(Properties.Resources.instruction))
                {
                    graph.DrawImage(instructionBitmap, canvasSettings.MarginLeft, (canvasSettings.MarginTop - canvasSettings.InstructionHeight) / 2, canvasSettings.CanvasWidth, canvasSettings.InstructionHeight);
                }

                // Bottom logo
                using (Bitmap logoBitmap = new Bitmap(Properties.Resources.diamonds_logo))
                {
                    graph.DrawImage(logoBitmap, canvasSettings.MarginLeft, canvasSettings.PageHeight - canvasSettings.MarginBottom + ((canvasSettings.MarginBottom - canvasSettings.BottomLogoHeight) / 2), canvasSettings.BottomLogoWidth, canvasSettings.BottomLogoHeight);
                }

                // Size
                string sizeStr = "(" + canvasSettings.SizeName + ") - ";
                sizeStr += IsVertical(diamondPath)
                           ? canvasSettings.SizeWidth + "x" + canvasSettings.SizeHeight + "см"
                           : canvasSettings.SizeHeight + "x" + canvasSettings.SizeWidth + "см";

                float marginRightDiamondname;
                using (Bitmap sizeBitmap = GraphicsService.CreateBitmapWithText(sizeStr, pfc.Families[0], canvasSettings.DiamondSizeFontSize))
                {
                    marginRightDiamondname = canvasSettings.PageWidth - canvasSettings.MarginRight - sizeBitmap.Width;
                    graph.DrawImage(sizeBitmap, marginRightDiamondname, canvasSettings.PageHeight - canvasSettings.MarginBottom + ((canvasSettings.MarginBottom - sizeBitmap.Height) / 2));
                }

                // Diamond Name
                using (Bitmap diamondNameBitmap = GraphicsService.CreateBitmapWithText("#" + diamondName, pfc.Families[0], canvasSettings.DiamondNameFontSize))
                {
                    graph.DrawImage(diamondNameBitmap, canvasSettings.MarginLeft + canvasSettings.BottomLogoWidth + ((marginRightDiamondname - (canvasSettings.MarginLeft + canvasSettings.BottomLogoWidth) - diamondNameBitmap.Width) / 2), canvasSettings.PageHeight - canvasSettings.MarginBottom + ((canvasSettings.MarginBottom - diamondNameBitmap.Height) / 2));
                }
            }

            return template;
        }

        /// <summary>
        /// Duplicates canvas elements, adds page offset, if its needed
        /// </summary>
        /// <param name="canvas">The created canvas</param>
        /// <returns>The fixed canvas bitmap</returns>
        private Bitmap DuplicateAndAddOffset(Bitmap canvas, CanvasSettings canvasSettings, string diamondSize)
        {
            using (Graphics graph = GraphicsService.GetGraphFromImage(canvas))
            {

                if (diamondSize == "XL")
                {
                    // Duplicating side elements
                    graph.DrawImage(GraphicsService.CutRectangleFromBitmap(canvas, canvasSettings.MarginLeft + canvasSettings.CanvasWidth, canvasSettings.MarginTop, canvasSettings.PageWidth - canvasSettings.MarginLeft - canvasSettings.CanvasWidth, canvasSettings.PageHeight - canvasSettings.MarginTop - canvasSettings.MarginBottom), 0, canvasSettings.MarginTop);
                }

                // Add canvas left offset
                if (canvasSettings.PageOffsetX != 0)
                {
                    using (Bitmap canvasCopy = new Bitmap(canvas))
                    {
                        graph.Clear(Color.White);
                        graph.DrawImage(canvasCopy, canvasSettings.PageOffsetX, 0);
                    }
                }
            }

            return canvas;
        }
    }
}