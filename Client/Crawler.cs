using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Client
{
    class Crawler
    {
        ChromeDriver driver;


        public List<Product> GetData_Ishopping(string Input)
        {
            driver = new ChromeDriver();
            List<Product> info = new List<Product>();
            driver.Navigate().GoToUrl("https://www.ishopping.pk/");
            var searchBar = driver.FindElementByXPath("//input[@id = 'search']");
            searchBar.SendKeys(Input);
            var searchBtn = driver.FindElementByXPath("//button[@type = 'submit'][@title = 'Search']");
            searchBtn.Click();

            Thread.Sleep(5000);
            IList<IWebElement> desc_list = driver.FindElements(By.XPath("//div[@class= 'kuNameDesc']/div[@class ='kuName']"));
            IList<IWebElement> price_list = driver.FindElements(By.XPath("//div[@class ='kuPrice']/div[@class = 'kuSalePrice']"));
            if (desc_list.Count > 0 && price_list.Count > 0)
            {
                for (int i = 0; i < desc_list.Count; i++)
                {

                    if (desc_list[i] != null && price_list[i] != null)
                    {
                        string desc = desc_list[i].Text;
                        string price = price_list[i].Text;
                        price = price.Substring(3).Replace(",", String.Empty);

                        info.Add(new Product()
                        {
                            price = int.Parse(price),
                            name = desc
                        });
                    }
                }
            }
            driver.Close();
            return info;
        }
        public List<Product> GetData_HomeShopping(string Input)
        {
            driver = new ChromeDriver();
            List<Product> info = new List<Product>();
            driver.Navigate().GoToUrl("https://homeshopping.pk/");
            var searchBar = driver.FindElementByXPath("//input[@class = 'form-control bordr searchbar'][@placeholder = 'What are you looking for ?']");
            searchBar.SendKeys(Input);
            var searchBtn = driver.FindElementByXPath("//button[@class = 'btn  btn-success searhicon searchbtv']");
            searchBtn.Click();

            Thread.Sleep(2000);
            IList<IWebElement> desc_list = driver.FindElements(By.XPath("//span[@class = 'findify-components--text findify-components--cards--product__title']"));
            IList<IWebElement> price_list = driver.FindElements(By.XPath("//span[@class = 'price findify-components--cards--product--price__price']"));
            if (desc_list.Count > 0 && price_list.Count > 0)
            {
                for (int i = 0; i < desc_list.Count; i++)
                {
                    if (desc_list[i] != null && price_list[i] != null)
                    {
                        string desc = desc_list[i].Text;
                        string price = price_list[i].Text.Substring(1).Replace(",", String.Empty);
                        info.Add(new Product()
                        {
                            price = int.Parse(price),
                            name = desc
                        });
                    }
                }
            }
            driver.Close();
            return info;
        }


        public List<Product> GetData_Shopbuzz(string Input)
        {
            driver = new ChromeDriver();
            List<Product> info = new List<Product>();
            driver.Navigate().GoToUrl("http://www.shopbuzz.pk/");
            var searchBar = driver.FindElementByXPath("//input[@class = 'search-query input-medium ui-autocomplete-input']");
            searchBar.SendKeys(Input);
            Thread.Sleep(3000);
            var searchBar_li = driver.FindElement(By.XPath("//li[@class = 'ui-menu-item']"));
            string desc = searchBar_li.Text;
            searchBar_li.Click();

            IList<IWebElement> price_list = driver.FindElements(By.XPath("//div[@id = 'used-phone-grid']/table/tbody/tr/td[8]"));
            if (price_list.Count > 0)
            {
                for (int i = 0; i < price_list.Count; i++)
                {
                    if (price_list[i] != null)
                    {

                        string price = price_list[i].Text.Replace("Rs. ", string.Empty).Replace("/-", string.Empty).Replace(",", string.Empty);

                        if (int.Parse(price) > 1500)
                            info.Add(new Product()
                            {
                                price = int.Parse(price),
                                name = desc
                            });
                    }
                }
            }
            driver.Close();
            return info;
        }
    }
}
