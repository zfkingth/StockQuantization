using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stock.WebAPI.ViewModels
{
    public class ArgTurnOverRate
    {
        public List<string> StockIdList { get; set; } = new List<string>();
        public bool SearchFromAllStocks { get; set; } = false;
        public DateTime BaseDate { get; set; } = DateTime.MaxValue;


        public float TurnOverRateLow { get; set; } = 8f;
        public float TurnOverRateHigh { get; set; } = 100f;
    }
}