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
            List<int> rowNumber = new List<int>();
            int i = 1;
            foreach (var trRowElement in trRowElements)
            {
                var textContent = trRowElement.GetAttribute("textContent");
                if (Int32.Parse(textContent) != 0)
                {
                    rowNumber.Add(i);
                }
                i++;
            }
            var rowNumberCount = rowNumber.Count;
            List<string> countries = new List<string>();
            List<string> textContents = new List<string>();
            foreach (var item in rowNumber)
            {
                driver.FindElement(By.CssSelector("tbody tr:nth-child("+(item+1)+") td:nth-child(5)>a")).Click();
                var trRowElementsZone = driver.FindElements(By.CssSelector("tr>td:nth-child(3)>input[type=hidden]"));
                foreach (var trRowElementsZoneCountry in trRowElementsZone)
                {
                    var textContent = trRowElementsZoneCountry.GetAttribute("value");
                    countries.Add(textContent);
                    textContents.Add(textContent);
                }
                countries.Sort();
                Assert.AreEqual(countries, textContents);
                
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

            for (int i=0; i < trRowElementsA.Count; i++)
            {
                var trRowElementsAs = driver.FindElements(By.CssSelector("tbody .row > td:nth-child(3) > a"));
                trRowElementsAs[i].Click();
                CheckZones();      
                driver.Navigate().GoToUrl("http://localhost:8080/litecart/admin/?app=geo_zones&doc=geo_zones");
            }  
        }

        public void CheckZones() 
        {
            var trRowElementsA = driver.FindElements(By.XPath("//select[contains(@name,'zone_code')]"));
            List<string> selectCountries = new List<string>();
            List<string> selectCountriesSort = new List<string>();

            foreach (var item in trRowElementsA)
            {
                //arrSelect.Add(new SelectElement(item));
                string selectedValue = (new SelectElement(item)).SelectedOption.Text.Trim();
                selectCountries.Add(selectedValue);
                selectCountriesSort.Add(selectedValue);
            }
            selectCountriesSort.Sort();
            Assert.AreEqual(selectCountries, selectCountriesSort);
        }


        [Test]
        public void Test_CheckItem()
        {
            driver.Navigate().GoToUrl(baseURL);
            var itemName = driver.FindElement(By.CssSelector("#box-campaigns div.name"));
            var itemNameContent = itemName.GetAttribute("textContent");
            var itemnameColor = itemName.GetCssValue("color");


            var itemRegularPrice = driver.FindElement(By.CssSelector("#box-campaigns .regular-price"));
            var itemRegularPriceContent = itemRegularPrice.GetAttribute("textContent");
            var itemRegularPriceColor = itemRegularPrice.GetCssValue("color");
            var itemRegularPriceFont = itemRegularPrice.GetCssValue("font-size");
            var itemRegularPriceFontDouble = (float)Convert.ToDouble(itemRegularPriceFont);

            string[] separators = { ",", "(", ")" };
            string[] itemRegularPriceColorRGBA = itemRegularPriceColor.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            var itemRegularPriceColorRGBATrim = MyTrim(itemRegularPriceColorRGBA);            


            var itemCampaignPrice = driver.FindElement(By.CssSelector("#box-campaigns .campaign-price"));
            var itemCampaignPriceContent = itemCampaignPrice.GetAttribute("textContent");
            var itemCampaignPriceColor = itemCampaignPrice.GetCssValue("color");
            var itemCampaignPriceFont = itemCampaignPrice.GetCssValue("font-size");
            var itemCampaignPriceFontDouble = (float)Convert.ToDouble(itemCampaignPriceFont);

            string[] itemCampaignPriceColorRGBA = itemCampaignPriceColor.Split(separators, StringSplitOptions.RemoveEmptyEntries);
            var itemCampaignPriceColorRGBATrim = MyTrim(itemCampaignPriceColorRGBA);

            var elem = driver.FindElement(By.CssSelector("#box-campaigns a.link"));

            elem.Click();

            var itemNameMain = driver.FindElement(By.CssSelector("h1.title"));
            var itemNameMainContent = itemNameMain.GetAttribute("textContent");

            var itemRegularPriceMain = driver.FindElement(By.CssSelector(".price-wrapper .regular-price"));
            var itemRegularPriceMainContent = itemRegularPriceMain.GetAttribute("textContent");
            var itemRegularPriceMainFont = itemRegularPriceMain.GetCssValue("font-size");
            var itemRegularPriceMainFontDouble = (float)Convert.ToDouble(itemRegularPriceMainFont);

            var itemCampaignPriceMain = driver.FindElement(By.CssSelector(".price-wrapper .campaign-price"));
            var itemCampaignPriceMainContent = itemCampaignPriceMain.GetAttribute("textContent");
            var itemCampaignPriceMainFont = itemCampaignPriceMain.GetCssValue("font-size");
            var itemCampaignPriceMainFontDouble = (float)Convert.ToDouble(itemCampaignPriceMainFont);

            Assert.AreEqual(itemNameContent, itemNameMainContent);
            Assert.AreEqual(itemRegularPriceContent, itemRegularPriceMainContent);
            Assert.AreEqual(itemCampaignPriceContent, itemCampaignPriceMainContent);

            Assert.AreEqual(itemRegularPriceColorRGBATrim[1], itemRegularPriceColorRGBATrim[2]);
            Assert.AreEqual(itemRegularPriceColorRGBATrim[2], itemRegularPriceColorRGBATrim[3]);
            Assert.AreEqual(itemRegularPriceColorRGBATrim[1], itemRegularPriceColorRGBATrim[3]);

            Assert.AreEqual(itemCampaignPriceColorRGBATrim[2], itemCampaignPriceColorRGBATrim[3]);
            Assert.AreEqual(itemCampaignPriceColorRGBATrim[2], "0");
            Assert.AreEqual(itemCampaignPriceColorRGBATrim[3], "0");
            
            Assert.Greater(itemCampaignPriceFontDouble, itemRegularPriceFontDouble);
            Assert.Greater(itemCampaignPriceMainFontDouble, itemRegularPriceMainFontDouble);
        }

        [Test]
        public void Test_UserRegistration()
        {
            string myemail = "1236@mail.ru";

            driver.Navigate().GoToUrl(baseURL);
            var newCustomer = driver.FindElement(By.CssSelector("table tbody tr td>a"));
            newCustomer.Click();
            Thread.Sleep(1000);

            var firstname = driver.FindElement(By.CssSelector("input[name=firstname]"));
            firstname.SendKeys("123");

            var lastname = driver.FindElement(By.CssSelector("input[name=lastname]"));
            lastname.SendKeys("123");

            var address1 = driver.FindElement(By.CssSelector("input[name=address1]"));
            address1.SendKeys("Voss");

            var postcode = driver.FindElement(By.CssSelector("input[name=postcode]"));
            postcode.SendKeys("12312");

            var city = driver.FindElement(By.CssSelector("input[name=city]"));
            city.SendKeys("Nov");

            var email = driver.FindElement(By.CssSelector("input[name=email]"));
            email.SendKeys(myemail);

            var phone = driver.FindElement(By.CssSelector("input[name=phone]"));
            phone.SendKeys("123123123");

            var password = driver.FindElement(By.CssSelector("input[name=password]"));
            password.SendKeys("123123");

            var confirmed_password = driver.FindElement(By.CssSelector("input[name=confirmed_password]"));
            confirmed_password.SendKeys("123123");

            SelectElement selectCountry = new SelectElement(driver.FindElement(By.CssSelector("select[name=country_code]")));
            selectCountry.SelectByText("United States");

            SelectElement selectZoneCode = new SelectElement(driver.FindElement(By.CssSelector("select[name=zone_code]")));
            selectZoneCode.SelectByValue("AK");

            driver.FindElement(By.CssSelector("button[type=submit]")).Click();

            Thread.Sleep(500);
            driver.FindElement(By.LinkText("Logout")).Click();
            Thread.Sleep(1000);
            driver.FindElement(By.CssSelector("input[name=email]")).SendKeys(myemail);
            driver.FindElement(By.CssSelector("input[name=password]")).SendKeys("123123");
            
            driver.FindElement(By.CssSelector("button[type=submit]")).Click();
        }


        //1) регистрация новой учётной записи с достаточно уникальным адресом 
        //    электронной почты(чтобы не конфликтовало с ранее созданными пользователями, в том числе при предыдущих запусках того же самого сценария),
        //2) выход(logout), потому что после успешной регистрации автоматически происходит вход,
        //3) повторный вход в только что созданную учётную запись,
        //4) и ещё раз выход.
        //В качестве страны выбирайте United States, штат произвольный. При этом формат индекса -- пять цифр.


        public void SelectFromDropDown(By locator, string text)
        {
            SelectElement select = new SelectElement(driver.FindElement(locator));
            string selectedValue = select.SelectedOption.Text.Trim(); //нужно, если есть пробелы в начале и конце элемента drop-down листа
            if (selectedValue != text) //эта проверка нужна, т.к. мб случай когда уже корректное значение выбрано
            {
                driver.FindElement(locator).Click();
                Assert.True(driver.FindElement(By.XPath("//option[contains(text(), '" + text + "')]")).Displayed, string.Format("Could not find '{0}' in the list", text));
                var list = driver.FindElement(locator);
                var selectElement = new SelectElement(list);
                selectElement.SelectByText(text);
            }
        }


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

        public string[] MyTrim(string[] c)
        {            
            for (int i = 0; i < c.Length; i++)
                c[i] = c[i].Trim();
            return c;
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
