using Newtonsoft.Json;

namespace DiamondListCreator.Models
{
    public class StretchedCanvasSettings
    {
        [JsonProperty("Розмір")]
        public string SizeName { get; set; }

        [JsonProperty("Ширина")]
        public int SizeWidth { get; set; }

        [JsonProperty("Висота")]
        public int SizeHeight { get; set; }

        [JsonProperty("Вертикальна орієнтація")]
        public bool IsVertical { get; set; }

        private int _pageWidth;
        [JsonProperty("Ширина листа")]
        public int PageWidth
        {
            get { return _pageWidth; }
            set
            {
                _pageWidth = value;
                if (IsSizePropertiesInitialized())
                {
                    SetMargins();
                }
            }
        }

        private int _pageHeight;
        [JsonProperty("Висота листа")]
        public int PageHeight
        {
            get { return _pageHeight; }
            set
            {
                _pageHeight = value;
                if (IsSizePropertiesInitialized())
                {
                    SetMargins();
                }
            }
        }

        private int _canvasWidth;
        [JsonProperty("Ширина холста")]
        public int CanvasWidth
        {
            get { return _canvasWidth; }
            set
            {
                _canvasWidth = value;
                if (IsSizePropertiesInitialized())
                {
                    SetMargins();
                }
            }
        }

        private int _canvasHeight;
        [JsonProperty("Висота холста")]
        public int CanvasHeight
        {
            get { return _canvasHeight; }
            set
            {
                _canvasHeight = value;
                if (IsSizePropertiesInitialized())
                {
                    SetMargins();
                }
            }
        }

        [JsonProperty("Ширина рамки")]
        public int BorderWidth { get; set; }

        [JsonProperty("Висота рамки")]
        public int BorderHeight { get; set; }

        [JsonProperty("Відступ зліва холста зі схеми")]
        public int CanvasFromShemeMarginLeft { get; set; }

        [JsonProperty("Відступ зверху холста зі схеми")]
        public int CanvasFromShemeMarginTop { get; set; }

        [JsonProperty("Ширина холста зі схеми")]
        public int CanvasFromShemeWidth { get; set; }

        [JsonProperty("Відступ збоку бокових елементів")]
        public int SideElementSideOffset { get; set; }

        [JsonProperty("Відступ від краю бокових елементів (вертикальна)")]
        public int SideElementOffsetVertical { get; set; }

        [JsonProperty("Відступ від краю бокових елементів (горизонтальна)")]
        public int SideElementOffsetHorizontal { get; set; }

        [JsonProperty("Висота бокових елементів")]
        public int SideElementHeight { get; set; }

        public int MarginLeft { get; set; }

        public int MarginTop { get; set; }

        public int MarginRight { get; set; }

        public int MarginBottom { get; set; }

        public StretchedCanvasSettings()
        {

        }

        public StretchedCanvasSettings(StretchedCanvasSettings stretchedCanvasSettings)
        {
            SizeName = stretchedCanvasSettings.SizeName;
            SizeWidth = stretchedCanvasSettings.SizeWidth;
            SizeHeight = stretchedCanvasSettings.SizeHeight;
            IsVertical = stretchedCanvasSettings.IsVertical;
            PageWidth = stretchedCanvasSettings.PageWidth;
            PageWidth = stretchedCanvasSettings.PageWidth;
            PageHeight = stretchedCanvasSettings.PageHeight;
            PageHeight = stretchedCanvasSettings.PageHeight;
            CanvasWidth = stretchedCanvasSettings.CanvasWidth;
            CanvasWidth = stretchedCanvasSettings.CanvasWidth;
            CanvasHeight = stretchedCanvasSettings.CanvasHeight;
            CanvasHeight = stretchedCanvasSettings.CanvasHeight;
            BorderWidth = stretchedCanvasSettings.BorderWidth;
            BorderHeight = stretchedCanvasSettings.BorderHeight;
            CanvasFromShemeMarginLeft = stretchedCanvasSettings.CanvasFromShemeMarginLeft;
            CanvasFromShemeMarginTop = stretchedCanvasSettings.CanvasFromShemeMarginTop;
            CanvasFromShemeWidth = stretchedCanvasSettings.CanvasFromShemeWidth;
            SideElementSideOffset = stretchedCanvasSettings.SideElementSideOffset;
            SideElementOffsetVertical = stretchedCanvasSettings.SideElementOffsetVertical;
            SideElementOffsetHorizontal = stretchedCanvasSettings.SideElementOffsetHorizontal;
            SideElementHeight = stretchedCanvasSettings.SideElementHeight;
            MarginLeft = stretchedCanvasSettings.MarginLeft;
            MarginTop = stretchedCanvasSettings.MarginTop;
            MarginRight = stretchedCanvasSettings.MarginRight;
            MarginBottom = stretchedCanvasSettings.MarginBottom;
        }

        public void SetSize(int sizeWidth, int sizeHeight)
        {
            int oldSizeWidth = SizeWidth;
            int oldSizeHeight = SizeHeight;
            int oldCanvasHeight = CanvasHeight;
            SizeWidth = sizeWidth;
            SizeHeight = sizeHeight;

            float pixelsInOneSm = oldCanvasHeight / oldSizeHeight;
            CanvasHeight = sizeWidth > sizeHeight
                ? ((int)pixelsInOneSm * sizeWidth)
                : sizeWidth < sizeHeight ? ((int)pixelsInOneSm * sizeHeight) : oldSizeWidth;

            PageHeight += CanvasHeight - oldCanvasHeight;

            SizeName += "+";
        }

        private bool IsSizePropertiesInitialized()
        {
            return PageWidth != 0 && PageHeight != 0 && CanvasWidth != 0 && CanvasHeight != 0;
        }

        private void SetMargins()
        {
            MarginLeft = (PageWidth - CanvasWidth) / 2;
            MarginTop = (PageHeight - CanvasHeight) / 2;
            MarginRight = MarginLeft + CanvasWidth - 3;
            MarginBottom = MarginTop + CanvasHeight - 3;
        }
    }
}
