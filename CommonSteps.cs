using Blackbaud.UAT.Core.Base;
using Blackbaud.UAT.Core.Crm;
using TechTalk.SpecFlow;
using OpenQA.Selenium;
using System.Configuration;
using System;

namespace UnitTestProject
{
    [Binding]
    public class CommonSteps : BaseSteps
    {
        static public string UserAccount;
        string[] creds = null;

        [Given(@"I have logged into the BBCRM home page")]
        public void GivenIHaveLoggedIntoTheBBCRMHomePage()
        {
            //Go to the home page
           // OpenQA.Selenium.IWebDriver driver = BBCRMHomePage.Driver;
            BBCRMHomePage.Login();
            UserAccount =  GetUserAccount();
       }
        private string GetUserAccount()
        {
            try
            {

                creds = ConfigurationManager.AppSettings["Credentials"].Split(':');  //read credentials from App.config
                string UserName = creds[0].ToString();  // find the username from the credentials in App.config
                return UserName;

            }
            catch (Exception e)
            {
                throw new Exception("Error: could not find the user account. " + e.Message);
            }
        }
    }
}
