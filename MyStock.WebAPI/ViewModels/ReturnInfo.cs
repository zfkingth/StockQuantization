using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyStock.WebAPI.ViewModels
{
    public class ReturnInfo
    {
        public ReturnInfo()
        {
            items = new List<StockInfo>();
        }
        public List<StockInfo> items { get; set; }
        public int total { get; set; }
        public int page_total { get; set; }
        public int page { get; set; }
        public int num_per_page { get; set; }
    }


    public class StockInfo
    {
        /// <summary>
        /// 股票代码
        /// </summary>
        public string symbol { get; set; }

        /// <summary>
        /// 股票名称
        /// </summary>
        public string name { get; set; }
    }


}
