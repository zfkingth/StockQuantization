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
        public void Update_allStock_price(UnitEnum unit)
        {
            using (StockContext db = new StockContext())
            {
                var secList = db.SecuritiesSet.AsNoTracking().ToList();
                QueryFun qf = new QueryFun();
                foreach (var sec in secList)
                {
                    //获取数据库中最新的时间
                    var query = from p in db.PriceSet
                                where p.Unit == unit && p.Code == sec.Code
                                orderby p.Date descending
                                select p.Date;
                    var startDate = query.FirstOrDefault();
                    if (startDate == default)
                    {
                        startDate = PubConstan.PriceStartDate;
                    }

                    int daysCnt = (int)(Math.Ceiling((DateTime.Now - startDate).TotalDays));//有多出来的数据
                    int numCnt = (int)(daysCnt * PubConstan.RecordCntPerDay[unit]);
                    DateTime endDate = startDate;//初始化


                    //不是每天都有数据，会丢弃很多数据
                    //循环次数
                    var circleCnt = Math.Ceiling((double)numCnt / PubConstan.MaxRecordCntPerFetch);
                    var tempdays = 1.0;
                    if (numCnt < PubConstan.MaxRecordCntPerFetch)
                    {

                        tempdays = numCnt / PubConstan.RecordCntPerDay[unit];
                    }
                    else
                    {
                        tempdays = PubConstan.MaxRecordCntPerFetch / PubConstan.RecordCntPerDay[unit];
                    }
                    for (int fetchIndex = 0; fetchIndex < circleCnt; fetchIndex++)
                    {
                        //分段操作
                        //把次数换成天

                        var lastDate = endDate;

                        endDate = endDate.AddDays(tempdays);

                        int cntForThisFetch = PubConstan.MaxRecordCntPerFetch;

                        if (endDate >= DateTime.Now)
                        {
                            var d1 = Math.Ceiling((DateTime.Now - lastDate).TotalDays);
                            var d2 = PubConstan.RecordCntPerDay[unit];
                            cntForThisFetch = (int)(d1 * d2);
                            endDate = DateTime.Now;

                        }





                        string res = qf.Get_price(unit, sec.Code, cntForThisFetch, endDate);
                        Update_Single_securities_Price(unit, sec.Code, res);

                    }


                }
            }

        }



        public void Update_allStock_basicInfo()
        {


            var qf = new QueryFun();


            string res = qf.Get_all_securities(SecuritiesEnum.Index);
            updateSecuritiesByResult(res);


            //获取数据 
             res = qf.Get_all_securities(SecuritiesEnum.Stock);
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
                        StartDate = DateTime.ParseExact(words[3], PubConstan.ShortDateFormat, CultureInfo.InvariantCulture),
                        EndDate = DateTime.ParseExact(words[4], PubConstan.ShortDateFormat, CultureInfo.InvariantCulture)
                    };
                    switch (words[5])
                    {
                        case "stock": sec.Type = Model.SecuritiesEnum.Stock; break;
                        case "index": sec.Type = Model.SecuritiesEnum.Index; break;
                        default: throw new Exception("未处理这个类型的标的。");
                    }
                    //var item = db.Securities.FirstOrDefault(s => string.Equals(s.Code, sec.Code, StringComparison.CurrentCultureIgnoreCase));
                    var item = db.SecuritiesSet.FirstOrDefault(s => s.Code == sec.Code);
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

        public void Update_Single_securities_Price(UnitEnum unit, string code, string res)
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
                    if (newItem.Date >= PubConstan.PriceStartDate)
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
