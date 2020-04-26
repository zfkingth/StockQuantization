using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace MyStock.Model
{
    public partial class DayData
    {


        [MaxLength(10)]
        public string StockId { get; set; }
        public DateTime Date { get; set; }

        public float Open { get; set; }
        public float Low { get; set; }
        public float High { get; set; }
        public float Close { get; set; }
        /// <summary>
        /// 成交量
        /// </summary>
        public float Volume { get; set; }


        /// <summary>
        /// 成交金额
        /// </summary>
        public float Amount { get; set; }

        /// <summary>
        /// 涨跌幅
        /// </summary>
        public float? ZhangDieFu { get; set; }
        /// <summary>
        /// 总市值
        /// </summary>
        public float? ZongShiZhi { get; set; }

        /// <summary>
        /// 流通市值
        /// </summary>
        public float? LiuTongShiZhi { get; set; }

        /// <summary>
        /// 换手率
        /// </summary>
        public float? HuanShouLiu { get; set; }


        public bool Permanent { get; set; } = true;

        /// <summary>
        /// 前一个交易日的收盘价
        /// </summary>
        public float PreClose { get; set; }

        /// <summary>
        /// 是否是涨停
        /// </summary>
        public bool IsZT { get; set; } = false;


        /// <summary>
        /// 是否跌停
        /// </summary>
        public bool IsDT { get; set; } = false;


    }
}
