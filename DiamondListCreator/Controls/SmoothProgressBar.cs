using System.Windows.Controls;

namespace DiamondListCreator.Controls
{
    public class SmoothProgressBar : ProgressBar
    {
        /// <summary>
        /// In seconds. Default = 1;
        /// </summary>
        public double AnimationDuration { get; set; } = 1;
    }
}
