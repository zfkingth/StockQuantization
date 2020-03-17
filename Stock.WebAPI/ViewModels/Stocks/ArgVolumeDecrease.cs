using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Stock.WebAPI.ViewModels
{
    public class ArgVolumeDecrease
    {
        public List<string> StockIdList { get; set; } = new List<string>();
        public bool SearchFromAllStocks { get; set; } = false;
        public DateTime BaseDate { get; set; } = DateTime.MaxValue;


        public int NRiseNum { get; set; } = 3;
        public int NearDaysNum { get; set; } = 3;

        public float VolumePercent { get; set; } = 50;
    }
}