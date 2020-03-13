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
                                where p.Code == sec.Code
                                orderby p.Date descending
                                select p.Date;
                    var date = query.FirstOrDefault();
                    if (date == default)
                    {
                        date = PubConstan.PriceStartDate;
                    }

                    int dayCnt = (int)(DateTime.Now.Subtract(date).TotalDays + 1);//有多出来的数据

                    string res = qf.Get_price(sec.Code, dayCnt, unit);
                    Update_Single_securities_Price(sec.Code, res,unit);
                }
            }

        }


        public void Update_allStock_basicInfo()
        {
            using (StockContext db = new StockContext())
            {
                //获取数据 
                var qf = new QueryFun();
                string res = qf.Get_all_securities();





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
                        StartDate = DateTime.ParseExact(words[3], PubConstan.DateFormatString, CultureInfo.InvariantCulture),
                        EndDate = DateTime.ParseExact(words[4], PubConstan.DateFormatString, CultureInfo.InvariantCulture)
                    };
                    if (words[5] == "stock")
                    {
                        sec.Type = Model.SecuritiesEnum.Stock;
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

        public void Update_Single_securities_Price(string code, string res, UnitEnum unit)
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
                        Unit=unit,
                        Code = code,
                        Date = DateTime.ParseExact(words[0], PubConstan.DateFormatString, CultureInfo.InvariantCulture),
                        Open = Convert.ToDouble(words[1]),
                        Close = Convert.ToDouble(words[2]),
                        High = Convert.ToDouble(words[3]),
                        Low = Convert.ToDouble(words[4]),
                        Volume = Convert.ToDouble(words[5]),
                        Money = Convert.ToDouble(words[6]),
                        Paused = words[7] == "0" ? false : true,
                        Highlimit = Convert.ToDouble(words[8]),
                        Lowlimit = Convert.ToDouble(words[9]),
                        Avg = Convert.ToDouble(words[10]),
                        Preclose = Convert.ToDouble(words[11]),

                    };

                    //只处理系统设置的起始时间以后的数据
                    if (newItem.Date >= PubConstan.PriceStartDate)
                    {
                        //var item = db.Securities.FirstOrDefault(s => string.Equals(s.Code, sec.Code, StringComparison.CurrentCultureIgnoreCase));
                        var exsit = db.PriceSet.Any(s => s.Code == newItem.Code && s.Date == newItem.Date);
                        if (exsit == false)
                        {
                            db.PriceSet.Add(newItem);
                        }
                        else if (i == records.Length - 1)
                        {
                            //已经在数据库中存在，但是最后一次
                            var itemIndb = db.PriceSet.FirstOrDefault(s => s.Code == newItem.Code && s.Date == newItem.Date);
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
