using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;
using System.Threading;
using System.Text.RegularExpressions;
using WebClient;
using IronXL;
using Chaos2;

namespace ThinClientTester
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            using var TSM = new JaggaerClient(true);
            var testitem = TSM.QuickSearch("PRICEWATERHOUSECOOPERS", "Supplier Profile");
            var count = testitem.Count;
            var pagesize = testitem.PageSize;
            testitem.PageSize = (200);
            pagesize = testitem.PageSize;
            testitem.Select();
            SupplierProfile SP = new SupplierProfile(TSM);
            //            SP.MenuSelect("About", "General");
            //            SP = SP.Previous();
            //            SP = SP.Next(2);
            //           SP.Previous(0);
            //            var JaggaerID = SP.JaggaerID;
            //            var BenNumber = SP.BENNumber;
            //            var RegTYpe = SP.RegistrationType;
            //            var RegStatus = SP.RegistrationStatus;

            //     SP.SelectedMenu = "About|General";
            //string sn = SP.GetSupplierField("Supplier Name");
            //           string vt = SP.GetSupplierField("Vendor Type");
            //         SP.SetSupplierField("Supplier Name", "Brian Caputo");
            //       SP.Save();
            //     SP.SetSupplierField("Supplier Name", sn);
            //   SP.Save();

            // SP.SetSupplierField("Vendor Type", "Individual");

            //            bool cXML = SP.cXML_Flag;
            //            SP.cXML_Flag = true;
            /*            SP.cXML_Flag = false;
                        SP.cXML_Flag = true;
                        SP.EDI_Flag = true;
                        SP.DisabledBusiness_Flag = true;
                        SP.ePayables_Flag = true;
                        SP.ForeignVendor_Flag = true;
                        SP.ISP_Flag = true;
                        SP.LGBTQ_Flag = true;
                        SP.MinorityBusiness_Flag = true;
                        SP.VeteranBusiness = true;
            */
            //          SP.WomenBusiness = true;
            //           SP.PO_Supplier_Flag = true;
            //           bool test3 = SP.Save();
            //        var test2 = SP.WomenBusiness;
            //           string tax = SP.TINInformation;
            //         SP.TINInformation = "0";
            //       SP.TINInformation = "";
            //            var legal = SP.GetSupplierField("Legal Structure");
            //          SP.SetSupplierField("Legal Structure", "Foreign Individual");
            //        var TIN = SP.GetSupplierField("Tax Identification Number");
            //      SP.SetSupplierField("Tax Identification Number", "9881234000");
            //    var PennID = SP.GetSupplierField("PennID");
            //  SP.SetSupplierField("PennID", "199999999");
            //var contract = SP.GetSupplierField("Contract?");
            //         var integration = SP.GetSupplierField("Integration");
            //var contract = SP.GetSupplierField("Contract?");
            //          var active = SP.GetSupplierField("Active for Shopping");
            //          SP.SetSupplierField("Active for Shopping", "true");
            //            var active = SP.Active4Shopping;
            //          SP.Active4Shopping = !active;
            //        var contract = SP.Contract;
            //      SP.Contract = !contract;
            //           SP.PaymentTerms = "Immediate-10005";
            //         SP.TermsBasis = "Invoice";
            //       var paymentTerms = SP.PaymentTerms;
            //     var termBasis = SP.TermsBasis;
            /*     var test = S P.GetPaymentMethods();
                 test[0].Link.Click();
                 var title = SP.PaymentMethods.Title;
                 SP.PaymentMethods.Title = title + "!";
                 var country = SP.PaymentMethods.Country;
                 var ERP = SP.PaymentMethods.ERP;
                 var Third = SP.PaymentMethods.ThirdParty;
                 var email = SP.PaymentMethods.RemitEmail;
                 var remit = SP.PaymentMethods.RemittanceAddress;
                 var active = SP.PaymentMethods.Active;
                 SP.PaymentMethods.Active = false;
             
            var testad = SP.Sites.SiteList;
            testad[1].Link.Click();
            var name = SP.Sites.Name;
            var AddrType = SP.Sites.AddressType;
            var AddrID = SP.Sites.AddressID;
            string[] add = { SP.Sites.Address1, SP.Sites.Address2, SP.Sites.Address3 };
            string[] ad2 = { SP.Sites.City, SP.Sites.State, SP.Sites.PostalCode, SP.Sites.Country };
            var prim = SP.Sites.Primary;
            var act = SP.Sites.Active;
            SP.Sites.Name = "Tesdt1";
            SP.Sites.Active = false;
            */
            TSM.QuickSearch("Jones, J", "Supplier Requests");
            var test = new NSR(TSM);
            var t2= test.GetSupplierField("goods/serv");
        }

        [TestMethod]
        public void DeactivateStudents()
        {
            var TSM = new JaggaerClient(true);
            var wb = new IronXL.WorkBook("C://Selenium/Students in PM.xlsx");
            WorkSheet ws = wb.DefaultWorkSheet;
            int rows = ws.RowCount;
            SupplierProfile sp;
            for(int i = 1; i < rows; i++)
            {
                var ben = ws.GetCellAt(i, 2).ToString();
                var jag = ws.GetCellAt(i, 0).ToString();
                //bool act = ws.GetCellAt(i, 6).ToString() == "TRUE";
                //if (!act)
                //{
                    PageResult pr=  TSM.QuickSearch(jag, "Supplier Profile").Select();
                    sp = new SupplierProfile(pr.Driver);
                    sp.VendorType = "Individual";                    
                    bool success = sp.Save();
                    if (success)
                    {
                        ws.SetCellValue(i, 7, "Updated to Individual - " + DateTime.Now.ToLongTimeString());
                    }
                    else
                    {
                        ws.SetCellValue(i, 7, "Failed to update ...");
                    }
                //}
                if (i % 5 ==0)
                    wb.SaveAs("C://Selenium/Students in PM.xlsx");
            }
        }
        
        /*
        [TestMethod]
        public void getContracts()
        {
            var d = jaggaer.Driver;
            var xl = new IronXL.WorkBook("C:\\Selenium\\ContractImport.csv");
            d.ExecuteJavaScript("window.open('https://box.upenn.edu')");

            var ws = xl.DefaultWorkSheet;
            var count = ws.RowCount;
            string BenID = "";
            string contractName;
            string contractType;
            for (int i = 1; i < count; i++)
            {
                contractName = ws.GetCellAt(i, 2).ToString();
                contractType = ws.GetCellAt(i, 3).ToString();
                BenID = ws.GetCellAt(i, 6).ToString();
                string xpath = "//div[@class='SelectorDropdown']/div/input";
                string ContractNumber = "";
                if (i % 200 == 0) { }
                ws.SaveAsCsv("C:\\Selenium\\ContractImport.csv");
            }
            ws.SaveAs("C:\\Selenium\\ContractImport.csv");
            int j = 0;
        }
        */
        [TestMethod]
        public void TryBenBuys()
        {
            OracleEBSClient Ben = new OracleEBSClient(true);
            Ben.SelectMenu("PO Sysadmin");
            Ben.SelectMenu("PO Sysadmin", "Suppliers");
            Ben.SelectMenu("PO Sysadmin", "Inquiry", "Suppliers");
        }
    }
}