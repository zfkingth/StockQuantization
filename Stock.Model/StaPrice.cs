using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyStock.Model
{
   public class StaPrice
    {
        [Key]
        public DateTime Date { get; set; }

        /// <summary>
        /// 涨停个数
        /// </summary>
        public int HighlimitNum { get; set; }

        /// <summary>
        /// 跌停个数
        /// </summary>
        public int LowlimitNum { get; set; }

        /// <summary>
        /// 破板个数
        /// </summary>
        public int FailNum { get; set; }



      }
}
