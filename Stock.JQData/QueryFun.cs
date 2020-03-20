﻿using Microsoft.EntityFrameworkCore;
using Stock.Data;
using Stock.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Stock.JQData
{
    public class QueryFun
    {
        private static string _token = null;
        private static object lockerForToken = new object();
        public string MyToken
        {
            get
            {
                if (_token == null)
                {
                    lock (lockerForToken)
                    {
                        if (_token == null)
                            RefreshTokenAsync().Wait();
                    }
                }
                return _token;
            }
        }

        public async Task RefreshTokenAsync()
        {
            System.Diagnostics.Debug.WriteLine("**************  refresh token start   **************");
            _token = await Get_tokenAsync();
            System.Diagnostics.Debug.WriteLine("**************  refresh token end     **************");
        }

        public static HttpClient SingleClient { get { return lazy.Value; } }

        private static List<DateTime> _allTradeDays = null;
        private static object lockerForDates = new object();
        public List<DateTime> AllTradeDays
        {
            get
            {

                if (_allTradeDays == null)
                {
                    lock (lockerForDates)
                    {
                        if (_allTradeDays == null)
                            RefreshAllTradeDays().Wait();
                    }
                }

                return _allTradeDays;
            }
        }

        private static readonly Lazy<HttpClient> lazy =
    new Lazy<HttpClient>(
        () =>
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip
                                  | DecompressionMethods.Deflate


            };
            var client = new HttpClient(handler);

            client.BaseAddress = new Uri("https://dataapi.joinquant.com");



            client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
            client.DefaultRequestHeaders.TryAddWithoutValidation("KeepAlive", "true");
            client.DefaultRequestHeaders.ExpectContinue = true;


            return client;
        }
        );

        public async Task RefreshAllTradeDays()
        {
            System.Diagnostics.Debug.WriteLine("**************  refresh all trade days start     **************");
            using (StockContext db = new StockContext())
            {
                var list = await (from i in db.TradeDays
                                  orderby i.Date ascending
                                  select i.Date).ToListAsync();
                var lastDate = list.DefaultIfEmpty(Constants.PriceStartDate).LastOrDefault();

                var body = new
                {
                    method = "get_trade_days",
                    token = MyToken,
                    code = Constants.IndexShangHai,
                    date = Utility.ToDateString(lastDate),
                    end_date = Utility.ToDateString(DateTime.Now)
                };
                string info = await QueryInfoAsync(body);

                var lines = info.Split('\n', '\r');
                foreach (var line in lines)
                {
                    DateTime date = Utility.ParseDateString(line, UnitEnum.Unit1d);
                    if (!list.Contains(date))
                    {
                        list.Add(date);
                        var item = new TradeDay();
                        item.Date = date;
                        db.TradeDays.Add(item);
                    }
                }

                await db.SaveChangesAsync();

                _allTradeDays = list;
            }

            System.Diagnostics.Debug.WriteLine("**************  refresh all trade days end   **************");
        }

        internal DateTime AddTradDays(DateTime endDate, double offset)
        {
            int index = AllTradeDays.IndexOf(endDate);
            var date = AllTradeDays[index + (int)offset];
            return date;
        }

        /// <summary>
        /// 两个时间之有多少个交易日
        /// /// </summary>
        /// <param name="startDate"></param>
        /// <param name="uptoDate"></param>
        /// <returns></returns>
        internal int getTradeDaysCntBetween(DateTime startDate, DateTime uptoDate)
        {
            var query = from i in AllTradeDays
                        where i.Date > startDate && i.Date <= uptoDate
                        select i;
            int cnt = query.Count();
            return cnt;
        }

        internal DateTime GetUptoDate()
        {
            return AllTradeDays.LastOrDefault();
        }



        /// <summary>
        /// 获取用户token，并保存到类的静态字段中。
        /// </summary>
        /// <returns></returns>
        private async Task<string> Get_tokenAsync()
        {

            object body = new
            {
                method = "get_token",
                mob = "18080802572", //mob是申请JQData时所填写的手机号
                pwd = "dragon00A" //Password为聚宽官网登录密码，新申请用户默认为手机号后6位
            };




            //读取返回的TOKEN
            var res = await QueryInfoAsync(body);

            return res;



        }



        public async Task<string> Get_all_securitiesAsync(SecuritiesEnum type)
        {
            //查询所有股票代码
            var body = new
            {
                method = "get_all_securities",
                token = MyToken, //token
                code = Constants.TypeParamDic[type],
                date = DateTime.Now.ToString(Constants.ShortDateFormat)
            };
            string info = await QueryInfoAsync(body);

            return info;


        }

        public async Task<string> Get_StockXrXdAsync(string code)
        {
            //查询所有股票代码
            var body = new
            {
                method = "run_query",
                token = MyToken, //token
                table = "finance.STK_XR_XD",
                columns = "code,a_xr_date,bonus_type,dividend_ratio,transfer_ratio,bonus_ratio_rmb",
                conditions = $"code#=#{code}&a_xr_date#>=#{Utility.ToDateString(Constants.PriceStartDate)}",
                count = 1000
            };
            string info = await QueryInfoAsync(body);

            return info;


        }


        public async Task<string> Get_priceAsync(UnitEnum unit_param, string secCode, int cnt, DateTime endDate)
        {
            var body = new
            {
                method = "get_price",
                token = MyToken,
                code = secCode,
                count = cnt,
                unit = Constants.UnitParamDic[unit_param],
                end_date = Utility.ToDateString(endDate)
            };
            string info = await QueryInfoAsync(body);

            return info;

        }

        public async Task<int> Get_query_countAsync()
        {


            var body = new
            {
                method = "get_query_count",
                token = MyToken, //token
            };

            var info = await QueryInfoAsync(body);
            int num = Convert.ToInt32(info, CultureInfo.InvariantCulture);

            return num;
        }



        protected async Task<string> QueryInfoAsync(object body)
        {
            var client = SingleClient;
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };


            var content = JsonSerializer.Serialize(body, options);

            StringContent bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            System.Diagnostics.Debug.WriteLine(content);
            //POST请求并等待结果
            var result = await client.PostAsync("apis", bodyContent);


            return await result.Content.ReadAsStringAsync();
        }


    }
}
