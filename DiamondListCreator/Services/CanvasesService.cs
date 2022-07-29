using DiamondListCreator.Models;
using DiamondListCreator.Services.ConsumablesCreators;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Threading.Tasks;

namespace DiamondListCreator.Services
{
    public class CanvasesService
    {
        /// <summary>
        /// Creating canvases and saving them into canvases saving folder
        /// </summary>
        /// <param name="diamonds">DiamondSettings list</param>
        /// <param name="paths">Path settings</param>
        public void CreateCanvasesFiles(List<DiamondSettings> diamonds, PathSettings paths)
        {
            FileService.SaveAllToNewFolder(paths.CanvasesSavePath, $"Old {DateTime.Now}".Replace(":", "_"));

            CanvasCreator canvasCreator = new CanvasCreator(FontCollectionService.InitCustomFont(Properties.Resources.VanishingSizeName_Regular));
            StretchedCanvasCreator stretchedCanvasCreator = new StretchedCanvasCreator(FontCollectionService.InitCustomFont(Properties.Resources.VanishingSizeName_Regular));
            string diamondsListString = "";
            for (int i = 0; i < diamonds.Count; i++)
            {
                if (diamonds[i].IsStretchedCanvas)
                {
                    diamondsListString += $"{diamonds[i].Name}P";

                    try
                    {
                        using (Bitmap canvas = stretchedCanvasCreator.Create(diamonds[i]))
                        {
                            canvas.SetResolution(72f, 72f);
                            FileService.SaveBitmapInTif(canvas, paths.CanvasesSavePath, diamonds[i].Name + "P");
                        }
                    }
                    catch (Exception ex)
                    {
                        diamondsListString += " - " + ex.Message;
                    }

                    diamondsListString += "\n";
                }
                else
                {
                    diamondsListString += $"{diamonds[i].Name}\n";

                    if (!CopySavedCanvas(diamonds[i].Name, paths.SavedCanvasesPath, paths.CanvasesSavePath))
                    {
                        using (Bitmap canvas = canvasCreator.Create(diamonds[i]))
                        {
                            canvas.SetResolution(72f, 72f);
                            FileService.SaveBitmapInTif(canvas, paths.CanvasesSavePath, diamonds[i].Name);

                            if (diamonds[i].DiamondType == DiamondType.Standard)
                            {
                                FileService.SaveBitmapInTif(canvas, paths.SavedCanvasesPath, diamonds[i].Name);
                            }
                        }
                    }
                }
            }

            File.WriteAllText($"{paths.CanvasesSavePath}/Canvases {DateTime.Now : dd.MM.yyyy}.txt", diamondsListString);
        }

        /// <summary>
        /// Creating canvases and saving them into canvases saving folder async
        /// </summary>
        /// <param name="diamonds">DiamondSettings list</param>
        /// <param name="paths">Path settings</param>
        public async void CreateCanvasesFilesAsync(List<DiamondSettings> diamonds, PathSettings paths)
        {
            await Task.Run(() => CreateCanvasesFiles(diamonds, paths));
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
