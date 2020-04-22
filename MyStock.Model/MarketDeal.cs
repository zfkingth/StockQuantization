using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyStock.Model
{
    public class MarketDeal
    {
        public MarketType MarketType { get; set; }
        public DateTime Date { get; set; }

        /// <summary>
        /// 当日净流入，单位million
        /// </summary>
        public float DRZJLR { get; set; }

        public bool Permanent { get; set; } = true;

    }
}
