using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Stock.WebAPI.ViewModels.Fillers
{
    public class PullAllStockNamesViewModel
    {
        internal Task PullAll()
        {
            JQData.HandleFun hf = new JQData.HandleFun();

            var res = hf.Update_allStock_Names();
            return res;

        }
    }
}
