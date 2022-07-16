using iTextSharp.text;
using iTextSharp.text.pdf;
using System.Drawing.Imaging;
using System.IO;
using Bitmap = System.Drawing.Bitmap;

namespace DiamondListCreator.Services
{
    public class PdfDocumentService
    {
        private readonly MemoryStream stream;
        private readonly Rectangle pageSize;
        private readonly Document document;
        private readonly PdfWriter writer;
        private readonly PdfContentByte cb;
        private readonly BaseFont bf;

        public PdfDocumentService(int pageWidth, int pageHeight)
        {
            stream = new MemoryStream();
            pageSize = new Rectangle(0, 0, pageWidth, pageHeight);
            document = new Document(pageSize, 0, 0, 0, 0);
            writer = PdfWriter.GetInstance(document, stream);
            document.Open();

            // the pdf content
            cb = writer.DirectContent;
            // select the font properties
            bf = BaseFont.CreateFont(BaseFont.HELVETICA, BaseFont.CP1252, BaseFont.NOT_EMBEDDED);
        }

        ~PdfDocumentService()
        {
            stream.Dispose();
            document.Dispose();
        }

        /// <summary>
        /// Writes text on page
        /// </summary>
        public void DrawText(string text, int fontSize, int x, int y)
        {
            cb.SetColorFill(BaseColor.BLACK);
            cb.SetFontAndSize(bf, fontSize);
            cb.BeginText();
            cb.ShowTextAligned(0, text, x, y, 0);
            cb.EndText();
        }

        /// <summary>
        /// Signals that a new page has to be started
        /// </summary>
        /// <returns>True if the page added, false if not</returns>
        public bool NewPage()
        {
            return document.NewPage();
        }

        /// <summary>
        /// Adds an element to the document
        /// </summary>
        /// <param name="image"></param>
        /// <returns>True if the element was added, otherwise false</returns>
        public bool AddPage(Image image)
        {
            return document.Add(image);
        }

        /// <summary>
        /// Adds an element to the document
        /// </summary>
        /// <param name="image"></param>
        /// <returns>True if the element was added, otherwise false</returns>
        public bool AddPage(Bitmap bitmap)
        {
            Image image = Image.GetInstance(bitmap, ImageFormat.Tiff);
            return document.Add(image);
        }

        /// <summary>
        /// Adds some elements to the document
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public void AddPages(Bitmap[] pages)
        {
            for (int i = 0; i < pages.Length; i++)
            {
                Image image = Image.GetInstance(pages[i], ImageFormat.Tiff);
                _ = document.Add(image);
            }
        }

        /// <summary>
        /// Adds some elements to the document in reverse order
        /// </summary>
        /// <param name="image"></param>
        /// <returns></returns>
        public void AddPagesReverse(Bitmap[] pages)
        {
            for (int i = pages.Length - 1; i >= 0; i--)
            {
                Image image = Image.GetInstance(pages[i], ImageFormat.Tiff);
                _ = document.Add(image);
            }
        }

        /// <summary>
        /// Saves pdf file by given path
        /// </summary>
        /// <param name="path"></param>
        public void Save(string path)
        {
            document.Close();
            byte[] content = stream.ToArray();
            using (FileStream fs = File.Create(path))
            {
                fs.Write(content, 0, content.Length);
            }
        }
    }
}
