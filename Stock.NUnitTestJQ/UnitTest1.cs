using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Stock.Data;
using Stock.JQData;
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


            var optionsBuilder = new DbContextOptionsBuilder<StockContext>();
            optionsBuilder.UseMySql("server=localhost;database=StockQuantization;user=root;password=dragon00");
            _options = optionsBuilder.Options;
        }

        [Test]
        public void Test_Update_allStock_basicInfo()
        {

            using (StockContext db = new StockContext(_options))
            {
                var hf = new HandleFun();
                hf.Update_allStock_basicInfo(db);
            }
            Assert.Pass();
        }
        [Test]
        public void Test_Update_allStock_price1d()
        {
            using (StockContext db = new StockContext(_options))
            {

                var hf = new HandleFun();
                hf.Update_allStock_price1d(db);
            }
            Assert.Pass();
        }

    }
}