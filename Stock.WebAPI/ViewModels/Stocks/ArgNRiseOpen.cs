using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyStock.WebAPI.ViewModels
{
    public class ArgNRiseOpen
    {
        public List<string> StockIdList { get; set; } = new List<string>();
        public bool SearchFromAllStocks { get; set; } = false;
        public DateTime BaseDate { get; set; } = DateTime.MaxValue;

        public bool DownTag { get; set; } = false;
        public int NRiseNum { get; set; } = 3;
        public int NearDaysNum { get; set; } = 3;
  
    }
}