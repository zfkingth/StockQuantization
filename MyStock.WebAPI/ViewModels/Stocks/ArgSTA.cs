using MyStock.WebAPI.ViewModels.Stocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyStock.WebAPI.ViewModels
{
    /// <summary>
    /// 用于统计涨幅的参数
    /// </summary>
    public class ArgSTA : IArg

    {
        public List<string> StockIdList { get; set; } = new List<string>();
        public bool SearchFromAllStocks { get; set; } = false;

        //数据的结束时间
        public DateTime BaseDate { get; set; } = DateTime.MaxValue;

        public List<DateTime> DateList { get; set; } = new List<DateTime>();

    }
}