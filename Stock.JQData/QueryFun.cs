using Stock.Data;
using Stock.Model;
using System;
using System.Collections.Generic;
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

        public string Get_all_securities()
        {
            //查询所有股票代码
            var body = new
            {
                method = "get_all_securities",
                token = _token, //token
                code = "stock",
                date = DateTime.Now.ToString(PubConstan.DateFormatString)
            };
            string info = QueryInfo(body);

            return info;


        }

        public string Get_price(StockContext db, string  secCode, int cnt, UnitEnum unit_param)
        {

            var body = new
            {
                method = "get_price",
                token = _token,
                code = secCode,
                count = cnt,
                unit = PubConstan.UnitParamDic[unit_param],
                end_date = DateTime.Now.ToString(PubConstan.DateFormatString)
            };
            string info = QueryInfo(body);

            return info;

        }

        public string Get_security_info()
        {

            throw new Exception();
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
