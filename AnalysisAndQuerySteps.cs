using System;
using Blackbaud.UAT.Core.Base;
using Blackbaud.UAT.Core.Crm;
using TechTalk.SpecFlow;
using Keys = OpenQA.Selenium.Keys;
using Blackbaud.UAT.Core.Crm.Dialogs;
using Blackbaud.UAT.Core.Crm.Panels;

namespace UnitTestProject
{
    [Binding]
    public class AdHocQuerySteps : BaseSteps
    {
        [When(@"I add ad-hoc query type '(.*)'")]
        public void WhenIAddAd_HocQueryType(string queryType)
        {
            try
            {
                BBCRMHomePage.OpenAnalysisFA(); //open the analysis functional area
                AnalysisFunctionalArea.InformationLibrary(); //open the information library
                InformationLibraryPanel.SelectTab("Queries"); //Select the queries tab

                string querystring = "//div/table/tbody/tr//td/table/tbody/tr//td/div/../..//td/table/tbody/tr/td/em/button[./text() = 'Add an ad-hoc query']";

                InformationLibraryPanel.WaitClick(querystring, 10); //click on the "ad-hoc query" button

                Dialog.SetDropDown(Dialog.getXInput("AdHocQueryNewForm", "RECORDTYPE_value"), queryType); //start the adhoc query for the selected querytype
                string gridRowXPath = "";

                if (queryType == "Constituent")
                {
                    //Fundraisers
                    gridRowXPath = "//div[contains(@id,'AdHocQueryNewForm')]//div[contains(@id, 'GROUPTYPE-Constituent-bd')]/div[3]/table";
                }
                else if (queryType == "Revenue")
                {
                    gridRowXPath = "//div[contains(@id,'AdHocQueryNewForm')]//div[contains(@id, 'GROUPTYPE-Revenue-bd')]/div[2]/table";
                }
                InformationLibraryPanel.WaitClick(gridRowXPath, 20); //click on that row
                InformationLibraryPanel.WaitClick(Dialog.getXOKButton, 20);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add an adhoc query. " + ex.Message);
            }

        }
        [When(@"filter by '(.*)'")]
        public void WhenFilterBy(string filter)
        {
            try
            {
                AdHocQueryDialog.FilterBy(filter); //filter the query
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not filter an adhoc query. " + ex.Message);
            }
           
        }

        [When(@"add filter field '(.*)'")]
        public void WhenAddFilterField(string filterField, Table criteria)
        {
            try
            {  //select criteria for the filtering
                foreach (var criteriaRow in criteria.Rows)
                {
                    AdHocQueryDialog.AddFilterField(filterField, criteriaRow);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a filter field to an adhoc query. " + ex.Message);
            }
        }

        [When(@"add output fields")]
        public void WhenAddOutputFields(Table outputFields)
        {
            try
            { //select output fields for the query
                foreach (var outputField in outputFields.Rows)
                {
                    AdHocQueryDialog.FilterBy(outputField["Path"]);
                    AdHocQueryDialog.AddOutputField(outputField["Field"]);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add output fields to an adhoc query. " + ex.Message);
            }
        }

        [When(@"set save options")]
        public void WhenSetSaveOptions(Table saveOptions)
        {
            try
            {
                //set save options for the adhoc query
                foreach (var option in saveOptions.Rows)
                {
                    option["Name"] = option["Name"] + uniqueStamp;
                    AdHocQueryDialog.SetSaveOptions(option);
                }
                AdHocQueryDialog.SaveAndClose();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not set save options to an adhoc query. " + ex.Message);
            }
        }

        [Then(@"ad-hoc query '(.*)' is saved")]
        public void ThenAd_HocQueryIsSaved(string queryName)
        {
            //verify that the adhoc query is saved
            if (!AdHocQueryPanel.IsPanelHeader(queryName))
                throw new ArgumentException(
                    String.Format("'{0}' is not the query name of the current ad-hoc query panel.", queryName));
        }

        [When(@"I add smart query type '(.*)'")]
        public void WhenIAddSmartQueryType(string queryType)
        {
            try
            {
                BBCRMHomePage.OpenAnalysisFA();  // Open the analysis functional area
                AnalysisFunctionalArea.InformationLibrary(); //open the information library
                InformationLibraryPanel.SelectTab("Queries");  // select the queries tab

                string querystring = "//div/table/tbody/tr//td/table/tbody/tr//td/div/../..//td/table/tbody/tr/td/em/button[./text() = 'Add a smart query']";

                InformationLibraryPanel.WaitClick(querystring, 10); //click on the "add a smart query" button
                Dialog.SetDropDown(Dialog.getXInput("SmartQueryNewForm", "RECORDTYPE_value"), queryType); //start the adhoc query for the selected querytype

                string gridRowXPath = "//div[contains(@id,'SmartQueryNewForm')]//div[contains(@id, 'GROUPTYPE-Constituent-bd')]/div[6]/table";
                InformationLibraryPanel.WaitClick(gridRowXPath, 10); // click on that selected row
                InformationLibraryPanel.WaitClick(Dialog.getXOKButton, 10);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not set add a smart query. " + ex.Message);
            }

        }

        [Then(@"smart query '(.*)' is exported")]
        public void ThenSmartQueryIsSaved(string queryName)
        {
            
            try
            {
                string xpathstring = getXInputForQuery("_SmartQueryInstanceAddDataForm", "_SELECTIONID_value");

                Dialog.SetSearchList(Dialog.getXInput("_SmartQueryInstanceAddDataForm", "_SELECTIONID_value"), Dialog.getXInput("_SelectionSearchbyrecordtype", "_NAME_value"), queryName); 
                Dialog.SetDropDown("//input[contains(@id,'_DATEOPTION_value')]", "Less than"); //enter the start and end dates for the query
                Dialog.SetTextField("//input[contains(@id,'_DATENUMBER_value')]", "150");
                Dialog.ClickButton("Export to CSV", 50);
                System.Threading.Thread.Sleep(6000); //pause because otherwise the code runs so rapidly that the next line is not invoked in time

                
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not save a smart query. " + ex.Message);
            }
        }
        [When(@"I open a constituent export definition")]
        public void WhenIOpenAConstituentExportDefinition()
        {
            try
            {
                string getXSearchInput = "//div[contains(@class,'bbui-pages-contentcontainer') and not(contains(@class,'hide'))]//input[@placeholder='Search']";
                BBCRMHomePage.OpenFunctionalArea("Administration"); // Open the administration functional aera
                //click on the link to get export definition
                Dialog.WaitClick("//div[contains(h3,'Tools')]//button[contains(@class,'linkbutton')]/div[text()='Export definitions']", 10);
                Dialog.SetTextField(getXSearchInput, "UDO FYXX Research Deceased Export Definition"); // select this export def.
                Dialog.GetDisplayedElement(getXSearchInput).SendKeys(Keys.Enter);  // press the enter key
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not export a constituent definition. " + ex.Message);
            }

        }

        [Then(@"Verify it is editable")]
        public void ThenVerifyItIsEditable()
        {
            try
            {
                System.Threading.Thread.Sleep(2000); //pause because otherwise the code runs so rapidly that the next line is not invoked in time
                Dialog.WaitClick(Dialog.getXGridRowSelector("bbui-gen-wrapper", "ext-comp", 1), 5);  //click on the selected row
                Dialog.WaitClick(getXButtonEdit("Edit"), 10); //click on the edit button
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not verify that the export definition was editable. " + ex.Message);
            }
        }

        /// <param name="caption">The caption of the button.</param> 
        protected static string getXButtonEdit(string caption)
        {
            //determine the xml to select the edit button
            string FormattedString = "";
            FormattedString = String.Format("//*[./text()=\"{0}\" and contains(@class,\"x-btn-text\")]", caption);
            return FormattedString;
        }
        [When(@"I add a Constituent Adhoc Query type")]
        public void WhenIAddAConstituentAdhocQueryType()
        {
            try
            {
                BBCRMHomePage.OpenAnalysisFA(); //open the analysis functional area
                AnalysisFunctionalArea.InformationLibrary(); //open the information library
                InformationLibraryPanel.SelectTab("Queries"); //Select the queries tab

                string querystring = "//div[contains(@class,'bbui-pages-contentcontainer')" +
                    "and not(contains(@class,'hide'))]//div[not(contains(@class,'x-hide-display'))" +
                    "and contains(@class,'bbui-pages-pagesection') and not(contains(@class,'row'))]//div[contains(@id,'pageSection')]" +
                    "/div/table/tbody/tr//td/table/tbody/tr//td/div/../..//td/table/tbody/tr/td/em/button[./text() = 'Add an ad-hoc query']";

                InformationLibraryPanel.WaitClick(querystring, 10); //click on the "ad-hoc query" button

                string gridRowXPath = "//div[contains(@id,'GROUPTYPE-Most commonly used')]/div[1]/table/tbody/tr";   //select one of the rows for that query type
                InformationLibraryPanel.WaitClick(gridRowXPath, 10); //click on that row
                InformationLibraryPanel.WaitClick(Dialog.getXOKButton, 10);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a constituent ad hoc query type. " + ex.Message);
            }
        }
        [When(@"set query save options")]
        public void WhenSetQuerySaveOptions(Table saveOptions)
        {
            try
            {
                //save the details which are in the table for the Save Options for the query
                foreach (var option in saveOptions.Rows)
                {
                    option["Name"] = option["Name"] + uniqueStamp;
                    AdHocQueryDialog.SetSaveOptions(option);
                }
                AdHocQueryDialog.SaveAndClose(); // save and close the query
                System.Threading.Thread.Sleep(4000); //pause because otherwise the code runs so rapidly that the next line is not invoked in time
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Could not save the query options. " + ex.Message);
            }
        }
        [When(@"Setup an Export Process")]
        public void WhenStartAnExportProcess()
        {
            try
            {
                BBCRMHomePage.OpenFunctionalArea("Administration"); // Open admin functional area
                //click on the link to start export 
                Dialog.WaitClick("//div[contains(h3,'Tools')]//button[contains(@class,'linkbutton')]/div[text()='Export']", 10);
                string getXLookupField = "//div[contains(@class,'bbui-pages-contentcontainer') and not(contains(@class,'hide'))]//input[@placeholder='Search']";
                SearchDialog.SetTextField(getXLookupField, "ALL FYXX Alumni Export Process");
                SearchDialog.GetDisplayedElement(getXLookupField).SendKeys(Keys.Enter);  // press the enter key

                System.Threading.Thread.Sleep(4000); //pause because otherwise the code runs so rapidly that the next line is not invoked in time
              //  Dialog.WaitClick(Dialog.getXGridRowSelector("bbui-gen-wrapper", "ext-comp", 1), 5);  //click on the selected row
                string xLink = "//a[contains(@class,'bbui-pages-datalistgrid-rowlinklinkaction')]/span[text()='ALL FYXX Alumni Export Process']";
                Dialog.WaitClick(xLink, 20);
                xLink = "//button[contains(@class,'linkbutton')]/div[text()='Edit process']";
                Dialog.WaitClick(xLink, 20); //click on the edit button
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Could not start an export process, " + ex.Message);
            }

        }
        [When(@"Edit the Export Process '(.*)'")]
        public void WhenEditTheExportProcess(string QueryName)
        {
            try
            {
                QueryName += uniqueStamp;
                Dialog.SetSearchList(Dialog.getXInput("ExportProcessEditForm", "_SELECTIONID_value"), Dialog.getXInput("SelectionSearch", "_NAME_value"), QueryName); 
                Dialog.SetDropDown("//input[contains(@id,'_SITEID_value')]", "Department of Athletics");
                Dialog.SetSearchList(Dialog.getXInput("ExportProcessEditForm", "_EXPORTDEFINITIONID_value"), Dialog.getXInput("ExportDefinitionSearch", "_NAME_value"), "ALL FYXX Alumni Basic Export Definition"); 
                Dialog.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Could not edit the export process. " + ex.Message);
            }

        }
        [When(@"Start Process")]
        public void WhenStartProcess()
        {
            try
            {
                System.Threading.Thread.Sleep(5000); //pause because otherwise the code runs so rapidly that the next line is not invoked in time
                string xLink = "//button[contains(@class,'linkbutton')]/div[text()='Start process']";
                Dialog.WaitClick(xLink, 20); //click on the start process button
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Could not start the process. " + ex.Message);
            }
             
        }
        [Then(@"Download Export")]
        public void ThenDownloadExport()
        {
            String ErrMessage = "";
            try
            {
                Panel.GetEnabledElement("//span[contains(@id,'_STATUS_value') and ./text()='Completed']", 350); //350 is the time to wait in seconds

                Panel.GetEnabledElement("//button[./text()='Download output']", 100);
                Panel.WaitClick("//button[./text()='Download output']", 100);
                System.Threading.Thread.Sleep(5000); //pause because otherwise the code runs so rapidly that the next line is not invoked in time
                try
                {
                    Panel.WaitClick(getXMenuItem("Download to CSV"), 30);
                }
                catch (Exception )
                {

                }       
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Could not download the export. " + ErrMessage + " " + ex.ToString());
            }
        }

        /// <summary>
        /// Given the caption name of a menu item, return a unique identifier xPath
        /// to find a menu item.
        /// </summary>
        /// <param name="caption">The caption of the functional area.</param>
        protected static string getXMenuItem(string caption)
        {
            return String.Format("//div[contains(@class,'x-menu') and contains(@style,'visibility: visible')]//span[./text()='{0}' and @class='x-menu-item-text']", caption);
        }

        [Given(@"Open the '(.*)' Functional Area")]
        public void GivenOpenTheFunctionalArea(string areaName)
        {
            if (areaName == "Constituent")
            { 
                BBCRMHomePage.OpenConstituentsFA();  //Open constituents functional area
            }
            if (areaName == "Revenue")  
            {
                BBCRMHomePage.OpenRevenueFA();  // open revenue functional area
            }
            if (areaName == "Analysis")
            {
                BBCRMHomePage.OpenAnalysisFA();  // open analysis functional area
                //Open prospect management reports
                Dialog.WaitClick("//button[contains(@class,'linkbutton')]/div[text()='" + "Prospect management reports" + "']", 10);
            }
        }

        [When(@"I Go to the '(.*)' Report")]
        public void WhenIGoToTheReport(string reportName)
        {
            try
            {
                Dialog.WaitClick("//button[contains(@class,'linkbutton')]/div[text()='" + reportName + "']", 10);  // go to the selected report
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Could not go to the report. " + ex.Message);
            }
        }

        [Then(@"I run the Report")]
        public void ThenIRunTheReport()
        {
            try
            {
                string xPath = "//*[./text()='View report' and contains(@class, 'x-btn-text')]";  //run the selected report
                Dialog.WaitClick(xPath, 15);
                System.Threading.Thread.Sleep(9000);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Could not run the report. " + ex.Message);
            }
        }
        [When(@"Choose '(.*)', '(.*)', '(.*)', '(.*)'")]
        public void WhenChoose(string address, string city, string state, string zip)
        {
            try
            {
                //enter the address, city, state and zip to the report parameters
                string xAddressPath = "//input[contains(@id, '_DEST_ADDRESS_value')]";
                string xCityPath = "//input[contains(@id, '_DEST_CITY_value')]";
                string xStatePath = "//input[contains(@id, '_DEST_STATE_value')]";
                string xZipPath = "//input[contains(@id, '_DEST_ZIP_value')]";
                Dialog.SetTextField(xAddressPath, address);
                Dialog.SetTextField(xCityPath, city);
                Dialog.SetTextField(xStatePath, state);
                Dialog.SetTextField(xZipPath, zip);
                System.Threading.Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Could not enter the address into the report parameters. " + ex.Message);
            }

        }

        [When(@"Choose a PID '(.*)'")]
        public void WhenChooseAPID(string PID)
        {
            try
            {
                string xPath;
                xPath = "//textarea[contains(@id,'_INPID_value')]";
                Dialog.SetTextField(xPath, PID);  //enter ther PID number
                System.Threading.Thread.Sleep(5000);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Could not enter the lookupID (PID) as a report parameter. " + ex.Message);
            }
        }
        public static string getXInputForQuery(string dialogId, string inputId)
        {
            return String.Format("//div[contains(@class,'bbui-dialog') and contains(@style,'visible')]//div[contains(@id, '{0}') and contains(@class,'bui-forms-sizable')]//input[contains(@id, '{1}')]", dialogId, inputId);
        } 


    }  

 
}