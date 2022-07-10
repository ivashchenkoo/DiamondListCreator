using System;
using System.Drawing.Text;
using System.Runtime.InteropServices;

namespace DiamondListCreator.Services
{
    public class FontCollectionService
    {
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
