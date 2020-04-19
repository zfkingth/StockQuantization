using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Linq;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyStock.Model
{
    public class Sharing
    {
        public Sharing()
        {
            SongGu = 0;
            ZhuanZeng = 0;
            PaiXi = 0;
        }




        [MaxLength(10)]
        public string StockId { get; set; }

        public DateTime DateGongGao { get; set; }
        public DateTime? DateChuXi { get; set; }
        public DateTime? DateDengJi { get; set; }
        public float SongGu { get; set; }
        public float ZhuanZeng { get; set; }
        public float PaiXi { get; set; }


    }
}
