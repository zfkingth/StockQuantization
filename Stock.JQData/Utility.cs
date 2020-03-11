using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;


namespace Stock.JQData
{
    public class Utility
    {
        public static string Get_token()
        {
            using (var client = new HttpClient())
            {
                //需要添加System.Web.Extensions
                //生成JSON请求信息



                object body = new
                {
                    method = "get_token",
                    mob = "18080802572", //mob是申请JQData时所填写的手机号
                    pwd = "dragon00A" //Password为聚宽官网登录密码，新申请用户默认为手机号后6位
                };




                //读取返回的TOKEN
                string token = QueryInfo(client, body);

                return token;

            }

        }

        public static string Get_all_securities()
        {

            throw new Exception();
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
