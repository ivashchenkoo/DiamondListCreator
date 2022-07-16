using System;

namespace DiamondListCreator.Models
{
    public class DiamondColor
    {
        public string Name { get; set; }

        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;
                Weight = Math.Round(_quantity < 1000 ? _quantity / 150f : _quantity / 160f, 1);
            }
        }

        public double Weight { get; set; }
    }
}
