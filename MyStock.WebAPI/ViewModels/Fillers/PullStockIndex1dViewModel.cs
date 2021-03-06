﻿using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyStock.Data;
using MyStock.Model;
using MyStock.WebAPI.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyStock.WebAPI.ViewModels.Fillers
{
    public class PullStockIndex1dViewModel : BaseDoWorkViewModel
    {


        private readonly ILogger _logger;
        protected readonly IHttpClientFactory _clientFactory;
        public PullStockIndex1dViewModel(IServiceScopeFactory serviceScopeFactory, IHttpClientFactory clientFactory,
            ILogger<PullStockIndex1dViewModel> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
            base._baseLogger = logger;
            _clientFactory = clientFactory;

            this.stockHandle = DayDataFiller_stockHandle;
            _eventName = SystemEvents.PullStockIndex1d;

        }


        async Task DayDataFiller_stockHandle(BaseDoWorkViewModel.StockArgs e)
        {
            System.Diagnostics.Debug.WriteLine($"******************pull daily data : {e.StockItem.StockId}***************************");
            await FillStockDataFormNetEase(e.StockItem, _startDate);

        }
        protected override List<Stock> GetStockList()
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();


                var list = (from i in db.StockSet

                            select i).AsNoTracking().ToList();
                //list.Add(Constants.IndexBase);
                return list;
            }
        }




        private bool IsNotNullForZhangDirFuInWords(string[] words)
        {
            float? zdf = null;
            float temp;
            if (float.TryParse(words[9], out temp))
            {
                //有可能是None
                zdf = temp;
            }

            return zdf != null ? true : false;
        }


        private static void setField(DayData item, string[] words)
        {
            item.Close = float.Parse(words[3]);
            item.High = float.Parse(words[4]);
            item.Low = float.Parse(words[5]);
            item.Open = float.Parse(words[6]);

            float temp = 0;
            if (float.TryParse(words[9], out temp))
            {
                //有可能是None
                item.ZhangDieFu = temp;
            }



            item.HuanShouLiu = Utility.convertToFloat(words[10]);//网易的指数数据没有换手率

            item.Volume = float.Parse(words[11]);
            item.Amount = float.Parse(words[12]);

            item.ZongShiZhi = Utility.convertToFloat(words[13]);
            item.LiuTongShiZhi = Utility.convertToFloat(words[14]);

            //从excel中获取的历史数据
            item.Permanent = true;


        }

        private async Task<DateTime> WriteStockDataToDb(string stockId, System.IO.Stream stream)
        {
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
            var encoding = Encoding.GetEncoding("GB2312");

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();

                using (StreamReader reader = new StreamReader(stream, encoding))
                {
                    string line;
                    int lineInStream = 0;

                    //真正写入数据库日线数据的最后一次的时间
                    DateTime lastDate = default;
                    while (reader.Peek() >= 0)
                    {
                        line = reader.ReadLine();

                        //  System.Diagnostics.Debug.WriteLine(line);

                        //bypass the first line
                        if (lineInStream != 0)
                        {

                            //write data to db
                            var words = line.Split(',');

                            DateTime date = DateTime.Parse(words[0]);
                            //统一一下，标准的历史数据，小时，分，秒都是0. 方便和实时数据对比
                            date = new DateTime(date.Year, date.Month, date.Day);
                            DayData item = await db.DayDataSet.FirstOrDefaultAsync(s => s.StockId == stockId && s.Date == date);
                            if (item == null)
                            {
                                if (IsNotNullForZhangDirFuInWords(words))
                                {

                                    item = new DayData();
                                    item.StockId = stockId;
                                    item.Date = date;

                                    //插入新的数据
                                    db.DayDataSet.Add(item);
                                }
                            }
                            else
                            {
                                //数据库中存在这个数据
                                if (item.Permanent)
                                {
                                    //已经有完整的历史数据
                                    //不能插入或者更新这个数据
                                    //有可能更新从page得到的数据
                                    item = null;
                                }
                            }

                            //不能重复
                            if (item != null)
                            {

                                setField(item, words);
                                if (item.ZhangDieFu != null)//停牌产生的空数据，有数据源导致
                                {

                                    await db.SaveChangesAsync();
                                    if (date > lastDate)
                                        lastDate = date;
                                }
                            }

                        }
                        lineInStream++;

                    }

                    return lastDate;

                }
            }


        }


        DateTime _startDate;

        /// <summary>
        /// 最新的一个交易日
        /// </summary>
        DateTime _lastTradeDay;

        /// <summary>
        /// 填充最近多少年的股票日线数据
        /// </summary>
        /// <returns></returns>
        public async Task PullAll(int lastYear)
        {
            CurrentThreadNum = 2;
            await setStartDate(_eventName);
            _lastTradeDay = await GetLastTradeDayFromWebPage();

            _startDate = DateTime.Now.AddYears(-lastYear);

            base.DoWork();


            await setFinishedDate(_eventName);

        }




        /// <summary>
        /// 从网易获取历史数据
        /// </summary>
        /// <param name="stockId"></param>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task FillStockDataFormNetEase(Stock stock, DateTime startDate)
        {
            string stockId = stock.StockId;
            DateTime lastDate = await GetLastDate(stockId);
            if (Utils.Utility.IsSameDay(lastDate, _lastTradeDay) == false)
            {
                lastDate = await WriteHistoryDayData(stockId, startDate);

                System.Diagnostics.Debug.WriteLine($"stock {stockId} excel last data {lastDate}");

                //如果这里还是default ，表明长期停牌，或者退市
                //指数没有page data
                if (stock.StockType == StockTypeEnum.Stock && lastDate != default && Utility.IsSameDay(lastDate, _lastTradeDay) == false)
                {
                    //没有从excel中没有取到最新交易日的数据。
                    //需要从当日的网页数据获取
                    lastDate = await WritePageDayData(stockId);
                    System.Diagnostics.Debug.WriteLine($"stock {stockId} page last data {lastDate}");
                }
                //前面两个操作都执行了,而且和交易日是同一天，才会标记
                //if (Utils.Utility.IsSameDay(lastDate, _lastTradeDay))
                //    await UpdateDateFlag(stockId, DateTime.Now);
            }

        }

        private async Task<DateTime> WriteHistoryDayData(string stockId, DateTime startDate)
        {
            DateTime lastDate = default;
            DateTime lastedDate = await GetLastDateForHistoryType(stockId);


            //已经拥有了最新的数据
            if (Utils.Utility.IsSameDay(lastedDate, _lastTradeDay))
                return lastedDate;


            //与数据库中的日期比较，只从最新的日期开始更新数据

            lastedDate = lastedDate.AddDays(1); //会更新从page 中解析的数据，因为只有excel中有市值的数据
                                                //数据库中必须保证数据是连续的
            startDate = startDate > lastedDate ? startDate : lastedDate;
            string requestUri = string.Format("service/chddata.html?code={0}&start={1}&fields=TCLOSE;HIGH;LOW;TOPEN;LCLOSE;CHG;PCHG;TURNOVER;VOTURNOVER;VATURNOVER;TCAP;MCAP",
                stockId, startDate.ToString("yyyyMMdd"));


            string msg = $"{stockId} 获取日线数据时 通讯错误";

            try
            {
                var client = _clientFactory.CreateClient("163.com"); ;
                //历史数据API获取数据
                var response = await client.GetAsync(requestUri);

                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();

                    lastDate = await WriteStockDataToDb(stockId, stream);


                }
                else
                {
                    throw new Exception(msg);
                }
                //System.Diagnostics.Debug.WriteLine("finish:" + stockId + "  time(ms): " + stop.ElapsedMilliseconds);


            }
            catch (Exception ex)
            {
                _logger.LogError(msg);
                System.Diagnostics.Debug.WriteLine(msg);
                throw ex;
            }





            return lastDate;

        }

        private async Task<DateTime> WritePageDayData(string stockId)
        {
            var client = _clientFactory.CreateClient("163.com"); ;

            string requestUriPage = string.Format("trade/lsjysj_{0}.html", stockId.Substring(1));

            string msg = $"{stockId} 获取日线数据时(从网页) 通讯错误";
            //网页的数据
            try
            {
                var response = await client.GetAsync(requestUriPage);
                if (response.IsSuccessStatusCode)
                {
                    var stream = await response.Content.ReadAsStreamAsync();

                    var list = ParsePageData(stockId, stream);

                    return await WritePageDataToDb(stockId, list);

                }
                else
                {
                    throw new Exception(msg + $"status code: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(msg);
                throw ex;
            }

        }

        private async Task<DateTime> WritePageDataToDb(string stockId, IEnumerable<DayData> list)
        {
            var date = await GetLastDate(stockId);

            var qlist = (from i in list
                         where i.Date > date
                         orderby i.Date descending
                         select i).ToList();

            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();


                foreach (var item in qlist)
                {

                    //插入数据
                    db.DayDataSet.Add(item);
                    await db.SaveChangesAsync();
                }
            }


            DateTime rt = default;
            var firstItem = qlist.FirstOrDefault();
            if (firstItem != null)
            {
                rt = firstItem.Date;
            }

            return rt;

        }

        /// <summary>
        /// 获取数据库中最新历史数据的日期(网易excel数据源),不从实时数据表中获取
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns></returns>
        protected async Task<DateTime> GetLastDateForHistoryType(string stockId)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();


                DateTime date = await (from i in db.DayDataSet
                                       where i.StockId == stockId && i.Permanent == true
                                       orderby i.Date descending
                                       select i.Date).FirstOrDefaultAsync();
                //.Take(1).DefaultIfEmpty(DateTime.MinValue).Single();
                return date;

            }
        }

        /// <summary>
        /// 获取数据库中最新历史数据的日期,不从实时数据表中获取
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns></returns>
        protected async Task<DateTime> GetLastDate(string stockId)
        {
            using (var scope = _serviceScopeFactory.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var db = scopedServices.GetRequiredService<StockContext>();


                DateTime date = await (from i in db.DayDataSet
                                       where i.StockId == stockId
                                       orderby i.Date descending
                                       select i.Date).FirstOrDefaultAsync();
                //.Take(1).DefaultIfEmpty(DateTime.MinValue).Single();
                return date;

            }
        }



        private IEnumerable<DayData> ParsePageData(string stockId, Stream stream)
        {


            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {

                var page = reader.ReadToEnd();
                HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

                doc.LoadHtml(page);


                var table = doc.DocumentNode.SelectSingleNode("//table[@class='table_bg001 border_box limit_sale']");


                System.Diagnostics.Debug.WriteLine($"parse page Data {stockId} ");

                var nodes = table.SelectNodes("tr");
                //停牌就找不到table
                if (nodes != null)
                {
                    foreach (HtmlAgilityPack.HtmlNode row in nodes)
                    {
                        ///This is the row.
                        var cells = row.SelectNodes("th|td");

                        string dateString = cells[0].InnerText.Trim();

                        DateTime tempDate;
                        if (DateTime.TryParse(dateString, out tempDate))
                        {
                            DayData item = new DayData();
                            item.Date = tempDate;
                            item.Permanent = false;
                            item.StockId = stockId;

                            float temp = 0;
                            if (float.TryParse(cells[1].InnerText.Trim(), out temp))
                            {
                                //
                                item.Open = temp;
                            }//else use default value which be set in contructor

                            if (float.TryParse(cells[2].InnerText.Trim(), out temp))
                            {
                                //
                                item.High = temp;
                            }//else use default value which be set in contructo

                            if (float.TryParse(cells[3].InnerText.Trim(), out temp))
                            {
                                //
                                item.Low = temp;
                            }//else use default value which be set in contructo

                            if (float.TryParse(cells[4].InnerText.Trim(), out temp))
                            {
                                //
                                item.Close = temp;
                            }//else use default value which be set in contructo

                            if (float.TryParse(cells[6].InnerText.Trim(), out temp))
                            {
                                //
                                item.ZhangDieFu = temp;
                            }//else use default value which be set in contructo

                            if (float.TryParse(cells[7].InnerText.Trim(), out temp))
                            {
                                //
                                item.Volume = temp * 100; //网页中是以手为单位
                            }//else use default value which be set in contructo

                            if (float.TryParse(cells[8].InnerText.Trim(), out temp))
                            {
                                //
                                item.Amount = temp * 10000; //网页中是已万元为单位
                            }//else use default value which be set in contructo



                            if (float.TryParse(cells[10].InnerText.Trim(), out temp))
                            {
                                //
                                item.HuanShouLiu = temp;
                            }//else use default value which be set in contructo

                            item.Permanent = false;

                            yield return item;
                        }

                    }
                }


            }
        }








    }
}
