using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyStock.WebAPI.ViewModels
{
    public class ArgApproach
    {
        public List<string> StockIdList { get; set; } = new List<string>();
        public bool SearchFromAllStocks { get; set; } = false;
        public DateTime BaseDate { get; set; } = DateTime.MaxValue;

        public float XiangchaLv { get; set; } = 2;
        public float HuiTiaoFuDuLow { get; set; } = 15;
        public float HuiTiaoFuDuHigh { get; set; } = 25;
        public int NearDaysNum { get; set; } = 2;
        public int CircleDaysNum { get; set; } = 60;
  
    }
}