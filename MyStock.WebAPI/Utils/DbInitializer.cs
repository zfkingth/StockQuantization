using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyStock.Data;
using MyStock.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyStock.WebAPI.Utils
{

    public static class DbInitializer
    {

        public static void MigrateLatest(IApplicationBuilder applicationBuilder)
        {
            var _serviceScopeFactory = applicationBuilder.ApplicationServices.GetService<IServiceScopeFactory>();
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var context = scopedServices.GetRequiredService<StockContext>();


                context.Database.Migrate();

            }
        }
        public static void Seed(IApplicationBuilder applicationBuilder)
        {
            //I'm bombing here
            var _serviceScopeFactory = applicationBuilder.ApplicationServices.GetService<IServiceScopeFactory>();
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                List<string> result = typeof(SystemEvents).GetAllPublicConstantValues<string>();
                foreach (var eventName in result)
                {
                    addEventRecord(db, eventName);
                }
                db.SaveChanges();

            }
        }


        static void addEventRecord(StockContext db, string eventName)
        {

            var exist = db.StockEvents.Any(s => s.EventName == eventName);
            if (!exist)
            {
                StockEvent se = new StockEvent();
                se.EventName = eventName;
                se.LastAriseStartDate = DateTime.MinValue;
                db.StockEvents.Add(se);

            }
        }

        /// <summary>
        /// 清除数据获取事件的所有标志，以便程序在重启后，执行把所有数据重新fetch一遍的操作。
        /// </summary>
        /// <param name="app"></param>
        internal static void ClearDataFetchFlag(IApplicationBuilder app)
        {
            var _serviceScopeFactory = app.ApplicationServices.GetService<IServiceScopeFactory>();
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();
                System.Diagnostics.Debug.WriteLine("clear data fetch flags");

                var list = db.StockEvents.ToList();
                foreach (var item in list)
                {
                    item.LastAriseEndDate = null;

                }

                db.SaveChanges();
            }

        }

        internal static void CheckIfClearFlags(IApplicationBuilder app)
        {
            var _configuration = app.ApplicationServices.GetService<IConfiguration>();

            bool refetch = _configuration.GetValue<bool>("RefetchAllDataWhenStart");
            if (refetch)
            {
                ClearDataFetchFlag(app);
            }
        }
    }

}
