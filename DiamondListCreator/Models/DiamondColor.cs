using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DiamondListCreator.Services;

namespace DiamondListCreator.Models
{
    public class DiamondColor
    {
        public static List<Threshold> Thresholds { get; set; } = JsonIOService.Read<List<Threshold>>(Path.Combine(Environment.CurrentDirectory, "Config", "weights_thresholds.json"));

        public string Name { get; set; }

        private int _quantity;
        public int Quantity
        {
            get { return _quantity; }
            set
            {
                _quantity = value;

                if (Thresholds != null && Thresholds.Any())
                {
                    if (Thresholds.FirstOrDefault(t => _quantity >= t.Min && _quantity < (t.Max == "Infinity" ? float.MaxValue : float.Parse(t.Max)))
                        is Threshold threshold)
                    {
                        Weight = Math.Round(_quantity / threshold.Divider, 1);
                    }
                    else
                    {
                        throw new Exception("Не знайдено відповідного порогу ваги!");
                    }
                }
                else
                {
                    throw new Exception("Список порогів ваги пустий!");
                }
            }
        }

        public double Weight { get; set; }
    }
}
