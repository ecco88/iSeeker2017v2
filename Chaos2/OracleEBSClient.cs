using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebClient;

namespace Chaos2
{
    public class OracleEBSClient : ThinWebClient
    {
        public OracleEBSClient() { }
        public OracleEBSClient(bool prod) : base("https:///benf" + (prod ? "prod" : "tst3") + ".isc-seo.upenn.edu", "C://Selenium")
        {
            Navigate().GoToUrl(platformURL);
            Manage().Window.Maximize();
            System.Threading.Thread.Sleep(3000);
            while (!PageLoaded()) 
            {
                System.Threading.Thread.Sleep(2000);
            }
            FillField("#usernameField", "BCAPUTO");
            FillField("#passwordField", "Av3nt!ne2oo9!");
            ClickWebElement("@OraButton left");
        }
        public void SelectMenu(params string[] names)
        {
            System.Threading.Thread.Sleep(3000);
            while (!PageLoaded()) { }
            if (names.Length==1)
                ClickWebElement("//a[.='" + names[0] + "']");
            else if(names.Length==2)
            {
                ClickWebElement("//a[.='" + names[0] + "']/../ul/li/a[.='"+names[1]+"']");
                //a[.='PO Sysadmin']/../ul/li/a[.='Suppliers']
            }
            else if (names.Length==3)
            {
                ClickWebElement("//a[.='" + names[0] + "']/../ul/li/a[.='" + names[1] + "']/../ul/li/a[.='" + names[2] + "']");
            }
        }
    }
}
