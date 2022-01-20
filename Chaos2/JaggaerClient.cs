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
    public JaggaerClient(){ }
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
    public int BENNumber { get { return Driver.GetNumberFromWebElement("//div[.='BEN Number']/../div[2]/div"); } }
    public string RegistrationStatus { get { return Driver.GetStringFromWebElement("//div[.='Registration Status']/../div[2]/div"); } }
    public int JaggaerID
    {
        get
        {
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
    public string GetSupplierField(string fieldName, string MenuLocation = "About|General")
    {
        SelectedMenu = MenuLocation;
        string result = "";
        string stub = "//*[starts-with(.,'" + fieldName + "')]/../../td[2]";
        var ele = Driver.FindWebElement(stub+"//input");
        if (ele != null)
        {
            string typ = ele.GetAttribute("type").ToLower();
            if (typ == "checkbox")
            {
                result = ele.GetAttribute("checked");
                result = (result == null || result.ToUpper() != "TRUE") ? "false" : "true";
            }
            else if (typ == "radio")
                ele = Driver.FindWebElement(stub + "//input[@type='radio' and @checked]");
            result = ele.GetAttribute("value");
        }
        else if (ele == null)// maybe it is a select/drop down
        {
            ele = Driver.FindWebElement(stub + "//select");
            if (ele != null)
                result = ele.GetAttribute("value");
            else
                result = Driver.GetStringFromWebElement(stub);
        }
        else
        {
            throw new Exception("WTF - ");
        }
        return result;
    }
    public void SetSupplierField(String fieldName, string value, string MenuLocation = "About|General")
    {
        SelectedMenu = MenuLocation;
        var ele = Driver.FindWebElement("//*[starts-with(.,'" + fieldName + "')]/../../td[2]//input");
        if (ele != null)//not a select element..
        {
            string typ = ele.GetAttribute("type").ToLower();
            if (typ == "checkbox")//handle the tricky checkbox...
            {
                var ticked = ele.GetAttribute("checked");
                string cbc = (ticked != null && ticked == "true").ToString();
                if (cbc.ToLower() != value.ToLower())
                    Driver.ExecuteJavaScript("arguments[0].click();", ele);
            }
            else if (typ == "radio")
            {
                ele = Driver.FindWebElement("//*[starts-with(.,'" + fieldName + "')]/../../td[2]//input[@value='" + value + "']");
                Driver.ExecuteJavaScript("arguments[0].click();", ele);                
            }
            else
                Driver.ExecuteJavaScript("arguments[0].value=arguments[1];", ele, value);
        }
        else//check to see if it is a select box.
        {
            ele = Driver.FindWebElement("//*[starts-with(.,'" + fieldName + "')]/../../td[2]//select//option[.='" + value + "']");
            if (ele != null)
            {
                Driver.ExecuteJavaScript("arguments[0].selected = 'selected';", ele);
                //    success = Save();
            }
        }
    }
    public bool Save()
    {
        bool success = false;
        IWebElement ele = Driver.FindWebElement("//input[@value='Save']");
        if (ele != null)
        {
            Driver.ClickButton("//input[@value='Save']");
            ////div/h4[.='Success']/../div[.='Settings Saved']
            success = (Driver.FindWebElement("//div/h4[.='Success']/../div[.='Settings Saved']") != null);
            if (!success)
            {
                //Give it another second...
                //
                System.Threading.Thread.Sleep(1000);
                success = (Driver.FindWebElement("//div/h4[.='Success']/../div[.='Settings Saved']") != null);
            }
        }
        return success;
    }
    public void Deactivate()
    {
        Driver.ExecuteJavaScript("performAction('GSP_Suppliers_Search_Results_Deactivate');");
    }
    public void ApproveWorkflowStep()
    {
        Driver.ExecuteJavaScript("performAction('SupRegAssignAndApprove');");
    }
    public void AssignWorkflowToMe()
    {
        Driver.ExecuteJavaScript("performAction('SupRegAssignToMySelf');");
    }
    public void InviteSupplierFromActions()
    {
        Driver.ExecuteJavaScript("performAction('GSP_Suppliers_SendEmailInvitation');");
    }
    private bool GetChecked(string name)
    {
        SelectedMenu = "About|Supplier Classes";
        bool result = Driver.FindWebElement("(//span[.='" + name + "']/../..//input)[1][@checked='checked']") != null;
        bool result2 = Driver.FindWebElement("(//span[.='" + name + "']/../..//img)[2][@title='Yes']") != null;
        return result || result2;
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
    public bool Active4Shopping
    {
        get { return GetSupplierField("Active for Shopping").ToLower() == "true"; }
        set { SetSupplierField("Active for Shopping", value.ToString()); }
    }
    public string VendorType
    {
        get { return GetSupplierField("Vendor Type"); }
        set { SetSupplierField("Vendor Type", value); }
    }
    public bool Contract
    {
        get { return GetSupplierField("Contract?").ToLower() == "true"; }
        set { SetSupplierField("Contract?", value.ToString()); }
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
    public string PennID
    {
        get { return GetSupplierField("PennID"); }
        set { SetSupplierField("PennID", value); }
    }
    public string TINInformation
    {
        get
        {
            SelectedMenu = "Legal and Compliance|TIN Information";
            return Driver.GetStringFromWebElement("//a[.='TIN Matching Status']/../../td[2]").Normalize().Trim();
        }
        set
        {
            SelectedMenu = "Legal and Compliance|TIN Information";
            Driver.ExecuteJavaScript("openEditStatus();");
            Driver.SwitchTo().Frame("ModalPopupIframe");
            string cond = ((value == "") ? "[1]" : ("[@value='" + value + "']"));
            Driver.SelectOption("//select[@id='CMM_RiskManagementTin_SupplierTinStatus']/option" + cond);
            Driver.ExecuteJavaScript("enableSave();");
            Driver.ExecuteJavaScript("hideModalPopup();");
            Driver.ClickButton("//input[@id='saveTinStatusButton']");
            Driver.SwitchTo().ParentFrame();
        }
    }
    public string PaymentTerms
    {
        get { return GetSupplierField("Payment Terms Exception", "Accounts Payable|Payment Custom Fields"); }
        set { SetSupplierField("Payment Terms Exception", value, "Accounts Payable|Payment Custom Fields"); }
    }
    public string TermsBasis
    {
        get { return GetSupplierField("Terms Basi", "Accounts Payable|Payment Custom Fields"); }
        set { SetSupplierField("Terms Basis", value, "Accounts Payable|Payment Custom Fields"); }
    }    
    public PayMethods PaymentMethods
    {
        get
        {
            return new PayMethods(this);
        }
        set
        {

        }
    }
    public List<PaymentMethod> GetPaymentMethods()
    {
        SelectedMenu = "Accounts Payable|Payment Methods";
        List<PaymentMethod> result = new List<PaymentMethod>();
        var eles = Driver.FindWebElements("//*[.='Manage Accounts Payable']/../../../../../../tr[2]//a");
        foreach(IWebElement ele in eles)
        {
            PaymentMethod pm = null; 
            string txt = ele.Text;
            bool act = ele.GetDomAttribute("class") == "ListActive";
            int index = txt.IndexOf("(Check - Net 45");
            if (index > 0)
                pm = new PaymentMethod(txt.Substring(0, index), act, "Check", ele); 
            else { 
                index = txt.IndexOf("(Direct Deposit");
                if (index > 0)
                    pm = new PaymentMethod(txt.Substring(0, index), act, "Direct Deposit",ele);
                else
                {
                    index = txt.IndexOf("(ePayable - Net");
                    if (index > 0)
                        pm = new PaymentMethod(txt.Substring(0, index), act, "ePayables",ele);
                    else
                    {
                        index = txt.IndexOf("(Wire Transfer - N");
                        if (index > 0)
                            pm = new PaymentMethod(txt.Substring(0, index), act, "Wire Transfer",ele);
                        else
                        {
                            index = txt.IndexOf("(Pay Mode (");
                            if (index > 0)
                                pm = new PaymentMethod(txt.Substring(0, index), act, "Paymode",ele);
                            else
                                pm = new PaymentMethod(txt, act, "Unrecognized payment method",ele);
                        }
                    }
                }
            }
            result.Add(pm);
        }
        return result;
    }
    public class PaymentMethod
    {
        public PaymentMethod(string name, bool active,string actType,IWebElement link)
        {
            Name = name;
            Type = actType;
            Active = active;
            Link = link;
        } 
        public bool Active { get; set; }
        public string Name { get; set; }
        public string Type { get; set; }
        public IWebElement Link { get; set; }
    }
    public class PayMethods 
    {   
        public SupplierProfile SP { get; }
        public List<PaymentMethod> PaymentMethods { get; set; }
        public PayMethods(in SupplierProfile sp) {
            SP = sp;
            SP.SelectedMenu = "Accounts Payable|Payment Methods";
            PaymentMethods = GetPaymentMethods();
        }
        public void AddPaymentMethod(PaymentMethod pm)
        {
        }
        private List<PaymentMethod> GetPaymentMethods()
        {
            SP.SelectedMenu = "Account Payable|Payment Methods";
            List<PaymentMethod> result = new List<PaymentMethod>();
            var eles = SP.Driver.FindWebElements("//*[.='Manage Accounts Payable']/../../../../../../tr[2]//a");
            foreach (IWebElement ele in eles)
            {
                PaymentMethod pm = null;
                string txt = ele.Text;
                bool act = ele.GetDomAttribute("class") == "ListActive";
                int index = txt.IndexOf("(Check - Net 45");
                if (index > 0)
                    pm = new PaymentMethod(txt.Substring(0, index), act, "Check", ele);
                else
                {
                    index = txt.IndexOf("(Direct Deposit");
                    if (index > 0)
                        pm = new PaymentMethod(txt.Substring(0, index), act, "Direct Deposit", ele);
                    else
                    {
                        index = txt.IndexOf("(ePayable - Net");
                        if (index > 0)
                            pm = new PaymentMethod(txt.Substring(0, index), act, "ePayables", ele);
                        else
                        {
                            index = txt.IndexOf("(Wire Transfer - N");
                            if (index > 0)
                                pm = new PaymentMethod(txt.Substring(0, index), act, "Wire Transfer", ele);
                            else
                            {
                                index = txt.IndexOf("(Pay Mode (");
                                if (index > 0)
                                    pm = new PaymentMethod(txt.Substring(0, index), act, "Paymode", ele);
                                else
                                    pm = new PaymentMethod(txt, act, "Unrecognized payment method", ele);
                            }
                        }
                    }
                }
                result.Add(pm);
            }
            return result;
        }
        public PaymentMethod PaymentMethod { get; set; }
        public void Save()
        {
            SP.Driver.ClickWebElement("//input[@value='Save' and @type='submit']");
        }
        public bool Active {
            get { return SP.GetSupplierField("Active","Accounts Payable|Payment Methods")=="Button_Yes"; }
            set { SP.SetSupplierField("Active", ("Button_"+(value?"Yes":"No")), "Accounts Payable|Payment Methods"); }
        }
        public string RemitEmail {
            get { return SP.GetSupplierField("Electronic Remittance", "Accounts Payable|Payment Methods"); }
            set { SP.SetSupplierField("Electronic Remittance", value, "Accounts Payable|Payment Methods"); }
        }
        public string RemittanceAddress {
            get { return SP.GetSupplierField("Remittance Address", "Accounts Payable|Payment Methods"); }
            set { SP.SetSupplierField("Remittance Address", value, "Accounts Payable|Payment Methods"); }
        }
        public string ThirdParty
        {
            get { return SP.GetSupplierField("Third Party", "Accounts Payable|Payment Methods"); }
            set { SP.SetSupplierField("Third Party", value, "Accounts Payable|Payment Methods"); }
        }
        public string ERP {
            get { return SP.GetSupplierField("ERP Number", "Accounts Payable|Payment Methods"); }
            set { SP.SetSupplierField("ERP Number", value, "Accounts Payable|Payment Methods"); }
        }
        public string Country {
            get { return SP.GetSupplierField("Country", "Accounts Payable|Payment Methods"); }
            set { SP.SetSupplierField("Country", value, "Accounts Payable|Payment Methods"); }
        }
        public string Title
        {
            get { return SP.GetSupplierField("Payment Title", "Accounts Payable|Payment Methods"); }
            set { SP.SetSupplierField("Payment Title", value, "Accounts Payable|Payment Methods"); }
        }
    }

    public class Address
    {
        public Address(string name, bool active, string siteType,IWebElement link)
        {
            Name = name;
            Active = active;
            SiteType = siteType;
            Link = link;
        }
        public string Name { get; set; }
        public bool Active { get; set; }
        public string SiteType { get; set; }
        public IWebElement Link { get; set; }
    }
    public class Addresses
    {
        public Addresses(in SupplierProfile sp)
        {
            SP = sp;
            SP.SelectedMenu = "Contacts and Addresses|Addresses";

            //
        }
        public bool ShowInactives { get; set; }
        public SupplierProfile SP { get;}
        public List<Address> SiteList { get { return GetAddresses(); } }
        public List<Address> GetAddresses()
        {
            SP.SelectedMenu = "Contacts and Addresses|Addresses";
            List<Address> result = new List<Address>();
            var eles = SP.Driver.FindWebElements("//*[.='Select an Address']/../../../../../../tr[2]//a");
            foreach (IWebElement ele in eles)
            {
                string txt = ele.Text;
                bool act = ele.GetDomAttribute("class") == "ListActive";
                bool prim = txt.Contains(" (Primary ");
                string siteType = "";
                if (txt.EndsWith("Fulfillment)"))
                    siteType = "Fulfillment";
                else if (txt.EndsWith("Remittance)"))
                    siteType = "Remittance";
                else if (txt.EndsWith("Physical)"))
                    siteType = "Physical";
                else
                    siteType = "Unknown Site Type";
                int len = txt.IndexOf("(" + (prim ? "Primary " : "") + siteType);
                result.Add(new Address(txt.Substring(0, len), act, siteType, ele));
            }
            return result;
        }
        public string Name
        {
            get { return SP.GetSupplierField("Name", "Contacts and Addresses|Addresses"); }
            set { SP.SetSupplierField("Name", value, "Contacts and Addresses|Addresses"); } 
        }
        public string AddressID
        {
            get { return SP.GetSupplierField("Address ID", "Contacts and Addresses|Addresses"); }
            set { SP.SetSupplierField("Address ID", value, "Contacts and Addresses|Addresses"); }
        }
        public string AddressType
        {
            get { return SP.GetSupplierField("Address Type", "Contacts and Addresses|Addresses"); }
            set { SP.SetSupplierField("Address Type", value, "Contacts and Addresses|Addresses"); }
        }
        public string Country
        {
            get { return SP.GetSupplierField("Country", "Contacts and Addresses|Addresses"); }
            set { SP.SetSupplierField("Country", value, "Contacts and Addresses|Addresses"); }
        }
        public bool Active
        {
            get { return SP.GetSupplierField("Active", "Contacts and Addresses|Addresses")=="true"; }
            set { SP.SetSupplierField("Active", value.ToString(), "Contacts and Addresses|Addresses"); }
        }
        public string Address1
        {
            get { return SP.GetSupplierField("Street Line 1", "Contacts and Addresses|Addresses"); }
            set { SP.SetSupplierField("Street Line 1", value, "Contacts and Addresses|Addresses"); }
        }
        public string Address2
        
        {
                get { return SP.GetSupplierField("Street Line 2", "Contacts and Addresses|Addresses"); }
                set { SP.SetSupplierField("Street Line 2", value, "Contacts and Addresses|Addresses"); }
            }
        public string Address3
        {
            get { return SP.GetSupplierField("Street Line 3", "Contacts and Addresses|Addresses"); }
            set { SP.SetSupplierField("Street Line 3", value, "Contacts and Addresses|Addresses"); }
        }
        public string City
        {
            get { return SP.GetSupplierField("City/Town", "Contacts and Addresses|Addresses"); }
            set { SP.SetSupplierField("City/Town", value, "Contacts and Addresses|Addresses"); }
        }
        public string State
        {
            get { return SP.GetSupplierField("State/Province", "Contacts and Addresses|Addresses"); }
            set { SP.SetSupplierField("State/Province", value, "Contacts and Addresses|Addresses"); }
        }
        public string PostalCode
        {
            get { return SP.GetSupplierField("Postal Code", "Contacts and Addresses|Addresses"); }
            set { SP.SetSupplierField("Postal Code", value, "Contacts and Addresses|Addresses"); }
        }
        public bool Primary
        {
            get { return SP.GetSupplierField("Primary", "Contacts and Addresses|Addresses")=="true"; }
            set { SP.SetSupplierField("Primary", value.ToString(), "Contacts and Addresses|Addresses"); }
        }
    }
    public Addresses Sites { 
        get { return new Addresses(this); } 
    }
}
public class NSR : PageResult
{
    public NSR(in ThinWebClient d):base (d)
    { }
    public string GetSupplierField(string fieldName)
    {
        string result = "(//*[contains(translate(., 'ABCDEFGHIJKLMNOPQRSTUVWXYZ', 'abcdefghijklmnopqrstuvwxyz'),'" + fieldName + "')])[13]/../div/*/*";
        IWebElement ele = Driver.FindWebElement(result);
        var tag = ele.TagName;
        return result;
    }
    public void SetSupplierField(string fieldName, string value)
    {
    }
    public string Name { get { return Driver.GetStringFromWebElement("//div[@class='sidebarName']/div"); } }
    public string Template { get { return Driver.GetStringFromWebElement("//div[.='Template']/../div[2]/div"); } }
    public string Status { get { return Driver.GetStringFromWebElement("//div[.='Request Status']/../div[2]/div"); } }
    public string GoodsServices { get; set; }
    public string SupplierUsage { get; set; }
    public string SupplierName { get; set; }
    public bool ForeignSupplier { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    public string SC_FirstName { get; set; }
    public string SC_LastName { get; set; }
    public string SC_Phone { get; set; }
    public bool isProxy { get; set; }
    public string ProxyDescription { get; }
    public string ProxyAffiliation { get; set; }
    public string ProxyPennID { get; set; }


}