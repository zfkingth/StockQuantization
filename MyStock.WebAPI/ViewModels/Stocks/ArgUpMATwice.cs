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
        /// ��һ��ͻ��֮ǰ���ٶ����촦�ھ���֮��
        /// </summary>
        public int MinDaysNumDownAvgBeforeFirst { get; set; } = 20;
        public int MaxDaysNumUpAvgBeforeFirst { get; set; } = 0;

        /// <summary>
        /// ��һ��ͻ�ƺ��ھ����ϵ�����������
        /// </summary>
        public int MaxDaysNumUpAvgAfterFirst { get; set; } = 8;

        /// <summary>
        /// �ڶ���ͻ��ʱ���ھ����£��������
        /// </summary>
        public int MaxDaysNumDownAvgBeforeTwice{ get; set; } = 3;


    }
}