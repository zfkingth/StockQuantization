using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Threading;

namespace MyTestConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.Write("test case started ");
            //create the reference for the browser  
            IWebDriver driver = new ChromeDriver();
            // navigate to URL  
            driver.Navigate().GoToUrl("http://data.eastmoney.com/hsgt/index.html");
            Thread.Sleep(2000);
            //// identify the Google search text box  
            //IWebElement ele = driver.FindElement(By.Name("q"));
            ////enter the value in the google search text box  
            //ele.SendKeys("javatpoint tutorials");
            //Thread.Sleep(2000);
            ////identify the google search button  
            //IWebElement ele1 = driver.FindElement(By.Name("btnK"));
            //// click on the Google search button  
            //ele1.Click();
            Thread.Sleep(3000);

            IWebElement ele = driver.FindElement(By.Id("zjlx_hgt"));
            ele = driver.FindElement(By.XPath("//*[@id=\"zjlx_hgt\"]/td[5]/span"));
            var str = ele.Text;

            //close the browser  
            driver.Close();
            Console.Write("test case ended ");
        }
    }
}
