using Microsoft.EntityFrameworkCore;
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
    public class PullStockF10ViewModel : BaseDoWorkViewModel
    {


        private readonly ILogger _logger;
        public PullStockF10ViewModel(IServiceScopeFactory serviceScopeFactory,

            ILogger<PullStockIndex1dViewModel> logger) : base(serviceScopeFactory)
        {
            _logger = logger;
            this.stockHandle = F10Filler_stockHandle;

        }


        async Task F10Filler_stockHandle(BaseDoWorkViewModel.StockArgs e)
        {

            System.Diagnostics.Debug.WriteLine($"pull f10 data for stock id: {e.Stock.StockId}");
            await FillStockF10_FHPG_FormNetEase(e.Stock.StockId);

        }



        public async Task PullAll()
        {
            await setStartDate(SystemEvents.PullStockF10);


            base.DoWork();


            await setFinishedDate(SystemEvents.PullStockF10);

        }





        private static readonly Lazy<HttpClient> lazyForF10 =
        new Lazy<HttpClient>(
            () =>
            {
                var handler = new HttpClientHandler
                {
                    AutomaticDecompression = DecompressionMethods.GZip
                                      | DecompressionMethods.Deflate


                };
                var client = new HttpClient(handler);

                client.BaseAddress = new Uri("http://quotes.money.163.com");


                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", "text/html, application/xhtml+xml, */*");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Language", "en-US,en;q=0.5");
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", "gzip, deflate");
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (Windows NT 6.3; WOW64; Trident/7.0; rv:11.0) like Gecko");


                client.DefaultRequestHeaders.TryAddWithoutValidation("KeepAlive", "true");
                client.DefaultRequestHeaders.ExpectContinue = true;


                return client;
            }
            );
        public static HttpClient ClientForF10 { get { return lazyForF10.Value; } }


        /// <summary>
        /// 从网易获取分红配股信息
        /// </summary>
        /// <param name="stockId"></param>
        /// <returns></returns>
        public async Task FillStockF10_FHPG_FormNetEase(string stockId)
        {



            var client = ClientForF10;




            string requestUri = string.Format("f10/fhpg_{0}.html ", stockId.Substring(1));




            HttpResponseMessage response = await client.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                var stream = await response.Content.ReadAsStreamAsync();

                await WriteStockStockF10_FHPG_ToDb(stockId, stream);
            }
            else
            {
                throw new Exception("通讯错误");
            }











        }

        private async Task WriteStockNumInfo_ToDb(string stockId, Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<StockContext>();


                    var page = reader.ReadToEnd();
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

                    doc.LoadHtml(page);

                    //写入到数据库中
                    //如果存在就更新，不存在就插入
                    var query = from i in db.StockNumSet
                                where i.StockId == stockId
                                select i;

                    var collection = await query.ToListAsync();


                    var table = doc.DocumentNode.SelectSingleNode("//table/tr[1]/th[1][contains(text(),'股份构成(万股)')]/ancestor::table");

                    if (table == null)
                    {
                        throw new Exception("东方财富数据解析错误!" + stockId);
                    }

                    var rows = table.SelectNodes("tr");

                    ///This is the row.
                    var row0cells = rows[0].SelectNodes("th|td");
                    var row1cells = rows[1].SelectNodes("th|td");
                    var row2cells = rows[2].SelectNodes("th|td");

                    if (row1cells[0].InnerText.Trim() != "股份总数")
                    {

                        throw new Exception("东方财富数据解析错误(股份总数的行不正确)!" + stockId);
                    }

                    if (row2cells[0].InnerText.Trim() != "已上市流通A股")
                    {

                        throw new Exception("东方财富数据解析错误(已上市流通A股)!" + stockId);
                    }

                    for (int i = 1; i < row0cells.Count; i++)
                    {
                        string dateString = row0cells[i].InnerText.Trim();

                        DateTime tempDate;
                        if (DateTime.TryParse(dateString, out tempDate))
                        {
                            //有可能存在暂无数据的情况
                            ///This the cell.
                            StockNum item = collection.FirstOrDefault(s => s.Date == tempDate);

                            if (item == null)
                            {
                                //不存在就新增，存在就更新 
                                item = new StockNum();


                                item.StockId = stockId;

                                item.Date = tempDate;
                                db.StockNumSet.Add(item);
                            }

                            double temp = 0;
                            if (double.TryParse(row1cells[i].InnerText.Trim(), out temp))
                            {

                                item.All = temp;
                            }
                            else
                            {

                                throw new Exception("东方财富数据解析错误(总股数)!" + stockId);
                            }

                            if (double.TryParse(row2cells[i].InnerText.Trim(), out temp))
                            {

                                item.LiuTongA = temp;
                            }
                            else
                            {
                                item.LiuTongA = 0;
                                // throw new Exception("东方财富数据解析错误(已上市流通A股)!"+stockId);
                            }

                        }
                    }


                    await db.SaveChangesAsync();

                }
            }
        }



        private async Task WriteStockStockF10_FHPG_ToDb(string stockId, Stream stream)
        {
            using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<StockContext>();


                    var page = reader.ReadToEnd();
                    HtmlAgilityPack.HtmlDocument doc = new HtmlAgilityPack.HtmlDocument();

                    doc.LoadHtml(page);

                    //写入到数据库中
                    //如果存在就更新，不存在就插入
                    var query = from i in db.SharingSet
                                where i.StockId == stockId
                                select i;

                    var collection = await query.ToListAsync();


                    var table = doc.DocumentNode.SelectSingleNode("//table[@class='table_bg001 border_box limit_sale']");


                    foreach (HtmlAgilityPack.HtmlNode row in table.SelectNodes("tr"))
                    {
                        ///This is the row.
                        var cells = row.SelectNodes("th|td");

                        string dateString = cells[0].InnerText.Trim();

                        DateTime tempDate;
                        if (DateTime.TryParse(dateString, out tempDate))
                        {
                            //有可能存在暂无数据的情况
                            ///This the cell.
                            Sharing sharing = collection.FirstOrDefault(s => s.DateGongGao == tempDate);

                            if (sharing == null)
                            {
                                //不存在就新增，存在就更新 
                                sharing = new Sharing();


                                sharing.StockId = stockId;

                                sharing.DateGongGao = tempDate;
                                db.SharingSet.Add(sharing);
                            }

                            float temp = 0;
                            if (float.TryParse(cells[2].InnerText.Trim(), out temp))
                            {
                                //
                                sharing.SongGu = temp;
                            }//else use default value which be set in contructor

                            if (float.TryParse(cells[3].InnerText.Trim(), out temp))
                            {
                                //
                                sharing.ZhuanZeng = temp;
                            }//else use default value which be set in contructo

                            if (float.TryParse(cells[4].InnerText.Trim(), out temp))
                            {
                                //
                                sharing.PaiXi = temp;
                            }//else use default value which be set in contructo

                            dateString = cells[5].InnerText.Trim();
                            if (DateTime.TryParse(dateString, out tempDate))
                                sharing.DateDengJi = tempDate;//日期可能为空


                            dateString = cells[6].InnerText.Trim();
                            if (DateTime.TryParse(dateString, out tempDate))
                                sharing.DateChuXi = tempDate;//日期可能为空

                        }

                    }
                    await db.SaveChangesAsync();
                }

            }
        }





    }
}
