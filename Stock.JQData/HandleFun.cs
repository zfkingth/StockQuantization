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

        public void Update_allStock_price1d()
        {
            using (StockContext db = new StockContext())
            {
                var secList = db.Securities.AsNoTracking().ToList();
                QueryFun qf = new QueryFun();
                foreach (var sec in secList)
                {
                    //获取数据库中最新的时间
                    var query = from p in db.Price1d
                                where p.Code == sec.Code
                                orderby p.Date descending
                                select p.Date;
                    var date = query.FirstOrDefault();

                    int dayCnt = (int)(DateTime.Now.Subtract(date).TotalDays + 1);

                    string res = qf.Get_price( sec.Code, dayCnt, UnitEnum.Unit_1d);
                    Update_Single_securities_Price1d(sec.Code, res);
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

                var config = new MapperConfiguration(cfg => cfg.CreateMap<Model.Securities, Model.Securities>());
                var mapper = config.CreateMapper();




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
                    var item = db.Securities.FirstOrDefault(s => s.Code == sec.Code);
                    if (item == null)
                    {
                        db.Securities.Add(sec);
                    }
                    else
                    {
                        mapper.Map(sec, item);
                    }

                }
                db.SaveChanges();
            }

        }

        public void Update_Single_securities_Price1d(string code, string res)
        {

            using (StockContext db = new StockContext())
            {
                var records = res.Split('\r', '\n');
                //跳过第一行
                for (int i = 1; i < records.Length; i++)
                {
                    var ss = records[i];
                    var words = ss.Split(',');
                    Stock.Model.Price1d newItem = new Model.Price1d
                    {
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
                    //var item = db.Securities.FirstOrDefault(s => string.Equals(s.Code, sec.Code, StringComparison.CurrentCultureIgnoreCase));
                    var item = db.Price1d.FirstOrDefault(s => s.Code == newItem.Code && s.Date == newItem.Date);
                    if (item == null)
                    {
                        db.Price1d.Add(newItem);
                    }
                }
                db.SaveChanges();
            }

        }



    }
}
