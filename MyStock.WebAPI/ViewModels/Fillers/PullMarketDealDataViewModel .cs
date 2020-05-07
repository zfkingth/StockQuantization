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
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyStock.WebAPI.ViewModels.Fillers
{
    public class PullMarketDealDataViewModel : BaseDoWorkViewModel
    {
        private readonly ILogger _logger;
        public PullMarketDealDataViewModel(IServiceScopeFactory serviceScopeFactory,

            ILogger<PullMarketDealDataViewModel> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
            _eventName = SystemEvents.PullMarketDealData;
        }



        internal async Task PullAll()
        {
            await setStartDate(_eventName);

            await pullWriteData();


            await setFinishedDate(_eventName);

        }


        private static readonly Lazy<HttpClient> lazyForMarket =
           new Lazy<HttpClient>(
               () =>
               {
                   var handler = new HttpClientHandler
                   {
                       AutomaticDecompression = DecompressionMethods.GZip
                                         | DecompressionMethods.Deflate


                   };
                   var client = new HttpClient(handler);



                   client.BaseAddress = new Uri("http://dcfm.eastmoney.com");


                   client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "application/javascript, */*;q=0.8");
                   client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.5");
                   client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                   client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko");
                   client.DefaultRequestHeaders.TryAddWithoutValidation("KeepAlive", "true");
                   client.DefaultRequestHeaders.ExpectContinue = true;


                   return client;
               }
               );

        public static HttpClient ClientForMarket { get { return lazyForMarket.Value; } }


        public async Task<Stream> pullMarketData(MarketType market, int page)
        {
            var client = ClientForMarket;


            Stream val = null;
            string request = $"EM_MutiSvcExpandInterface/api/js/get?type=HSGTHIS&token=70f12f2f4f091e459a279469fe49eca5&filter=(MarketType={(int)market})&js=var%20ANUtXcyP={{% 22data%22:(x),%22pages%22:(tp)}}&ps=50&p={page}&sr=-1&st=DetailDate&rt=52918531";



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
        public async Task pullWriteData()
        {
            System.Array values = System.Enum.GetValues(typeof(MarketType));
            foreach (MarketType market in values)
            {
                //获取300次数据,page size为50

                for (int i = 1; i < 6; i++)
                {
                    var val = await pullMarketData(market, i);
                    int changed = await Deserialize_WriteDataAsync(market, val);
                    if (changed < 10) break;//如果改变的数据小于10，则不用取下一页
                }

            }



        }

        private async Task<int> Deserialize_WriteDataAsync(MarketType market, Stream stream)
        {
            int addedOrChangedItemNum = 0;

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

                        int type = (int)item.MarketType;

                        DateTime date = item.DetailDate;
                        //dateString = dateString.Replace('T', ' ');
                        //DateTime date = DateTime.ParseExact(dateString, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                        float DRZJLR = item.DRZJLR;

                        var itemInDb = await db.MarketDeal.FirstOrDefaultAsync(s => s.MarketType == market && s.Date == date);
                        if (itemInDb == null)
                        {
                            var newItem = new MarketDeal();
                            newItem.MarketType = market;
                            newItem.Date = date;
                            newItem.DRZJLR = DRZJLR;
                            newItem.Permanent = true;

                            db.MarketDeal.Add(newItem);
                            addedOrChangedItemNum++;
                        }
                        else
                            if (itemInDb.Permanent == false)
                        {
                            itemInDb.DRZJLR = DRZJLR;
                            itemInDb.Permanent = true;
                            addedOrChangedItemNum++;
                        }

                    }


                }

                await db.SaveChangesAsync();
            }

            return addedOrChangedItemNum;





        }


    }
}
