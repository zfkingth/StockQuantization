using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Stock.Model
{
    public class StockXRXD
    {
        //code,display_name,name,start_date,end_date,type

        [MaxLength(15)]
        public string Code { get; set; }

        /// <summary>
        /// 除权时间
        /// </summary>
        public DateTime AXrDate { get; set; }


        [MaxLength(10)]
        public string BonusType { get; set; }

        public double DividendRatio { get; set; }
        public double TransferRatio { get; set; }
        public double BonusRatioRmb { get; set; }



    }
}
