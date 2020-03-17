using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stock.WebAPI.ViewModels
{
    public class ArgVolumeBreak
    {
        public List<string> StockIdList { get; set; } = new List<string>();
        public bool SearchFromAllStocks { get; set; } = false;
        public DateTime BaseDate { get; set; } = DateTime.MaxValue;

        public float VolTimesNum { get; set; } = 2;
        public float ZhangFu { get; set; } = 4;
        public int NearDaysNum { get; set; } = 1;
        public int CircleDaysNum { get; set; } = 10;
   
    }
}