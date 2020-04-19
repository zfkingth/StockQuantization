using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyStock.Model
{
    public class MarketDeal
    {
        [Key]
        public DateTime Date { get; set; }
        /// <summary>
        /// 买入成交额(亿元)
        /// </summary>
        public double BuyAmount { get; set; }

        /// <summary>
        /// 卖出成交额(亿元)
        /// </summary>
        public double SellAmount { get; set; }
    }
}
