using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Linq;
using DiamondListCreator.Models;

namespace DiamondListCreator.Services.ConsumablesCreators
{
    public class StretchedCanvasCreator : IDisposable
    {
        private readonly PrivateFontCollection pfc;
        private readonly List<StretchedCanvasSettings> canvasesSettings;

        public StretchedCanvasCreator()
        {
            pfc = FontCollectionService.InitCustomFont(Properties.Resources.VanishingSizeName_Regular);
            canvasesSettings = StretchedCanvasSettingsService.ReadSettings().ToList();
        }

        public void Dispose()
        {
            pfc.Dispose();
        }

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
            StretchedCanvasSettings canvasSettings = GetCanvasSettings(width, height);
            // if didn't find, then create the canvas settings by aspect ratio from standard and save it to json
            if (canvasSettings == null)
            {
                canvasSettings = GetCanvasSettings(diamond.SizeLetter);
                if (canvasSettings == null)
                {
                    throw new Exception($"Не знайдено розмір {diamond.SizeLetter} у файлі stretched_canvases.json!");
                }
                canvasSettings = new StretchedCanvasSettings(canvasSettings);
                canvasSettings.SetSize(width, height);
                canvasesSettings.Add(canvasSettings);
                StretchedCanvasSettingsService.WriteSettings(canvasesSettings.ToArray());
            }

            return canvasSettings.IsVertical
                   ? CreateVerticalCanvas(canvasSettings, diamond)
                   : CreateHorizontalCanvas(canvasSettings, diamond);
        }

        /// <summary>
        /// Creates a horizontally positioned canvas
        /// </summary>
        private Bitmap CreateHorizontalCanvas(StretchedCanvasSettings canvasSettings, DiamondSettings diamond)
        {
            Bitmap canvas = new Bitmap(canvasSettings.PageWidth, canvasSettings.PageHeight);

            using (Graphics graph = GraphicsService.GetGraphFromImage(canvas))
            {
                graph.Clear(Color.White);

                int sideBarWidth = canvasSettings.CanvasWidth;
                int sideBarHeight = canvasSettings.BorderHeight;
                int sideElementOffset = canvasSettings.SideElementOffsetHorizontal;

                using (Bitmap shemeBitmap = new Bitmap(Path.Combine(diamond.Path, "Схема для печати.png")))
                {
                    Bitmap thumbnail;
                    using (Bitmap thumbnailTemp = new Bitmap(Path.Combine(diamond.Path, "Вид вышивки.png")))
                    {
                        thumbnail = new Bitmap(thumbnailTemp.Width, thumbnailTemp.Height);
                        using (Graphics thumbnailGraph = GraphicsService.GetGraphFromImage(thumbnail))
                        {
                            thumbnailGraph.Clear(Color.Black);
                            thumbnailGraph.DrawImage(thumbnailTemp, 0, 0, thumbnailTemp.Width, thumbnailTemp.Height);
                        }
                    }

                    if (IsVertical(thumbnail))
                    {
                        thumbnail.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        shemeBitmap.RotateFlip(RotateFlipType.Rotate90FlipNone);

                        sideBarWidth = canvasSettings.CanvasHeight;
                        sideBarHeight = canvasSettings.BorderWidth;
                        sideElementOffset = canvasSettings.SideElementOffsetVertical;
                    }

                    thumbnail = new Bitmap(thumbnail, new Size(canvasSettings.CanvasWidth, canvasSettings.CanvasHeight));
                    graph.DrawImage(CreateBorders(canvas, thumbnail, canvasSettings), 0, 0);

                    Bitmap sheme = GraphicsService.CutRectangleFromBitmap(shemeBitmap, canvasSettings.CanvasFromShemeMarginLeft, canvasSettings.CanvasFromShemeMarginTop, shemeBitmap.Width - 2 - (canvasSettings.CanvasFromShemeMarginLeft * 2), canvasSettings.CanvasFromShemeWidth);
                    sheme = GraphicsService.RemoveBorders(sheme, Color.FromArgb(255, 255, 255, 255));
                    if (IsVertical(diamond.Path))
                    {
                        sheme.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    }
                    graph.DrawImage(sheme, canvasSettings.MarginLeft, canvasSettings.MarginTop, canvasSettings.CanvasWidth, canvasSettings.CanvasHeight);
                    sheme.Dispose();

                    using (Bitmap sideElements = CreateSideElements(sideBarWidth, sideBarHeight, canvasSettings.SideElementHeight, canvasSettings.SideElementSideOffset,
                                                                    sideElementOffset, diamond))
                    {
                        if (!IsVertical(diamond.Path))
                        {
                            sideElements.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            graph.DrawImage(sideElements, canvasSettings.MarginLeft, canvasSettings.MarginBottom, sideElements.Width, sideElements.Height);
                        }
                        else
                        {
                            sideElements.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            graph.DrawImage(sideElements, canvasSettings.MarginRight, canvasSettings.MarginTop, sideElements.Width, sideElements.Height);
                        }
                    }

                    thumbnail.Dispose();
                }
            }

            return canvas;
        }

        /// <summary>
        /// Creates a vertically arranged canvas
        /// </summary>
        private Bitmap CreateVerticalCanvas(StretchedCanvasSettings canvasSettings, DiamondSettings diamond)
        {
            Bitmap canvas = new Bitmap(canvasSettings.PageWidth, canvasSettings.PageHeight);

            using (Graphics graph = GraphicsService.GetGraphFromImage(canvas))
            {
                graph.Clear(Color.White);

                int sideBarWidth = canvasSettings.CanvasWidth;
                int sideBarHeight = canvasSettings.BorderHeight;
                int sideElementOffset = canvasSettings.SideElementOffsetVertical;

                using (Bitmap shemeBitmap = new Bitmap(Path.Combine(diamond.Path, "Схема для печати.png")))
                {
                    Bitmap thumbnail;
                    using (Bitmap thumbnailTemp = new Bitmap(Path.Combine(diamond.Path, "Вид вышивки.png")))
                    {
                        thumbnail = new Bitmap(thumbnailTemp.Width, thumbnailTemp.Height);
                        using (Graphics thumbnailGraph = GraphicsService.GetGraphFromImage(thumbnail))
                        {
                            thumbnailGraph.Clear(Color.Black);
                            thumbnailGraph.DrawImage(thumbnailTemp, 0, 0, thumbnailTemp.Width, thumbnailTemp.Height);
                        }
                    }

                    if (!IsVertical(thumbnail))
                    {
                        thumbnail.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        shemeBitmap.RotateFlip(RotateFlipType.Rotate270FlipNone);

                        sideBarWidth = canvasSettings.CanvasHeight;
                        sideBarHeight = canvasSettings.BorderWidth;
                        sideElementOffset = canvasSettings.SideElementOffsetHorizontal;
                    }

                    thumbnail = new Bitmap(thumbnail, new Size(canvasSettings.CanvasWidth, canvasSettings.CanvasHeight));
                    graph.DrawImage(CreateBorders(canvas, thumbnail, canvasSettings), 0, 0);

                    Bitmap sheme = GraphicsService.CutRectangleFromBitmap(shemeBitmap, canvasSettings.CanvasFromShemeMarginLeft, canvasSettings.CanvasFromShemeMarginTop, canvasSettings.CanvasFromShemeWidth, shemeBitmap.Height - 2 - (canvasSettings.CanvasFromShemeMarginTop * 2));
                    sheme = GraphicsService.RemoveBorders(sheme, Color.FromArgb(255, 255, 255, 255));
                    graph.DrawImage(sheme, canvasSettings.MarginLeft, canvasSettings.MarginTop, canvasSettings.CanvasWidth, canvasSettings.CanvasHeight);
                    sheme.Dispose();

                    using (Bitmap sideElements = CreateSideElements(sideBarWidth, sideBarHeight, canvasSettings.SideElementHeight, canvasSettings.SideElementSideOffset,
                                                                    sideElementOffset, diamond))
                    {
                        if (IsVertical(diamond.Path))
                        {
                            sideElements.RotateFlip(RotateFlipType.Rotate180FlipNone);
                            graph.DrawImage(sideElements, canvasSettings.MarginLeft, canvasSettings.MarginBottom, sideElements.Width, sideElements.Height);
                        }
                        else
                        {
                            sideElements.RotateFlip(RotateFlipType.Rotate90FlipNone);
                            graph.DrawImage(sideElements, canvasSettings.MarginRight, canvasSettings.MarginTop, sideElements.Width, sideElements.Height);
                        }
                    }

                    thumbnail.Dispose();
                }
            }

            return canvas;
        }

        /// <summary>
        /// Creates borders to the canvas from thumbnail
        /// </summary>
        private Bitmap CreateBorders(Bitmap canvas, Bitmap thumbnail, StretchedCanvasSettings canvasSettings)
        {
            using (Graphics graph = GraphicsService.GetGraphFromImage(canvas))
            {
                using (Bitmap leftBorder = GraphicsService.CutRectangleFromBitmap(thumbnail, 0, 0, canvasSettings.BorderWidth, thumbnail.Height))
                {
                    leftBorder.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    graph.DrawImage(leftBorder, canvasSettings.MarginLeft - leftBorder.Width, canvasSettings.MarginTop, leftBorder.Width, leftBorder.Height);
                }

                using (Bitmap rightBorder = GraphicsService.CutRectangleFromBitmap(thumbnail, thumbnail.Width - canvasSettings.BorderWidth, 0, canvasSettings.BorderWidth, thumbnail.Height))
                {
                    rightBorder.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    graph.DrawImage(rightBorder, canvasSettings.MarginRight, canvasSettings.MarginTop, rightBorder.Width, rightBorder.Height);
                }

                using (Bitmap topBorder = GraphicsService.CutRectangleFromBitmap(thumbnail, 0, 0, thumbnail.Width, canvasSettings.BorderHeight))
                {
                    topBorder.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    graph.DrawImage(topBorder, canvasSettings.MarginLeft, canvasSettings.MarginTop - topBorder.Height, topBorder.Width, topBorder.Height);

                    using (Bitmap leftTopCorner = GraphicsService.CutRectangleFromBitmap(topBorder, 0, 0, canvasSettings.BorderWidth, topBorder.Height))
                    {
                        leftTopCorner.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        graph.DrawImage(leftTopCorner, canvasSettings.MarginLeft - leftTopCorner.Width, canvasSettings.MarginTop - topBorder.Height, leftTopCorner.Width, leftTopCorner.Height);
                    }

                    using (Bitmap rightTopCorner = GraphicsService.CutRectangleFromBitmap(topBorder, topBorder.Width - canvasSettings.BorderWidth, 0, canvasSettings.BorderWidth, topBorder.Height))
                    {
                        rightTopCorner.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        graph.DrawImage(rightTopCorner, canvasSettings.MarginRight, canvasSettings.MarginTop - topBorder.Height, rightTopCorner.Width, rightTopCorner.Height);
                    }
                }

                using (Bitmap bottomBorder = GraphicsService.CutRectangleFromBitmap(thumbnail, 0, thumbnail.Height - canvasSettings.BorderHeight, thumbnail.Width, canvasSettings.BorderHeight))
                {
                    bottomBorder.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    graph.DrawImage(bottomBorder, canvasSettings.MarginLeft, canvasSettings.MarginBottom, bottomBorder.Width, bottomBorder.Height);

                    using (Bitmap leftBottomCorner = GraphicsService.CutRectangleFromBitmap(bottomBorder, 0, 0, canvasSettings.BorderWidth, bottomBorder.Height))
                    {
                        leftBottomCorner.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        graph.DrawImage(leftBottomCorner, canvasSettings.MarginLeft - leftBottomCorner.Width, canvasSettings.MarginBottom, leftBottomCorner.Width, leftBottomCorner.Height);
                    }

                    using (Bitmap rightBottomCorner = GraphicsService.CutRectangleFromBitmap(bottomBorder, bottomBorder.Width - canvasSettings.BorderWidth, 0, canvasSettings.BorderWidth, bottomBorder.Height))
                    {
                        rightBottomCorner.RotateFlip(RotateFlipType.RotateNoneFlipX);
                        graph.DrawImage(rightBottomCorner, canvasSettings.MarginRight, canvasSettings.MarginBottom, rightBottomCorner.Width, rightBottomCorner.Height);
                    }
                }
            }

            return canvas;
        }

        /// <summary>
        /// Creates a bitmap with horizontal orientation with side elements (size, name, logo)
        /// </summary>
        /// <param name="width">The width of the result bitmap</param>
        /// <param name="height">The height of the result bitmap</param>
        /// <returns>The horizontal orientation bitmap with transparent background and side elements</returns>
        private Bitmap CreateSideElements(int width, int height, int sideElementHeight, int sideElementSideOffset, int sideElementBottomOffset, DiamondSettings diamond)
        {
            Bitmap sideBitmap = new Bitmap(width, height);
            sideBitmap.MakeTransparent();

            using (Graphics graph = GraphicsService.GetGraphFromImage(sideBitmap))
            {
                // Logo
                using (Bitmap logoBitmap = new Bitmap(Properties.Resources.diamonds_logo_white))
                {
                    int newWidth = logoBitmap.Width / logoBitmap.Height * sideElementHeight;

                    // Logo shadow
                    using (Bitmap shadowBitmap = new Bitmap(Properties.Resources.diamonds_logo_black))
                    {
                        graph.DrawImage(shadowBitmap, sideElementSideOffset + 2, sideElementBottomOffset + 2, newWidth, sideElementHeight);
                    }

                    graph.DrawImage(logoBitmap, sideElementSideOffset, sideElementBottomOffset, newWidth, sideElementHeight);
                }

                // Diamond Name
                using (Bitmap diamondNameBitmap = GraphicsService.CreateTextBitmapWithShadow(diamond.Name, pfc.Families[0], 200, FontStyle.Bold, Color.White, Color.Black, new Size(5, 5)))
                {
                    int newWidth = diamondNameBitmap.Width / diamondNameBitmap.Height * sideElementHeight;
                    graph.DrawImage(diamondNameBitmap, (width - newWidth) / 2, sideElementBottomOffset, newWidth, sideElementHeight);
                }

                // Size
                string sizeStr = "(" + diamond.SizeLetter.Replace("+", "") + ") - " + diamond.Width + "x" + diamond.Height + "см";

                using (Bitmap sizeBitmap = GraphicsService.CreateTextBitmapWithShadow(sizeStr, pfc.Families[0], 200, FontStyle.Bold, Color.White, Color.Black, new Size(5, 5)))
                {
                    int newWidth = sizeBitmap.Width / sizeBitmap.Height * sideElementHeight;
                    graph.DrawImage(sizeBitmap, width - sideElementSideOffset - newWidth, sideElementBottomOffset, newWidth, sideElementHeight);
                }
            }

            return sideBitmap;
        }

        private bool IsVertical(string diamondPath)
        {
            using (Bitmap thumbnailBitmap = new Bitmap(Path.Combine(diamondPath, "Вид вышивки.png")))
            {
                return thumbnailBitmap.Height >= thumbnailBitmap.Width;
            }
        }

        private bool IsVertical(Bitmap thumbnail)
        {
            return thumbnail.Height >= thumbnail.Width;
        }

        private StretchedCanvasSettings GetCanvasSettings(int width, int height)
        {
            return canvasesSettings.FirstOrDefault(x => x.SizeWidth == width && x.SizeHeight == height);
        }

        private StretchedCanvasSettings GetCanvasSettings(string sizeLetter)
        {
            return canvasesSettings.FirstOrDefault(x => x.SizeName == sizeLetter);
        }
    }
}
