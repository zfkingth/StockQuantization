using Microsoft.EntityFrameworkCore;
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
        public static DateTime GetTokenTime { get; set; }
        public static string _token=null;
        public string MyToken
        {
            get
            {
                if(_token==null)
                {
                    _token = Get_tokenAsync().Result;
                }
                return _token;
            }
        }

        public static HttpClient SingleClient { get { return lazy.Value; } }

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



        /// <summary>
        /// 获取用户token，并保存到类的静态字段中。
        /// </summary>
        /// <returns></returns>
        public async Task<string> Get_tokenAsync()
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


        /// <summary>
        /// 从指数30分钟数据里，获取最新的30分钟数据的结束时间。
        /// </summary>
        /// <returns></returns>
        public async Task<DateTime> GetLastTradeEndDateTimeAsync()
        {
            using (StockContext db = new StockContext())
            {
                var query = (from p in db.PriceSet
                             where p.Unit == UnitEnum.Unit30m && p.Code == Constants.IndexsCode[0]
                             orderby p.Date descending
                             select p.Date).FirstOrDefaultAsync();
                var res = await query;
                Constants.LastTradeEndDateTime = res;
                return res;
            }
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

        public async Task<string> Get_priceAsync(UnitEnum unit_param, string secCode, int cnt, DateTime endDate)
        {
            var body = new
            {
                method = "get_price",
                token = MyToken,
                code = secCode,
                count = cnt,
                unit = Constants.UnitParamDic[unit_param],
                end_date = Utility.ToDateString(endDate, unit_param)
            };
            string info = await QueryInfoAsync(body);

            return info;

        }

        public int Get_query_count()
        {


            var body = new
            {
                method = "get_query_count",
                token = MyToken, //token
            };

            var info = QueryInfoAsync(body);
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

            //POST请求并等待结果
            var result = await client.PostAsync("apis", bodyContent);


            return await result.Content.ReadAsStringAsync();
        }


    }
}
