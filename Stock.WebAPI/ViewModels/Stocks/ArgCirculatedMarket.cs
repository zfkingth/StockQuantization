using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stock.WebAPI.ViewModels
{
    public class ArgCirculatedMarket
    {
        public List<string> StockIdList { get; set; } = new List<string>();
        public bool SearchFromAllStocks { get; set; } = false;
        public DateTime BaseDate { get; set; } = DateTime.MaxValue;


        public float MarketLow { get; set; } = 10;
        public float MarketHigh { get; set; } = 100;
    }
}