using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace DiamondListCreator.Services
{
    public class FileService
    {
        public static void SaveBitmapsInTif(Bitmap[] bitmaps, string savingPath, string fileName)
        {
            for (int i = 0; i < bitmaps.Length; i++)
            {
                SaveBitmapInTif(bitmaps[i], savingPath, fileName + (i > 0 ? $"_{i}" : ""));
            }
        }

        public static void SaveBitmapInTif(Bitmap bitmap, string savingPath, string fileName)
        {
            ImageCodecInfo myImageCodecInfo = GetEncoderInfo("image/tiff");
            Encoder myEncoder = Encoder.Compression;
            EncoderParameter myEncoderParameter = new EncoderParameter(myEncoder, (long)EncoderValue.CompressionNone);
            EncoderParameters myEncoderParameters = new EncoderParameters(1);
            myEncoderParameters.Param[0] = myEncoderParameter;

            bitmap.Save($"{savingPath}/{fileName}.tif", myImageCodecInfo, myEncoderParameters);
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
                string newFolder = $"{directory}/{newFolderName}";
                foreach (FileInfo file in new DirectoryInfo(directory).GetFiles())
                {
                    if (file.Name.Contains(".db"))
                    {
                        continue;
                    }
                    if (!Directory.Exists(newFolder))
                    {
                        _ = Directory.CreateDirectory($"{newFolder}");
                    }
                    file.MoveTo($@"{newFolder}\{file.Name}");
                }
            }
        }
    }
}
