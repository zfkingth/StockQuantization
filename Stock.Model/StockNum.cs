using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStock.Model
{
    public partial class StockNum
    {



        [MaxLength(10)]
        public string StockId { get; set; }
        public DateTime Date { get; set; }

        /// <summary>
        /// 所有股本数量 ，单位为万股
        /// </summary>
        public double All { get; set; }

        /// <summary>
        /// 所有流通A股数量，单位为万股
        /// </summary>
        public double LiuTongA { get; set; }







    }
}
