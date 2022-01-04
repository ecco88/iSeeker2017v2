using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using WebClient;


    public class JaggaerClient : ThinWebClient
    {
        public JaggaerClient()
        {

        }
        public JaggaerClient(bool prod) : base("https://" + (prod ? "solutions" : "usertest") + ".sciquest.com/apps/Router/Login?OrgName=UPenn", "C://Selenium")
        {
            Navigate().GoToUrl(platformURL);
            Manage().Window.Maximize();
            FindElement(By.Id("Username")).SendKeys((prod) ? "Otto.Mate" : "caputo");
            FindElement(By.Id("Password")).SendKeys((prod) ? "Av3nt!ne2009"/* "Aut0m@te88"*/ : "blue72");
            FindElement(By.TagName("button")).Click();
        }

        public void LogInUser(string username, string pwd)
        {
            /// TODO:  Implement this as needed
            /// 
            throw new NotImplementedException();
        }
        /// <summary>
        /// Selects the menu trail on the left nav bar using the labels/text as parameters.  For example:
        /// "Suppliers","Manage Suppliers","View Saved Searches" or just "Home".
        /// </summary>
        /// <param name="names">list of the menu labels as they appear textually in the application nav side bar.</param>
        public void SelectMenu(params string[] names)
        {
            if (names[0] == "Home")
            {
                ClickWebElement("//a[@aria-label='Home']");
            }
            else
            {
                ClickWebElement("//a[@aria-label='" + names[0] + "']");
                ClickWebElement("//h2[.='" + names[1] + "']");
                ClickWebElement("//a[@title='" + names[2] + "']");
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="names"></param>
        /// <returns></returns>
        public int SelectAction(params string[] names)
        {
            int count = FindWebElements("//a[@id='Phoenix_Nav_ActionItems_Invoker' and @aria-expanded='true']").Count;
            if (count == 0) ClickWebElement("Phoenix_Nav_ActionItems_Invoker");
            count = 0;
            var eles = FindElements(By.XPath("//h5[.='" + names[0] + "']/../ul/li/span/a[.='" + names[1] + "']/../../div"));
            if (eles.Count > 0)
            {
                count = int.Parse(eles[0].Text);
                ClickWebElement("//h5[.='" + names[0] + "']/../ul/li/span/a[.='" + names[1] + "']");
            }
            return count;
        }
        /// <summary>
        /// Using the quick search menu item/ feature along the top 
        /// </summary>
        /// <param name="search">the criterium to searched</param>
        /// <param name="searchOption">the quick search options search as Supplier Profile, All, etc..</param>
        public SearchResultPage QuickSearch(string search, string searchOption)
        {
            //        ClickWebElement("#Phoenix_Notifications_QuickSearchType", tagName: "SELECT");
            SelectOption("//select[@id='Phoenix_Notifications_QuickSearchType']/option[.='" + searchOption + "']");
            FillField("//input[@id='Phoenix_Notifications_QuickSearchTerms' and @type='text']", search);
            ClickButton("//button[@id='Phoenix_Notifications_QuickSearchSubmit']");
            return new SearchResultPage(this);
        }

    }
    public class PageResult
    {
        public ThinWebClient Driver { get; set; }
        public PageResult(in ThinWebClient d)
        {
            Driver = d;
        }
    }
    public class SupplierResultPage : PageResult
    {
        public SupplierResultPage(in ThinWebClient dr) : base(dr)
        {
        }
        public void Next(int times = 1)
        {

        }
        public void Previous(int times = 1) { }
        public void GoTo(int index)
        { }
    }
    public class SearchResultPage : PageResult
    {
        public SearchResultPage(in ThinWebClient dr) : base(dr)
        {

        }
        public int Count
        {
            get
            {
                string text = Driver.GetStringFromWebElement("<Showing", "SPAN");
                if (text == "")
                    return 0;
                int index = text.IndexOf("of ");
                string number = text.Substring(index);
                return Int32.Parse(Regex.Match(number, @"\d+").Value);
            }
        }
        public int PageSize
        {
            get
            {
                return Driver.GetNumberFromWebElement("#GeneralSearch_PageSize0", "select");
            }
            set
            {
                Driver.ExecuteJavaScript("pageSizeChange('" + value + "');");
            }
        }
        public PageResult Select(int index = 1)
        {
            Driver.ClickWebElement("((//button[.='Manage'])[" + index.ToString() + "]/../../..//a)[1]");
            return new PageResult(Driver);
        }
        public PageResult Select(string name)
        {
            Driver.ClickWebElement("?" + name, tagName: "a");
            return new PageResult(Driver);
        }
        public PageResult Select(ResultSelectionOption opt, int index = 1)
        {
            var ele = Driver.FindWebElement("((//button[.='Manage'])[" + index.ToString() + "]/../../..//a)[" + ((int)opt).ToString() + "]");
            string js = ele.GetAttribute("onclick");
            Driver.ExecuteJavaScript(js);
            return new PageResult(Driver);
        }
        public PageResult Select(string name, ResultSelectionOption opt)
        {
            Driver.ClickWebElement("?" + name, tagName: "a");
            return new PageResult(Driver);
        }
        public enum ResultSelectionOption
        {
            Edit = 2, Activate = 3, Invite = 4, Reject = 5, SendEmail = 6
        }
    }
    public class SupplierProfile : PageResult
    {
        public SupplierProfile(in ThinWebClient d) : base(d) { }
        public int BENNumber
        {
            get
            {
                return Driver.GetNumberFromWebElement("//div[.='BEN Number']/../div[2]/div");
            }
        }
        public string RegistrationStatus
        {
            get
            {
                return Driver.GetStringFromWebElement("//div[.='Registration Status']/../div[2]/div");
            }
        }
        public int JaggaerID
        {
            get
            {
                //return Driver.ExecuteJavaScript("document.forms['SelectOrg'].elements['CMMSP_SupplierID'].value").ToString();
                return Driver.GetNumberFromWebElement("(//input[@name='CMMSP_SupplierID' and @type='hidden'])[1]");
            }
        }
        public string Name
        {
            get
            {
                return Driver.GetStringFromWebElement("//div[.='Registration Status']/../../../../div/div/span");
            }
        }
        public string RegistrationType
        {
            get
            {
                return Driver.GetStringFromWebElement("//div[.='Registration Type']/../div[2]/div/div");
            }
        }
        public SupplierProfile Next(int index = 1)
        {
            SupplierProfile sp = new SupplierProfile(this.Driver);
            while (index > 0)
            {
                IWebElement ele = Driver.FindWebElement("//button[@aria-label='Next page']");
                if (ele.GetAttribute("disabled") == "disabled")
                    return this;
                else
                {
                    ele.Click();
                    index--;
                    sp.Driver.PageLoaded();
                    sp = new SupplierProfile(this.Driver);
                }
            }
            return sp;
        }

        public SupplierProfile Previous(int index = 1)
        {
            SupplierProfile sp = new SupplierProfile(this.Driver);
            while (index > 0)
            {
                IWebElement ele = Driver.FindWebElement("//button[@aria-label='Previous page']");
                if (ele.GetAttribute("disabled") == "disabled")
                    return this;
                else
                {
                    ele.Click();
                    index--;
                    sp.Driver.PageLoaded();
                    sp = new SupplierProfile(this.Driver);
                }
            }
            return sp;
        }
        public void MenuSelect(string menu, string menuitem)
        {
            if (Driver.FindElements(By.XPath("//a[@aria-expanded='false']/span[.='" + menu + "']")).Count == 1)
            {
                Driver.ClickWebElement("//a[@aria-expanded='false']/span[.='" + menu + "']");
            }
            Driver.ClickWebElement("//ul/li/a[.='" + menuitem + "']");
        }
        public string SelectedMenu
        {
            get
            {
                return Driver.GetStringFromWebElement("(//div[@class='phx title-bar']/div/span)[2]");
            }
            set
            {
                //There is overlap between About|General and Diversity|General!!
                string[] mens = value.Split('|');
                if (mens.Length == 2 && (mens[1] != SelectedMenu))
                    MenuSelect(mens[0], mens[1]);
            }
        }
        public string GetSupplierField(string fieldName)
        {
            var ele = Driver.FindWebElement("//td[@class='FormCell']/a[starts-with(.,'" + fieldName + "')]/../../td[2]//input");
            if (ele != null)
                return ele.GetAttribute("value");
            if (ele == null)// maybe it is a select/drop down
            {
                ele = Driver.FindWebElement("//td[@class='FormCell']/a[starts-with(.,'" + fieldName + "')]/../../td[2]//select");
                return ele.GetAttribute("value");
            }
            else if (ele == null)//
            {
                //possibly a non form element - 
                return ele.Text;
            }
            else
            {
                throw new Exception("WTF - ");
            }
        }

        public void SetSupplierField(String fieldName, string value)
        {
            bool success = false;
            var ele = Driver.FindWebElement("//td[@class='FormCell']/a[starts-with(.,'" + fieldName + "')]/../../td[2]//input");
            if (ele != null)
            {
                Driver.ExecuteJavaScript("arguments[0].value=arguments[1];", ele, value);
                success=Save();
            }
            else
            {
                ele = Driver.FindWebElement("//td[@class='FormCell']/a[starts-with(.,'" + fieldName + "')]/../../td[2]//select//option[.='" + value + "']");
                if (ele != null)
                    Driver.ExecuteJavaScript("arguments[0].selected = 'selected';", ele);
            }
        }
        //td[@class='FormCell']/a[.='Supplier Name']/../../td[2]//input

        public bool Save()
        {
            bool success = false;
            IWebElement ele = Driver.FindWebElement("//input[@value='Save']");
            if (ele != null)
            {
                Driver.ClickButton("//input[@value='Save']");
                ////div/h4[.='Success']/../div[.='Settings Saved']
                success = (Driver.FindWebElement("//div/h4[.='Success']/../div[.='Settings Saved']") != null);
            }
            return success;
        }
        private bool GetChecked(string name)
        {
            SelectedMenu = "About|Supplier Classes";

            bool result = Driver.FindWebElement("(//span[.='" + name + "']/../..//input)[1][@checked='checked']") != null;
            bool result2 = Driver.FindWebElement("(//span[.='" + name + "']/../..//img)[2][@title='Yes']") != null;
            return result || result2;
            /*
                var ele = Driver.FindWebElement("(//span[.='" + name + "']/../..//input)[1]");
            var el2 = Driver.FindWebElement("(//span[.='" + name + "']/../..//img)[2][@title='Yes']");
            return (Driver.GetElementAttributeValue("(//span[.='" + name + "']/../..//input)[1]", "checked").ToLower() == "checked");
      //      return (Driver.FindWebElement("(//span[.='" + name + "']/../..//input)[1]")).GetAttribute("checked").ToLower() == "checked");
        */
        }
        private void SetChecked(string name, bool val)
        {
            SelectedMenu = "About|Supplier Classes";
            var eles = Driver.FindWebElements("(//span[.='" + name + "']/../..//input)");
            if (!eles[0].Selected)
                Driver.ExecuteJavaScript("arguments[0].click();", eles[0]);
            if (eles[1].Selected != val)
                Driver.ExecuteJavaScript("arguments[0].click();", eles[1]);
        }
        public bool cXML_Flag
        {
            get { return GetChecked("cXML Supplier"); }
            set { SetChecked("cXML Supplier", value); }
        }
        public bool EDI_Flag
        {
            get { return GetChecked("EDI Supplier"); }
            set { SetChecked("EDI Supplier", value); }
        }
        public bool ePayables_Flag
        {
            get { return GetChecked("ePayables"); }
            set { SetChecked("ePayables", value); }
        }
        public bool PO_Supplier_Flag
        {
            get { return GetChecked("PO Supplier"); }
            set { SetChecked("PO Supplier", value); }
        }
        public bool DisabledBusiness_Flag
        {
            get { return GetChecked("Disabled Business Enterprise"); }
            set { SetChecked("Disabled Business Enterprise", value); }
        }
        public bool ForeignVendor_Flag
        {
            get { return GetChecked("Foreign Vendor"); }
            set { SetChecked("Foreign Vendor", value); }
        }
        public bool ISP_Flag
        {
            get { return GetChecked("ISP - Individual Service Provider"); }
            set { SetChecked("ISP - Individual Service Provider", value); }
        }
        public bool LGBTQ_Flag
        {
            get { return GetChecked("LGBTQ Owned Business Enterprise"); }
            set { SetChecked("LGBTQ Owned Business Enterprise", value); }
        }
        public bool MinorityBusiness_Flag
        {
            get { return GetChecked("Minority Business Enterprise"); }
            set { SetChecked("Minority Business Enterprise", value); }
        }
        public bool WomenBusiness
        {
            get { return GetChecked("Women Owned Business Enterprise"); }
            set { SetChecked("Women Owned Business Enterprise", value); }
        }
        public bool VeteranBusiness
        {
            get { return GetChecked("Veteran Owned Business Enterprise"); }
            set { SetChecked("Veteran Owned Business Enterprise", value); }
        }
    }