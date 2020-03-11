using Stock.Data;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Globalization;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace Stock.JQData
{
    public class HandleFun
    {
        public static void Update_all_securities(StockContext db, string res)
        {
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
                var item = db.Securities.FirstOrDefault(s => s.Code==sec.Code);
                if (item == null)
                {
                    //db.Securities.Add(sec);
                }
                else
                {
                    //mapper.Map(sec, item);
                }

                db.SaveChanges();
                db.Entry(sec).State = EntityState.Detached;
            }

        }
    }
}
