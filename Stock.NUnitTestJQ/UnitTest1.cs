using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using Stock.Data;

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
            StockContext db = new StockContext(_options);
            JQData.HandleFun.Update_all_securities(db, res);
            Assert.Pass();
        }
    }
}