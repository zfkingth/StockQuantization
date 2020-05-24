using MyStock.WebAPI.ViewModels.Stocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyStock.WebAPI.ViewModels
{
    public class ArgUpMACD : IArg

    {
        public List<string> StockIdList { get; set; } = new List<string>();
        public bool SearchFromAllStocks { get; set; } = false;
        public DateTime BaseDate { get; set; } = DateTime.MaxValue;


        public int AvgDays { get; set; } = 60;

        public int UpDaysNum { get; set; } = 1;


        public int NearDaysNum { get; set; } = 30;
        public int ExceptionNum { get; set; } = 0;
        public float LowDiff { get; set; } = 0;
        public float LowDea { get; set; } = 0;
        public float PreLowDiff { get; set; } = 0;
        public float PreLowDea { get; set; } = 0;

    }
}