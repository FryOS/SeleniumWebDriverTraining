﻿using System;
using System.Text;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;
using System.Linq;
using System.Threading;

namespace SeleniumTests
{
    [TestFixture]
    public class TestsLitecart
    {
        private IWebDriver driver;
        private StringBuilder verificationErrors;
        private string baseURL;
        private bool acceptNextAlert = true;
        private WebDriverWait wait;

        [SetUp]
        public void SetupTest()
        {
            driver = new ChromeDriver();
            baseURL = "http://localhost:8080/litecart/";
            verificationErrors = new StringBuilder();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(2);
        }

        [TearDown]
        public void TeardownTest()
        {
            try
            {
                driver.Quit();
            }
            catch (Exception)
            {
                // Ignore errors if unable to close the browser
            }
            Assert.AreEqual("", verificationErrors.ToString());
        }

        [Test]
        public void Test_LoginAdminPart()
        {
            LoginAdminPart();

        }

        [Test]
        public void Test_ClickOnMenuItems()
        {
            LoginAdminPart();

            var uls = driver.FindElement(By.Id("box-apps-menu"));
            var lis = uls.FindElements(By.Id("app-"));

            for (int i = 0; i < lis.Count; i++)
            {
                var liItem = driver.FindElements(By.CssSelector("li#app-"));
                liItem[i].Click();
                Thread.Sleep(1000);

                var h1s = driver.FindElement(By.Id("content")).FindElements(By.TagName("h1"));
                var lenthH1 = h1s.Count;
                Assert.NotZero(lenthH1);

                var ulsSub = driver.FindElements(By.CssSelector("ul.docs > li"));
                for (int y = 0; y < ulsSub.Count; y++)
                {
                    var lisSub = driver.FindElements(By.CssSelector("ul.docs > li"));
                    lisSub[y].Click();
                    var h1sSub = driver.FindElements(By.CssSelector("h1"));
                    Assert.IsTrue(h1sSub.Count > 0);
                }
            }

        }

        [Test]
        public void Test_CheckSticker()
        {
            driver.Navigate().GoToUrl(baseURL);
            Thread.Sleep(1000);
            var lis = driver.FindElements(By.ClassName("product"));
            foreach (var item in lis)
            {
                var stickerCount = item.FindElements(By.CssSelector("a.link>div.image-wrapper>div.sticker")).Count;
                var isStickerPresent = IsElementPresent(By.CssSelector("a.link>div.image-wrapper>div.sticker"), item);
                Assert.IsTrue(isStickerPresent);
                Assert.AreEqual(1, stickerCount);
            }

        }

        [Test]
        public void Test_CheckSortCountries()
        {
            driver.Navigate().GoToUrl(baseURL + "admin");
            driver.FindElement(By.Name("username")).SendKeys("admin");
            driver.FindElement(By.Name("password")).SendKeys("admin");
            driver.FindElement(By.Name("login")).Submit();
            driver.Navigate().GoToUrl("http://localhost:8080/litecart/admin/?app=countries&doc=countries");
            var trRowElements = driver.FindElements(By.CssSelector("tr> td:nth-child(5)>a"));
            List<string> countries = new List<string>();
            List<string> textContents = new List<string>();

            foreach (var trRowElement in trRowElements)
            {
                var textContent = trRowElement.GetAttribute("text");
                countries.Add(textContent);
                textContents.Add(textContent);
            }
            countries.Sort();
            Assert.AreEqual(countries, textContents);
        }

        [Test]
        public void Test_CheckSortCountriesZoneIfZoneEqual_0()
        {
            driver.Navigate().GoToUrl(baseURL + "admin");
            driver.FindElement(By.Name("username")).SendKeys("admin");
            driver.FindElement(By.Name("password")).SendKeys("admin");
            driver.FindElement(By.Name("login")).Submit();
            driver.Navigate().GoToUrl("http://localhost:8080/litecart/admin/?app=countries&doc=countries");
            var trRowElements = driver.FindElements(By.CssSelector("tr> td:nth-child(6)"));
            List<IWebElement> rowNumber = new List<IWebElement>();
            foreach (var trRowElement in trRowElements)
            {
                var textContent = trRowElement.GetAttribute("textContent");
                if (Int32.Parse(textContent) != 0)
                {
                    rowNumber.Add(trRowElement);                    
                }                
            }
            var rowNumberCount = rowNumber.Count;

            foreach (var item in rowNumber)
            {
                
            }

        }

        [Test]
        public void Test_CheckSortCountriesZone()
        {
            driver.Navigate().GoToUrl(baseURL + "admin");
            driver.FindElement(By.Name("username")).SendKeys("admin");
            driver.FindElement(By.Name("password")).SendKeys("admin");
            driver.FindElement(By.Name("login")).Submit();
            driver.Navigate().GoToUrl("http://localhost:8080/litecart/admin/?app=geo_zones&doc=geo_zones");
            var trRowElementsA = driver.FindElements(By.CssSelector("tbody .row > td:nth-child(3) > a"));
            foreach (var item in trRowElementsA)
            {
                item.Click();                
            }
        }


        //б) для тех стран, у которых количество зон отлично от нуля -- 
        //открыть страницу этой страны и там проверить, что зоны расположены в алфавитном порядке

        //2) на странице http://localhost/litecart/admin/?app=geo_zones&doc=geo_zones
        //зайти в каждую из стран и проверить, что зоны расположены в алфавитном порядке

        [Test]
        public void Test_CheckItem()
        {
            driver.Navigate().GoToUrl(baseURL);
            var itemName = driver.FindElement(By.CssSelector("#box-campaigns div.name")).GetAttribute("textContent");
            var itemRegularPrice = driver.FindElement(By.CssSelector("#box-campaigns .regular-price")).GetAttribute("textContent"); 
            var itemCampaignPrice = driver.FindElement(By.CssSelector("#box-campaigns .campaign-price")).GetAttribute("textContent"); 
            var elem = driver.FindElement(By.CssSelector("#box-campaigns a.link"));
            elem.Click();
            var itemNameMain = driver.FindElement(By.CssSelector("h1.title")).GetAttribute("textContent"); ;
            var itemRegularPriceMain = driver.FindElement(By.CssSelector(".price-wrapper .regular-price")).GetAttribute("textContent"); 
            var itemCampaignPriceMain = driver.FindElement(By.CssSelector(".price-wrapper .campaign-price")).GetAttribute("textContent");
            Assert.AreEqual(itemName, itemNameMain);
            Assert.AreEqual(itemRegularPrice, itemRegularPriceMain);
            Assert.AreEqual(itemCampaignPrice, itemCampaignPriceMain);




        }

    //а) на главной странице и на странице товара совпадает текст названия товара
    //б) на главной странице и на странице товара совпадают цены(обычная и акционная)
    //в) обычная цена зачёркнутая и серая(можно считать, что "серый" цвет это такой, у которого в RGBa представлении одинаковые значения для каналов R, G и B)
    //г) акционная жирная и красная(можно считать, что "красный" цвет это такой, у которого в RGBa представлении каналы G и B имеют нулевые значения)
    //(цвета надо проверить на каждой странице независимо, при этом цвета на разных страницах могут не совпадать)
    //д) акционная цена крупнее, чем обычная(это тоже надо проверить на каждой странице независимо)

        public void LoginAdminPart()
        {
            driver.Navigate().GoToUrl(baseURL + "admin");
            driver.FindElement(By.Name("username")).SendKeys("admin");
            driver.FindElement(By.Name("password")).SendKeys("admin");
            driver.FindElement(By.Name("login")).Submit();
        }

        public static Func<IWebDriver, IWebElement> Condition(By locator)
        {
            return (driver) =>
            {
                var element = driver.FindElements(locator).FirstOrDefault();
                return element != null && element.Displayed && element.Enabled ? element : null;
            };
        }
        protected void Click(By locator)
        {
            WaitUntilClickable(locator).Click();
        }


        public void WaitUntilVisible(By locator)
        {
            wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
            wait.Message = "Element with locator '" + locator + "' was not visible in 10 seconds";
            wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementIsVisible(locator));
        }

        public IWebElement WaitUntilClickable(By locator)
        {
            wait = new WebDriverWait(driver, new TimeSpan(0, 0, 10));
            wait.Message = "Element with locator '" + locator + "' was not clickable in 10 seconds";
            return wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(locator));
        }

        private bool IsElementPresent(By by)
        {
            try
            {
                driver.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private bool IsElementPresent(By by, IWebElement element)
        {
            try
            {
                element.FindElement(by);
                return true;
            }
            catch (NoSuchElementException)
            {
                return false;
            }
        }

        private bool IsAlertPresent()
        {
            try
            {
                driver.SwitchTo().Alert();
                return true;
            }
            catch (NoAlertPresentException)
            {
                return false;
            }
        }

        private string CloseAlertAndGetItsText()
        {
            try
            {
                IAlert alert = driver.SwitchTo().Alert();
                string alertText = alert.Text;
                if (acceptNextAlert)
                {
                    alert.Accept();
                }
                else
                {
                    alert.Dismiss();
                }
                return alertText;
            }
            finally
            {
                acceptNextAlert = true;
            }
        }
    }
}
