using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using OpenQA.Selenium.Edge;
using System.Collections.ObjectModel;
using System.Threading;

namespace SeleniumRevolt
{
    public class WebClient : IDisposable
    {
        public IWebDriver Driver { get; set; }
        public string DriverURL { get; set; }
        public enum BrowserType { Chrome, FireFox, Edge, Other };
        public BrowserType Browser { get; set; }
        public Poller Poller { get; set; }

        public WebClient(string URL, BrowserType browser=BrowserType.Chrome, OpenQA.Selenium.DriverOptions options=null, Poller poller=null)
        { 
            Browser = browser;
            switch (Browser)
            { 
                case BrowserType.Chrome:
                    DriverURL = "C://Selenium";                    
                    Driver = (options == null) ? new ChromeDriver(DriverURL) : new ChromeDriver(DriverURL, (ChromeOptions)options);
                    break;
                case BrowserType.FireFox:
                    DriverURL = "/Drivers/GeckoDriver.exe";
                    break;
                case BrowserType.Edge:
                    DriverURL = "Drivers/EdgeDriver.exe";
                    break;
                case BrowserType.Other:
                    DriverURL = "Drivers/WebDriver.exe";
                    break;
            }
            Poller = (poller != null) ? poller : new Poller();
            Driver.Url = URL;
        }
        /// <summary>
        /// Returns a collection of webElements based on search paramtete
        /// </summary>
        /// <param name="selector">Search criteria - first character denotes mechanism - no special character uses XPath.
        /// flags for mechanisms are: head character # - ID, @ - class name, < - text starts with, > - ends with text,
        /// ? - contains text, = - text matches, ~ - name attribute matches.
        /// </param>
        /// <param name="tagName">[optional] - return only elements of this tag name.</param>
        /// <param name="poller">[optional] - Poller object which indicates number of attempts before gracefully failing.</param>
        /// <returns> a list of all matching IWebElements and null if not found.</returns>
        public ReadOnlyCollection<IWebElement> FindWebElements(string selector, string tagName = "*", Poller poller = null)
        {
            if (poller == null) poller = this.Poller;           //use driver default poller if none supplied.            
            ReadOnlyCollection<IWebElement> result;
            string xPath = selector;
            string path = selector.Substring(1, selector.Length - 1);
            int tries = 0;
            while (tries < poller.Attempts)
            {
                if (selector.StartsWith("#"))
                {
                    string p2 = "//" + tagName + "[@id='" + path + "']";
                    result = Driver.FindElements(By.XPath(p2));
                }
                else if (selector.StartsWith("@"))
                    result = Driver.FindElements(By.XPath("//" + tagName + "[@class='" + path + "']"));
                else if (selector.StartsWith("<"))
                    result = Driver.FindElements(By.XPath("//" + tagName + "[starts-with(.,'" + path + "')]"));
                else if (selector.StartsWith(">)"))
                    result = Driver.FindElements(By.XPath("//" + tagName + "['" + path + "'=substring(.,string.length(.)-string.length('" + path + "')+1)]"));
                else if (selector.StartsWith("?"))
                    result = Driver.FindElements(By.XPath("//" + tagName + "[contains(.,'" + path + "')]"));
                // seems not to work all of the time.  - unreliable./
                else if (selector.StartsWith("="))
                {
                    string p2 = "//" + tagName + "[.='" + path + "']";
                    result = Driver.FindElements(By.XPath(p2));
                }
                else if (selector.StartsWith("~"))
                    result = Driver.FindElements(By.XPath("//" + tagName + "[@name='" + path + "']"));
                else result = Driver.FindElements(By.XPath(selector));
                if (result.Count == 0)      //didn't find it try again...
                {
                    tries++;
                    if (tries < poller.Attempts)
                    {
                        if (poller.Attempts > 0 && tries % poller.RefreshAttempts == 0)
                            Driver.Navigate().Refresh();
                        Thread.Sleep(poller.Interval);
                    }
                }
                else 
                    return result;   // found it 
            }
            return null; // its just not there...after we tried as many times as in the poller.
        }
        /// <summary>
        /// Finds one specific element on the web document.
        /// </summary>
        /// <param name="selector">Search criteria - first character denotes mechanism - no special character uses XPath.
        /// flags for mechanisms are: head character # - ID, @ - class name, < - text starts with, > - ends with text,
        /// ? - contains text, = - text matches, ~ - name attribute matches.</param>
        /// <param name="tagName">[optional] - return only elements of this tag name - default is all tags.</param>
        /// <param name="returnIndex">If multiple matches returns this index - defaults to zero.</param>
        /// <param name="poller" required="no">Poller object which indicates number of attempts before gracefully failing.</param>
        /// <returns>IWebElement match and null if not found.</returns>
        public IWebElement FindWebElement(string selector, string tagName = "*", int returnIndex=0, Poller poller = null)
        {
            var result = FindWebElements(selector, tagName, poller);
            return (result == null || result.Count==0) ? null : result[returnIndex];
        }
        /// <summary>
        /// Execute the javascript script.
        /// </summary>
        /// <param name="script">the script itself</param>
        /// <param name="args">list of items to pass to the script as parameters.</param>
        public void ExecuteJavaScript(string script, params object[] args)
        {
            var js = (IJavaScriptExecutor)this;
            js.ExecuteScript(script, args);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="selector"></param>
        /// <param name="shadowClick"></param>
        /// <param name="index"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
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
        public void ClickButton(string xPath)
        {
            ClickWebElement(xPath, tagName: "button");
        }

        public void Dispose()
        {
            Driver.Quit();
        }
    }
}
