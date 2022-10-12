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
                Weight = Math.Round(_quantity < 500 ? _quantity / 155f :
                                    _quantity >= 500 && _quantity < 1000 ? _quantity / 165f :
                                    _quantity >= 1000 && _quantity < 2000 ? _quantity / 175f :
                                    _quantity / 185f, 1);
            }
        }

        public double Weight { get; set; }
    }
}
