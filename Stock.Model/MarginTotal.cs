using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Stock.Model
{
    public class MarginTotal
    {
        public DateTime Date { get; set; }
        /// <summary>
        /// 交易市场
        /// </summary>
        [MaxLength(12)]
        public string ExchangeCode { get; set; }


        /// <summary>
        /// 融资余额（元）
        /// /// </summary>
        public double FinValue { get; set; }

        /// <summary>
        /// 融资买入额（元）
        /// </summary>
        public double FinBuyValue { get; set; }

        /// <summary>
        /// 融券余量（股）
        /// </summary>
        public int SecVolume { get; set; }

        /// <summary>
        /// 融券余量金额（元）
        /// </summary>
        public double SecValue { get; set; }


        /// <summary>
        /// 融券卖出量（股）
        /// </summary>
        public int SecSellVolume { get; set; }


        /// <summary>
        /// 融资融券余额（元）
        /// </summary>
        public double FinSecValue { get; set; }

    }
}
