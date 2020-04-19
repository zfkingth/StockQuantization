using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace MyStock.Model
{
    public class MarginTotal
    {
        [Key]
        public DateTime Date { get; set; }
      

        /// <summary>
        /// 融资余额（元）
        /// /// </summary>
        public double FinValue { get; set; }

          
    }
}
