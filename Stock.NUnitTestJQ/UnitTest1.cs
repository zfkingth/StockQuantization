using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Stock.Data;
using Stock.JQData;
using Stock.Model;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Stock.NUnitTestJQ
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            var qf = new QueryFun();

            //set context options;

        }

        [Test]
        public void TestUpdateallStockbasicInfoAsync()
        {

            var hf = new HandleFun();
            hf.Update_allStock_Names().Wait();
            Assert.Pass();
        }



        [Test]
        public void TestUpdateallStockPriceAsync()
        {

            var hf = new HandleFun();

            var qf = new QueryFun();

             hf.UpdateMainIndexAsync().Wait();

            //set in static field
            DateTime date =  qf.GetLastTradeEndDateTimeAsync().Result;

             hf.Update_allStock_priceAsync(SecuritiesEnum.Index, UnitEnum.Unit30m).Wait();

             hf.Update_allStock_priceAsync(SecuritiesEnum.Index, UnitEnum.Unit120m).Wait();

             hf.Update_allStock_priceAsync(SecuritiesEnum.Index, UnitEnum.Unit1d).Wait();

             hf.Update_allStock_priceAsync(SecuritiesEnum.Stock, UnitEnum.Unit30m).Wait();
             hf.Update_allStock_priceAsync(SecuritiesEnum.Stock, UnitEnum.Unit120m).Wait();
             hf.Update_allStock_priceAsync(SecuritiesEnum.Stock, UnitEnum.Unit1d).Wait();
            Assert.Pass();
        }

        [Test]
        public void TestGetQueryCount()
        {


            var qf = new QueryFun();
            int cnt = qf.Get_query_countAsync();
            TestContext.WriteLine($"还剩余调用次数{cnt}");

            Assert.Pass();

        }

    }
}