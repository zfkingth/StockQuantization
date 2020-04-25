using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MyStock.Data;
using MyStock.Model;
using MyStock.WebAPI.Utils;
using Newtonsoft.Json;
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
    public class PullHuShenTongInTradeTimeViewModel : BaseDoWorkViewModel
    {
        private readonly ILogger _logger;
        public PullHuShenTongInTradeTimeViewModel(IServiceScopeFactory serviceScopeFactory,

            ILogger<PullHuShenTongInTradeTimeViewModel> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
        }



        internal async Task PullAll()
        {
            await setStartDate(SystemEvents.PullMarketDealData);

            await pullWriteData();


            await setFinishedDate(SystemEvents.PullMarketDealData);

        }


        private static readonly Lazy<HttpClient> lazyForHuShenTong =
           new Lazy<HttpClient>(
               () =>
               {
                   var handler = new HttpClientHandler
                   {
                       AutomaticDecompression = DecompressionMethods.GZip
                                         | DecompressionMethods.Deflate


                   };
                   var client = new HttpClient(handler);



                   client.BaseAddress = new Uri("http://data.eastmoney.com/");


                   client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html, application/xhtml+xml, image/jxr, */*");
                   client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "zh-Hans-CN,zh-Hans;q=0.8,en-US;q=0.5,en;q=0.3");
                   client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                   client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 10.0; WOW64; Trident/7.0; rv:11.0) like Gecko Core/1.70.3754.400 QQBrowser/10.5.4020.400");
                   client.DefaultRequestHeaders.TryAddWithoutValidation("KeepAlive", "true");
                   client.DefaultRequestHeaders.ExpectContinue = true;


                   return client;
               }
               );

        public static HttpClient ClientForHuShenTong { get { return lazyForHuShenTong.Value; } }


        public async Task<Stream> pullMarketData(MarketType market)
        {
            var client = ClientForHuShenTong;


            Stream val = null;
            string request = "hsgt/index.html";


         

            HttpResponseMessage response = await client.GetAsync(request);

            if (response.IsSuccessStatusCode)
            {
                val = await response.Content.ReadAsStreamAsync();


            }
            else
            {
                throw new Exception($"从东方财富数据中心获取沪股通，深股股通资金流入数据错误");
            }

            return val;

        }


        /// <summary>
        /// 从东方财富获取融资融券数据
        /// </summary>
        /// <returns></returns>
        public async Task pullWriteData()
        {
            System.Array values = System.Enum.GetValues(typeof(MarketType));
            foreach (MarketType market in values)
            {
                var val = await pullMarketData(market);
                await Deserialize_WriteDataAsync(market, val);
            }



        }

        private async Task<int> Deserialize_WriteDataAsync(MarketType market, Stream stream)
        {
            int addedOrChangedItemNum = 0;

            using (var db = new StockContext())
            {

                using (StreamReader reader = new StreamReader(stream, Encoding.GetEncoding("gb2312")))
                {


                    var page = reader.ReadToEnd();
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

                    doc.LoadHtml(page);

                    var query = from i in db.MarketDeal
                                where i.MarketType == market
                                orderby i.Date descending
                                select i;
                    var itemIndb =await query.FirstOrDefaultAsync();

                    var node = doc.DocumentNode.SelectSingleNode("//*[@id=\"zjlx_hgt\"]/td[5]/span");

                    if (node == null)
                    {
                        throw new Exception("东方财富 深沪港通数据解析错误!");
                    }

                }

                await db.SaveChangesAsync();
            }

            return addedOrChangedItemNum;





        }


    }
}
