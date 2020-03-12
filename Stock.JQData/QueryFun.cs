using Stock.Data;
using Stock.Model;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Stock.JQData
{
    public class QueryFun
    {
        public static string _token;

        /// <summary>
        /// 获取用户token，并保存到类的静态字段中。
        /// </summary>
        /// <returns></returns>
        public static string Get_token()
        {
            using (var client = new HttpClient())
            {


                object body = new
                {
                    method = "get_token",
                    mob = "18080802572", //mob是申请JQData时所填写的手机号
                    pwd = "dragon00A" //Password为聚宽官网登录密码，新申请用户默认为手机号后6位
                };




                //读取返回的TOKEN
                _token = QueryInfo(client, body);

                return _token;

            }

        }

        public static string Get_all_securities()
        {
            using (var client = new HttpClient())
            {

                //查询所有股票代码
                var body = new
                {
                    method = "get_all_securities",
                    token = _token, //token
                    code = "stock",
                    date = DateTime.Now.ToString(PubConstan.DateFormatString)
                };
                string info = QueryInfo(client, body);

                return info;
            }

        }

        public static string Get_price(StockContext db, Securities sec)
        {
            using (var client = new HttpClient())
            {


                var body = new
                {
                    method = "get_price",
                    token = _token,
                    code = sec.Code,
                    count = 100,
                    unit = "1d",
                    end_date = DateTime.Now.ToString(PubConstan.DateFormatString)
                };
                string info = QueryInfo(client, body);

                return info;
            }
        }

        public static string Get_security_info()
        {

            throw new Exception();
        }



        protected static string QueryInfo(HttpClient client, object body)
        {

            const string url = "https://dataapi.joinquant.com/apis";
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };


            var content = JsonSerializer.Serialize(body, options);

            StringContent bodyContent = new StringContent(content, Encoding.UTF8, "application/json");

            //POST请求并等待结果
            var result = client.PostAsync(url, bodyContent).Result;


            return result.Content.ReadAsStringAsync().Result;
        }


    }
}
