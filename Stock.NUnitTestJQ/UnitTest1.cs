using NUnit.Framework;

namespace Stock.NUnitTestJQ
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            string token = JQData.Utility.Get_token();

        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}