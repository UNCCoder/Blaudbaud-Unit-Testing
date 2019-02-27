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

    public class ChangeRequestSteps : BaseSteps
    {
        public const string xPathBeginning = "//div[contains(@class, 'bbui-pages-contentcontainer') and not(contains (@class, 'x-hide-display'))]";

        //readonly captionnames  and input fields
        protected static readonly IDictionary<string, CrmField> SupportedFields = new Dictionary<string, CrmField>
        {
            {"Name", new CrmField("_NAME_value", FieldType.TextInput)},
            {"Last name", new CrmField("_LASTNAME_value", FieldType.TextInput)},
            {"First name", new CrmField("_FIRSTNAME_value", FieldType.TextInput)},
            {"Middle name", new CrmField("_MIDDLENAME_value", FieldType.TextInput)},
            {"Nickname", new CrmField("_NICKNAME_value", FieldType.TextInput)},
            {"ZIP", new CrmField("_POSTALCODE_value", FieldType.TextInput)},
            {"Number", new CrmField("_PHONE_NUMBER_value", FieldType.TextInput)},
            {"Email Address", new CrmField("_EMAILADDRESS_EMAILADDRESS_value", FieldType.TextInput)},
            {"Title", new CrmField("_TITLECODEID_value", FieldType.Dropdown)},
            {"Gender", new CrmField("_GENDERCODE_value", FieldType.Dropdown)},
            {"Address type", new CrmField("TYPECODEID_value", FieldType.Dropdown)},
            {"Type", new CrmField("TYPECODEID_value", FieldType.Dropdown)},
            {"State", new CrmField("_STATEID_value", FieldType.Dropdown)},
            {"Phone Type", new CrmField("_PHONE_PHONETYPECODEID_value", FieldType.Dropdown)},
            {"Address", new CrmField("BLOCK_value", FieldType.TextArea)},
            {"Reason for request", new CrmField("_SUBMITTERCOMMENTS_value", FieldType.TextArea)},
            {"Reason requested", new CrmField("_REQUESTREASON_value", FieldType.TextArea)},
            {"Personal Relationship", new CrmField("_RELATIONSHIPINFO_value", FieldType.TextArea)},
            {"Business Relationship", new CrmField("_BUSINESSRELATIONSHIPINFO_value", FieldType.TextArea)},
            {"Other Changes", new CrmField("_OTHERCHANGES_value", FieldType.TextArea)},
            {"Industry", new CrmField("_INDUSTRYCODEID_value", FieldType.Dropdown)},
            {"No. of employees", new CrmField("_NUMEMPLOYEES_value", FieldType.TextInput)},
            {"No. of subsidiaries", new CrmField("_NUMSUBSIDIARIES_value", FieldType.TextInput)},
            {"Address Type", new CrmField("ADDRESSTYPECODEID_value", FieldType.Dropdown)},
            {"City", new CrmField("_CITY_value", FieldType.TextInput)},
        };
        [Given(@"I start an add change request for constituent")]
        public void GivenIStartAnAddChangeRequestForConstituent()
        {
            try
            {
                BBCRMHomePage.OpenConstituentsFA();  // open the constituent functional area
                //click on the link to open form to add a new individual by request
                ConstituentPanel.WaitClick("//button[contains(@class,'linkbutton')]/div[text()='UNC Request Add a New Individual']", 10);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not start a change request. " + ex.Message);
            }
        }

        [When(@"I input data into the change request for constituent '(.*)'")]
        public void WhenIInputDataIntoTheChangeRequestForConstituent(string lastName, Table batchRows)
        {
            try
            {
                lastName += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other
                if (batchRows.RowCount != 1)
                    throw new ArgumentException("Only provide one row to select.");

                var batchRow = batchRows.Rows[0];  // select only the first row of the batch
                if (batchRow.Keys.Contains("Last name") && (!String.IsNullOrEmpty(batchRow["Last name"])))
                {
                    batchRow["Last name"] += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other
                }

                Dialog.SetFields("dataformdialog", batchRow, SupportedFields);
                Dialog.Save();
                System.Threading.Thread.Sleep(16000); //pause because otherwise the code runs so rapidly that the next line is not invoked in time
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not input data into the change request. " + ex.Message);
            }
        }


        [Given(@"An Edit Personal Info Change")]
        public void GivenAnEditPersonalInfoChange(Table requestTable)
        {
            // Add a constituent change request and add request reason, personal info, relationship info, and other changes
            try
            {
                //text to go in the various text areas
                string requestreason = "This is the requested reason";
                string personalrelationship = "This is the personal relationship";
                string businessrelationship = "This is the business relationship";
                string otherchange = "This is the other change";

                //click on the link to edit a change request
                Dialog.WaitClick("//button[contains(@class,'linkbutton')]/div[text()='Add a Constituent Change Request']", 10);

                SearchDialog.WaitClick("//a[contains(@id,'_SHOWPERSONALINFO_action')]", 20);

                if (requestTable.RowCount != 1)
                    throw new ArgumentException("Only provide one row to select.");

                var batchRow = requestTable.Rows[0];  // select only the first row of the batch
                Dialog.SetFields("dataformdialog", batchRow, SupportedFields);

                //fill in the textareas -  request reason, relationships and other changes.
                Dialog.SetTextField(Dialog.getXTextArea("UNCConstituentChangeRequestConsolidatedAddDataForm", "REQUESTREASON"), requestreason); // enter the reason for the request.

                SearchDialog.WaitClick("//a[contains(@id,'_SHOWRELATIONSHIPS_action')]", 20);

                Dialog.SetTextField(Dialog.getXTextArea("UNCConstituentChangeRequestConsolidatedAddDataForm", "RELATIONSHIPINFO"), personalrelationship); // enter the personal relationship info
                Dialog.SetTextField(Dialog.getXTextArea("UNCConstituentChangeRequestConsolidatedAddDataForm", "BUSINESSRELATIONSHIPINFO"), businessrelationship); // enter the business relationship info

                SearchDialog.WaitClick("//a[contains(@id,'_SHOWOTHERCHANGES_action')]", 20);
                Dialog.SetTextField(Dialog.getXTextArea("UNCConstituentChangeRequestConsolidatedAddDataForm", "OTHERCHANGES"), otherchange); // enter the business relationship info

            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not edit a personal change request. " + ex.Message);
            }
        }
        [Then(@"the Personal Change is Requested")]
        public void ThenThePersonalChangeIsRequested()
        {
            try
            {
                Dialog.Save();  //save personal info
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not save personal info. " + ex.Message);
            }
        }

        //Change Request for Organization
        [Given(@"I add an organization")]
        public void GivenIAddOrganization(Table orgTable)
        {

            try
            {
                var organizationDialogId = "OrganizationAddForm";
                if (orgTable.RowCount != 1)
                    throw new ArgumentException("Only provide one row to select.");

                var batchRow = orgTable.Rows[0];  // select only the first row of the batch

                // give the organization a unique name wih the stamp
                if (batchRow.Keys.Contains("Name") && (!String.IsNullOrEmpty(batchRow["Name"])))
                {
                    batchRow["Name"] += uniqueStamp;
                }

                BBCRMHomePage.OpenConstituentsFA();
                //open the add org form
                ConstituentsFunctionalArea.OpenLink("Add an organization");

                //fill in a couple of fields for the org
                Dialog.SetFields(organizationDialogId, batchRow, SupportedFields);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add an organization. " + ex.Message);
            }
        }

        [Then(@"organization is created")]
        public void ThenOrganizationIsCreated()
        {
            try
            {
                Dialog.Save();  //save organization
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not save an organization. " + ex.Message);
            }
        }

        [Given(@"An Edit Organization Info Change")]
        public void GivenAnEditOrganizationInfoChange(Table orgTable)
        {
            // Add a constituent change request and add request reason, personal info, relationship info, and other changes
            try
            {
                //text to go in the various text areas
                string requestreason = "This is the requested reason";
                string personalrelationship = "This is the personal relationship";
                string businessrelationship = "This is the business relationship";
                string otherchange = "This is the other change";

                //click on the link to edit a change request
                Dialog.WaitClick("//button[contains(@class,'linkbutton')]/div[text()='Add a Constituent Change Request']", 10);

                SearchDialog.WaitClick("//a[contains(@id,'_SHOWPERSONALINFO_action')]", 20);

                if (orgTable.RowCount != 1)
                    throw new ArgumentException("Only provide one row to select.");

                var batchRow = orgTable.Rows[0];  // select only the first row of the batch
                Dialog.SetFields("dataformdialog", batchRow, SupportedFields);

                //fill in the textareas -  request reason, relationships and other changes.
                Dialog.SetTextField(Dialog.getXTextArea("UNCConstituentChangeRequestConsolidatedAddDataForm", "REQUESTREASON"), requestreason); // enter the reason for the request.

                SearchDialog.WaitClick("//a[contains(@id,'_SHOWRELATIONSHIPS_action')]", 20);

                Dialog.SetTextField(Dialog.getXTextArea("UNCConstituentChangeRequestConsolidatedAddDataForm", "RELATIONSHIPINFO"), personalrelationship); // enter the personal relationship info
                Dialog.SetTextField(Dialog.getXTextArea("UNCConstituentChangeRequestConsolidatedAddDataForm", "BUSINESSRELATIONSHIPINFO"), businessrelationship); // enter the business relationship info

                SearchDialog.WaitClick("//a[contains(@id,'_SHOWOTHERCHANGES_action')]", 20);
                Dialog.SetTextField(Dialog.getXTextArea("UNCConstituentChangeRequestConsolidatedAddDataForm", "OTHERCHANGES"), otherchange); // enter the business relationship info

            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not edit a personal change request. " + ex.Message);
            }
        }

        [Then(@"the Organization Change is Requested")]
        public void ThenTheOrganizationChangeIsRequested()
        {
            try
            {
                Dialog.Save();  //save organization change request
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not save an organization change request. " + ex.Message);
            }
        }

        [Given(@"a new Address Moved Change Request")]
        public void GivenANewAddressMovedChangeRequest(Table batchRows)
        {

            //text to go in the various text areas
            string requestreason = "This is the requested for a new address change request.";

            try
            {

                //click on the link to edit a change request
                Dialog.WaitClick("//button[contains(@class,'linkbutton')]/div[text()='Add a Constituent Change Request']", 10);

                //fill in the textarea - request reason
                Dialog.SetTextField(Dialog.getXTextArea("UNCConstituentChangeRequestConsolidatedAddDataForm", "REQUESTREASON"), requestreason); // enter the reason for the request.
                SearchDialog.WaitClick("//a[contains(@id,'_SHOWADDRESSES_action')]", 20);

                //save the new address information in the grid
                var columnCaptionToIndex = Dialog.MapColumnCaptionsToIndex(batchRows.Rows[0].Keys,
                     Dialog.getXGridHeaders("UNCConstituentChangeRequestConsolidatedAddDataForm", "NEWADDRESSINFO_value"));

                Dialog.SetGridRows("UNCConstituentChangeRequestConsolidatedAddDataForm", "NEWADDRESSINFO_value", batchRows, 1, columnCaptionToIndex, SupportedFields);

            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a new address change request. " + ex.Message);
            }
        }
        [Then(@"the new address is requested")]
        public void ThenThneNewAddressChangeIsRequested()
        {
            try
            {
                Dialog.Save();  //save new address change request
                System.Threading.Thread.Sleep(6000);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not save a new address change request. " + ex.Message);
            }
        }
        [Given(@"a new Phone request")]
        public void GivenANewPhoneRequest(Table PhoneTable)
        {
            try
            {
                string requestreason = "Reason for new phone and email request.";
                //click on the link to edit a change request;
                Dialog.WaitClick("//button[contains(@class,'linkbutton')]/div[text()='Add a Constituent Change Request']", 10);
                //fill in the textarea - request reason
                Dialog.SetTextField(Dialog.getXTextArea("UNCConstituentChangeRequestConsolidatedAddDataForm", "REQUESTREASON"), requestreason); // enter the reason for the request.


                SearchDialog.WaitClick("//a[contains(@id,'_SHOWPHONES_action')]", 20);

                //save the new phone info in the grid.
                var columnCaptionToIndex = Dialog.MapColumnCaptionsToIndex(PhoneTable.Rows[0].Keys,
                     Dialog.getXGridHeaders("UNCConstituentChangeRequestConsolidatedAddDataForm", "NEWPHONEINFO_value"));

                Dialog.SetGridRows("UNCConstituentChangeRequestConsolidatedAddDataForm", "NEWPHONEINFO_value", PhoneTable, 1, columnCaptionToIndex, SupportedFields);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not save a new phone change request. " + ex.Message);
            }

        }

        [Given(@"a new Email Request")]
        public void GivenANewEmailRequest(Table emailTable)
        {
            try
            {
                //fill in a couple of fields for a new email request
                SearchDialog.WaitClick("//a[contains(@id,'_SHOWEMAILS_action')]", 20);
                var columnCaptionToIndex = Dialog.MapColumnCaptionsToIndex(emailTable.Rows[0].Keys,
                     Dialog.getXGridHeaders("UNCConstituentChangeRequestConsolidatedAddDataForm", "NEWEMAILINFO_value"));

                // fill in the grid with the captions and values from the table
                Dialog.SetGridRows("UNCConstituentChangeRequestConsolidatedAddDataForm", "NEWEMAILINFO_value", emailTable, 1, columnCaptionToIndex, SupportedFields);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not save a new email change request. " + ex.Message);
            }
        }

        [Then(@"the new phone and email info is requested.")]
        public void ThenTheNewPhoneAndEmailInfoIsRequested()
        {
            try
            {
                Dialog.Save();  //save new email and phone change request
                System.Threading.Thread.Sleep(6000); //pause because otherwise the code runs so rapidly that the next line is not invoked in time
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not save a new phone and email change request. " + ex.Message);
            }
        }
        [Given(@"a delete Phone Request")]
        public void GivenADeletePhoneRequest(Table phonedeleteTable)
        {
             try
            {
                string requestreason = "Reason for delete phone and email request.";
                //click on the link to edit a change request;
                Dialog.WaitClick("//button[contains(@class,'linkbutton')]/div[text()='Add a Constituent Change Request']", 10);

                //fill in the textarea - request reason
                Dialog.SetTextField(Dialog.getXTextArea("UNCConstituentChangeRequestConsolidatedAddDataForm", "REQUESTREASON"), requestreason); // enter the reason for the request.

                SearchDialog.WaitClick("//a[contains(@id,'_SHOWPHONES_action')]", 20);

                //save the new phone info in the grid.
                var columnCaptionToIndex = Dialog.MapColumnCaptionsToIndex(phonedeleteTable.Rows[0].Keys,
                     Dialog.getXGridHeaders("UNCConstituentChangeRequestConsolidatedAddDataForm", "EDITPHONEINFO_value"));

                var cellxPath = Dialog.getXGridCell("UNCConstituentChangeRequestConsolidatedAddDataForm",  "EDITPHONEINFO_value", 1, columnCaptionToIndex["Delete?"]);

                Dialog.SetCheckbox(cellxPath, true);  //set the delete checkbox to true
            }
             catch (Exception ex)
             {
                 throw new Exception("Error: could not update a delete phone request. " + ex.Message);
             }         
        }
        [Given(@"a delete Email Request")]
        public void GivenADeleteEmailRequest(Table emailTable)
        {
            try
            {
                SearchDialog.WaitClick("//a[contains(@id,'_SHOWEMAILS_action')]", 20);

                //save the new email info in the grid.
                var columnCaptionToIndex = Dialog.MapColumnCaptionsToIndex(emailTable.Rows[0].Keys,
                     Dialog.getXGridHeaders("UNCConstituentChangeRequestConsolidatedAddDataForm", "EDITEMAILINFO_value"));

                var cellxPath = Dialog.getXGridCell("UNCConstituentChangeRequestConsolidatedAddDataForm", "EDITEMAILINFO_value", 1, columnCaptionToIndex["Delete?"]);

                Dialog.SetCheckbox(cellxPath, true);  // set the delete checkbox to true
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not update a delete email request. " + ex.Message);
            }      
        }
        [Then(@"the delete phone request is saved")]
        public void ThenTheDeletePhonerequestIsSaved()
        {
            try
            {
                Dialog.Save();  //save new email and phone change request
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not save a delete phone and email change request. " + ex.Message);
            }
        }
        [When(@"I go to the UNC Change Management Page")]
        public void WhenIGoToTheUNCChangeManagementPage(Table changeRows)
        {

            try
            {  
                BBCRMHomePage.OpenConstituentsFA();  // open constituent functional area
                string xPathLink = "//button[contains(@class,'bui-linkbutton')]/div[text()='UNC Change Management']";
                //click on the  UNC Change Management BUtton
                ConstituentPanel.WaitClick(xPathLink);

                if (changeRows.RowCount != 1) throw new ArgumentException("Only provide one row to select.");
                var changeRow = changeRows.Rows[0];  // select only the first row 

                changeRow["Name"] = changeRow["Name"] + uniqueStamp;  //Use the unique stamp at the end of the name

                //select the row for the particular constituent
                Panel.SelectSectionDatalistRow(changeRow, "Constituent Change Requests");

                //Change the Personal/Org information -- middle name to William
                Dialog.WaitClick("*//button[./text()='Personal/Org Info' and contains(@class,'x-btn-text')]", 20);

                if (changeRow["Name"].Contains("Testings"))
                {
                    string xMiddleNamePath = "//input[contains(@id, '_MIDDLENAME_value')]";
                    Dialog.SetTextField(xMiddleNamePath, "William");
                }
                else
                {
                    string xNumberEmployeesPath = "//input[contains(@id, 'NUMEMPLOYEES_value')]";
                    Dialog.SetTextField(xNumberEmployeesPath, "400");
                }

                //Approve personal info with changes
                string xRequestStatusPath = "//input[contains(@id, '_REQUESTSTATUSCODEID_value')]";
                Dialog.SetDropDown(xRequestStatusPath, "Approved with changes");
                Dialog.ClickButton("Commit Changes", 50);

                System.Threading.Thread.Sleep(5000); 
                //select the row for the particular constituent
                Panel.SelectSectionDatalistRow(changeRow, "Constituent Change Requests");

                 //Reject individual relationships
                Dialog.WaitClick("*//button[./text()='Individual Relationships' and contains(@class,'x-btn-text')]", 20);

                xRequestStatusPath = "//input[contains(@id, '_RELATIONSHIPSTATUSCODEID_value')]";
                Dialog.SetDropDown(xRequestStatusPath, "Rejected");
                Dialog.Save();

                System.Threading.Thread.Sleep(5000); 

                //select the row for the particular constituent
                Panel.SelectSectionDatalistRow(changeRow, "Constituent Change Requests");

                //Accept Business relationships
                Dialog.WaitClick("*//button[./text()='Business/Org Relationships' and contains(@class,'x-btn-text')]", 20);

                xRequestStatusPath = "//input[contains(@id, '_BUSINESSREQUESTSTATUSCODEID_value')]";
                Dialog.SetDropDown(xRequestStatusPath, "Approved as requested");
                Dialog.Save();

                System.Threading.Thread.Sleep(5000); 
                //select the row for the particular constituent
                Panel.SelectSectionDatalistRow(changeRow, "Constituent Change Requests");

                //Accept with Changes - Other changes
                Dialog.WaitClick("*//button[./text()='Other' and contains(@class,'x-btn-text')]", 20);

                xRequestStatusPath = "//input[contains(@id, '_OTHERREQUESTSTATUSCODEID_value')]";
                Dialog.SetDropDown(xRequestStatusPath, "Approved with changes");
                Dialog.Save();

            }

            catch (Exception ex)
            {
                throw new Exception("Error: could not go to the UNC Change Management page. " + ex.Message);
            }
        }
        [Then(@"constituent '(.*)' exists")]
        public void ThenConstituentExists(string middlename)
        {
            IWebElement element;

            try
            {

                string xPath = xPathBeginning + "//h2[contains(@class,'bbui-pages-header')]//span[contains(text(), '" + middlename + "')]";
                element = Dialog.GetEnabledElement(xPath, 50);  //find the middle name on the constituent page to verify that it was changed.
                if (element == null)
                {
                    throw new XPathLookupException("The constituentwith the middle name of " + middlename + " could not be found. ");
                }
            }
            catch (XPathLookupException)
            {
                throw;
            }

            catch (Exception ex)
            {   
                throw new Exception("Error: could not affirm that the constituent exists. " + ex.Message);
            }
        }
        [Then(@"Organization Change Request is Completed for '(.*)'")]
        public void ThenOrganizationChangeRequestIsCompletedFor(string organizationName)
        {
            organizationName += uniqueStamp;
            if (!Panel.IsPanelHeader(organizationName))
            {
                throw new Exception("Error: could not verify that the organization exists. ");
            }
            string Target;
            Target = Panel.GetDisplayedElement("//tr[contains(@id,'_NUMEMPLOYEES_container')]//span[contains(@id,'_NUMEMPLOYEES_value')]", 20).Text;
            // if the number of employees does not exist or there is a different number from 400, then send an error.
            if ((Target != "400") || (Target == null))
            {
                throw new Exception("Error: The organization change request could not be saved. ");
            }
        }

        [Given(@"Edit Address Change Request")]
        public void GivenEditAddressChangeRequest(Table editaddressTable)
        {
            //text to go in the various text areas
            string requestreason = "This is the requested for an edit address change request.";

            try
            {

                //click on the link to edit a change request
                Dialog.WaitClick("//button[contains(@class,'linkbutton')]/div[text()='Add a Constituent Change Request']", 10);

                //fill in the textarea - request reason
                Dialog.SetTextField(Dialog.getXTextArea("UNCConstituentChangeRequestConsolidatedAddDataForm", "REQUESTREASON"), requestreason); // enter the reason for the request.
                SearchDialog.WaitClick("//a[contains(@id,'_SHOWADDRESSES_action')]", 20);

                //save the new address information in the grid
                var columnCaptionToIndex = Dialog.MapColumnCaptionsToIndex(editaddressTable.Rows[0].Keys,
                     Dialog.getXGridHeaders("UNCConstituentChangeRequestConsolidatedAddDataForm", "EDITADDRESSINFO_value"));

                Dialog.SetGridRows("UNCConstituentChangeRequestConsolidatedAddDataForm", "EDITADDRESSINFO_value", editaddressTable, 1, columnCaptionToIndex, SupportedFields);

            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not edit an address change request. " + ex.Message);
            }
        }
        [Then(@"the edit address change is requested")]
        public void ThenTheEditAddressChangeIsRequested()
        {
            try
            {
                Dialog.Save();  // save the edit address change reqeust
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not save an edit address change request. " + ex.Message);
            }           
        }
        [When(@"I go to the UNC Change Management Page for editing")]
        public void WhenIGoToTheUNCChangeManagementPageForEditing(Table changeRows)
        {
            try
            {
                BBCRMHomePage.OpenConstituentsFA();  // open constituent functional area
                string xPathLink = "//button[contains(@class,'bui-linkbutton')]/div[text()='UNC Change Management']";

                //click on the  UNC Change Management Button
                ConstituentPanel.WaitClick(xPathLink);

                if (changeRows.RowCount != 1) throw new ArgumentException("Only provide one row to select.");
                var changeRow = changeRows.Rows[0];  // select only the first row 

                changeRow["Name"] = changeRow["Name"] + uniqueStamp;  //Use the unique stamp at the end of the name

                //select the row for the particular constituent
                Panel.SelectSectionDatalistRow(changeRow, "Constituent Change Requests");
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not go to the Change Management page for an edit address change request. " + ex.Message);
            }           
        }

        [When(@"Save Edit Address")]
        public void WhenSaveEditAddressFor(Table AddressTable)
        {
            try
            {
                //click on the Contact Info button to view form to manage contact infor for a constituent
                string xPath = "//*[./text()='Contact Info' and contains(@class,'x-btn-text')]";
                Dialog.WaitClick(xPath);

                if (AddressTable.RowCount != 1)
                    throw new ArgumentException("Only provide one row to select.");
                var changeRow = AddressTable.Rows[0];  // select only the first row 

                Panel.SelectSectionDatalistRow(changeRow, "Address Change Requests");

                //click on the edit pencil
                xPath = "//*[./text()='Edit' and contains(@class,'x-btn-text')]";
                Dialog.WaitClick(xPath);

                // set a status of "Approved as requested."
                string xRequestStatusPath = "//input[contains(@id, '_REQUESTSTATUSCODEID_value')]";
                Dialog.SetDropDown(xRequestStatusPath, "Approved as requested");
                Dialog.ClickButton("Commit Changes", 50);  // save the changes
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not approve an edit address change request. " + ex.Message);
            }
        }
        [Then(@"Verify that Edit Address Change was Saved for '(.*)'")]
        public void ThenVerifyThatEditAddressChangeWasSavedFor(string lastName)
        {
            try
            {

                string Target;
                Target = Panel.GetDisplayedElement("//div[contains(@id, '_ConstituentSummaryAddressesTileViewForm')]//tr[contains(@id,'_ADDRESSROW2_container')]//div[contains(@id,'_ADDRESSROW2_value')]", 20).Text;
                // if the city is blank or not equal to Charlottte then show an error msg.
                if ((Target.Substring(0, 9) != "Charlotte") || (Target == null))
                {
                    throw new Exception("Error: The edit address change request could not be saved for the city name. ");
                }
                // if the address type is not home mailing then send an error msg.
                Target = Panel.GetDisplayedElement("//div[contains(@id, '_ConstituentSummaryAddressesTileViewForm')]//td[contains(@id,'_ADDRESSTYPE_container')]//div[contains(@id,'_ADDRESSTYPE_value')]", 20).Text;
                if (!Target.Contains( "Home Mailing"))
                {
                    throw new Exception("Error: The edit address change request could not be saved for the address type. ");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not verify an edit address change request. " + ex.Message);
            }
        }
        [Given(@"Edit Phone Change Request")]
        public void GivenEditPhoneChangeRequest(Table editPhoneTable)
        {
            {
                try
                {
                    string requestreason = "Reason for edit phone and email requests.";
                    //click on the link to edit a change request;
                    Dialog.WaitClick("//button[contains(@class,'linkbutton')]/div[text()='Add a Constituent Change Request']", 10);
                    //fill in the textarea - request reason
                    Dialog.SetTextField(Dialog.getXTextArea("UNCConstituentChangeRequestConsolidatedAddDataForm", "REQUESTREASON"), requestreason); // enter the reason for the request.


                    SearchDialog.WaitClick("//a[contains(@id,'_SHOWPHONES_action')]", 20);

                    //save the edit phone info in the grid.
                    var columnCaptionToIndex = Dialog.MapColumnCaptionsToIndex(editPhoneTable.Rows[0].Keys,
                         Dialog.getXGridHeaders("UNCConstituentChangeRequestConsolidatedAddDataForm", "EDITPHONEINFO_value"));

                    Dialog.SetGridRows("UNCConstituentChangeRequestConsolidatedAddDataForm", "EDITPHONEINFO_value", editPhoneTable, 1, columnCaptionToIndex, SupportedFields);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error: could not save an edit phone change request. " + ex.Message);
                }

            }            
        }
        [Given(@"Edit Email Change Request")]
        public void GivenEditEmailChangeRequest(Table emailTable)
        {
            try
            {
                //fill in a couple of fields for a new email request
                SearchDialog.WaitClick("//a[contains(@id,'_SHOWEMAILS_action')]", 20);
                var columnCaptionToIndex = Dialog.MapColumnCaptionsToIndex(emailTable.Rows[0].Keys,
                     Dialog.getXGridHeaders("UNCConstituentChangeRequestConsolidatedAddDataForm", "EDITEMAILINFO_value"));

                // fill in the grid with the captions and values from the table
                Dialog.SetGridRows("UNCConstituentChangeRequestConsolidatedAddDataForm", "EDITEMAILINFO_value", emailTable, 1, columnCaptionToIndex, SupportedFields);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not save an edit email change request. " + ex.Message);
            }           
        }
        [Then(@"the edit phone change is requested")]
        public void ThenTheEditPhoneChangeIsRequested()
        {
            try
            {
                Dialog.Save();  // save the edit phone/email change reqeust
                System.Threading.Thread.Sleep(6000); //pause because otherwise the code runs so rapidly that the next line is not invoked in time
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not save an edit phone and email change request. " + ex.Message);
            }                
        }

    }
}
