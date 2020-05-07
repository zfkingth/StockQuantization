using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyStock.Data;
using MyStock.Model;
using MyStock.WebAPI.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyStock.WebAPI.ViewModels.Fillers
{
    public class PullMarginDataViewModel : BaseDoWorkViewModel
    {
        private readonly ILogger _logger;
        public PullMarginDataViewModel(IServiceScopeFactory serviceScopeFactory,

            ILogger<PullMarginDataViewModel> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
            _eventName = SystemEvents.PullMarginData;
        }



        internal async Task PullAll()
        {
            await setStartDate(_eventName);

            await pullWriteMarginData();

            await setFinishedDate(_eventName);

        }

        private static readonly Lazy<HttpClient> lazyForMargin =
           new Lazy<HttpClient>(
               () =>
               {
                   var handler = new HttpClientHandler
                   {
                       AutomaticDecompression = DecompressionMethods.GZip
                                         | DecompressionMethods.Deflate


                   };
                   var client = new HttpClient(handler);



                   client.BaseAddress = new Uri("http://datacenter.eastmoney.com");


                   client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/javascript, */*;q=0.8");
                   client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.5");
                   client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                   client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko");
                   client.DefaultRequestHeaders.TryAddWithoutValidation("KeepAlive", "true");
                   client.DefaultRequestHeaders.ExpectContinue = true;


                   return client;
               }
               );

        public static HttpClient ClientForMargin { get { return lazyForMargin.Value; } }


        public async Task<Stream> pullMarginData(int page)
        {
            var client = ClientForMargin;


            Stream val = null;
            string request = $"api/data/get?type=RPTA_RZRQ_LSHJ&sty=ALL&source=WEB&st=dim_date&sr=-1&p={page}&ps=50&var=ARwjpefq&rt=52913049";



            HttpResponseMessage response = await client.GetAsync(request);

            if (response.IsSuccessStatusCode)
            {
                val = await response.Content.ReadAsStreamAsync();


            }
            else
            {
                throw new Exception($"从东方财富获取融资融券余额数据错误");
            }

            return val;

        }


        /// <summary>
        /// 从东方财富获取融资融券数据
        /// </summary>
        /// <returns></returns>
        public async Task pullWriteMarginData()
        {

            //获取300次数据
            for (int i = 1; i < 6; i++)
            {
                var val = await pullMarginData(i);
                int changedNum = await Deserialize_WriteMarginDataAsync(val);
                if (changedNum < 10) break;//新增小于10，不读取下一页
            }


        }

        private async Task<int> Deserialize_WriteMarginDataAsync(Stream stream)
        {
            int addedItemNum = 0;

            using (var db = new StockContext())
            {

                using (StreamReader sr = new StreamReader(stream))
                {
                    string str = sr.ReadToEnd();

                    int firstCurlyBraces = str.IndexOf('[');
                    int lastCurlyBraces = str.LastIndexOf(']');

                    var subStr = str[firstCurlyBraces..(lastCurlyBraces + 1)];

                    Regex regex = new Regex(@"{[^{}]+}");

                    var matches = regex.Matches(subStr);

                    for (int m = 0; m < matches.Count; m++)
                    {
                        string ss = matches[m].Value;

                        var item = JsonConvert.DeserializeObject<dynamic>(ss);

                        string dateString = item.DIM_DATE;
                        DateTime DIM_DATE = DateTime.ParseExact(dateString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                        double RZRQYE = item.RZRQYE;

                        var exist = await db.MarginTotal.AnyAsync(s => s.Date == DIM_DATE);
                        if (exist == false)
                        {
                            var margin = new MarginTotal();
                            margin.Date = DIM_DATE;
                            margin.FinValue = RZRQYE;

                            db.MarginTotal.Add(margin);
                            addedItemNum++;
                        }

                    }


                }

                await db.SaveChangesAsync();
            }

            return addedItemNum;





        }
    }
}
