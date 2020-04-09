using Stock.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Stock.Model;
using Stock.JQData;

namespace Stock.CalcOnServer
{
    public class StaFun
    {
        public async Task CalcLimitNum(UnitEnum unit)
        {
            using (StockContext db = new StockContext())
            {
                var query1 = from s in db.StaPrice
                             where s.Unit == unit
                             orderby s.Date descending
                             select s.Date;
                DateTime lastDate = query1.FirstOrDefault();
                var secList = await db.SecuritiesSet.Where(s => s.Type == SecuritiesEnum.Stock).AsNoTracking().ToListAsync();

                QueryFun qf = new QueryFun();

                //重复统计一个，避免数据不全。
                foreach (var date in qf.AllTradeDays.Where(s => s >= lastDate))
                {
                    await statisticAsync(unit, date, secList);
                }




            }


        }

        private async Task statisticAsync(UnitEnum unit, DateTime currentDate, List<Securities> secList)
        {
            using (StockContext db = new StockContext())
            {


                var staItem = await db.StaPrice.FirstOrDefaultAsync(s => s.Unit == unit && s.Date == currentDate);

                if (staItem == null)
                {
                    staItem = new StaPrice()
                    {
                        Unit = unit,
                        Date = currentDate,

                    };

                    db.StaPrice.Add(staItem);
                }

                //初始化，避免重复计数
                staItem.HighlimitNum = 0;
                staItem.LowlimitNum = 0;
                staItem.FailNum = 0;


                //一次查询指定所有股票的的数据
                var query2 = from p in db.PriceSet
                             where p.Unit == UnitEnum.Unit1d && p.Date == currentDate
                             select p;

                var priceList = await query2.AsNoTracking().ToListAsync();


                foreach (var stock in secList)
                {
                    //30个交易日的新股不统计
                    var days30 = new TimeSpan(30, 0, 0, 0);
                    string diplayName = stock.Displayname;
                    if (
                        diplayName.Contains("*")
                        || diplayName.Contains("ST", StringComparison.OrdinalIgnoreCase)
                        || currentDate - stock.StartDate < days30
                        || stock.EndDate < currentDate
                        )
                    {
                        continue;
                    }
                    else
                    {
                        //查询已有的统计
                        //有可能已经存在

                        var price = priceList.FirstOrDefault(s => s.Code == stock.Code);

                        if (price != null)
                        {
                            if (price.Close == price.Highlimit)
                            {
                                staItem.HighlimitNum++;
                            }
                            else if (price.Close == price.Lowlimit)
                            {
                                staItem.LowlimitNum++;
                            }
                            else if (price.High == price.Highlimit && price.Close < price.Highlimit)
                            {
                                staItem.FailNum++;
                            }
                        }


                    }
                }

                //解决出数据

                //如果都是0就不写入

                if (staItem.HighlimitNum + staItem.LowlimitNum + staItem.FailNum > 0)
                {
                    await db.SaveChangesAsync();

                    System.Diagnostics.Debug.WriteLine($"****************  write sta price : {currentDate} end  ***************************");
                }
            }

        }
    }
}
