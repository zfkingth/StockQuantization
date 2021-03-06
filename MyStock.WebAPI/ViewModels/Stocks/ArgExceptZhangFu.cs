using MyStock.WebAPI.ViewModels.Stocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyStock.WebAPI.ViewModels
{
    public class ArgExceptZhangFu : IArg

    {
        public List<string> StockIdList { get; set; } = new List<string>();
        public bool SearchFromAllStocks { get; set; } = false;
        public DateTime BaseDate { get; set; } = DateTime.MaxValue;


        public float ZhangFuLow { get; set; } = -10.5f;
        public float ZhangFuHigh { get; set; } = 10.5f;

        public int CircleDaysNum { get; set; } = 1;
    }
}