using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace DiamondListCreator.Services
{
    static class FileService
    {
        /// <summary>
        /// Saves bitmaps to tif extension
        /// </summary>
        public static void SaveBitmapsInTif(Bitmap[] bitmaps, string savingPath, string fileName)
        {
            for (int i = 0; i < bitmaps.Length; i++)
            {
                SaveBitmapInTif(bitmaps[i], savingPath, fileName + (i > 0 ? $"_{i}" : ""));
            }
        }

        /// <summary>
        /// Saves a bitmap to tif extension
        /// </summary>
        public static void SaveBitmapInTif(Bitmap bitmap, string savingPath, string fileName)
        {
            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/tiff");
            Encoder myEncoder = Encoder.Compression;
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, (long)EncoderValue.CompressionNone);
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            myEncoderParameters.Param[0] = myEncoderParameter;

            bitmap.Save(Path.Combine(savingPath, fileName + ".tif"), myImageCodecInfo, myEncoderParameters);
        }

        private static ImageCodecInfo GetEncoderInfo(string mimeType)
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

        /// <summary>
        /// Saves all files in directory to new folder in this directory
        /// </summary>
        public static void SaveAllToNewFolder(string directory, string newFolderName)
        {
            if (Directory.Exists(directory))
            {
                string newFolder = Path.Combine(directory, newFolderName);
                foreach (FileInfo file in new DirectoryInfo(directory).GetFiles())
                {
                    if (new string[] { ".tif", ".tiff", ".png", ".jpg", ".jpeg", ".txt" }.Any(x => !file.Name.EndsWith(x)))
                    {
                        continue;
                    }
                    if (!Directory.Exists(newFolder))
                    {
                        _ = Directory.CreateDirectory(newFolder);
                    }
                    file.MoveTo(Path.Combine(newFolder, file.Name));
                }
            }
        }

        /// <summary>
        /// Opens a dialog window to choose a directory
        /// </summary>
        /// <returns>The path to a picked directory if dialog was successfully closed, null if dialog was closed by close button</returns>
        public static string OpenDirectory(string title = "Оберіть папку", string selectedPath = "C:\\Users")
        {
            using (FolderBrowserDialog dialog = new FolderBrowserDialog { Description = title, SelectedPath = selectedPath })
            {
                DialogResult result = dialog.ShowDialog();
                if (result == DialogResult.OK)
                {
                    return dialog.SelectedPath;
                }
                else
                {
                    return selectedPath;
                }
            }
        }

        /// <summary>
        /// Opens a dialog window to choose a file
        /// </summary>
        /// <returns>The path to a picked file if dialog was successfully closed, null if dialog was closed by close button</returns>
        public static string OpenFile(string title = "Оберіть файл", string selectedPath = "C:\\Users", string filter = "Excel Worksheets (*.xls;*.xlsx)|*.xls;*.xlsx")
        {
            using (OpenFileDialog dialog = new OpenFileDialog
            {
                InitialDirectory = selectedPath,
                Filter = filter,
                Title = title
            })
            {
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.FileName;
                }
                else
                {
                    return selectedPath;
                }
            }
        }
    }
}
