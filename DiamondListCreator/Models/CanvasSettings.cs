using Newtonsoft.Json;

namespace DiamondListCreator.Models
{
    public class CanvasSettings
    {
        [JsonProperty("Розмір")]
        public string SizeName { get; set; }

        [JsonProperty("Розмір Ширина")]
        public int SizeWidth { get; set; }

        [JsonProperty("Розмір Висота")]
        public int SizeHeight { get; set; }

        private int _pageWidth;
        [JsonProperty("Ширина листа")]
        public int PageWidth
        {
            get { return _pageWidth; }
            set
            {
                _pageWidth = value;
                SetMargins();
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
                SetMargins();
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
                SetMargins();
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
                SetMargins();
            }
        }

        private int? _canvasMarginLeft;
        [JsonProperty("Відступ холста зліва")]
        public int? CanvasMarginLeft
        {
            get { return _canvasMarginLeft; }
            set
            {
                _canvasMarginLeft = value;
                SetMargins();
            }
        }

        [JsonProperty("Зсув готового холста ліворуч")]
        public int PageOffsetX { get; set; }

        [JsonProperty("Відступ між боковими елементами")]
        public int Spacing { get; set; }

        [JsonProperty("Висота інструкції")]
        public int InstructionHeight { get; set; }

        [JsonProperty("Ширина нижнього лого")]
        public int BottomLogoWidth { get; set; }

        [JsonProperty("Висота нижнього лого")]
        public int BottomLogoHeight { get; set; }

        [JsonProperty("Ширина горизонтального бокового лого")]
        public int RightHorizontalLogoWidth { get; set; }

        [JsonProperty("Висота горизонтального бокового лого")]
        public int RightHorizontalLogoHeight { get; set; }

        [JsonProperty("Ширина виду вишивки")]
        public int ThumbnailWidth { get; set; }

        [JsonProperty("Висота виду вишивки")]
        public int ThumbnailHeight { get; set; }

        [JsonProperty("Розмір рамки у виду вишивки")]
        public int ThumbnailBorderThickness { get; set; }

        [JsonProperty("Ширина вертикального бокового лого")]
        public int RightVerticalLogoWidth { get; set; }

        [JsonProperty("Висота вертикального бокового лого")]
        public int RightVerticalLogoHeight { get; set; }

        [JsonProperty("Ширина urls")]
        public int UrlsWidth { get; set; }

        [JsonProperty("Висота urls")]
        public int UrlsHeight { get; set; }

        [JsonProperty("Розмір шрифту назви алмазки")]
        public int DiamondNameFontSize { get; set; }

        [JsonProperty("Розмір шрифту розміру алмазки")]
        public int DiamondSizeFontSize { get; set; }

        [JsonProperty("Відсуп зліва холста зі схеми")]
        public int CanvasFromShemeMarginLeft { get; set; }

        [JsonProperty("Відсуп зверху холста зі схеми")]
        public int CanvasFromShemeMarginTop { get; set; }

        [JsonProperty("Ширина холста зі схеми")]
        public int CanvasFromShemeWidth { get; set; }

        [JsonProperty("Відступ зліва до легенди на верикальній схемі")]
        public int LegendsFromShemeVerticalMarginLeft { get; set; }

        [JsonProperty("Відступ зверху до легенди на верикальній схемі")]
        public int LegendsFromShemeVerticalMarginTop { get; set; }

        [JsonProperty("Ширина легенди на верикальній схемі")]
        public int LegendsFromShemeVerticalWidth { get; set; }

        [JsonProperty("Висота легенди на верикальній схемі")]
        public int LegendsFromShemeVerticalHeight { get; set; }

        [JsonProperty("Висота одного рядка (з відступом) легенди на схемі")]
        public int LegendsFromShemeRowHeight { get; set; }

        [JsonProperty("Кількість рядків у легенді холста")]
        public int RowsCountInLegend { get; set; }

        [JsonProperty("Відступ між стовпцями у легенді холста")]
        public int PaddingInLegend { get; set; }

        [JsonProperty("Легенда відцентрована - true, на лінії виду вишивки - false")]
        public bool LegendAlignCenter { get; set; }

        [JsonProperty("Відступ зверху до легенди на горизонтальній схемі")]
        public int LegendsFromShemeHorizontalMarginTop { get; set; }

        [JsonIgnore]
        public int MarginTop { get; set; }

        [JsonIgnore]
        public int MarginBottom { get; set; }

        [JsonIgnore]
        public int MarginLeft { get; set; }

        [JsonIgnore]
        public int MarginRight { get; set; }

        public CanvasSettings()
        {

        }

        public CanvasSettings(CanvasSettings canvasSettings)
        {
            SizeName = canvasSettings.SizeName;
            SizeWidth = canvasSettings.SizeWidth;
            SizeHeight = canvasSettings.SizeHeight;
            PageWidth = canvasSettings.PageWidth;
            PageHeight = canvasSettings.PageHeight;
            CanvasWidth = canvasSettings.CanvasWidth;
            CanvasHeight = canvasSettings.CanvasHeight;
            CanvasMarginLeft = canvasSettings.CanvasMarginLeft;
            PageOffsetX = canvasSettings.PageOffsetX;
            Spacing = canvasSettings.Spacing;
            InstructionHeight = canvasSettings.InstructionHeight;
            BottomLogoWidth = canvasSettings.BottomLogoWidth;
            BottomLogoHeight = canvasSettings.BottomLogoHeight;
            RightHorizontalLogoWidth = canvasSettings.RightHorizontalLogoWidth;
            RightHorizontalLogoHeight = canvasSettings.RightHorizontalLogoHeight;
            ThumbnailWidth = canvasSettings.ThumbnailWidth;
            ThumbnailHeight = canvasSettings.ThumbnailHeight;
            ThumbnailBorderThickness = canvasSettings.ThumbnailBorderThickness;
            RightVerticalLogoWidth = canvasSettings.RightVerticalLogoWidth;
            RightVerticalLogoHeight = canvasSettings.RightVerticalLogoHeight;
            UrlsWidth = canvasSettings.UrlsWidth;
            UrlsHeight = canvasSettings.UrlsHeight;
            DiamondNameFontSize = canvasSettings.DiamondNameFontSize;
            DiamondSizeFontSize = canvasSettings.DiamondSizeFontSize;
            CanvasFromShemeMarginLeft = canvasSettings.CanvasFromShemeMarginLeft;
            CanvasFromShemeMarginTop = canvasSettings.CanvasFromShemeMarginTop;
            CanvasFromShemeWidth = canvasSettings.CanvasFromShemeWidth;
            LegendsFromShemeVerticalMarginLeft = canvasSettings.LegendsFromShemeVerticalMarginLeft;
            LegendsFromShemeVerticalMarginTop = canvasSettings.LegendsFromShemeVerticalMarginTop;
            LegendsFromShemeVerticalWidth = canvasSettings.LegendsFromShemeVerticalWidth;
            LegendsFromShemeVerticalHeight = canvasSettings.LegendsFromShemeVerticalHeight;
            LegendsFromShemeRowHeight = canvasSettings.LegendsFromShemeRowHeight;
            RowsCountInLegend = canvasSettings.RowsCountInLegend;
            PaddingInLegend = canvasSettings.PaddingInLegend;
            LegendAlignCenter = canvasSettings.LegendAlignCenter;
            LegendsFromShemeHorizontalMarginTop = canvasSettings.LegendsFromShemeHorizontalMarginTop;
        }

        /// <summary>
        /// Calculates margins by existing fields of heights and widths
        /// </summary>
        private void SetMargins()
        {
            if (!IsSizePropertiesInitialized())
            {
                return;
            }

            MarginTop = (PageHeight - CanvasHeight) / 2;
            MarginBottom = PageHeight - CanvasHeight - MarginTop;
            if (SizeName == "XL")
            {
                MarginRight = (PageWidth - CanvasWidth) / 2;
                MarginLeft = PageWidth - CanvasWidth - MarginRight;
            }
            else
            {
                MarginLeft = (int)CanvasMarginLeft;
                MarginRight = PageWidth - MarginLeft - CanvasWidth;
            }
        }

        /// <summary>
        /// Sets new parameters of height and width and other necessary parameters by aspect ratio
        /// </summary>
        /// <param name="canvasSettings"></param>
        /// <param name="sizeWidth">New SizeWidth</param>
        /// <param name="sizeHeight">New SizeHeight</param>
        /// <returns></returns>
        public void SetSize(int sizeWidth, int sizeHeight)
        {
            int oldSizeHeight = SizeHeight;
            int oldCanvasHeight = CanvasHeight;
            SizeWidth = sizeWidth;
            SizeHeight = sizeHeight;

            if (sizeWidth == sizeHeight)
            {
                CanvasHeight = CanvasWidth;
            }
            else
            {
                float pixelsInOneSm = oldCanvasHeight / oldSizeHeight;
                CanvasHeight = (int)pixelsInOneSm * SizeHeight;
            }
            PageHeight += CanvasHeight - oldCanvasHeight;

            SizeName += "+";
        }

        private bool IsSizePropertiesInitialized()
        {
            return PageWidth != 0 && PageHeight != 0 && CanvasWidth != 0 && CanvasHeight != 0 && CanvasMarginLeft != null;
        }
    }
}
