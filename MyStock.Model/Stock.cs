using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Linq;


namespace MyStock.Model
{
    public partial class Stock
    {

        [Key]
        [MaxLength(10)]
        public string StockId { get; set; }

        [MaxLength(10)]

        public string StockName { get; set; }


        public DateTime RealDataUpdated { get; set; }

        public StockTypeEnum StockType { get; set; } = StockTypeEnum.Stock;

        /// <summary>
        /// 上市时间
        /// </summary>
        public DateTime MarketStartDate { get; set; }

        /// <summary>
        /// 分红，送股信息
        /// </summary>
    }
}
