using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Stock.Model
{
    public class MarketDeal
    {
        public DateTime Day { get; set; }
        /// <summary>
        /// 市场通编码
        /// /// </summary>
        public int LinkId { get; set; }

        [MaxLength(16)]
        public string LinkName { get; set; }

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
