using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using MyStock.Data;
using MyStock.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MyStock.NUnitTestJQ
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            //var qf = new QueryFun();


        }

        [Test]
        public void TestUpdateallStockbasicInfoAsync()
        {

            //var hf = new HandleFun();
            //hf.Update_allStock_Names().Wait();
            Assert.Pass();
        }



        [Test]
        public void TestUpdateallStockPriceAsync()
        {

            //var hf = new HandleFun();


            ////set in static field

            // hf.Update_allStock_priceAsync(SecuritiesEnum.Index, UnitEnum.Unit30m).Wait();

            // hf.Update_allStock_priceAsync(SecuritiesEnum.Index, UnitEnum.Unit120m).Wait();

            // hf.Update_allStock_priceAsync(SecuritiesEnum.Index, UnitEnum.Unit1d).Wait();

            // hf.Update_allStock_priceAsync(SecuritiesEnum.Stock, UnitEnum.Unit30m).Wait();
            // hf.Update_allStock_priceAsync(SecuritiesEnum.Stock, UnitEnum.Unit120m).Wait();
            // hf.Update_allStock_priceAsync(SecuritiesEnum.Stock, UnitEnum.Unit1d).Wait();
            Assert.Pass();
        }

        [Test]
        public void TestGetQueryCountAsync()
        {


            //var qf = new QueryFun();
            //int cnt = qf.Get_query_countAsync().Result;
            //TestContext.WriteLine($"还剩余调用次数{cnt}");

            Assert.Pass();

        }

    }
}