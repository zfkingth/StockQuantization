using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Stock.Model
{
    public class TradeDay
    {


        [Key]
        public DateTime Date { get; set; }
    }
}
