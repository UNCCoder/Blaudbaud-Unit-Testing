using System;
using Blackbaud.UAT.Core.Base;
using Blackbaud.UAT.Core.Crm;
using TechTalk.SpecFlow;
using OpenQA.Selenium; 
using OpenQA.Selenium.Support.UI;
using Blackbaud.UAT.Core.Crm.Dialogs;
using Blackbaud.UAT.Core.Crm.Panels;


namespace UnitTestProject
{
    [Binding]
    public class ConstituentSearchSteps : BaseSteps
    {
        [When(@"Display The Recognition History Tab")]
        public void WhenDisplayTheRecognitionHistoryTab()
        {
            try
            {
                //display the recognition subtab under the Revenue tab for a constituent
                ConstituentPanel.SelectTab("Revenue");
                ConstituentPanel.SelectInnerTab("Recognition History");
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not display the recognition history tab. " + ex.Message);
            }
        }

        [Given(@"I have opened the constituent search dialog")]
        public void GivenIHaveOpenedTheConstituentSearchDialog()
        {
            //start a constituent search by opening the search dialog
            BBCRMHomePage.OpenConstituentsFA();  //Open constituent functional area
            ConstituentsFunctionalArea.OpenConstituentSearchDialog();
        }
        [When(@"I search for ""(.*)""")]
        public void WhenISearchFor(string name)
        {
            try
            {
                if (name != "Baltimore") {
                    name += uniqueStamp;
                }
                SearchDialog.SetLastNameToSearch(name); // search for the person's last name
                //For the advanced search options, allow for including organizations, groups and individuals
                SearchDialog.WaitClick("//a[contains(@id,'_SHOWADVANCEDOPTIONS_action')]", 20);
                SearchDialog.SetCheckbox("//input[contains(@id,'_INCLUDEORGANIZATIONS_value')]", true);
                SearchDialog.SetCheckbox("//input[contains(@id,'_INCLUDEGROUPS_value')]", false);
                SearchDialog.SetCheckbox("//input[contains(@id,'_INCLUDEINDIVIDUALS_value')]", true);
                //search for a specific last name
                SearchDialog.Search();

                if (name != "Baltimore") {          
                    SearchDialog.SelectFirstResult();
                }
        
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not search for specified name. " + ex.Message);
            }
        }

        //search for a specific first and last name
        [When(@"I search for '(.*)', '(.*)'")]
        public void WhenISearchFor(string firstname, string lastname)
        {
            try
            {
                //Display the advanced options because the necessary options are for an individual and not a group or org.
                SearchDialog.WaitClick("//a[contains(@id,'_SHOWADVANCEDOPTIONS_action')]", 20);
                SearchDialog.SetCheckbox("//input[contains(@id,'_INCLUDEORGANIZATIONS_value')]", false);
                SearchDialog.SetCheckbox("//input[contains(@id,'_INCLUDEGROUPS_value')]", false);
                SearchDialog.SetCheckbox("//input[contains(@id,'_INCLUDEINDIVIDUALS_value')]", true);
                SearchDialog.SetLastNameToSearch(lastname);
                SearchDialog.SetFirstNameToSearch(firstname);
                SearchDialog.Search();
                SearchDialog.SelectFirstResult();
                SearchDialog.ClickButton("Close", 50);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not search for specified first and last name. " + ex.Message);
            }

        }

        [Then(@"The results should contain ""(.*)""")]
        public void ThenTheResultsShouldContain(string result)
        {
            try
            {
                //verify that the results are what is needed
                SearchDialog.CheckConstituentSearchResultsContain(result);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not find correct results ( " + result + "). " + ex.Message);
            }
        }

        [When(@"I have opened the constituent search dialog")]
        public void WhenIHaveOpenedTheConstituentSearchDialog()
        {
            //start a constituent search by opening the search dialog
            BBCRMHomePage.OpenConstituentsFA();  //Open constituent functional area
            ConstituentsFunctionalArea.OpenConstituentSearchDialog();
        }

    }
}
