using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium;
using System.Threading;
using OpenQA.Selenium.Remote;

namespace SeleniumRevolt
{
    /*
    public class SeleniumExtras { 
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
        public ReadOnlyCollection<IWebElement> FindElements(this IWebDriver Driver, string selector, string tagName = "*", Poller poller = new Poller(1000,5,3))
        {
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
        public IWebElement FindElement(this IWebDriver Driver,string selector, string tagName = "*", int returnIndex = 0, Poller poller = new Poller(1000,5,3))
        {
            var result = FindElements(Driver, selector, tagName, poller);
            return (result == null || result.Count == 0) ? null : result[returnIndex];
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

    }
    */
}