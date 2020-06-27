using MyStock.WebAPI.ViewModels.Stocks;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MyStock.WebAPI.ViewModels
{
    public class ArgUpMATwice : IArg

    {
        public List<string> StockIdList { get; set; } = new List<string>();
        public bool SearchFromAllStocks { get; set; } = false;
        public DateTime BaseDate { get; set; } = DateTime.MaxValue;


        public int AvgDays { get; set; } = 60;

        public int RecentDaysNum { get; set; } = 1;

        /// <summary>
        /// 第一次突破之前最少多少天处于均线之下
        /// </summary>
        public int MinDaysNumDownAvgBeforeFirst { get; set; } = 20;
        public int MaxDaysNumUpAvgBeforeFirst { get; set; } = 0;

        /// <summary>
        /// 第一次突破后，在均线上的最大持续天数
        /// </summary>
        public int MaxDaysNumUpAvgAfterFirst { get; set; } = 8;

        /// <summary>
        /// 第二次突破时，在均线下，最长多少天
        /// </summary>
        public int MaxDaysNumDownAvgBeforeTwice{ get; set; } = 3;


    }
}