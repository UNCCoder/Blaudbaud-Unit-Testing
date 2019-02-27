using System;
using Blackbaud.UAT.Core.Base;
using Blackbaud.UAT.Core.Crm;
using TechTalk.SpecFlow;
using System.Collections.Generic;
using Blackbaud.UAT.Base;
using Keys = OpenQA.Selenium.Keys;
using OpenQA.Selenium;
using Blackbaud.UAT.Core.Crm.Dialogs;
using Blackbaud.UAT.Core.Crm.Panels;

namespace UnitTestProject
{
    [Binding]
    public class PreliminarySteps
    {

        [When(@"I add registration options to event '(.*)'")]
        public void WhenIAddRegistrationOptionsToEvent(string eventName, Table registrationOptions)
        {
            try
            {

                foreach (var option in registrationOptions.Rows)
                {
                    if (!EventPanel.RegistrationOptionExists(option))
                        EventPanel.AddRegistrationOption(option);  //add a registration option
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a registration option. " + ex.Message);
            }
        }

        [When(@"I search for event '(.*)'")]
        public void WhenISearchForEvent(string eventName, Table events)
        {
            TableRow eventrow = events.Rows[0];
            try
            {
                BBCRMHomePage.OpenEventsFA();  // open event functional area
                try
                {
                    EventsFunctionalArea.EventSearch(eventName);  // search for an event
                }
                catch
                {
                    EventsFunctionalArea.AddEvent(eventrow);  //add an event if it does not exist
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not create a test event. " + ex.Message);
            }              
        }
        [When(@"I search for '(.*)' Organization")]
        public void WhenISearchForOrganization(string organizationName)
        {
            try
            {
                SearchDialog.SetLastNameToSearch(organizationName); // search for the person's last name
                //For the advanced search options, allow for including organizations, groups and individuals
                SearchDialog.WaitClick("//a[contains(@id,'_SHOWADVANCEDOPTIONS_action')]", 20);
                SearchDialog.SetCheckbox("//input[contains(@id,'_INCLUDEORGANIZATIONS_value')]", true);
                SearchDialog.SetCheckbox("//input[contains(@id,'_INCLUDEGROUPS_value')]", true);
                SearchDialog.SetCheckbox("//input[contains(@id,'_INCLUDEINDIVIDUALS_value')]", true);
                SearchDialog.Search();

                try
                {
                    //verify that the results are what is needed
                    SearchDialog.CheckConstituentSearchResultsContain(organizationName);
                    SearchDialog.SelectFirstResult();
                }
                catch
                {
                    Dialog.Cancel();
                    AddOrganization(organizationName);
                    AddMatchingGiftConditions();
                }
                
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not create a test organization. " + ex.Message);
            }
        }

        private void AddOrganization(string organizationName)
        {
            try
            {
                // add an organization with the org. name given
                ConstituentsFunctionalArea.AddAnIndividual("Organizations", "Add an organization");

                Dialog.SetTextField("//div[contains(@id, '_OrganizationAddForm')]//input[contains(@id,'_NAME_value')]", organizationName);
                Dialog.SetDropDown("//div[contains(@id, '_OrganizationAddForm')]//input[contains(@id,'_INDUSTRYCODEID_value')]", "Foundation");
                Dialog.SetTextField("//div[contains(@id, '_OrganizationAddForm')]//input[contains(@id,'_PHONE_PHONETYPECODEID_value')]", "Business");
                Dialog.SetTextField("//div[contains(@id, '_OrganizationAddForm')]//input[contains(@id,'_PHONE_NUMBER_value')]", "1 (111) 111-1111");
                Dialog.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not enter fields for a test organization. " + ex.Message);
            }
        }

        private void AddMatchingGiftConditions()
        {
            try
            {

                ConstituentPanel.SelectInfoTab();
                ConstituentPanel.SelectInnerTab("Organization");
                string xButtonPath = "//div[contains(@class,'bbui-pages-contentcontainer') and not(contains(@class,'hide'))]//div[not(contains(@class,'x-hide-display')) and contains(@class,'bbui-pages-pagesection') and not(contains(@class,'row'))]//div[contains(@id,'pageSection')]/div/table/tbody/tr//td/table/tbody/tr//td/div[./text() = 'Matching gift conditions']/../../../../../../../../../../../div[contains(@class,'bbui-pages-section-tbar')]//tr//button[text()='Add']";
                ConstituentPanel.WaitClick(xButtonPath, 20);
                Dialog.SetDropDown("//div[contains(@id, '_MatchingGiftConditionAddForm2')]//input[contains(@id,'_MATCHINGGIFTCONDITIONTYPECODEID_value')]", "Employee");
                Dialog.SetTextField("//div[contains(@id, '_MatchingGiftConditionAddForm2')]//input[contains(@id,'_MATCHINGFACTOR_value')]", "0.50");
                Dialog.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not enter fields for a test organization. " + ex.ToString());
            }
        }

        [Then(@"organization '(.*)' exists")]
        public void ThenOrganizationExists(string organizationName)
        {
                if (!Panel.IsPanelHeader(organizationName))
                {
                    throw new Exception("Error: could not verify that the organization exists. ");
                }
        }
        [When(@"I add test constituent ""(.*)"" Lastname ""(.*)""")]
        public void WhenIAddTestConstituentLastname(string firstName, string lastName, Table batchRows)
        {
            try
            {
                string fullName = lastName + ", " + firstName;
                if (batchRows.RowCount != 1)
                    throw new ArgumentException("Only provide one row to select.");
                var batchRow = batchRows.Rows[0];  // select only the first row of the batch

                BBCRMHomePage.OpenConstituentsFA();  //Open constituent functional area

                ConstituentsFunctionalArea.OpenConstituentSearchDialog();
                SearchDialog.SetLastNameToSearch(lastName);
                SearchDialog.SetFirstNameToSearch(firstName);
                SearchDialog.Search();
                try
                {
                    SearchDialog.CheckConstituentSearchResultsContain(fullName);
                    SearchDialog.SelectFirstResult();
                }
                catch
                {
                    Dialog.Cancel();
                    ConstituentsFunctionalArea.AddAnIndividual();
                    IndividualDialog.SetLastName(lastName);
                    IndividualDialog.SetIndividualFields(batchRow);
                    IndividualDialog.ClickButton("Validate", 50);
                    IndividualDialog.Save();
                }   
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a constituent with the specified last name. " + ex.Message);
            }
        }
        [Then(@"event ""(.*)"" has registration options")]
        public void ThenEventHasRegistrationOptions(string eventName, Table registrationOptions)
        {
                //GetEventPanel(eventName);  // open event panel  
                foreach (var option in registrationOptions.Rows)
                {
                    if (!EventPanel.RegistrationOptionExists(option))
                        throw new ArgumentException("'" + option +
                                                    "' was not found on the Registration options datalist for event " +
                                                    eventName + ".");
                }
        }

        private void GetEventPanel(string eventName)
        {
            try
            {
                if (!Panel.IsPanelHeader(eventName))
                {
                    BBCRMHomePage.OpenEventsFA();  // open event functional area
                    EventsFunctionalArea.EventSearch(eventName);  // search for an event
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not get the event panel. " + ex.Message);
            }
        }
        [When(@"I search for a marketing selection '(.*)'")]
        public void WhenISearchForAMarketingSelection(string selection)
        {
            try
            {

                BBCRMHomePage.OpenMarketingAndCommunicationsFA();  //Open marketing functional area
                string xPathLink = "//div[contains(@class,'bbui-pages-contentcontainer') and not(contains(@class,'x-hide-display'))]//button[contains(@class,'bui-linkbutton')]/div[text()='Selections']";
                Panel.WaitClick(xPathLink, 20);  // click on the selections link     
                xPathLink = "//button[contains(@class,'bui-linkbutton')]/div[text()='Selection search']";
                Panel.WaitClick(xPathLink, 20);  // click on the selection search link     
                string xSearchPath = "//*[contains(@class,'bbui-dialog-search') and not(contains(@style,'hidden'))]//*[starts-with(@id, 'ctrl_') and contains(@id, '_NAME_value')]";
                Dialog.SetTextField(xSearchPath, selection);
                Dialog.ClickButton("Search", 30);
                SearchDialog.SelectFirstResult();
                //save the selection
                Dialog.Save();
            }

            catch (Exception ex)
            {

                throw new Exception("Error: could not find the selection " + ex.Message);
            }
        }

        [Then(@"the Marketing selection '(.*)' exists")]
        public void ThenTheMarketingSelectionExists(string selection)
        {
            try
            {
                selection = "Edit Ad-Hoc Query - " + selection;
                Panel.GetEnabledElement("//span[contains(@class,'x-window-header-text') and ./text()='" + selection + "']", 20); //20 is the time to wait in seconds

            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not find the marketing selection. " + ex.Message);
            }
        }
        [When(@"I add selection ad-hoc query type '(.*)'")]
        public void WhenIAddSelectionAd_HocQueryType(string selection)
        {
            try
            {
                BBCRMHomePage.OpenMarketingAndCommunicationsFA(); //open the analysis functional area
                string xPathLink = "//div[contains(@class,'bbui-pages-contentcontainer') and not(contains(@class,'x-hide-display'))]//button[contains(@class,'bui-linkbutton')]/div[text()='Selections']";
                Panel.WaitClick(xPathLink, 20);  // click on the selections link     
  
                xPathLink = "//button[contains(@class,'bui-linkbutton')]/div[text()='Add a selection']";
                Panel.WaitClick(xPathLink, 20);  // click on the selections link   

                //string querystring = "//button[./text() = 'Add an ad-hoc query']";

                //Panel.WaitClick(querystring, 10); //click on the "ad-hoc query" button
                string queryType = "Constituent";
                Dialog.SetDropDown(Dialog.getXInput("AdHocQueryNewForm", "RECORDTYPE_value"), queryType); //start the adhoc query for the selected querytype
                string gridRowXPath = "";
                gridRowXPath = "//div[contains(@id,'AdHocQueryNewForm')]//div[contains(@id, 'GROUPTYPE-Constituent-bd')]/div[3]/table";

                Panel.WaitClick(gridRowXPath, 20); //click on that row
                Panel.WaitClick(Dialog.getXOKButton, 20);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add an adhoc query. " + ex.Message);
            }

        }
        [When(@"set save options for Exclusion")]
        public void WhenSetSaveOptions(Table saveOptions)
        {
            try
            {
                //set save options for the adhoc query
                foreach (var option in saveOptions.Rows)
                {
                    AdHocQueryDialog.SetSaveOptions(option);
                }
                AdHocQueryDialog.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not set save options to an adhoc query. " + ex.Message);
            }
        }

        [When(@"I search for a exclusion selection '(.*)'")]
        public void WhenISearchForAExclusionSelection(string selection)
        {
            IWebElement statusElement = null;

            try
            {

                BBCRMHomePage.OpenMarketingAndCommunicationsFA();  //Open marketing functional area
                string xPathLink = "//div[contains(@class,'bbui-pages-contentcontainer') and not(contains(@class,'x-hide-display'))]//button[contains(@class,'bui-linkbutton')]/div[text()='Selections']";
                Panel.WaitClick(xPathLink, 20);  // click on the selections link     
                xPathLink = "//button[contains(@class,'bui-linkbutton')]/div[text()='Selection search']";
                Panel.WaitClick(xPathLink, 20);  // click on the selection search link     
                string xSearchPath = "//*[contains(@class,'bbui-dialog-search') and not(contains(@style,'hidden'))]//*[starts-with(@id, 'ctrl_') and contains(@id, '_NAME_value')]";
                Dialog.SetTextField(xSearchPath, selection);
                Dialog.ClickButton("Search", 30);

                string xPath = "//div[contains(@class,'x-grid-empty')and text()='No records found']";
                // use x-grid-empty class to see if there was a result.
                statusElement = Panel.GetDisplayedElement(xPath, 15);
                Dialog.Cancel();
            }

            catch (Exception ex)
            {
                if (statusElement != null)
                {
                    throw new Exception("Error: Could not search for an exclusion. " + ex.Message);
                }
                else
                {
                    throw new Exception("Error: the exclusion already exists. " + ex.Message);
                }
                
            }        
        }

        protected string getXMenuItem(string caption = "")
        {
            return String.Format("//div[contains(@class,'x-menu') and contains(@style,'visibility: visible')]//span[./text()='{0}' and @class='x-menu-item-text']", caption);
        }

        [Given(@"Import Mapping File '(.*)', '(.*)'")]
        public void GivenImportMappingFile(string filename, string mappingname, Table mappingTable)
        {
            try
            {

                string batchtemplatename;

                if (mappingname.ToLower().Contains("interaction"))
                {
                    batchtemplatename = "Interaction Batch";
                }
                else
                {
                    batchtemplatename = "UNC Educational Involvement Import Batch";

                }
                BBCRMHomePage.OpenFunctionalArea("Administration"); //open the admin functional area and then click on import button
                Panel.WaitClick("//div[contains(h3,'Tools')]//button[contains(@class,'linkbutton')]/div[text()='Import']", 15);
                Panel.WaitClick("//a[contains(@class,'linkbutton')]/div[text()='Import file templates']", 10);

                //var mappingRow = mappingTable.Rows[0];
                //Panel.SelectSectionDatalistRow(mappingRow, "Import file templates");

                Panel.ClickSectionAddButton("Import file templates");
                Panel.WaitClick(getXMenuItem("Delimited template"));

                Dialog.SetTextField("//div[contains(@id, '_ImportDelimitedFileTemplateAddDataForm')]//input[contains(@id,'_NAME_value')]", mappingname);
                Dialog.SetDropDown("//div[contains(@id, '_ImportDelimitedFileTemplateAddDataForm')]//input[contains(@id,'_BATCHTEMPLATEID_value')]", batchtemplatename);
                Dialog.SetDropDown("//div[contains(@id, '_ImportDelimitedFileTemplateAddDataForm')]//input[contains(@id,'_IMPORTFILESERVER_value')]", filename);
                Dialog.ClickButton("Auto-map", 50);
                Dialog.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: The mapping file does not exist. Could not create it. " + ex.Message);
            }        
        }
        [Then(@"the mappingfile '(.*)' exists")]
        public void ThenThemappingfileExists(string mappingfilename)
        {
            try
            {
                string xPathString = "//div[contains (@title, '" + mappingfilename + "')]";
                Panel.WaitClick(xPathString, 25);
            }
            catch {
                string Target;
                Target = Panel.GetDisplayedElement("//div[contains(@class,'ext-mb-content')]//span[contains(@class,'ext-mb-text')]", 20).Text;
                // if that message does not exist or there is a different one, then send an error
                if ((Target.Substring(0, 24) != "Data could not be saved.") || (Target == null))
                {
                    throw new Exception("Error: An import mapping template file could not be created. ");
                }
                else
                {
                    Dialog.OK();
                }

            }
        }
    }
}
