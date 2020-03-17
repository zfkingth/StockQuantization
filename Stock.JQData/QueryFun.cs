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
        public static string _token;

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
        public string Get_token()
        {

            object body = new
            {
                method = "get_token",
                mob = "18080802572", //mob是申请JQData时所填写的手机号
                pwd = "dragon00A" //Password为聚宽官网登录密码，新申请用户默认为手机号后6位
            };




            //读取返回的TOKEN
            _token = QueryInfo(body);

            return _token;



        }


        /// <summary>
        /// 从指数30分钟数据里，获取最新的30分钟数据的结束时间。
        /// </summary>
        /// <returns></returns>
        public DateTime GetLastTradeEndDateTime()
        {
            using (StockContext db = new StockContext())
            {
                var query = (from p in db.PriceSet
                             where p.Unit == UnitEnum.Unit30m && p.Code == Constants.IndexsCode[0]
                             orderby p.Date descending
                             select p.Date).FirstOrDefault();
                Constants.LastTradeEndDateTime = query;

                return query;
            }
        }


        public string Get_all_securities(SecuritiesEnum type)
        {
            //查询所有股票代码
            var body = new
            {
                method = "get_all_securities",
                token = _token, //token
                code = Constants.TypeParamDic[type],
                date = DateTime.Now.ToString(Constants.ShortDateFormat)
            };
            string info = QueryInfo(body);

            return info;


        }

        public string Get_price(UnitEnum unit_param, string secCode, int cnt,DateTime endDate)
        {
            var body = new
            {
                method = "get_price",
                token = _token,
                code = secCode,
                count = cnt,
                unit = Constants.UnitParamDic[unit_param],
                end_date = Utility.ToDateString(endDate, unit_param)
            };
            string info = QueryInfo(body);

            return info;

        }

        public int Get_query_count()
        {


            var body = new
            {
                method = "get_query_count",
                token = _token, //token
            };

            var info = QueryInfo(body);
            int num = Convert.ToInt32(info, CultureInfo.InvariantCulture);

            return num;
        }



        protected string QueryInfo(object body)
        {
            var client = SingleClient;
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };


            var content = JsonSerializer.Serialize(body, options);

            StringContent bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            //POST请求并等待结果
            var result = client.PostAsync("apis", bodyContent).Result;


            return result.Content.ReadAsStringAsync().Result;
        }


    }
}
