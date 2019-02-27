using System;
using Blackbaud.UAT.Core.Base;
using Blackbaud.UAT.Core.Crm;
using TechTalk.SpecFlow;
using System.Collections.Generic;
using Blackbaud.UAT.Base;
using Keys = OpenQA.Selenium.Keys;
using Blackbaud.UAT.Core.Crm.Dialogs;
using Blackbaud.UAT.Core.Crm.Panels;

namespace UnitTestProject
{
    [Binding]
    
    public class AdminSteps : BaseSteps
    {

        [Given(@"I Add A Special Code '(.*)'")]
        public void GivenIAddASpecialCode(string codeName)
        {
            try
            {
                //I am using only part of the unique stamp since it is too long to fit into the code name input box
                string uniqueString = uniqueStamp;
                uniqueString = uniqueString.Replace(".", "");
                int len = uniqueString.Length;
                len -= 6;
                uniqueString = uniqueString.Substring(len);
                codeName += uniqueString;
                BBCRMHomePage.OpenFunctionalArea("Administration"); // open the administration functional area

                //click on the link to open form to add a new special code
                Panel.WaitClick("//button[contains(@class,'linkbutton')]/div[text()='UNC Manage Special Codes']", 10);
                Panel.ClickSectionAddButton("UNC Special Codes", "Add"); // press the add special code button

                //add the name and description
                Dialog.SetTextField("//div[contains(@id,'_UNCSpecialCodeDataFormAddDataForm')]//textarea[contains(@id,'DESCRIPTION_value')]", "Test Description"); //add in the desc.
                System.Threading.Thread.Sleep(2000); //delay so that the selected special code will be displayed

                Dialog.SetTextField("//div[contains(@id,'UNCSpecialCodeDataFormAddDataForm')]//input[contains(@id,'_SPECIALCODE_value')]", codeName);
       
                Dialog.Save();
                System.Threading.Thread.Sleep(5000); 
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not start to add a special code. " + ex.Message);
            }

        }

        [Then(@"a specialCode '(.*)' is added")]
        public void ThenASpecialCodeIsAdded(string codeName)
        {
            string sErrorMsg = "";
            try
            {
             
                //path for the edit button
                string xButtonPath = "//button[./text()='Edit' and contains(@class,'x-btn-text')]";
                //I am using only part of the unique stamp since it is too long to fit into the code name input box
                string uniqueString = uniqueStamp;
                uniqueString = uniqueString.Replace(".", "");
                int len = uniqueString.Length;
                len -= 6;
                uniqueString = uniqueString.Substring(len);
                codeName += uniqueString;

                //path for the description
                string xPath = "//div[contains(@id,'bbui-gen-pagecontainer')]//input[contains(@id, '_DESCRIPTION_value')]";
                //enter the special code in the search textbox

                Dialog.SetTextField("//div[contains(@id,'bbui-gen-pagecontainer')]//input[contains(@id, '_SPECIALCODE_value')]", codeName);
               //add the description
            
                Dialog.SetTextField(xPath, "Test Description");
                Dialog.GetDisplayedElement(xPath, 8).SendKeys(Keys.Enter); // the enter key will allwo the search to filter
                System.Threading.Thread.Sleep(2000); //delay so that the selected special code will be displayed
 
                Dialog.WaitClick(xButtonPath, 8); //if we can press the edit button, then the special code actually exists

            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not verify adding a special code. " + sErrorMsg + ex.Message );
            }
        }

    }
}

