using System;
using System.Collections.Generic;
using Blackbaud.UAT.Core;
using Blackbaud.UAT.Core.Base;
using Blackbaud.UAT.Core.Bbis;
using Blackbaud.UAT.Core.Crm;
using TechTalk.SpecFlow;
using OpenQA.Selenium;
using Blackbaud.UAT.Core.Crm.Dialogs;
using Blackbaud.UAT.Core.Crm.Panels;

namespace UnitTestProject
{
    [Binding]
    public class DirectMarketingSteps : BaseSteps
    {
        public string UserName = CommonSteps.UserAccount.ToLower();

        protected static readonly string[] DialogIds = { "MarketingAcknowledgementTemplateAddForm" };

        [When(@"I create a receipt process")]
        public void WhenICreateAReceiptProcess(Table receipts)
        {
            try
            {
                foreach (var receipt in receipts.Rows)
                {
                    BBCRMHomePage.OpenMarketingAndCommunicationsFA();  //Open marketing functional area
                    MarketingAndCommFunctionalArea.Receipts();  // Open receipts area
                    if (receipt.ContainsKey("Name") && !string.IsNullOrEmpty(receipt["Name"]))
                        receipt["Name"] += uniqueStamp;
                    ReceiptsPanel.AddReceipt(receipt);  // add a receipt
                }
            }
            catch (Exception ex)
            {
                    throw new Exception("Error: could not create a receipt process. " + ex.Message);
            }
        }

        [When(@"run receipt process")]
        public void WhenRunReceiptProcess(Table receiptProcesses)
        {
            try
            {
                foreach (var receiptProcess in receiptProcesses.Rows)
                {
                    BBCRMHomePage.OpenMarketingAndCommunicationsFA();  //Open marketing functional area
                    MarketingAndCommFunctionalArea.Receipts(); //Open receipts area
                    if (receiptProcess.ContainsKey("Name") && !string.IsNullOrEmpty(receiptProcess["Name"]))
                        receiptProcess["Name"] += uniqueStamp;
                    ReceiptsPanel.RunReceiptProcess(receiptProcess);  // run a receipt process
                }
            }

            catch (Exception ex)
            {
                    throw new Exception("Error: could not run a receipt process. " + ex.Message);
            }
        }
        [Given(@"Setup An Acknowledgement '(.*)'")]
        public void GivenSetupAnAcknowledgement(string processName)
        {
            try
            {
                BBCRMHomePage.OpenMarketingAndCommunicationsFA();  //Open marketing functional area
                string xPathLink = "//div[contains(@class,'bbui-pages-contentcontainer') and not(contains(@class,'x-hide-display'))]//button[contains(@class,'bui-linkbutton')]/div[text()='Acknowledgements']";
                Panel.WaitClick(xPathLink, 50);  // click on the Acknowledgements link
                Panel.SelectTab("Letters");  //select Letters tab

                string xPath = "//div[contains (@title, '" +  processName + "')]/a[./text()='" + processName + "']";
                Panel.WaitClick(xPath, 15);  // click on the selected anchor link according to the xPath above.
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not setup an acknowledgement. " + ex.Message);
            }
        }

        [Given(@"Run the Process")]
        public void GivenRunTheProcess()
        {
            try
            {
                Dialog.WaitClick("//button[contains(@class,'bbui-linkbutton')]/div[text()='Start process']", 30); // start the assign letters process

                Dialog.SetCheckbox("//input[contains(@id, '_USEDATEFILTER_value')]", true);  //  only consider revenue since
                Dialog.SetCheckbox("//input[contains(@id, '_DATAFILTERRADIO_0')]", true);  //  only since last run

                //if we want a date, then use these two below
                //  Dialog.SetCheckbox("//input[contains(@id, '_DATAFILTERRADIO_1')]", true);  //since specified date
                //Dialog.SetTextField("//input[contains(@id, '_DATELASTRUN_value')]", "10/10/2016");  //set a date

                Dialog.ClickButton("Start", 50);  //click on the start button
                System.Threading.Thread.Sleep(3000);  //allow some time for the process to actually run.
                Dialog.GetEnabledElement("//span[contains(@id,'_STATUS_value') and ./text()='Completed']", 350); //350 is the time to wait in seconds for a completed message
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not run an acknowledgement process. " + ex.Message);
            }
        }

        [Then(@"Export the Acknowledgement '(.*)'")]
        public void ThenExportTheAcknowledgement(string processName)
        {
            string xPath = "//div[contains(@id,'-formattedValues[7]-Revenue-bd')]//div[contains(@title,'" + processName + "')]/a[1][text()='" + processName + "']";
            string ErrString = "";

            try
            {
                //click on the Acknowledge revenue list link
                Panel.WaitClick("//button[contains(@class,'bbui-linkbutton')][text()='Acknowledge revenue list']", 80); // return to the acknowledgement page
                Panel.SelectTab("Acknowledgements");
                Panel.WaitClick(xPath, 20);
                Dialog.WaitClick("//button[contains(@class,'bbui-linkbutton')]/div[text()='Start process']", 30); // start the assign letters process
                Dialog.ClickButton("Start", 50);  //click on the start button
                System.Threading.Thread.Sleep(3000);  //allow some time for the process to actually run.
                //350 is the maximum time to wait in seconds for the process to display a completed message
                Dialog.GetEnabledElement("//div[contains(@id,'_AcknowledgementProcessRecentStatusViewForm')]//span[contains(@id,'_STATUS_value') and ./text()='Completed']", 350);

                //export the results
                try
                {
                    Dialog.WaitClick("//*[./text()=\"Download output\"]", 20);
                    Dialog.WaitClick("//*[./text()=\"Download to CSV\"]", 20);
                    System.Threading.Thread.Sleep(5000);  //allow some time for the process to actually run.
                }
                catch (Exception exc)
                {
                    ErrString = "Error: could not download the acknowlegement results since there were no records. ";
                    throw new Exception(ErrString + exc.Message);
                }
            }
            catch (Exception ex)
            {
                if (String.IsNullOrEmpty(ErrString)) {
                    ErrString =" Error: could not export an acknowledgement process. ";
                    throw new Exception(ErrString + ex.Message);
                }
                else
                {
                    throw new Exception(ErrString);
                }
            }

        }

        [Given(@"Edit a General Correspondence Description '(.*)'")]
        public void GivenEditAGeneralCorresopndenceDescription(String correspondenceName, Table correspondenceTable)
        {
            try
            {
                TableRow CorrespondenceRow = correspondenceTable.Rows[0];
                correspondenceName += uniqueStamp;   //The unique stamp is a series of numbers to keep names different from each other

                if (CorrespondenceRow.Keys.Contains("Name") && (!String.IsNullOrEmpty(CorrespondenceRow["Name"])))
                {
                    CorrespondenceRow["Name"] += uniqueStamp;
                }
                BBCRMHomePage.OpenMarketingAndCommunicationsFA();  //Open marketing functional area

                string xPathLink = "//div[contains(@class,'bbui-pages-contentcontainer') and not(contains(@class,'x-hide-display'))]//button[contains(@class,'bui-linkbutton')]/div[text()='Manage correspondence']";
                Panel.WaitClick(xPathLink, 50);  // click on the General correspondence link}

                Panel.SelectSectionDatalistRow(CorrespondenceRow, "UNC Correspondence processes");  //find the correct row which has the correct general correspondence process
                Panel.WaitClick(Panel.getXSelectedDatalistRowButton("Edit"));
                Dialog.SetTextField("//input[contains(@id,'_DESCRIPTION_value')]", "Test Desc Edited Now"); //change the desc.
                Dialog.SetTextField("//textarea[contains(@id,'_COMMENTS_value')]", "This is an edited comment for " + DateTime.Today.ToShortDateString());  //change the comment

            }
            catch (Exception ex )
            {
                if (UserName != "bbtest2.gst")
                {
                    throw new Exception("Error: could not edit a general correspondence. " + ex.Message);
                }
            }          
        }


        [Given(@"Change Query and Exclusions")]
        public void GivenChangeQueryAndExclusions()
        {
            int rowCount = 1;
            string formName = "_BusinessProcessCommPrefForm";

            try
            {

                //change query for the general correspondence
                Dialog.SetSearchList(Dialog.getXInput("_CorrespondenceProcessEditForm3", "_IDSETREGISTERID_value"), Dialog.getXInput("_SelectionSearch", "_NAME_value"), "UDO DEV FY16 UNC vs. UCLA Basketball Game Event Additional Invitee"); 

                //change one of the exclusions.
                Dialog.ClickButton("Exclusions", 50);
                string gridXPath = Dialog.getXGridCell("dataformdialog_", formName, rowCount, 3);
                Dialog.SetGridDropDown(gridXPath, "UDO-No Email Communications - University Development Office");
                Dialog.OK();
                Dialog.Save();

            }
            catch (Exception ex)
            {
                if (UserName != "bbtest2.gst")
                {
                    throw new Exception("Error: could not change the query or exclusion in  general correspondence. " + ex.Message);
                }
            }   
        }

        [Given(@"Setup A General Correspondence '(.*)'")]
        public void GivenSetupAGeneralCorrespondence(string correspondenceName)
        {
            try
            {
                int rowCount = 1;
                string formName = "_BusinessProcessCommPrefForm";
                correspondenceName += uniqueStamp;   //The unique stamp is a series of numbers to keep names different from each other
                BBCRMHomePage.OpenMarketingAndCommunicationsFA();  //Open marketing functional area

                string xPathLink = "//div[contains(@class,'bbui-pages-contentcontainer') and not(contains(@class,'x-hide-display'))]//button[contains(@class,'bui-linkbutton')]/div[text()='Manage correspondence']";
                Panel.WaitClick(xPathLink, 50);  // click on the General correspondence link}

                // set fields on the add correspondence dialog box
                Panel.ClickSectionAddButton("UNC Correspondence processes");
   
                Dialog.SetTextField("//input[contains(@id,'_NAME_value')]", correspondenceName);
                Dialog.SetTextField("//input[contains(@id,'_DESCRIPTION_value')]", "Test Desc");
                Dialog.SetDropDown("//input[contains(@id,'_OUTPUTTYPE_value')]", "Export definition");

                Dialog.SetSearchList(Dialog.getXInput("_CorrespondenceProcessAddForm3", "_EXPORTDEFINITIONID_value"), Dialog.getXInput("_ExportDefinitionSearch2", "_NAME_value"), "UDO FYXX Special Events Export Definition");
                Dialog.SetSearchList(Dialog.getXInput("_CorrespondenceProcessAddForm3", "_IDSETREGISTERID_value"), Dialog.getXInput("_SelectionSearch", "_NAME_value"), "UDO FY17 McCleery Prospects"); 

                Dialog.SetCheckbox("//input[contains(@id,'_UPDATECONSTITUENTRECORD_value')]", true);
                Dialog.SetDropDown("//input[contains(@id,'_CORRESPONDENCECODEID_value')]", "UDO-General Correspondence");
                Dialog.SetTextField("//textarea[contains(@id,'_COMMENTS_value')]", "This is a general correspondence comment for " + DateTime.Today.ToShortDateString());

                //ad exclusions
                Dialog.ClickButton("Exclusions", 50);  
                string gridXPath = Dialog.getXGridCell("dataformdialog_", formName, rowCount, 3);
                Dialog.SetGridDropDown(gridXPath, "UDO-No Email Events - University Development Office");
                rowCount++;
                gridXPath = Dialog.getXGridCell("dataformdialog_", formName, rowCount, 3);
                Dialog.SetGridDropDown(gridXPath, "UDO-No Email Solicitations - University Development Office");


                Dialog.OK();
                Dialog.Save();  // save the input

            }
            catch (Exception ex )
            {
                if (UserName != "bbtest2.gst")
                {
                    throw new Exception("Error: could not set up a general correspondence. " + ex.Message);
                }
            }          
        }

        [Given(@"Run the Correspondence Process '(.*)'")]
        public void GivenRunTheCorrespondenceProcess(string correspondenceName)
        {

            correspondenceName += uniqueStamp;
            string xFilterPath = "//div[contains(@class, 'bbui-pages-contentcontainer') and not(contains (@class, 'x-hide-display'))]//input[contains(@id,'_SITEFILTERMODE_value')]";

            try
            {
                try
                {

                    // verify whether the filter mode dropdown appears
                    Dialog.GetEnabledElement(xFilterPath, 5);
                }
                catch (WebDriverTimeoutException)
                {
                    //if not, then click on the filter button to make it appear
                    string xButtonPath = "//button[./text()='Filters' and contains(@class,'x-btn-text')]";
                    Dialog.WaitClick(xButtonPath, 15);
                }

                Dialog.SetDropDown(xFilterPath, "My site"); //Select My Site from the filter mode dropdown
                xFilterPath = "//div[contains(@class, 'bbui-pages-contentcontainer') and not(contains (@class, 'x-hide-display'))]//button[text()='Apply']";
                Panel.WaitClick(xFilterPath, 20);  //click on the apply button
                string xPath = "//div/table/tbody/tr[1]/td[3]/div/a[./text() = '" + correspondenceName + "']";
                Panel.WaitClick(xPath, 50);  // click on the specific name for the correspondence
                Dialog.WaitClick("//button[contains(@class,'bbui-linkbutton')]/div[text()='Start process']", 30); // start the correspondence process
                Dialog.ClickButton("Start", 50); // click the start button
                System.Threading.Thread.Sleep(6000);  //allow some time for the process to actually run.
                Dialog.GetEnabledElement("//div[contains(@id,'_BusinessProcessParameterSetRecentStatusViewForm')]//span[contains(@id,'_STATUS_value') and ./text()='Completed']", 350);
            }
            catch (Exception ex)
            {
                if (UserName != "bbtest2.gst")
                {
                    throw new Exception("Error: could not run a general correspondence. " + ex.Message);
                }
            }       
        }

        [Then(@"Export the Correspondence")]
        public void ThenExportTheCorrespondence()
        {
            //export the results of the general correspondence
            try
            {
                Dialog.WaitClick("//*[./text()=\"Download output\"]", 20);
                Dialog.WaitClick("//*[./text()=\"Download to CSV\"]", 20);
                System.Threading.Thread.Sleep(6000);  //allow some time for the process to actually run.
            }

            catch (Exception exc)
            {
                if (UserName != "bbtest2.gst")
                {
                    throw new Exception("Error: download the correspondence results since there were no records. " + exc.Message);
                }
            }
        }

        [Given(@"Create an Email Package '(.*)'")]
        public void GivenCreateAnEmailPackage(string emailPackageName)
        {
            try
            {
                BBCRMHomePage.OpenMarketingAndCommunicationsFA();  //Open marketing functional area
                Dialog.WaitClick("//button[contains(@class,'bbui-linkbutton')]/div[text()='Packages']", 30); // start the assign letters process
                Dialog.WaitClick("//button[contains(@class,'bbui-linkbutton')]/div[text()='Add an email package']", 30); // start the assign letters process
                AddEmailPackage(emailPackageName);
                System.Threading.Thread.Sleep(9000);  //allow some time for the process to actually run.
            }
            catch (Exception exc)
            {
                throw new Exception("Error: could not create an email package. " + exc.Message);
            }
        }

        private void AddEmailPackage(string emailPackageName)
        {

            //Add the input to the various fields on the add package dialog box
            Dialog.SetTextField("//input[contains(@id,'_NAME_value')]", emailPackageName);
            Dialog.SetTextField("//textarea[contains(@id,'_DESCRIPTION_value')]", "Test Desc");
            Dialog.SetTextField("//input[contains(@id,'_SITEID_value')]", "Ackland Art Museum");
            Dialog.SetDropDown("//input[contains(@id,'_CATEGORYCODEID_value')]", "Email");
            Dialog.SetDropDown("//input[contains(@id,'_CODEVALUEID_value')]", "Package code");
            Dialog.Save();  // save the package
        }

        [Given(@"Add A Constituent Segment '(.*)'")]
        public void GivenAddAConstituentSegment(string segmentName )
        {
            segmentName += uniqueStamp;   //The unique stamp is a series of numbers to keep names different from each other
            BBCRMHomePage.OpenMarketingAndCommunicationsFA();  //Open marketing functional area
            string xPathLink = "//div[contains(@class,'bbui-pages-contentcontainer') and not(contains(@class,'x-hide-display'))]//button[contains(@class,'bui-linkbutton')]/div[text()='Segments']";
            Panel.WaitClick(xPathLink, 50);  // click on the Segments link     
            Panel.ClickSectionAddButton("Segments"); //click on the Segments link
            Panel.WaitClick(getXMenuItem("Constituent segment"));  //add a new Constituent segment
            AddSegment(segmentName);
        }
        [Given(@"Add A List Segment '(.*)'")]
        public void GivenAddAListSegment(string listSegmentName)
        {
            Panel.ClickSectionAddButton("Segments"); //click on the Segments link
            Panel.WaitClick(getXMenuItem("List segment"));  //add a new list segment
            AddListSegment(listSegmentName);
        }
        private void AddListSegment(string segmentName)
        {
            // input the data into the various fields on the add segment dialog box
            Dialog.SetTextField("//input[contains(@id,'_NAME_value')]", segmentName);
            Dialog.SetTextField("//textarea[contains(@id,'_DESCRIPTION_value')]", "Test Desc");

            Dialog.SetSearchList(Dialog.getXInput("_ListSegmentAddForm", "_SITEID_value"), Dialog.getXInput("searchdialog_", "_NAME_value"), "Ackland Art Museum");

            Dialog.SetDropDown("//input[contains(@id,'_CODEVALUEID_value')]", "Segment code");
            string xSearchPath;
            // selection name
            xSearchPath = "//*[contains(@class,'bbui-dialog-search') and not(contains(@style,'hidden'))]//*[starts-with(@id, 'ctrl_') and contains(@id, '_NAME_value')]";
            Dialog.SetTextField(xSearchPath, segmentName);
            Dialog.ClickButton("Search", 30);
            SearchDialog.SelectFirstResult();
            //save the segment
            //Dialog.Save();
        }

        private void AddSegment(string segmentName)
        {
            // input the data into the various fields on the add segment dialog box
            Dialog.SetTextField("//input[contains(@id,'_NAME_value')]", segmentName);
            Dialog.SetTextField("//textarea[contains(@id,'_DESCRIPTION_value')]", "Test Desc");
           // Dialog.SetTextField("//input[contains(@id,'_SITEID_value')]", "Ackland Art Museum");
           // 
            Dialog.SetSearchList(Dialog.getXInput("_ConstituentSegmentAddForm", "_SITEID_value"), Dialog.getXInput("searchdialog_", "_NAME_value"), "Ackland Art Museum"); 

            Dialog.SetDropDown("//input[contains(@id,'_CODEVALUEID_value')]", "Segment code");
            string xSearchPath;
            //click on add a selection
            xSearchPath = "//table[contains(@id,'_SELECTIONADD_action')]//button[text()='Add']";
            Dialog.WaitClick(xSearchPath, 15);
            // selection name
            xSearchPath = "//*[contains(@class,'bbui-dialog-search') and not(contains(@style,'hidden'))]//*[starts-with(@id, 'ctrl_') and contains(@id, '_NAME_value')]";
            Dialog.SetTextField(xSearchPath, "ACK FY15 Spring Annual Giving Additions Selection (Ad-hoc Query)");
            Dialog.ClickButton("Search", 30);
            SearchDialog.SelectFirstResult();
            //save the segment
            Dialog.Save();
        }
        [When(@"Add Segment '(.*)' and Package '(.*)' and List Segment '(.*)' To Effort")]
        public void WhenAddSegmentAndPackageToEffort(string segmentName, string emailPackageName, string listSegmentName, Table batchRows)
        {
            segmentName += uniqueStamp;   //The unique stamp is a series of numbers to keep names different from each other
            //emailPackageName += uniqueStamp;
            AddMarketingSegment("Constituent", segmentName, emailPackageName, batchRows);  //add marketing segment to marketing effort
            AddMarketingSegment("List", listSegmentName , emailPackageName, batchRows); //add a segment list to the marketing effort
        }

        public void AddMarketingSegment(string segmentType, string segmentName, string emailPackageName, Table batchRows){

            int rowCount = 1;

            //click on the add marketing segment button
            string xPathTab = "//span[./text()='Segments' and contains(@class,'x-tab-strip-text')]";
            string xSearchPath = "//table[contains(@id,'_ADDACTIONGROUP_value')]//button[text()='Add']";
            string MenuItemName;
            string formName;

            if (segmentType == "Constituent")  //add a constituent segment
            {
                MenuItemName = "Constituent segment";
                formName = "_MarketingEffortSegmentAddForm";
            }
            else
            {
                MenuItemName = "List segment";  //add a list segement
                formName = "_MarketingEffortListSegmentAddForm";
            }
            
            try
            {
                Dialog.WaitClick(xPathTab, 190);  //go to the Segments tab
                Panel.WaitClick(xSearchPath, 190);  //Add a new segment
                Panel.WaitClick(getXMenuItem(MenuItemName));  //click on the constituent segment item
        
                // select a segment and a package
                Dialog.SetSearchList(Dialog.getXInput(formName, "_SEGMENTID_value"), Dialog.getXInput("searchdialog_", "_NAME_value"), segmentName);
                Dialog.SetSearchList(Dialog.getXInput(formName, "_PACKAGEID_value"), Dialog.getXInput("searchdialog_", "_NAME_value"), emailPackageName);

                Dialog.OpenTab("Source Code");

                //map the column captions to their index
                IDictionary<string, int> columnCaptionToIndex = new Dictionary<string, int>();
                foreach (string caption in batchRows.Rows[0].Keys)
                {
                    columnCaptionToIndex.Add(caption,
                        Dialog.GetDatalistColumnIndex(Dialog.getXGridHeaders("dataformdialog_", formName), caption));
                }
                //add the rows
                foreach (TableRow row in batchRows.Rows)
                {
                    foreach (string caption in row.Keys)
                    {
                        //Add if value provided using appropriate field set.  Use rowCount and column caption mapping when making XPath
                        if (row[caption] == string.Empty) continue;
                        string gridXPath = Dialog.getXGridCell("dataformdialog_", formName, rowCount, columnCaptionToIndex[caption]);
                        string gridRowXPath = Dialog.getXGridRow("dataformdialog_", formName, rowCount);
                        string value = row[caption];
                        switch (caption)
                        {
                            //add the values to the various columns (which have the following captions)

                            case "Format":
                                Dialog.SetGridDropDown(gridXPath, value);
                                break;
                            case "Value":
                                Dialog.SetGridTextField(gridXPath, value);
                                break;

                            default:
                                throw new NotSupportedException(String.Format("Column '{0}' is not supported by the default AddPlanOutline method.  Additional implementation is required.", caption));
                        }
                    }
                    rowCount++;
                }
                Dialog.Save();
            }
            catch (Exception exc)
            {
                throw new Exception("Error: could not add segments and a package to a marketing effort. " + exc.ToString());
            }
        }

        [When(@"I Create a Marketing Effort '(.*)'")]
        public void WhenICreateAMarketingEffort(string marketingName)
        {
            try
            {
                marketingName += uniqueStamp;   //The unique stamp is a series of numbers to keep names different from each other
                BBCRMHomePage.OpenMarketingAndCommunicationsFA();  //Open marketing functional area
                string xPathLink = "//div[contains(@class,'bbui-pages-contentcontainer') and not(contains(@class,'x-hide-display'))]//button[contains(@class,'bui-linkbutton')]/div[text()='Direct marketing efforts']";
                Panel.WaitClick(xPathLink, 50);  // click on the Direct Marketing Efforts link     
                Panel.ClickSectionAddButton("Direct marketing efforts");    // click on Direct marketing effots link
                string xPath = "//div[contains(@id,'_TEMPLATES_value')]//div[text()='ACK FYXX E-Mail Marketing Effort Template']";
                Dialog.WaitClick(xPath, 45);  // select the template: 'ACK FYXX E-Mail Marketing Effort Template'
                Dialog.ClickButton("Next", 30);

                //input values into the create effort fields 
                Dialog.SetTextField("//input[contains(@id,'_FREEFORMPART_value')]", marketingName);
                Dialog.SetTextField("//textarea[contains(@id,'_EFFORTDESCRIPTION_value')]", "Test Desc");
                Dialog.SetTextField("//input[contains(@id,'EFFORTSITEID_value')]", "Ackland Art Museum");
                Dialog.SetSearchList(Dialog.getXInput("_CREATEEFFORTTAB_caption_tab", "_INDIVIDUALAPPEAL_value"), Dialog.getXInput("_AppealSearch", "_NAME_value"), "17Y - 1/12 Appeal Former Charlotte");
                Dialog.SetTextField("//input[contains(@id,'_EFFORTLAUNCHDATE_value')]", "11/17/2017");
                
                Dialog.Save(); // Save the fields
            }
            catch (Exception exc)
            {
                throw new Exception("Error: could not create a marketing effort. " + exc.ToString());
            }

        }

        protected string getXMenuItem(string caption = "")
        {
            return String.Format("//div[contains(@class,'x-menu') and contains(@style,'visibility: visible')]//span[./text()='{0}' and @class='x-menu-item-text']", caption);
        }

        [When(@"Add Export definition '(.*)' for '(.*)'")]
        public void WhenAddExportDefinition(string exportName, string marketingName)
        {
            try
            {

                marketingName += uniqueStamp;   //The unique stamp is a series of numbers to keep names different from each other
                string xLinkPath = "//button[contains(@class,'bui-linkbutton')]/div[text()='Go to " + marketingName + "']";
                Panel.WaitClick(xLinkPath, 35);  //click on the go to link that has the current marketing effort name

                xLinkPath = "//button[contains(@class,'bui-linkbutton')]/div[text()='Export effort']";
                Panel.WaitClick(xLinkPath, 45);  //click on the export effort link
                Dialog.SetTextField("//textarea[contains(@id,'_DESCRIPTION_value')]", "Test Description"); // enter an effort desc.

                Dialog.ClickButton("Start", 30);  //start the process
                xLinkPath = "//div[text()='" + exportName + "']";
                Panel.GetEnabledElement("//span[contains(@id,'_STATUS_value') and ./text()='Completed']", 350); //350 is the time to wait in seconds

                Dialog.WaitClick(xLinkPath, 100);
                //download a CSV file
                Dialog.WaitClick("//button[./text()='Download output']", 30);
                Panel.WaitClick(getXMenuItem("Download to CSV"), 30);
                System.Threading.Thread.Sleep(6000);  //allow some time for the process to actually run.
            }
            catch (Exception exc)
            {
                throw new Exception("Error: could not add an export definition. " + exc.Message);
            }
        }

        [When(@"Calculate Segment Counts '(.*)'")]
        public void WhenCalculateSegmentCounts(string marketingName)
        {
            marketingName += uniqueStamp;   //The unique stamp is a series of numbers to keep names different from each other
            try
            {
                
                string xPathLink = "//button[contains(@class,'bui-linkbutton')]/div[text()='Calculate segment counts']";
                Panel.WaitClick(xPathLink, 35); // click on the calculate segment counts
                Dialog.ClickButton("Start", 50);  // start button
                Dialog.GetEnabledElement("//span[contains(@id,'_STATUS_value') and ./text()='Completed']", 480); //480 is the time to wait in seconds

                Dialog.WaitClick("//button[contains(@class,'linkbutton')]/div[text()='Go to " + marketingName + "']", 10); // go to the marketing effort itself
            }
            catch (Exception exc)
            {
                throw new Exception("Error: could not create a marketing effort. " + exc.Message);
            }
        }
        [Then(@"the Marketing Effort '(.*)' has been created and exported")]
        public void ThenTheMarketingEffortHasBeenCreatedAndExported(string marketingName)
        {
            try
            {
                marketingName += uniqueStamp;   //The unique stamp is a series of numbers to keep names different from each other
                string xPath = "//h2[contains(@class,'bbui-pages-header')]//span[text()='Marketing Effort Export Status for " + marketingName + "']";
                Dialog.GetEnabledElement(xPath, 50);  // See if the Export Status Header for the marketing effort exists which lets us know it is available.
            }
            catch (Exception exc)
            {
                throw new Exception("Error: could not verify that a marketing effort was created and exported. " + exc.Message);
            }
        }
        [When(@"An Exclusion '(.*)' is Added So Recalculate")]
        public void WhenAnExclusionIsAddedSoRecalculate(string exclusionName)
        {
            try
            {
                string xPathTab = "//span[./text()='Exclusions' and contains(@class,'x-tab-strip-text')]";

                Dialog.WaitClick(xPathTab, 190);  //go to the Exclusion tab
                Panel.ClickSectionAddButton("Selections to exclude");  //add a selection to exclude
                SearchDialog.SetTextField("//input[contains(@id,'_NAME_value')]", exclusionName);
                SearchDialog.Search();
                SearchDialog.SelectFirstResult(); // after searching, select the first result
                string xPathLink = "//button[contains(@class,'bui-linkbutton')]/div[text()='Calculate segment counts']";
                Panel.WaitClick(xPathLink, 35); // click on the calculate segment counts
                Dialog.ClickButton("Start", 50);  // start button
                Dialog.GetEnabledElement("//span[contains(@id,'_STATUS_value') and ./text()='Completed']", 480); //480 is the time to wait in seconds

            }
            catch (Exception exc)
            {
                throw new Exception("Error: could not add an exclusion. " + exc.Message);
            }
        }


        [Then(@"the Marketing Effort '(.*)' has been recalculated")]
        public void ThenTheMarketingEffortHasBeenRecalculated(string marketingName)
        {
            marketingName += uniqueStamp;   //The unique stamp is a series of numbers to keep names different from each other
            Dialog.WaitClick("//button[contains(@class,'linkbutton')]/div[text()='Go to " + marketingName + "']", 20); // go to the marketing effort itself

        }

        [When(@"Check for Correspondence Comment for LookupID '(.*)'")]
        public void ThenCheckForCorrespondenceCommentForLookupID(string lookupID, Table batchRows)
        {
            try
            {
                WhenISeekLookupID(lookupID);  //find the constituent who has a comment under Communications
                Panel.SelectTab("Communications");
                Panel.SelectInnerTab("Communications");
                var batchRow = batchRows.Rows[0];
                Panel.SelectSectionDatalistRow(batchRow, "Communications");  //find the correct row which has the correspondenc comment

            }
            catch (Exception ex)
            {
                if (UserName != "bbtest2.gst")
                {
                    throw new Exception("Error: could not find the general correspondence comment. " + ex.Message);
                }
            }
        }
        [When(@"I seek LookupID '(.*)'")]
        public void WhenISeekLookupID(string LookupID)
        {

            try
            {
                string XPathStart = "//div[(contains (@class,'x-window bbui-dialog-search bbui-dialog x-resizable-pinned') and contains(@style, 'visibility: visible'))]//div[contains(@id,'_ConstituentSearchbyNameorLookupID')]";
                string getXLookupField = "//*[contains(@class,'bbui-dialog-search') and not(contains(@style,'hidden'))]//*[starts-with(@id, 'ctrl_') and contains(@id, '_CONSTITUENTQUICKFIND_value')]";
                SearchDialog.SetTextField(getXLookupField, LookupID); // enter lookup ID for a search

                string xPathLookup = "//div[(contains (@class,'x-window bbui-dialog-search bbui-dialog x-resizable-pinned') and contains(@style, 'visibility: visible'))]//div[contains(@id,'_ConstituentSearchbyNameorLookupID')]//a[contains(@id,'_SHOWADVANCEDOPTIONS_action')and not(contains(@style, 'display: none;'))]";
                SearchDialog.WaitClick(xPathLookup, 15);

                // set advanced options so that we are looking for individuals who have an active status
                SearchDialog.SetCheckbox(XPathStart + "//input[contains(@id,'_INCLUDEORGANIZATIONS_value')]", false);
                SearchDialog.SetCheckbox(XPathStart + "//input[contains(@id,'_INCLUDEGROUPS_value')]", false);
                SearchDialog.SetCheckbox(XPathStart + "//input[contains(@id,'_INCLUDEINDIVIDUALS_value')]", true);
                SearchDialog.SetCheckbox(XPathStart + "//input[contains(@id,'_INCLUDEINACTIVE_value')]", true);

                SearchDialog.Search(); //search for a specific lookupid
                SearchDialog.SelectFirstResult();

            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not find the constituent with the specified Lookup ID. " + ex.Message);
            }
        }

    }
}

