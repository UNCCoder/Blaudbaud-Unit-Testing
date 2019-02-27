using System;
using Blackbaud.UAT.Core.Base;
using Blackbaud.UAT.Core.Crm;
using TechTalk.SpecFlow;
using OpenQA.Selenium;
using Blackbaud.UAT.Core.Crm.Dialogs;
using Blackbaud.UAT.Core.Crm.Panels;


namespace UnitTestProject
{
    [Binding]
    public class RevenueSteps : BaseSteps
    {
        //add a pledge
        [When(@"I add a pledge")]
        public void WhenIAddAPledge(Table pledges)
        {
            try
            {
                foreach (var pledge in pledges.Rows)
                {
                    
                    BBCRMHomePage.OpenRevenueFA();  //Open revenue functional area
                    if (pledge.ContainsKey("Constituent") && pledge["Constituent"] != string.Empty)
                        pledge["Constituent"]  += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other
                   //add and save a pledge
                    RevenueFunctionalArea.AddAPledge(pledge);  //add a pledge
                    Dialog.Save();   //save the pledge
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a pledge. " + ex.Message);
            }
        }

        [Then(@"a pledge exists for constituent ""(.*)"" with amount ""(.*)""")]
        public void ThenAPledgeExistsForConstituentWithAmount(string constituent, string amount)
        {
            //find the pledge for the given constituent at a certain amount
            constituent  += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other
            if (!PledgePanel.IsPledgeConstituent(constituent))
                throw new ArgumentException(String.Format("Current pledge panel is not for constituent '{0}'",
                    constituent));
            if (!PledgePanel.IsPledgeAmount(amount))
                throw new ArgumentException(String.Format("Current pledge panel is not for amount '{0}'", amount));

        }
        [Given(@"designation exists '(.*)'")]
        public void GivenDesignationExists(string designation)
        {
            
             BBCRMHomePage.OpenFundraisingFA();  //Open fundraising functional area
            if (!FundraisingFunctionalArea.DesignationExists(designation))
                throw new ArgumentException(String.Format("Designation '{0}' does not exist.", designation));
        }

        [When(@"I start to add a pledge")]
        public void WhenIStartToAddAPledge(Table pledgeValues)
        {
            try
            {
                BBCRMHomePage.OpenRevenueFA();  //Open revenue functional area
                RevenueFunctionalArea.AddAPledge(); //create a new pledge
                foreach (var pledgeValueRow in pledgeValues.Rows)
                {
                    if (pledgeValueRow.ContainsKey("Constituent") && pledgeValueRow["Constituent"] != string.Empty) 
                        pledgeValueRow["Constituent"]  += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other
                    PledgeDialog.SetFields(pledgeValueRow);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not start to add a pledge. " + ex.Message);
            }

        }

        [When(@"split the pledge designations evenly")]
        public void WhenSplitThePledgeDesignations(Table designations)
        {
            try
            {
                PledgeDialog.SplitDesignationsEvenly(designations); //evenly divide the pledges into the various designatinos
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not split pledge designations evenly. " + ex.Message);
            }
        }

        [When(@"save the pledge")]
        public void WhenSaveThePledge()
        {
            try
            {
                Dialog.Save(); //save the pledge
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not save the pledge . " + ex.Message);
            }
        }

        [Then(@"a pledge exists with designations")]
        public void ThenAPledgeExistsWithDesignations(Table designations)
        {
            try
            {
                foreach (var designation in designations.Rows)  //find the designations for the pledges in the batch
                {
                    if (!PledgePanel.DesignationExists(designation))
                        throw new ArgumentException(String.Format("Designation '{0}' does not exist for current pledge.",
                            designation));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not find a pledge with designations. " + ex.Message);
            }
        }

        //[Then(@"a batch exists")]
        //public void ThenABatchExists(Table batches)
        //{
        //    BatchDialog.SaveAndClose();
        //    foreach (var batch in batches.Rows)
        //    {
        //        if (batch.Keys.Contains("Description") && batch["Description"] != null &&
        //            batch["Description"] != string.Empty)
        //            batch["Description"]  += uniqueStamp;  //The unique stamp is a series of numbers to keep descriptions different from each other
        //        if (!BatchEntryPanel.UncommittedBatchExists(batch))  //search for the unique uncommitted batch
        //            throw new ArgumentException(String.Format("Uncommitted batch '{0}' does not exist", batch.Values));
        //    }
        //}

        [When(@"I add a batch with check template ""(.*)"" and description ""(.*)"" and Projected Number ""(.*)"" and  Projected Amount ""(.*)""")]
        public void WhenIAddABatchWithCheckTemplateAndDescriptionAndProjectedNumberAndProjectedAmount(string template, string description, string ProjectedNumber, string ProjectedAmount, Table batchRows)
        {
            try
            {
                description += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other
                BBCRMHomePage.OpenRevenueFA();  //Open revenue functional area
                RevenueFunctionalArea.BatchEntry();   //Open the batch entry page
                BatchEntryPanel.OpenTab("Uncommitted Batches");  // Open the Uncommitted Batches tab
                BatchEntryPanel.ClickSectionAddButton("Uncommitted batches", "Add"); // add an uncommitted batch
                Dialog.SetDropDown(Dialog.getXInput("Batch2AddForm", "BATCHTEMPLATEID"), template); // enter the template name
                Dialog.SetTextField(Dialog.getXTextArea("Batch2AddForm", "DESCRIPTION"), description); // enter the batch desx.
                Dialog.SetTextField("//input[contains(@id, '_PROJECTEDNUMBEROFRECORDS_value')]", ProjectedNumber); // add projected number of records
                Dialog.SetTextField("//input[contains(@id, '_PROJECTEDTOTALAMOUNT_value')]", ProjectedAmount); // add propected dollar amount
                Dialog.Save();
                
                foreach (var batchRow in batchRows.Rows)
                {
                    if (batchRow.Keys.Contains("Constituent") && batchRow["Constituent"] != null &&
                        batchRow["Constituent"] != string.Empty)
                        batchRow["Constituent"] += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other
                }
                EnhancedRevenueBatchDialog.SetGridRows(batchRows);
                EnhancedRevenueBatchDialog.UpdateProjectedTotals();  //click on the update projected totals button and  receive updated totals
                BatchDialog.Validate();  //click the validate button
                BatchDialog.SaveAndClose();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a batch with template and description. " + ex.Message);
            }
        }
        public void WhenIAddABatchWithTemplateAndDescription(string template, string description, Table batchRows)
        {
            try
            {
                description += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other
                BBCRMHomePage.OpenRevenueFA();  //Open revenue functional area
                RevenueFunctionalArea.BatchEntry();   //Open the batch entry page
                BatchEntryPanel.AddBatch(template, description);  //add a new batch according to the template name and description
                foreach (var batchRow in batchRows.Rows)
                {
                    if (batchRow.Keys.Contains("Constituent") && batchRow["Constituent"] != null &&
                        batchRow["Constituent"] != string.Empty)
                        batchRow["Constituent"] += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other
                }
                EnhancedRevenueBatchDialog.SetGridRows(batchRows);
                EnhancedRevenueBatchDialog.UpdateProjectedTotals();  //click on the update projected totals button and  receive updated totals
                BatchDialog.Validate();  //click the validate button
                BatchDialog.SaveAndClose();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a batch with template and description. " + ex.Message);
            }
        }
        [Given(@"a batch ""(.*)"" with description ""(.*)"" exists")]
        public void GivenAnWithDescriptionExists(string template, string description, Table batchRows)
        {
            WhenIAddABatchWithTemplateAndDescription(template, description, batchRows);  // add a batch with the template and desc. given
        }

        [When(@"I commit the batch")]
        public void WhenICommitTheBatch(Table batchRows)
        {
            try
            {
                if (batchRows.RowCount != 1) throw new ArgumentException("Only provide one row to select.");
                BBCRMHomePage.OpenRevenueFA();  //Open revenue functional area
                RevenueFunctionalArea.BatchEntry();
                var batchRow = batchRows.Rows[0];  // select only the first row of the batch
                if (batchRow.ContainsKey("Description") && batchRow["Description"] != null &&
                    batchRow["Description"] != string.Empty)
                    batchRow["Description"]  += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other


                BatchEntryPanel.SelectUncommittedBatch(batchRow); //select uncommitted batch according to the table parameter
                if (BatchEntryPanel.UncommittedBatchExists(batchRow))  // if the selected batch exists
                {
                    System.Threading.Thread.Sleep(2000); //pause because otherwise the code runs so rapidly that the next line is not invoked in time
                    BatchEntryPanel.SelectSectionDatalistRow(batchRow, "Uncommitted batches"); 
                    BatchEntryPanel.CommitSelectedBatch();  //commit the batch
                }
                else
                {
                    throw new ArgumentException(String.Format("Uncommitted batch '{0}' does not exist", batchRow.Values));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not commit the batch. " + ex.Message);
            }
            
        }

        [When(@"the batch completes successfully")]
        public void WhenTheBatchCompletesSuccessfully()
        {
            ThenTheBatchCommitsWithoutErrorsOrExceptions();  // batch commits without errors or exceptions
        }

        [Then(@"the batch commits without errors or exceptions")]
        public void ThenTheBatchCommitsWithoutErrorsOrExceptions()
        {
            if (!BusinessProcess.IsCompleted()) throw new Exception("Batch committed with exceptions or errors.");
        }

        [Then(@"the batch commits without errors or exceptions and (.*) record processed")]
        public void ThenTheBatchCommitsWithoutErrorsOrExceptionsAndRecordProcessed(int numRecords)
        {
            if (!BusinessProcess.IsCompleted()) throw new Exception("Batch committed with exceptions or errors.");
            if (!BusinessProcess.IsNumRecordsProcessed(numRecords))
                throw new Exception(String.Format("'{0}' was not the number of records processed.", numRecords));
        }
        [When(@"I start to add a batch with template ""(.*)"" and description ""(.*)""")]
        public void WhenIStartToAddABatchWithTemplateAndDescription(string template, string description, Table batchRows)
        {
            try
            {
                description += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other
                BBCRMHomePage.OpenRevenueFA();  //Open revenue functional area
                RevenueFunctionalArea.BatchEntry();
                BatchEntryPanel.AddBatch(template, description);  //add a new batch according to the template name and description
                foreach (var batchRow in batchRows.Rows)
                {
                    if (batchRow.Keys.Contains("Constituent") && batchRow["Constituent"] != null &&
                        batchRow["Constituent"] != string.Empty)
                        batchRow["Constituent"] += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other
                }
                EnhancedRevenueBatchDialog.SetGridRows(batchRows);
                // ThenIAddBatchAmounts(100);
                EnhancedRevenueBatchDialog.UpdateProjectedTotals();  //click on the update projected totals button and  receive updated totals

            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a batch with specified template and description. " + ex.Message);
            }
        }

        [When(@"I start to add a batch with template ""(.*)"" and description ""(.*)"" for a payment")]
        public void WhenIStartToAddABatchWithTemplateAndDescriptionforAPledge(string template, string description, Table batchRows)
        {
            try
            {
                description  += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other
                BBCRMHomePage.OpenRevenueFA();  //Open revenue functional area
                RevenueFunctionalArea.BatchEntry();
                BatchEntryPanel.AddBatch(template, description);  //add a new batch according to the template name and description
                foreach (var batchRow in batchRows.Rows)
                {
                    if (batchRow.Keys.Contains("Constituent") && batchRow["Constituent"] != null && batchRow["Constituent"].ToString() != "Testmatchingcompany"
                        && batchRow["Constituent"] != string.Empty)
                        batchRow["Constituent"]  += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other
                }
                EnhancedRevenueBatchDialog.SetGridRows(batchRows);
                if (template == "DEV-Check-MG Payments"){
                    Dialog.Cancel();  // cancel the unneeded popup 
                }

                EnhancedRevenueBatchDialog.UpdateProjectedTotals();  //click on the update projected totals button and  receive updated totals
                EnhancedRevenueBatchDialog.Validate(); //validate batch
                EnhancedRevenueBatchDialog.ClickButton("Update status", 50); // update the batch status
                EnhancedRevenueBatchDialog.SetDropDown("//input[contains(@id, '_NEXTBATCHWORKFLOWTASKID_value')]", "Validate Data Totals and Other Information");
                EnhancedRevenueBatchDialog.SetDropDown("//input[contains(@id, '_OWNERID_value')]", "ad\\bbtest1.gst");
                Dialog.WaitClick("//div[contains(@id,'dataformdialog_')]//button[text()='Save']", 20); //save the batch

                EnhancedRevenueBatchDialog.UpdateProjectedTotals();  //click on the update projected totals button and  receive updated totals
                EnhancedRevenueBatchDialog.Validate();  //validate the batch

                EnhancedRevenueBatchDialog.ClickButton("Update status", 20); //update the status
                EnhancedRevenueBatchDialog.SetDropDown("//input[contains(@id, '_NEXTBATCHWORKFLOWTASKID_value')]", "Special Review");
                EnhancedRevenueBatchDialog.SetDropDown("//input[contains(@id, '_OWNERID_value')]", "ad\\bbtest1.gst");
                Dialog.WaitClick("//div[contains(@id,'dataformdialog_')]//button[text()='Save']", 20); //save the batch

                EnhancedRevenueBatchDialog.UpdateProjectedTotals();  //click on the update projected totals button and  receive updated totals
                EnhancedRevenueBatchDialog.Validate();  //validate the batch
                EnhancedRevenueBatchDialog.ClickButton("Update status", 20); //update the status
                EnhancedRevenueBatchDialog.SetDropDown("//input[contains(@id, '_NEXTBATCHWORKFLOWTASKID_value')]", "Commit Batch");
                EnhancedRevenueBatchDialog.SetDropDown("//input[contains(@id, '_OWNERID_value')]", "ad\\bbtest1.gst");
                Dialog.WaitClick("//div[contains(@id,'dataformdialog_')]//button[text()='Save']", 20); //save the batch

                EnhancedRevenueBatchDialog.SaveAndClose();
  
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a batch with specified template and description for a pledge. " + ex.Message);
            }
        }

        [When(@"split the designations evenly")]
        public void WhenSplitTheDesignationsEvenly(Table designations)
        {
            EnhancedRevenueBatchDialog.SplitDesignations(designations, true); // split the designations evenly
        }

        [Then(@"the '(.*)' cell value is '(.*)' for row (.*)")]
        public void ThenTheCellValueIsForRow(string caption, string value, int rowNumber)
        {
            if (EnhancedRevenueBatchDialog.GetGridCellValue(caption, rowNumber) != value)  //check the value of the pledge for a certain designation in the grid
                throw new Exception(String.Format("Row '{0}', column '{1}' does not have the value '{2}'",
                    rowNumber, caption, value));
        }

        [When(@"navigate to the revenue record for ""(.*)""")]
        public void WhenNavigateToTheRevenueRecordFor(string constituent)
        {
            try
            {
                constituent  += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other
                BBCRMHomePage.OpenRevenueFA();  //Open revenue functional area
                RevenueFunctionalArea.TransactionSearchByConstituent(constituent); //search for transactions by constituent
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not navigate to the revenue record for selected constituent. " + ex.Message);
            }
        }


        [When(@"apply the payment to designations")]
        public void WhenApplyThePaymentToDesignations(Table designations)
        {
            try
            {
                EnhancedRevenueBatchDialog.Apply(designations); //apply payment to the selected designation
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not apply payment to designations. " + ex.Message);
            }
        }

        [When(@"save the batch and commit it")]
        public void WhenSaveTheBatchAndCommitIt(Table batch)
        {
            try
            {
                Dialog.OpenTab("Main");
                EnhancedRevenueBatchDialog.UpdateProjectedTotals();  //click on the update projected totals button and  receive updated totals
                BatchDialog.Validate();  //click the validate button
                BatchDialog.SaveAndClose();
                WhenICommitTheBatch(batch); //commit the batch
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not save the batch and commit it. " + ex.Message);
            }
        }

        [Then(@"the revenue record for ""(.*)"" has payments")]
        public void ThenTheRevenueRecordForHasPayments(string constituent, Table payments)
        {
            try
            {
                WhenNavigateToTheRevenueRecordFor(constituent);
                //find the payment in the revenue panel
                foreach (var payment in payments.Rows)
                {
                    if (!RevenueRecordPanel.PaymentExists(payment))
                        throw new ArgumentException(String.Format(
                            "Payment '{0}' does not exist on the revenue record panel", payment.Values));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not find the revenue record which has payments . " + ex.Message);
            }
        }

        [Then(@"the revenue record for '(.*)' is marked as receipted")]
        public void ThenTheRevenueRecordForIsMarkedAsReceipted(string constituent)
        {
            WhenNavigateToTheRevenueRecordFor(constituent);  // go to the revenue record for a particular constituent
            if (!RevenueRecordPanel.IsReceipted()) throw new ArgumentException("Revenue record not receipted.");
        }

        [Then(@"the revenue record for '(.*)' is marked as acknowledged")]
        public void ThenTheRevenueRecordForIsMarkedAsAcknowledged(string constituent)
        {
            WhenNavigateToTheRevenueRecordFor(constituent); // go to the revenue record for a particular constituent
            if (!RevenueRecordPanel.IsAcknowledged()) throw new ArgumentException("Revenue record not acknowledged.");
        }

        [When(@"split the designations")]
        public void WhenSplitTheDesignations(Table designations)
        {
            try
            {
                //split the money between designations
                EnhancedRevenueBatchDialog.SplitDesignations(designations, false);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not split the designations. " + ex.Message);
            }
        }

        [Then(@"the revenue record for ""(.*)"" has designations")]
        public void ThenTheRevenueRecordForHasDesignations(string constituent, Table designations)
        {
            try
            {
                WhenNavigateToTheRevenueRecordFor(constituent);
                foreach (var designation in designations.Rows)  //find out if a designation exists for a certain pledge
                {
                    if (!PledgePanel.DesignationExists(designation))
                        throw new ArgumentException(String.Format("Designation '{0}' does not exist for current pledge.",
                            designation));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not find the revenue record with designations. " + ex.Message);
            }
        }

        [Then(@"the recurring gift revenue record for ""(.*)"" has designations")]
        public void ThenTheRecurringGiftRevenueRecordForHasDesignations(string constituent, Table designations)
        {
            WhenNavigateToTheRevenueRecordFor(constituent);
            foreach (var designation in designations.Rows)
            {
                if (!RecurringGiftPanel.DesignationExists(designation))  //see if the specific designation exists for the recurring gift
                    throw new ArgumentException(
                        String.Format("Designation '{0}' does not exist for current recurring gift.", designation));
            }
        }

        [Given(@"pledges exist")]
        public void GivenPledgesExist(Table pledges)
        {
            WhenIAddAPledge(pledges);  // add a pledge
        }

        [When(@"set the revenue type for row (.*) to ""(.*)""")]
        public void WhenSetTheRevenueTypeForRowTo(int rowIndex, string paymentMethod)
        {
            try
            {
                EnhancedRevenueBatchDialog.SetGridCell("Revenue type", paymentMethod, rowIndex); //set the revenue type for the specific row
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not set the revenue type in a grid row. " + ex.Message);
            }
         
        }

        [Then(@"I start to add a payment")]
        public void GivenIStartToAddAPayment(Table paymentFieldRows)
        {
            try
            {
                BBCRMHomePage.OpenRevenueFA();  //Open revenue functional area
                RevenueFunctionalArea.AddAPayment(); // add a payment
                foreach (var paymentFieldValues in paymentFieldRows.Rows)
                {
                    if (paymentFieldValues.ContainsKey("Constituent") && paymentFieldValues["Constituent"] != string.Empty)
                        paymentFieldValues["Constituent"]  += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other
                    PaymentDialog.SetFields(paymentFieldValues); //set the fields in the payment according to the table parameter
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not start to add a payment. " + ex.Message);
            }
        }

        [Then(@"add applications to the payment")]
        public void GivenAddApplicationsToThePayment(Table applications)
        {
            try
            {
                foreach (var application in applications.Rows) //for each application in the batch, add it to the payment
                {
                    PaymentDialog.AddApplication(application);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add applications to the payment. " + ex.Message);
            }
        }

        [Then(@"save the the payment")]
        public void GivenSaveTheThePayment()
        {
            try
            {
                Dialog.Save();  //save the payment
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not save the payment. " + ex.Message);
            }
        }
        [Then(@"I Add Batch Amounts '(.*)'")]
        public void ThenIAddBatchAmounts(int iAmount)
        {
            try
            {
                RevenueFunctionalArea.SetTextField("//input[contains(@id,'_AMOUNT_value')]", iAmount.ToString()); // set the text field to the amount parameter passed in
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add batch amounts. " + ex.Message);
            }           

        }
        [Given(@"Add a Matching Company '(.*)' as Employer")]
        public void GivenAddAMatchingCompanyAsEmployer(string companyName)
        {
            try
            {
                //open the relationships tab and the relationships inner tab
                ConstituentPanel.SelectTab("Relationships");
                ConstituentPanel.SelectInnerTab("Relationships");
                //add an organization relationsip for this constituent as an employee 
                ConstituentPanel.ClickButton("Add organization");
                Dialog.SetSearchList(Dialog.getXInput("_RelationshipIndividualtoOrganizationAddForm2", "_RECIPROCALCONSTITUENTID_value"), Dialog.getXInput("searchdialog", "_KEYNAME_value"), companyName);
                Dialog.SetDropDown("//input[contains(@id,'_RELATIONSHIPTYPECODEID_value')]", "Employee");
                Dialog.SetCheckbox("//input[contains(@id,'_ISPRIMARYBUSINESS_value')]", true);
                Dialog.SetCheckbox("//input[contains(@id,'_ISMATCHINGGIFTRELATIONSHIP_value')]", true);
                Dialog.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a matching company. " + ex.Message);
            }   
        }
        [Then(@"I add a matching gift for company '(.*)'")]
        public void ThenIAddAMatchingGiftForCompany(string companyName)
        {
            try
            {
                Panel.OpenTab("Matching Gifts");
                Panel.ClickSectionAddButton("Matching gifts"); // open matching gifts popup
                Dialog.SetSearchList(Dialog.getXInput("_MatchingGiftClaimAddForm2", "_MATCHINGORGANIZATIONID_value"), Dialog.getXInput("searchdialog", "_KEYNAME_value"), companyName);
                Dialog.SetDropDown("//input[contains(@id,'_RELATIONSHIPID_value')]", "Employee");
             //   Dialog.SetDropDown("//input[contains(@id,'_MATCHINGGIFTCONDITIONID_value')]", "Employee");
                Dialog.Save(); //save the matching gift info
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a matching gift. " + ex.Message);
            }    
        }
        [Then(@"I add a purpose with a designation named '(.*)' with lookupID '(.*)'")]
        public void ThenIAddAPurposeWithADesignationNamedWithLookupID(string designationName, string lookupID)
        {
            try
            {
                designationName += uniqueStamp; //The unique stamp is a series of numbers to keep purpose names different from each other
                lookupID += uniqueStamp; // the unique stamp is a series of numbers to keep the lookup ids different
                BBCRMHomePage.OpenFundraisingFA();  //open the fundraising functional area
                FundraisingFunctionalArea.OpenLink("Add purpose and designation"); //
                AddFieldsToPurpose(designationName, lookupID); // input info in the various form controls
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add purpose/designation. " + ex.Message);
            }    
        }
        public void AddFieldsToPurpose(string designationName, string lookupID)
        {
            // input info in the various form controls
            try
            {
                Dialog.SetCheckbox("//input[contains(@id, '_ACCEPTINGREVENUE_1')]", true);  //set the purpose will need a designation checkbox to true
                Dialog.SetCheckbox("//input[contains(@id, '_PURPOSELOCATION_0')]", true);  //set the new hierarchy checkbox to true
                Dialog.SetTextField("//input[contains(@id, '_PURPOSENAME_value')]", designationName);  // set the internal purpose name
                Dialog.SetTextField("//input[contains(@id, '_PURPOSELOOKUPID_value')]", lookupID);  // set the lookup id for the purpose
                Dialog.SetTextField("//input[contains(@id, '_PURPOSEDESCRIPTION_value')]", "Test Description");  // set the desc.
                Dialog.SetDropDown("//input[contains(@id, '_PURPOSETYPEID_value')]", "Fund"); // use "Fund" purpose type
                Dialog.Save();

            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add data to the purpose/designation. " + ex.Message);
            }    


        }
        [Given(@"Search for a Reminder Process '(.*)'")]
        public void GivenSearchForAReminderProcess(string reminderName)
        {
            try
            {
                Panel.WaitClick("//a[./text()='" + reminderName + "']", 20);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not find the reminder process. " + ex.Message);
            }    
        }

        [Given(@"Edit the Reminder Process")]
        public void GivenEditTheReminderProcess()
        {
            try
            {
                Dialog.WaitClick("//button[contains(@class,'bbui-linkbutton')]/div[text()='Edit process']", 20); //click the edit process link
                Dialog.SetTextField("//input[contains(@id,'_DATE_value')]", "10/31/2016"); //enter the date for the reminder process
                Dialog.Save();
                Dialog.WaitClick("//button[contains(@class,'bbui-linkbutton')]/div[text()='Start process']", 20); // click on the start process link
                Dialog.ClickButton("Start", 50);
                System.Threading.Thread.Sleep(4000);  //allow some time for the process to actually run.
                Dialog.GetEnabledElement("//span[contains(@id,'_STATUS_value') and ./text()='Completed']", 480); //480 is the time to wait in seconds
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add edit the reminder process. " + ex.Message);
            }    
        }

        [Given(@"Start the Reminder Process")]
        public void GivenStartTheReminderProcess()
        {
 
            BBCRMHomePage.OpenMarketingAndCommunicationsFA(); // open the marketing & commun. functional area
            //click on the reminders link
            try
            {
                Panel.WaitClick("//button[contains(@class,'bbui-linkbutton bbui-pages-task-link')]/div[text()='Reminders']", 20);
                Panel.OpenTab("Reminders");
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not start the reminder process. " + ex.Message);
            }    
        }
    }
}