using System.Drawing;
using System.Drawing.Imaging;

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
    }
}
