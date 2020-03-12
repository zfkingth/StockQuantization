using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Stock.Data;
using System.Linq;

namespace Stock.NUnitTestJQ
{
    public class Tests
    {
        protected DbContextOptions<StockContext> _options;
        [SetUp]
        public void Setup()
        {
            JQData.QueryFun.Get_token();

            //set context options;


            var optionsBuilder = new DbContextOptionsBuilder<StockContext>();
            optionsBuilder.UseMySql("server=localhost;database=StockQuantization;user=root;password=dragon00");
            _options = optionsBuilder.Options;
        }

        [Test]
        public void Test_Get_all_securities()
        {
            string res = JQData.QueryFun.Get_all_securities();
            using (StockContext db = new StockContext(_options))
            {
                JQData.HandleFun.Update_all_securities(db, res);
            }
            Assert.Pass();
        }
        [Test]
        public void Test_Get_price()
        {
            string res = JQData.QueryFun.Get_all_securities();
            using (StockContext db = new StockContext(_options))
            {
                var sec = db.Securities.AsNoTracking().FirstOrDefault();
                res = JQData.QueryFun.Get_price(db, sec);
            }
            Assert.Pass();
        }

    }
}