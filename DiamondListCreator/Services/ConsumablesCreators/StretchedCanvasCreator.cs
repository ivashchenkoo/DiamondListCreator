using DiamondListCreator.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;

namespace DiamondListCreator.Services.ConsumablesCreators
{
    public class StretchedCanvasCreator
    {
        private readonly PrivateFontCollection pfc;
        private readonly List<StretchedCanvasSettings> canvasesSettings;

        public StretchedCanvasCreator(PrivateFontCollection pfc)
        {
            this.pfc = pfc;
            canvasesSettings = StretchedCanvasSettingsService.ReadSettings().ToList();
        }

        ~StretchedCanvasCreator()
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
                canvasSettings = new StretchedCanvasSettings(GetCanvasSettings(diamond.SizeLetter));
                if (!canvasSettings.IsVertical)
                {
                    throw new Exception("Creating horizontally placed canvases on a sheet is currently not available.");
                }
                canvasSettings.SetSize(diamond.Width, diamond.Height);
                canvasesSettings.Add(canvasSettings);
                StretchedCanvasSettingsService.WriteSettings(canvasesSettings.ToArray());
            }

            return canvasSettings.IsVertical
                   ? CreateVerticalCanvas(canvasSettings, diamond.Path)
                   : CreateHorizontalCanvas(canvasSettings, diamond.Path);
        }

        private Bitmap CreateHorizontalCanvas(StretchedCanvasSettings canvasSettings, string path)
        {
            Bitmap canvas = new Bitmap(canvasSettings.PageWidth, canvasSettings.PageHeight);

            Bitmap thumbnail = new Bitmap(path + "/Вид вышивки.png");
            if (!IsVertical(thumbnail))
            {
                thumbnail = new Bitmap(thumbnail, new Size(canvasSettings.CanvasWidth, canvasSettings.CanvasHeight));
            }
            else
            {
                thumbnail = new Bitmap(thumbnail, new Size(canvasSettings.CanvasHeight, canvasSettings.CanvasWidth));
            }

            using (Graphics graph = GraphicsService.GetGraphFromImage(canvas))
            {
                graph.Clear(Color.White);

                using (Bitmap shemeBitmap = new Bitmap(path + "/Схема для печати.png"))
                {
                    if (IsVertical(thumbnail))
                    {
                        thumbnail.RotateFlip(RotateFlipType.Rotate270FlipNone);

                        Bitmap sheme = GraphicsService.CutRectangleFromBitmap(shemeBitmap, canvasSettings.CanvasFromShemeMarginLeft, canvasSettings.CanvasFromShemeMarginTop, canvasSettings.CanvasFromShemeWidth, shemeBitmap.Height - 2 - (canvasSettings.CanvasFromShemeMarginTop * 2));

                        sheme = GraphicsService.RemoveRightBorder(sheme, Color.FromArgb(255, 255, 255, 255));
                        sheme = GraphicsService.RemoveTopBorder(sheme, Color.FromArgb(255, 255, 255, 255));
                        sheme = GraphicsService.RemoveLeftBorder(sheme, Color.FromArgb(255, 255, 255, 255));
                        sheme = GraphicsService.RemoveBottomBorder(sheme, Color.FromArgb(255, 255, 255, 255));

                        sheme.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        graph.DrawImage(sheme, canvasSettings.MarginLeft, canvasSettings.MarginTop, canvasSettings.CanvasWidth, canvasSettings.CanvasHeight);
                    }
                    else
                    {
                        Bitmap sheme = GraphicsService.CutRectangleFromBitmap(shemeBitmap, canvasSettings.CanvasFromShemeMarginLeft, canvasSettings.CanvasFromShemeMarginTop, shemeBitmap.Width - 2 - (canvasSettings.CanvasFromShemeMarginLeft * 2), canvasSettings.CanvasFromShemeWidth);

                        sheme = GraphicsService.RemoveRightBorder(sheme, Color.FromArgb(255, 255, 255, 255));
                        sheme = GraphicsService.RemoveTopBorder(sheme, Color.FromArgb(255, 255, 255, 255));
                        sheme = GraphicsService.RemoveLeftBorder(sheme, Color.FromArgb(255, 255, 255, 255));
                        sheme = GraphicsService.RemoveBottomBorder(sheme, Color.FromArgb(255, 255, 255, 255));

                        graph.DrawImage(sheme, canvasSettings.MarginLeft, canvasSettings.MarginTop, canvasSettings.CanvasWidth, canvasSettings.CanvasHeight);
                        sheme.Dispose();
                    }
                }

                graph.DrawImage(CreateBorders(canvas, thumbnail, canvasSettings), 0, 0);
            }

            thumbnail.Dispose();

            return canvas;
        }

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

        private Bitmap CreateVerticalCanvas(StretchedCanvasSettings canvasSettings, string path)
        {
            Bitmap canvas = new Bitmap(canvasSettings.PageWidth, canvasSettings.PageHeight);

            Bitmap thumbnail = new Bitmap(path + "/Вид вышивки.png");
            if (IsVertical(thumbnail))
            {
                thumbnail = new Bitmap(thumbnail, new Size(canvasSettings.CanvasWidth, canvasSettings.CanvasHeight));
            }
            else
            {
                thumbnail = new Bitmap(thumbnail, new Size(canvasSettings.CanvasHeight, canvasSettings.CanvasWidth));
            }
            
            using (Graphics graph = GraphicsService.GetGraphFromImage(canvas))
            {
                graph.Clear(Color.White);

                using (Bitmap shemeBitmap = new Bitmap(path + "/Схема для печати.png"))
                {
                    if (!IsVertical(thumbnail))
                    {
                        thumbnail.RotateFlip(RotateFlipType.Rotate270FlipNone);

                        Bitmap sheme = GraphicsService.CutRectangleFromBitmap(shemeBitmap, canvasSettings.CanvasFromShemeMarginLeft, canvasSettings.CanvasFromShemeMarginTop, shemeBitmap.Width - 2 - (canvasSettings.CanvasFromShemeMarginLeft * 2), canvasSettings.CanvasFromShemeWidth);

                        sheme = GraphicsService.RemoveRightBorder(sheme, Color.FromArgb(255, 255, 255, 255));
                        sheme = GraphicsService.RemoveTopBorder(sheme, Color.FromArgb(255, 255, 255, 255));
                        sheme = GraphicsService.RemoveLeftBorder(sheme, Color.FromArgb(255, 255, 255, 255));
                        sheme = GraphicsService.RemoveBottomBorder(sheme, Color.FromArgb(255, 255, 255, 255));

                        sheme.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        graph.DrawImage(sheme, canvasSettings.MarginLeft, canvasSettings.MarginTop, canvasSettings.CanvasWidth, canvasSettings.CanvasHeight);

                    }
                    else
                    {
                        Bitmap sheme = GraphicsService.CutRectangleFromBitmap(shemeBitmap, canvasSettings.CanvasFromShemeMarginLeft, canvasSettings.CanvasFromShemeMarginTop, canvasSettings.CanvasFromShemeWidth, shemeBitmap.Height - 2 - (canvasSettings.CanvasFromShemeMarginTop * 2));
                        sheme = GraphicsService.RemoveRightBorder(sheme, Color.FromArgb(255, 255, 255, 255));
                        sheme = GraphicsService.RemoveTopBorder(sheme, Color.FromArgb(255, 255, 255, 255));
                        sheme = GraphicsService.RemoveLeftBorder(sheme, Color.FromArgb(255, 255, 255, 255));
                        sheme = GraphicsService.RemoveBottomBorder(sheme, Color.FromArgb(255, 255, 255, 255));

                        graph.DrawImage(sheme, canvasSettings.MarginLeft, canvasSettings.MarginTop, canvasSettings.CanvasWidth, canvasSettings.CanvasHeight);
                        sheme.Dispose();
                    }
                }

                graph.DrawImage(CreateBorders(canvas, thumbnail, canvasSettings), 0, 0);
            }

            thumbnail.Dispose();

            return canvas;
        }

        private bool IsVertical(string diamondPath)
        {
            using (Bitmap thumbnailBitmap = new Bitmap(diamondPath + "/Вид вышивки.png"))
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
