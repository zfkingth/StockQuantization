using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stock.WebAPI.ViewModels
{
    public class ArgUpwardGap
    {
        public List<string> StockIdList { get; set; } = new List<string>();
        public float GapPercent { get; set; } = 1;
        public float LimitPercent { get; set; } = 7;
        public int NearDaysNum { get; set; } = 2;
        public bool SearchFromAllStocks { get; set; } = false;
        public DateTime BaseDate { get; set; } = DateTime.MaxValue;

    }
}