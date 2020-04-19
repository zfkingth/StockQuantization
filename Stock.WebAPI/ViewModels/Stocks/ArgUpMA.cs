using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyStock.WebAPI.ViewModels
{
    public class ArgUpMA
    {
        public List<string> StockIdList { get; set; } = new List<string>();
        public bool SearchFromAllStocks { get; set; } = false;
        public DateTime BaseDate { get; set; } = DateTime.MaxValue;


        public int ExceptionNum { get; set; } = 0;
        public int AvgDays { get; set; } = 60;

        public int UpDaysNum { get; set; } = 1;

        public int NearDaysNum { get; set; } = 30;


    }
}