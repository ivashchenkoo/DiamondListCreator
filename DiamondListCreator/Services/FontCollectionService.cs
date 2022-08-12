using System;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace DiamondListCreator.Services
{
    static class FontCollectionService
    {
        /// <summary>
        /// Creates a PrivateFontCollection array with the specified font as 0 element of the array
        /// </summary>
        /// <param name="fontData"></param>
        /// <returns></returns>
        public static PrivateFontCollection InitCustomFont(byte[] fontData)
        {
            PrivateFontCollection pfc = new PrivateFontCollection();
            int fontLength = fontData.Length;
            IntPtr data = Marshal.AllocCoTaskMem(fontLength);
            Marshal.Copy(fontData, 0, data, fontLength);
            pfc.AddMemoryFont(data, fontLength);

            return pfc;
        }
    }
}
