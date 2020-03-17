using Stock.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Globalization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using Stock.Model;

namespace Stock.JQData
{
    public class HandleFun
    {
        static IMapper _mapper = null;

        static HandleFun()
        {
            var mappingConfig = new MapperConfiguration(mc => mc.AddProfile(new MappingProfile()));
            _mapper = mappingConfig.CreateMapper();
        }
        public async Task Update_allStock_priceAsync(SecuritiesEnum secType, UnitEnum unit)
        {
            using (StockContext db = new StockContext())
            {
                var secList = (from s in db.SecuritiesSet
                               where s.Type == secType
                               select s.Code).ToList();

                foreach (var secCode in secList)
                {

                 await   Update_PriceAsync(unit, secCode);
                }
            }

        }

        /// <summary>
        /// 更新主板，上证指数30m的数据
        /// </summary>
        public async Task UpdateMainIndexAsync()
        {
         await   Update_PriceAsync(UnitEnum.Unit30m, Constants.IndexsCode[0],true);
        }

        private async Task Update_PriceAsync(UnitEnum unit, string secCode, bool forceUpdate = false)
        {
            using (StockContext db = new StockContext())
            {

                QueryFun qf = new QueryFun();
                //获取数据库中最新的时间
                var query = from p in db.PriceSet
                            where p.Unit == unit && p.Code == secCode
                            orderby p.Date descending
                            select p.Date;
                var startDate = query.FirstOrDefault();
                if (startDate == default)
                {
                    startDate = Constants.PriceStartDate;
                }

                //日线数据的小时，分钟都是0
                if (unit == UnitEnum.Unit1d)
                {
                    startDate = startDate.Add(Constants.MarketEndTime);
                }

                if (forceUpdate || startDate < Constants.LastTradeEndDateTime)
                {

                    int daysCnt = (int)(Math.Ceiling((DateTime.Now - startDate).TotalDays));//有多出来的数据
                    int numCnt = (int)(daysCnt * Constants.RecordCntPerDay[unit]);
                    DateTime endDate = startDate;//初始化


                    //不是每天都有数据，会丢弃很多数据
                    //循环次数
                    var circleCnt = Math.Ceiling((double)numCnt / Constants.MaxRecordCntPerFetch);
                    var tempdays = 1.0;
                    if (numCnt < Constants.MaxRecordCntPerFetch)
                    {

                        tempdays = numCnt / Constants.RecordCntPerDay[unit];
                    }
                    else
                    {
                        tempdays = Constants.MaxRecordCntPerFetch / Constants.RecordCntPerDay[unit];
                    }
                    for (int fetchIndex = 0; fetchIndex < circleCnt; fetchIndex++)
                    {
                        //分段操作
                        //把次数换成天

                        var lastDate = endDate;

                        endDate = endDate.AddDays(tempdays);

                        int cntForThisFetch = Constants.MaxRecordCntPerFetch;

                        if (endDate >= DateTime.Now)
                        {
                            var d1 = Math.Ceiling((DateTime.Now - lastDate).TotalDays);
                            var d2 = Constants.RecordCntPerDay[unit];
                            cntForThisFetch = (int)(d1 * d2);
                            endDate = DateTime.Now;

                        }





                        string res =await qf.Get_priceAsync(unit, secCode, cntForThisFetch, endDate);
                        Parse_WriteDb_Price(unit, secCode, res);

                    }
                }
            }


        }

        public async Task Update_allStock_Names()
        {


            var qf = new QueryFun();


            string res =await qf.Get_all_securitiesAsync(SecuritiesEnum.Index);
            updateSecuritiesByResult(res);


            //获取数据 
            res =await qf.Get_all_securitiesAsync(SecuritiesEnum.Stock);
            updateSecuritiesByResult(res);

        }

        private void updateSecuritiesByResult(string res)
        {
            using (StockContext db = new StockContext())
            {

                var records = res.Split('\r', '\n');
                //跳过第一行
                for (int i = 1; i < records.Length; i++)
                {
                    var ss = records[i];
                    var words = ss.Split(',');
                    Stock.Model.Securities sec = new Model.Securities
                    {
                        Code = words[0],
                        Displayname = words[1],
                        Name = words[2],
                        StartDate = DateTime.ParseExact(words[3], Constants.ShortDateFormat, CultureInfo.InvariantCulture),
                        EndDate = DateTime.ParseExact(words[4], Constants.ShortDateFormat, CultureInfo.InvariantCulture)
                    };
                    switch (words[5])
                    {
                        case "stock": sec.Type = Model.SecuritiesEnum.Stock; break;
                        case "index": sec.Type = Model.SecuritiesEnum.Index; break;
                        default: throw new Exception("未处理这个类型的标的。");
                    }
                    //var item = db.Securities.FirstOrDefault(s => string.Equals(s.Code, sec.Code, StringComparison.CurrentCultureIgnoreCase));
                    var item = db.SecuritiesSet.FirstOrDefault(s => s.Code == sec.Code); ;

                    //只处理三种指数
                    if (sec.Type == SecuritiesEnum.Index)
                    {
                        if (!Constants.IndexsCode.Contains(sec.Code))
                        {
                            continue;
                        }
                    }

                    if (item == null)
                    {
                        db.SecuritiesSet.Add(sec);
                    }
                    else
                    {
                        _mapper.Map(sec, item);
                    }

                }
                db.SaveChanges();
            }
        }

        public void Parse_WriteDb_Price(UnitEnum unit, string code, string res)
        {

            using (StockContext db = new StockContext())
            {
                var records = res.Split('\r', '\n');
                //跳过第一行
                for (int i = 1; i < records.Length; i++)
                {
                    var ss = records[i];
                    var words = ss.Split(',');
                    Stock.Model.Price newItem = new Model.Price
                    {
                        Unit = unit,
                        Code = code,
                        Date = Utility.ParseDateString(words[0], unit),
                        Open = Convert.ToDouble(words[1]),
                        Close = Convert.ToDouble(words[2]),
                        High = Convert.ToDouble(words[3]),
                        Low = Convert.ToDouble(words[4]),
                        Volume = Convert.ToDouble(words[5]),
                        Money = Convert.ToDouble(words[6]),


                    };
                    if (unit == UnitEnum.Unit1d)
                    {
                        //目前,只有日线数据有这个
                        newItem.Paused = words[7] == "0" ? false : true;
                        newItem.Highlimit = Convert.ToDouble(words[8]);
                        newItem.Lowlimit = Convert.ToDouble(words[9]);
                        newItem.Avg = Convert.ToDouble(words[10]);
                        newItem.Preclose = Convert.ToDouble(words[11]);
                    }

                    //只处理系统设置的起始时间以后的数据
                    if (newItem.Date >= Constants.PriceStartDate)
                    {
                        //var item = db.Securities.FirstOrDefault(s => string.Equals(s.Code, sec.Code, StringComparison.CurrentCultureIgnoreCase));
                        var exsit = db.PriceSet.Any(s => s.Unit == unit && s.Code == newItem.Code && s.Date == newItem.Date);
                        if (exsit == false)
                        {
                            db.PriceSet.Add(newItem);
                        }
                        else if (i == records.Length - 1)
                        {
                            //已经在数据库中存在，但是最后一次
                            var itemIndb = db.PriceSet.FirstOrDefault(s => s.Unit == unit && s.Code == newItem.Code && s.Date == newItem.Date);
                            //如果是最后一次就更新。
                            _mapper.Map(newItem, itemIndb);
                        }
                    }
                }
                db.SaveChanges();
            }

        }



    }
}
