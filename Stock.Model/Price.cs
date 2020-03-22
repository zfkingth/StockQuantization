using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Stock.Model
{
    public class Price
    {
        [MaxLength(15)]
        public string Code { get; set; }
        public UnitEnum Unit { get; set; }


        public DateTime Date { get; set; }
        public double Open { get; set; }
        public double Close { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Volume { get; set; }
        public double Money { get; set; }
        public bool Paused { get; set; }
        public double Highlimit { get; set; }
        public double Lowlimit { get; set; }
        public double Avg { get; set; }
        public double Preclose { get; set; }

    }
}
