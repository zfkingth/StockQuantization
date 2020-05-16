using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyStock.WebAPI.ViewModels.Stocks
{
    public interface IArg
    {
        public bool SearchFromAllStocks { get; set; }
        public List<string> StockIdList { get; set; }

    }
}
