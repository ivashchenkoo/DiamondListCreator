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

        [JsonProperty("dpi")]
        public int Dpi { get; set; }

        [JsonIgnore]
        public int MarginLeft { get; set; }

        [JsonIgnore]
        public int MarginTop { get; set; }

        [JsonIgnore]
        public int MarginRight { get; set; }

        [JsonIgnore]
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
            PageHeight = stretchedCanvasSettings.PageHeight;
            CanvasWidth = stretchedCanvasSettings.CanvasWidth;
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
            if (!IsVertical)
            {
                IsVertical = true;
                PageHeight = PageWidth - (PageWidth - CanvasWidth - BorderWidth * 2);
                SwapCanvasHeightAndWidth();
            }

            int oldSizeHeight = SizeHeight;
            int oldCanvasHeight = CanvasHeight;
            SizeWidth = sizeWidth;
            SizeHeight = sizeHeight;

            float pixelsInOneSm = oldCanvasHeight / oldSizeHeight;
            CanvasHeight = (int)pixelsInOneSm * SizeHeight;
            PageHeight += CanvasHeight - oldCanvasHeight;

            SizeName += "+";
        }

        private void SwapCanvasHeightAndWidth()
        {
            (CanvasHeight, CanvasWidth) = (CanvasWidth, CanvasHeight);
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
