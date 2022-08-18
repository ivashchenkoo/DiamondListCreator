using System;
using System.Drawing;
using System.IO;
using DiamondListCreator.Models;
using DiamondListCreator.Services.ConsumablesCreators;

namespace DiamondListCreator.Services
{
    public class CanvasesService : IDisposable
    {
        private readonly CanvasCreator canvasCreator;
        private readonly StretchedCanvasCreator stretchedCanvasCreator;

        public CanvasesService()
        {
            canvasCreator = new CanvasCreator();
            stretchedCanvasCreator = new StretchedCanvasCreator();
        }

        public void Dispose()
        {
            canvasCreator.Dispose();
            stretchedCanvasCreator.Dispose();
        }

        /// <summary>
        /// Creating canvases and saving them into canvases saving folder
        /// </summary>
        /// <param name="diamond"></param>
        /// <param name="paths">Path settings</param>
        /// <returns>Message with a diamond name and an error if it is exists</returns>
        public string CreateAndSaveCanvas(DiamondSettings diamond, PathSettings paths)
        {
            string savedCanvasDirectory = Path.Combine(paths.SavedCanvasesPath, $"{diamond.ShortName.Substring(0, 2)}000");
            string diamondName = diamond.Name + (diamond.IsStretchedCanvas ? "P" : "");

            if (diamond.DiamondType == DiamondType.Standard && CopySavedCanvas(diamondName, savedCanvasDirectory, paths.CanvasesSavePath))
            {
                return diamondName;
            }

            try
            {
                using (Bitmap canvas = diamond.IsStretchedCanvas ? stretchedCanvasCreator.Create(diamond) : canvasCreator.Create(diamond))
                {
                    canvas.SetResolution(72f, 72f);
                    FileService.SaveBitmapInTif(canvas, paths.CanvasesSavePath, diamondName);

                    if (diamond.DiamondType == DiamondType.Standard)
                    {
                        if (!Directory.Exists(savedCanvasDirectory))
                        {
                            Directory.CreateDirectory(savedCanvasDirectory);
                        }
                        FileService.SaveBitmapInTif(canvas, savedCanvasDirectory, diamondName);
                    }
                }

                return diamondName;
            }
            catch (Exception ex)
            {
                return $"{diamondName} - {ex.Message}";
            }
        }

        /// <summary>
        /// Checking for already created canvas in saved canvases folder and copy it to canvases saving folder if exist
        /// </summary>
        /// <param name="diamondName"></param>
        /// <param name="savedCanvasesPath">The directory, where previously created canvases are saved</param>
        /// <param name="canvasesSavePath">The directory, where needed canvases should be saved</param>
        /// <returns>True if the file was successfully copied and false if its not</returns>
        private bool CopySavedCanvas(string diamondName, string savedCanvasDirectory, string canvasesSavePath)
        {
            if (!Directory.Exists(savedCanvasDirectory))
            {
                Directory.CreateDirectory(savedCanvasDirectory);
            }

            if (File.Exists(Path.Combine(savedCanvasDirectory, $"{diamondName}.tif")))
            {
                File.Copy(Path.Combine(savedCanvasDirectory, $"{diamondName}.tif"), Path.Combine(canvasesSavePath, $"{diamondName}.tif"), true);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
