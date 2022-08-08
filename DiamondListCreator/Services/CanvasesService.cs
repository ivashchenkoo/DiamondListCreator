using DiamondListCreator.Models;
using DiamondListCreator.Services.ConsumablesCreators;
using System;
using System.Drawing;
using System.IO;

namespace DiamondListCreator.Services
{
    public class CanvasesService
    {
        private readonly CanvasCreator canvasCreator;
        private readonly StretchedCanvasCreator stretchedCanvasCreator;

        public CanvasesService()
        {
            canvasCreator = new CanvasCreator(FontCollectionService.InitCustomFont(Properties.Resources.VanishingSizeName_Regular));
            stretchedCanvasCreator = new StretchedCanvasCreator(FontCollectionService.InitCustomFont(Properties.Resources.VanishingSizeName_Regular));
        }

        /// <summary>
        /// Creating canvases and saving them into canvases saving folder
        /// </summary>
        /// <param name="diamond"></param>
        /// <param name="paths">Path settings</param>
        /// <returns>Message with a diamond name and an error if it is exists</returns>
        public string CreateAndSaveCanvas(DiamondSettings diamond, PathSettings paths)
        {
            if (diamond.IsStretchedCanvas)
            {
                try
                {
                    using (Bitmap canvas = stretchedCanvasCreator.Create(diamond))
                    {
                        canvas.SetResolution(72f, 72f);
                        FileService.SaveBitmapInTif(canvas, paths.CanvasesSavePath, diamond.Name + "P");
                    }

                    return $"{diamond.Name}P";
                }
                catch (Exception ex)
                {
                    return $"{diamond.Name}P - {ex.Message}";
                }
            }
            else
            {
                if (!CopySavedCanvas(diamond.Name, paths.SavedCanvasesPath, paths.CanvasesSavePath))
                {
                    try
                    {
                        using (Bitmap canvas = canvasCreator.Create(diamond))
                        {
                            canvas.SetResolution(72f, 72f);
                            FileService.SaveBitmapInTif(canvas, paths.CanvasesSavePath, diamond.Name);

                            if (diamond.DiamondType == DiamondType.Standard)
                            {
                                if (Directory.Exists(paths.SavedCanvasesPath))
                                {
                                    FileService.SaveBitmapInTif(canvas, paths.SavedCanvasesPath, diamond.Name);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        return $"{diamond.Name} - {ex.Message}";
                    }
                }

                return $"{diamond.Name}";
            }
        }

        /// <summary>
        /// Checking for already created canvas in saved canvases folder and copy it to canvases saving folder if exist
        /// </summary>
        /// <param name="diamondName"></param>
        /// <param name="savedCanvasesPath">The directory, where previously created canvases are saved</param>
        /// <param name="canvasesSavePath">The directory, where needed canvases should be saved</param>
        /// <returns>True if the file was successfully copied and false if its not</returns>
        private bool CopySavedCanvas(string diamondName, string savedCanvasesPath, string canvasesSavePath)
        {
            string canvasPath = $"{savedCanvasesPath}/{diamondName.Replace("TWD", "").Substring(0, 2)} 000";

            if (File.Exists($"{canvasPath}/{diamondName}.tif"))
            {
                File.Copy($"{canvasPath}/{diamondName}.tif", $"{canvasesSavePath}/{diamondName}.tif", true);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
