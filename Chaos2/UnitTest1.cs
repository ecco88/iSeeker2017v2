using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.ObjectModel;
using System.Threading;
using System.Text.RegularExpressions;
using WebClient;

namespace ThinClientTester
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestMethod1()
        {
            using var TSM = new JaggaerClient(true);
            var testitem = TSM.QuickSearch("Caputo", "Supplier Profile");
            var count = testitem.Count;
            var pagesize = testitem.PageSize;
            testitem.PageSize=(200);
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

            SP.SelectedMenu = "About|General";
            //string sn = SP.GetSupplierField("Supplier Name");
            //           string vt = SP.GetSupplierField("Vendor Type");
            //         SP.SetSupplierField("Supplier Name", "Brian Caputo");
            //       SP.Save();
            //     SP.SetSupplierField("Supplier Name", sn);
            //   SP.Save();

            // SP.SetSupplierField("Vendor Type", "Individual");

            bool cXML = SP.cXML_Flag;
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
            SP.WomenBusiness = true;
 //           SP.PO_Supplier_Flag = true;
            bool test3 = SP.Save();
            var test2 = SP.WomenBusiness;
        }
    }
}