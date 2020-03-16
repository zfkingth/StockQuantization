using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Stock.Data;
using Stock.JQData;
using Stock.Model;
using System.Linq;

namespace Stock.NUnitTestJQ
{
    public class Tests
    {
        protected DbContextOptions<StockContext> _options;
        [SetUp]
        public void Setup()
        {
            var qf = new QueryFun();
            qf.Get_token();

            //set context options;

        }

        [Test]
        public void Test_Update_allStock_basicInfo()
        {

            var hf = new HandleFun();
            hf.Update_allStock_basicInfo();
            Assert.Pass();
        }
        [Test]
        public void Test_Update_allStock_price()
        {

            var hf = new HandleFun();
            hf.Update_allStock_price(SecuritiesEnum.Stock, UnitEnum.Unit120m);
            hf.Update_allStock_price(SecuritiesEnum.Stock, UnitEnum.Unit30m);
            hf.Update_allStock_price(SecuritiesEnum.Stock, UnitEnum.Unit1d);
            Assert.Pass();
        }

        [Test]
        public void Test_Get_query_count()
        {


            var qf = new QueryFun();
            int cnt = qf.Get_query_count();
            TestContext.WriteLine($"还剩余调用次数{cnt}");

            Assert.Pass();

        }

    }
}