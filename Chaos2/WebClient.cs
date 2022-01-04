using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace WebClient
{
    public class ThinWebClient : ChromeDriver, IDisposable
    {
        public enum BrowserType
        {
            Chrome,
            FireFox,
            IE,
            Edge,
            Safari,
            Opera
        }
        public ThinWebClient()
        {

        }
        public ThinWebClient(string platform, string driverPath = "C://Selenium", string username = "", string password = ""/*, BrowserType browser = BrowserType.Chrome*/) : base(driverPath)
        {
            this.Navigate().GoToUrl(platform);
            platformURL = platform;
            pollAttempts = 5;
            pollInterval = 1000;
            refreshInterval = 3;
        }
        public new void Dispose()
        {
            this.Close();
            base.Dispose();
        }
        public BrowserType Browser { get; set; }
        public int pollAttempts { get; set; }
        public int pollInterval { get; set; }
        public int refreshInterval { get; set; }
        public string platformURL { get; private set; }
        public string Username { get; private set; }
        /// <summary>
        /// Returns a collection of webElements based on search paramtete
        /// </summary>
        /// <param name="selector">Search criteria - first character denotes mechanism - no special character uses XPath.
        /// flags for mechanisms are: head character # - ID, @ - class name, < - text starts with, > - ends with text,
        /// ? - contains text, = - text matches, ~ - name attribute matches.
        /// </param>
        /// <param name="tagName">[optional] - return only elements of this tag name.</param>
        /// <returns></returns>
        public ReadOnlyCollection<IWebElement> FindWebElements(string selector, string tagName = "*")
        {
            ReadOnlyCollection<IWebElement> result = null;
            string xPath = selector;
            string path = selector.Substring(1, selector.Length - 1);
            int attempts = 0;
            do
            {
                if (attempts > 0)
                {
                    if (refreshInterval > 0 && attempts % refreshInterval == 0)
                        Navigate().Refresh();
                    Thread.Sleep(pollInterval);
                }
                if (selector.StartsWith("#"))
                {
                    string p2 = "//" + tagName + "[@id='" + path + "']";
                    result = FindElements(By.XPath(p2));
                }
                else if (selector.StartsWith("@"))
                    result = FindElements(By.XPath("//" + tagName + "[@class='" + path + "']"));
                else if (selector.StartsWith("<"))
                    result = FindElements(By.XPath("//" + tagName + "[starts-with(.,'" + path + "')]"));
                else if (selector.StartsWith(">)"))
                    result = FindElements(By.XPath("//" + tagName + "['" + path + "'=substring(.,string.length(.)-string.length('" + path + "')+1)]"));
                else if (selector.StartsWith("?"))
                    result = FindElements(By.XPath("//" + tagName + "[contains(.,'" + path + "')]"));
                else if (selector.StartsWith("="))
                    result = FindElements(By.XPath("//" + tagName + "[.='" + path + "']"));
                else if (selector.StartsWith("~"))
                    result = FindElements(By.XPath("//" + tagName + "[@name='" + path + "']"));
                else result = FindElements(By.XPath(selector));

            }
            while (++attempts < pollAttempts && result == null);
            return result;
        }
        public bool PageLoaded(int seconds = 1)
        {
            bool result = false;
            DateTime exp = DateTime.Now.AddMilliseconds(seconds * 1000);
            while (DateTime.Now < exp && !result)
            {
                result = ExecuteJavaScript("return document.readyState").ToString() == "complete";
            }
            return result;
        }
        public IWebElement FindWebElement(string selector, string tagName = "*")
        {
            IWebElement result = null;
            ReadOnlyCollection<IWebElement> results = FindWebElements(selector, tagName);
            if (results.Count > 0)
                result = results[0];// take only the first result.
            return result;
        }
        public object ExecuteJavaScript(string script, params object[] args)
        {
            var js = (IJavaScriptExecutor)this;
            return js.ExecuteScript(script, args);
        }
        public IWebElement ClickWebElement(string selector, bool shadowClick = true, int index = 0, string tagName = "*")
        {
            IWebElement el = FindWebElements(selector, tagName)[index];
            if (el != null)
            {
                if (shadowClick)
                    ExecuteJavaScript("arguments[0].click();", el);
                else
                {
                    ExecuteJavaScript("arguments[0].scrollintoView();");
                    el.Click();
                }
            }
            return el;

        }
        public IWebElement SelectOption(string xPath)
        {
            var ele = FindWebElement(xPath);
            ExecuteJavaScript("arguments[0].selected='selected';", ele);
            return ele;
        }
        public IWebElement FillField(string xPath, string value)
        {
            var ele = FindWebElement(xPath);
            ExecuteJavaScript("arguments[0].value=arguments[1];", ele, value);
            return ele;
        }
        public string GetElementAttributeValue(string selector, string attrName, string tagName = "*")
        {
            return FindWebElement(selector).GetAttribute(attrName);
        }
        public void ClickButton(string xPath)
        {
            ClickWebElement(xPath, tagName: "button");
        }
        public string GetStringFromWebElement(string selector, string tagName = "*")
        {
            string value = "";
            var els = FindWebElements(selector);
            if (els.Count > 0)
            {
                switch (els[0].TagName.ToUpper())
                {
                    case "SELECT":
                    case "INPUT":
                        value = els[0].GetAttribute("value");
                        break;
                    default:
                        value = els[0].Text;
                        break;
                }
            }
            return value;
        }
        public int GetNumberFromWebElement(string selector, string tagName = "*")
        {
            int value = -1;
            var els = FindWebElements(selector, tagName);
            if (els.Count > 0)
            {
                string ele = (els[0].TagName.ToUpper() != "SELECT") ? GetStringFromWebElement(selector) : els[0].GetAttribute("value");
                if (ele != "")
                    value = Convert.ToInt32(Regex.Replace(ele, "[^0-9]+", string.Empty));
            }
            return value;
        }
    }
    public class BenHelpsClient : ThinWebClient
    {
        public BenHelpsClient() : base("https://benhelps.upenn.edu", "C://Selenium")
        {
            Navigate().GoToUrl(platformURL);
        }
    }

    public class OutlookClient : ThinWebClient
    {
        public OutlookClient() : base("https://outlook.office.com/mail/", "C://Selenium")
        {
            Navigate().GoToUrl(platformURL);
        }
    }
}
