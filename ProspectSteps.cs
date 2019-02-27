using System;
using Blackbaud.UAT.Core.Base;
using Blackbaud.UAT.Core.Crm;
using TechTalk.SpecFlow;
using System.Collections.Generic;
using Keys = OpenQA.Selenium.Keys;
using System.Text;
using Blackbaud.UAT.Core.Crm.Dialogs;
using Blackbaud.UAT.Core.Crm.Panels;


namespace UnitTestProject
{
    [Binding]
    public class ProspectSteps: BaseSteps
    {
        public string UserName = CommonSteps.UserAccount.ToLower();

        [Given(@"Start Import of Interactions")]
        public void GivenStartImportOfInteractions(Table interactions)
        {
            try
            {
                string getXSearchInput = "//div[contains(@class,'bbui-pages-contentcontainer') and not(contains(@class,'hide'))]//input[@placeholder='Search']";
                BBCRMHomePage.OpenFunctionalArea("Administration"); //open the admin functional area
                Dialog.WaitClick("//div[contains(h3,'Tools')]//button[contains(@class,'linkbutton')]/div[text()='Import']", 15);

                var interaction = interactions.Rows[0];
                if (interaction.ContainsKey("Name") && !String.IsNullOrEmpty(interaction["Name"]))
                {
                    Dialog.SetTextField(getXSearchInput, interaction["Name"]);
                    Dialog.GetDisplayedElement(getXSearchInput).SendKeys(Keys.Tab);
                }
                System.Threading.Thread.Sleep(4000); //pause because otherwise the code runs so rapidly that the next line is not invoked in time
                string xSelectedRow = "//div[contains(@id,'bbui-gen-pagecontainer')]//div[contains(@id, '')]//div[@class='x-grid3-body']/div[1]//td[2]";
                xSelectedRow = "//div[contains(@id,'ext-gen')]/div/table/tbody/tr[1]/td[3]/div/a";
                Dialog.WaitClick(xSelectedRow, 15);

            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not start the import of interactions. " + ex.Message);
            }

        }

        [When(@"I add prospect constituency to '(.*)'")]
        public void WhenIAddProspectConstituencyTo(string lastName, Table prospects)
        {
            try
            {
                lastName += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive
                BBCRMHomePage.OpenProspectsFA();  // open the prospect functional area

                if (prospects != null || prospects.Rows.Count > 0)
                {
                    BBCRMHomePage.OpenProspectsFA(); // open the prospect functional area
                    //click on the link to create a prospect from a constituent
                    Dialog.WaitClick("//button[contains(@class,'linkbutton')]/div[text()='Add a Prospect']", 15);
                    // add prospect status for the constituent
                    Dialog.SetSearchList(Dialog.getXInput("ProspectWithoutConstituentAddForm", "_PROSPECTID_value"), Dialog.getXInput("NonProspectSearch", "_KEYNAME_value"), lastName);
                    Dialog.Save();
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a prospect constituency. " + ex.Message);
            }
        }

        [Then(@"a prospect constituency is added to '(.*)'")]
        public void ThenAProspectConstituencyIsAddedTo(string lastName, Table prospects)
        {
            try
            {
                lastName += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive
                GetConstituentPanel(lastName); // open the constituent functional area and start search

                foreach (var prospect in prospects.Rows)  // see if the constituent has prospect status
                {
                    if (!ConstituentPanel.ConstituencyExists(prospect))
                        throw new ArgumentException(String.Format("Constituent page '{0}' does have prospect '{1}'",
                            lastName, prospect));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not determine that a prospect constituency was added. " + ex.Message);
            }
        }

        private void GetConstituentPanel(string lastName)
        {
            if (!ConstituentPanel.IsLastName(lastName))
            {
                BBCRMHomePage.OpenConstituentsFA(); //Open the Constituent Functional area 

                ConstituentsFunctionalArea.ConstituentSearch(lastName);  // search for the constituent using the last name
            }
        }
        [Given(@"prospect '(.*)' exists")]
        public void GivenProspectExists(string lastName)
        {
            try
            {

                lastName += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive
                BBCRMHomePage.OpenConstituentsFA(); //Open the Constituent Functional area 
                ConstituentsFunctionalArea.AddAnIndividual();  // add a constituent
                IndividualDialog.SetLastName(lastName);  // enter the last name
                IndividualDialog.SetTextField("//input[contains(@id,'_EMAILADDRESS_EMAILADDRESS_value')]", "testemail@unc.edu");
                IndividualDialog.SetTextField("//input[contains(@id,'_BIRTHDATE_value')]", "12/10/1952");
                IndividualDialog.Save();

                BBCRMHomePage.OpenProspectsFA();  // open the prospect functional area
                //click on the link to create a prospect from a constituent
                Dialog.WaitClick("//button[contains(@class,'linkbutton')]/div[text()='Add a Prospect']", 15);
                // add prospect status for the constituent
                Dialog.SetSearchList(Dialog.getXInput("ProspectWithoutConstituentAddForm", "_PROSPECTID_value"), Dialog.getXInput("NonProspectSearch", "_KEYNAME_value"), lastName);
                Dialog.Save();
              
                 
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not determine that the given prospect exists. " + ex.Message);
            }
        }

        [Given(@"fundraiser '(.*)' exists")]
        public void GivenFundraiserExists(string lastName)
        {
            try
            {
                lastName += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive
                BBCRMHomePage.OpenConstituentsFA(); //Open the Constituent Functional area 
                ConstituentsFunctionalArea.AddAnIndividual(); // add a constituent
                IndividualDialog.SetLastName(lastName);  // enter the last name
                IndividualDialog.Save();
                ConstituentPanel.AddConstituency("Fundraiser"); // add the fundrasiser constituency
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a fundraiser. " + ex.Message);
            }
        }

        [Given(@"prospect team role '(.*)' exists for '(.*)'")]
        public void GivenProspectTeamRoleExistsFor(string teamRole, string lastName)
        {
            try
            {
                lastName += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive
                GetConstituentPanel(lastName);  //open the constituent functional area and do a search
                AddTeamRole(teamRole); // add a team role
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not find the prospect team role for a constituent. " + ex.Message);
            }
        }
        [When(@"I add team member to '(.*)'")]
        public void WhenIAddTeamMemberTo(string lastName, Table teamMembers)
        {
            try
            {
                lastName += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive
                GetConstituentPanel(lastName); //open the constituent functional area and do a search
                foreach (var teamMember in teamMembers.Rows)  // add a team member
                {
                    teamMember["Team member"] = teamMember["Team member"] + uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive
                    AddTeamMember(teamMember);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a team member. " + ex.Message);
            }
        }

        //[Then(@"the '(.*)' team member exists")]
        //public void ThenTheTeamMemberExists(string groupCaption, Table teamMembers)
        //{
        //    try
        //    {
        //        foreach (var teamMember in teamMembers.Rows)  // search for a team member
        //        {
        //            if (teamMember["Name"] != string.Empty) teamMember["Name"] = teamMember["Name"] + uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive
        //            if (!TeamMemberExists(teamMember))
        //                throw new ArgumentException(
        //                    String.Format("Current constituent page does not have the team member '{0}'", teamMember));
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        throw new Exception("Error: could not find that a team member exists. " + ex.Message);
        //    }
        //}
        public static void AddTeamRole(string teamRole)
        {
            try
            {
                ConstituentPanel.SelectTab("Prospect"); // open the prospect tab
                ConstituentPanel.SelectInnerTab("UNC Prospect Team");  // prospect team subtab
                ConstituentPanel.ClickSectionAddButton("UNC Prospect Team", "Add team member");  // add a team member
                // set the role for the team member
                ConstituentPanel.SetTextField(Dialog.getXInput("ProspectTeamAddDataForm", "PROSPECTTEAMROLECODEID"), teamRole);
                //Dialog.Yes();
                Dialog.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a team role. " + ex.Message);
            }
        }
        [When(@"add Prospect Manager '(.*)'")]
        public void WhenAddProspectManager(string prospectManager)
        {
            try
            {
                prospectManager += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive
                ConstituentPanel.ClickButton("Edit prospect manager");  // edit the prospect manager
                Dialog.SetSearchList(Dialog.getXInput("ProspectManagerEditForm2", "_PROSPECTMANAGERFUNDRAISERID_value"), Dialog.getXInput("FundraiserSearch", "_KEYNAME_value"), prospectManager);
                //set the search to the fundraiser name as the prospect manager
                Dialog.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could notadd a prospect manager. " + ex.Message);
            }
        }
        [When(@"I add plan outline ""(.*)"" to major giving setup")]
        public void WhenIAddPlanOutlineToMajorGivingSetup(string planName, Table planSteps)
        {
            try
            {
                planName += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive
                BBCRMHomePage.OpenProspectsFA();  // open the prospect functional area
                ProspectsFunctionalArea.MajorGivingSetup();  // setup major giving
                MajorGivingSetupPanel.AddPlanOutline(planName, planSteps);  // add a plan
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a plan outline to a major giving setup. " + ex.Message);
            }
        }

        [Then(@"the plan outline ""(.*)"" is created with ""(.*)"" steps")]
        public void ThenThePlanOutlineIsCreatedWithSteps(string planName, int numberOfSteps)
        {
            planName += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive
            // add the plan with the various steps below
            var headers = new string[2];
            headers[0] = "Type";
            headers[1] = "Steps";
            var plans = new Table(headers);
            var firstRow = new string[2];
            firstRow[0] = planName;
            firstRow[1] = numberOfSteps.ToString();
            plans.AddRow(firstRow);
            foreach (var plan in plans.Rows)
            {
                if (!MajorGivingSetupPanel.PlanOutlineExists(plan))
                    throw new ArgumentException("Plan outline " + planName + " does not exist.");
            }
        }
        [When(@"I Add a Plan to a Planned-Gift Prospect '(.*)' With Fundraiser '(.*)' and Plan '(.*)'")]
        public void WhenIAddAPlanToAPlanned_GiftProspectWithFundraiserAndPlan(string prospectName, string fundraiserName, string planName)
        {
            try
            {

                prospectName += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive
                fundraiserName += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive
                planName += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive

                BBCRMHomePage.OpenConstituentsFA();  //Open constituent functional area
                SearchProspect(prospectName);  //find the prospect

                ConstituentPanel.SelectTab("Prospect");
                ConstituentPanel.SelectInnerTab("Plans"); // go to the plans tab for the prospect
                Panel.ClickSectionAddButton("Plans", "Add"); // click to add a plan
                string xLinkPath = "//a[contains(@id,'bbui-gen-tbaraction-')]/span[./text() = 'Add Prospect Plan']";
                Panel.WaitClick(xLinkPath, 15);
                Dialog.OpenTab("Details");  //open the details tab 
                // add plan name, plan type, start date and fundraiser data to the new plan
                Dialog.SetTextField("//input[contains(@id,'_PROSPECTPLAN_NAME_value')]", planName);
                Dialog.SetDropDown("//input[contains(@id,'_PROSPECTPLANTYPECODEID_value')]", "Annual Giving");
                Dialog.SetTextField("//input[contains(@id,'_STARTDATE_value')]", DateTime.Today.ToShortDateString());
                Dialog.SetSearchList(Dialog.getXInput("dataformdialog_", "_PRIMARYMANAGERFUNDRAISERID_value"), Dialog.getXInput("searchdialog", "_KEYNAME_value"), fundraiserName);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a plan to a planned-gift prospect. " + ex.Message);
            }

        }

        [When(@"I Add a Plan to a Prospect '(.*)' With Fundraiser '(.*)' and Plan '(.*)'")]
        public void WhenIAddAPlanToAAProspectWithFundraiser(string prospectName, string fundraiserName, string planName)
        {
            try
            {
                prospectName += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive
                fundraiserName += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive
                planName += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive

                BBCRMHomePage.OpenConstituentsFA();  //Open constituent functional area
                SearchProspect(prospectName);  //find the prospect

                ConstituentPanel.SelectTab("Prospect");
                ConstituentPanel.SelectInnerTab("Plans"); // go to the plans tab for the prospect
                Panel.ClickSectionAddButton("Plans", "Add"); // click to add a plan
                string xLinkPath = "//a[contains(@id,'bbui-gen-tbaraction-')]/span[./text() = 'Add Prospect Plan']";
                Panel.WaitClick(xLinkPath, 15);
                Dialog.OpenTab("Details");  //open the details tab 
                // add plan name, plan type, start date and fundraiser data to the new plan
                Dialog.SetTextField("//input[contains(@id,'_PROSPECTPLAN_NAME_value')]", planName);
                Dialog.SetDropDown("//input[contains(@id,'_PROSPECTPLANTYPECODEID_value')]", "Annual Giving");
                Dialog.SetTextField("//input[contains(@id,'_STARTDATE_value')]", DateTime.Today.ToShortDateString());
                Dialog.SetSearchList(Dialog.getXInput("dataformdialog_", "_PRIMARYMANAGERFUNDRAISERID_value"), Dialog.getXInput("searchdialog", "_KEYNAME_value"), fundraiserName);

            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a plan to a prospect. " + ex.Message);
            }
        }

        public void SearchProspect(string prospectName)
        {

            BBCRMHomePage.OpenProspectsFA();

            Dialog.WaitClick("//button[contains(@class,'linkbutton')]/div[text()='Search Prospects']", 15);
            //For the advanced search options, allow for including organizations, groups and individuals
            Dialog.SetTextField("//input[contains(@id,'_KEYNAME_value')]", prospectName);
            Dialog.WaitClick("//a[contains(@id,'_SHOWADVANCEDOPTIONS_action')]", 15);
            Dialog.SetCheckbox("//input[contains(@id,'_INCLUDEORGANIZATIONS_value')]", false);
            Dialog.SetCheckbox("//input[contains(@id,'_INCLUDEGROUPS_value')]", false);
            Dialog.SetCheckbox("//input[contains(@id,'_INCLUDEINDIVIDUALS_value')]", true);
            //search for a specific last name
            SearchDialog.Search();
            SearchDialog.SelectFirstResult();
        }
        [When(@"Add an Opportunity to a Prospect '(.*)' With Fundraiser '(.*)' for Plan '(.*)'")]
        public void WhenAddAnOpportunityToAProspectWithFundraiserForPlan(string prospectName,  string fundraiserName, string planName, Table batchRows)
        {
            try
            {
                prospectName += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive
                fundraiserName += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive
                planName += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive
                string xPath = "*//div/table/tbody/tr[1]/td[9]/div/a[text()='" + planName + "']";  //this path goes to the link which opens to the plan page
                Dialog.WaitClick(xPath, 15);
                Panel.OpenTab("Opportunities"); // open the opportunities tab

                string xPathAdd = "//div[contains(@class,'datalist-repeaterview')]//td[contains(@class,'x-toolbar-cell') and not (contains(@class,'x-hide-display'))]" +
                        "//table[contains(@id,'bbui-gen-tbaraction-')and not(contains(@class,'hide'))]/tbody/tr[2]/td[2]/em/button[./text() = 'Add']";
                //click on the add opportunities button
                Panel.WaitClick(xPathAdd, 20);
                // add the various strings to the controls on the opportuntiies dialog
                Dialog.SetDropDown("//input[contains(@id,'_STATUSCODE_value')]", "Accepted");
                Dialog.SetDropDown("//input[contains(@id,'_LIKELIHOODTYPECODEID_value')]", "Even (50%)");
                Dialog.SetTextField("//input[contains(@id,'_EXPECTEDASKDATE_value')]", "12/10/2016");
                Dialog.SetTextField("//input[contains(@id,'_EXPECTEDCLOSEDATE_value')]", "12/17/2016");
                Dialog.SetTextField("//input[contains(@id,'_EXPECTEDASKAMOUNT_value')]", "$300.00");
                Dialog.SetTextField("//input[contains(@id,'_ASKAMOUNT_value')]", "$300.00");
                Dialog.SetTextField("//input[contains(@id,'_ASKDATE_value')]", "11/10/2016");
                Dialog.SetTextField("//input[contains(@id,'_RESPONSEDATE_value')]", "11/11/2016");
                Dialog.SetTextField("//input[contains(@id,'_RESPONSEDATE_value')]", "11/11/2016");
                Dialog.SetDropDown("//input[contains(@id,'_EXPECTEDCOMMITMENTTYPE_value')]", "Cash");

                int rowCount = 1;
                //map the column captions to their index
                IDictionary<string, int> columnCaptionToIndex = new Dictionary<string, int>();
                foreach (string caption in batchRows.Rows[0].Keys)
                {
                    columnCaptionToIndex.Add(caption,
                        Dialog.GetDatalistColumnIndex(Dialog.getXGridHeaders("dataformdialog_", "_OpportunityAddForm"), caption));
                }

                //add the rows
                foreach (TableRow row in batchRows.Rows)
                {
                    foreach (string caption in row.Keys)
                    {
                        //Add if value provided using appropriate field set.  Use rowCount and column caption mapping when making XPath
                        if (row[caption] == string.Empty) continue;
                        string gridXPath = Dialog.getXGridCell("dataformdialog_", "_OpportunityAddForm", rowCount, columnCaptionToIndex[caption]);
                        string gridRowXPath = Dialog.getXGridRow("dataformdialog_", "_OpportunityAddForm", rowCount);
                        string value = row[caption];
                        switch (caption)
                        {
                            //add the values to the various columns (which have the following captions)
                            case "Designation":
                                Dialog.SetGridTextField(gridXPath, value);
                                break;
                            case "Funding Method":
                                Dialog.SetGridDropDown(gridXPath, value);
                                break;
                            case "Amount":
                                Dialog.SetGridTextField(gridXPath, value);
                                break;

                            default:
                                throw new NotSupportedException(String.Format("Column '{0}' is not supported by the default AddPlanOutline method.  Additional implementation is required.", caption));
                        }
                    }
                    rowCount++;
                }

                Dialog.ClickButton("Distribute evenly", 50);
                Dialog.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a opportunity to a prospect. " + ex.Message);
            }
        }
        [When(@"Add Contact Report '(.*)'")]
        public void WhenAddContactReport(string planName, Table batchRows)
        {
            try
            {
                planName += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive
                string xPath = "*//div/table/tbody/tr[1]/td[9]/div/a[text()='" + planName + "']";  //this path goes to the link which opens to the plan page
                Dialog.WaitClick(xPath, 20);

                if (batchRows.RowCount != 1) throw new ArgumentException("Only provide one row to select.");
                var batchRow = batchRows.Rows[0];  // select only the first row of the batch
                if (batchRow.ContainsKey("Owner") && batchRow["Owner"] != null &&
                    batchRow["Owner"] != string.Empty)
                    batchRow["Owner"] += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other

                PlanPanel.SelectSectionDatalistRow(batchRow, "Planned and pending steps");  //Open the row that has the data in the batchRows batch
                PlanPanel.ClickButton("Contact report"); //click on the contact report button
                PlanPanel.WaitClick("//a[contains(@id,'bbui-gen-tbaraction')]/span[text()='File a contact report']", 20); // click on the "file a contact report" button

            }
            catch (Exception ex)
            {               
                throw new Exception("Error: could not add a contact report to a prospect. " + ex.Message);
            }
        }

        [Then(@"the Plan and Opportunity Is Added")]
        public void ThenThePlanAndOpportunityIsAdded()
        {
            try
            {
                Panel.ClickButton("Go to opportunity");  // The button functions if the opportunity exists.
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add an opportunity to a prospect. " + ex.Message);
            }
        }
        [When(@"I add plan outline ""(.*)"" to prospect")]
        public void WhenIAddPlanOutlineToProspect(string fundraiserName, Table steps)
        {
            try
            {
                Dialog.OpenTab("Steps");
                Dialog.SetTextField("//input[contains(@id,'_OUTLINELIST_value')]", "Commitment");
                int rowCount = 1;
                //map the column captions to their index
                IDictionary<string, int> columnCaptionToIndex = new Dictionary<string, int>();
                foreach (string caption in steps.Rows[0].Keys)
                {
                    columnCaptionToIndex.Add(caption,
                        Dialog.GetDatalistColumnIndex(Dialog.getXGridHeaders("_ProspectPlanAddForm", "STEPS_value"), caption));
                }

                //add the rows
                foreach (TableRow step in steps.Rows)
                {
                    foreach (string caption in step.Keys)
                    {
                        //Add if value provided using appropriate field set.  Use rowCount and column caption mapping when making XPath
                        if (step[caption] == string.Empty) continue;
                        string gridXPath = Dialog.getXGridCell("_ProspectPlanAddForm", "STEPS_value", rowCount, columnCaptionToIndex[caption]);
                        string gridRowXPath = Dialog.getXGridRow("_ProspectPlanAddForm", "STEPS_value", rowCount);
                        string value = step[caption];
                        switch (caption)
                        {
                                //add the values in the various columns (which have the following captions)
                            case "Owner":
                                value += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive  
                                Dialog.SetGridTextField(gridXPath, value);
                                break;
                            case "Stage":
                                Dialog.SetGridDropDown(gridXPath, value);
                                break;
                            case "Expected date":
                                Dialog.SetGridTextField(gridXPath, value);
                                break;
                            case "Actual date":
                                Dialog.SetGridTextField(gridXPath, value);
                                break;
                            case "Objective":
                                Dialog.SetGridTextField(gridXPath, value);
                                break;
                            case "Status":
                                Dialog.SetGridDropDown(gridXPath, value);
                                break;
                            case "Contact method":
                                Dialog.SetGridDropDown(gridXPath, value);
                                break;
                            default:
                                throw new NotSupportedException(String.Format("Column '{0}' is not supported by the default AddPlanOutline method.  Additional implementation is required.", caption));
                        }
                    }
                    rowCount++;
                }
                Dialog.Save();
                System.Threading.Thread.Sleep(4000);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a plan outline to a prospect. " + ex.Message);
            }
        }
        [Then(@"the Plan '(.*)' is added with plan type '(.*)' for '(.*)'")]
        public void ThenThePlanIsAddedWithPlanType(string planName, string planType, string errMsg,  Table planTable)
        {
           // (the cancelled status constraint comes from C:\svn\CRM4\UNC.Catalog\UNC.Catalog\Prospect\UNCAddConstraintInteractionStatusCode.RecordOperation.xml)
            //UNCAddConstraintInteractionActualDate.RecordOperation.xml has the constraint that actual dates may not be in the future

            planName += uniqueStamp; //The unique stamp is a series of numbers added to the end to keep names distinctive  
            TableRow planRow = planTable.Rows[0];

            try
            {
                planRow["Plan name"] = planName;
                planRow["Start date"] = DateTime.Today.ToShortDateString();  // use today as a start date

                if (errMsg == "cancelled")
                {
                    errMsg = "One of the steps was cancelled. ";
                }
                if (errMsg == "future")
                {
                    errMsg = "The actual date is in the future. ";
                }
                //see if the message that data could not be saved is showing
                string Target;
                Target = Panel.GetDisplayedElement("//div[contains(@class,'ext-mb-content')]//span[contains(@class,'ext-mb-text')]", 20).Text;

                // if that message does not exist or there is a different one, then send an error
                if ((Target.Substring(0, 24) != "Data could not be saved.") || (Target == null))
                {
                    throw new Exception("Error: A prospect plan was added when it should not have been. ");
                }
                else
                {
                    Dialog.OK();  // close the error popup
                    Dialog.Cancel(); // cancel saving the relationship
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error: A prospect plan was added when it should not have been. " + ex.Message);
            }
        }

        [Then(@"the Plan Interactions have been saved")]
        public void ThenThePlanInteractionsHaveBeenSaved()
        {
            try
            {
                //UNCAddConstraintInteractionActualDate.RecordOperation.xml has the constraint that actual dates may not be in the future
                //go to the add contact dialog form
                DateTime futureDate = DateTime.Today.AddYears(5);  // use a future date to verify that the date won't be saved
                //save the actual date and the comment
                Dialog.SetTextField("//input[contains(@id,'_ACTUALDATE_value')]", futureDate.ToShortDateString() );
                Dialog.SetTextField("//textarea[contains(@id,'_COMMENT_value')]", "Short test comment" );
                Dialog.Save();

                string Target;
                Target = Panel.GetDisplayedElement("//div[contains(@class,'ext-mb-content')]//span[contains(@class,'ext-mb-text')]", 20).Text;

                // if that message does not exist or there is a different one, then send an error
                if ((Target.Substring(0, 24) != "Data could not be saved.") || (Target == null))
                {
                    throw new Exception("Error: A prospect plan was added when it should not have been. ");
                }
                else
                {
                    Dialog.OK();  // close the error popup
                    Dialog.Cancel(); // cancel saving the relationship
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not save the interaction. " + ex.Message);
            }
        }
        [Given(@"Edit Import Interaction Process '(.*)'")]
        public void GivenEditImportInteractionProcess(string fileName)
        {
            try
            {
                Dialog.WaitClick("//button[contains(@class,'bbui-linkbutton')]/div[text()='Edit process']", 15);  // click on the Edit Process link
                Dialog.OpenTab("Configure import file"); // open the configure import file tab
                Dialog.SetDropDown("//input[contains(@id, '_IMPORTSOURCEID_value')]", "Default network directory"); // use default network directory
                Dialog.SetDropDown("//input[contains(@id, '_FILE_value')]", fileName); // use the selected file
                Dialog.OpenTab("Map fields");
                Dialog.OpenTab("Map fields");
                Dialog.SetCheckbox("//input[contains(@id, '_MAPPINGTYPECODEID_1')]", true);  // Use file mapping template
                Dialog.SetDropDown("//input[contains(@id, '_IMPORTFILETEMPLATEID_value')]", "CAS-InteractionImport (delimited file)"); // use a comma delimited file
                Dialog.OpenTab("Set options");
                Dialog.SetCheckbox("//input[contains(@id, '_BATCHCOMMITOPTION_2')]", true);  // Commit batches if they have no batch exceptions
                Dialog.Save();
                Dialog.WaitClick("//button[contains(@class,'bbui-linkbutton')]/div[text()='Start process']", 15); // start the importing
                System.Threading.Thread.Sleep(4000);
            }

            catch (Exception ex)
            {
                throw new Exception("Error: could not edit the interaction import process. " + ex.Message);
            }
        }


        public static void AddTeamMember(TableRow teamMember)
        {
            Panel.SelectTab("Prospect");  // go to the Prospect tab
            Panel.SelectInnerTab("Prospect Team");  // go to the Prospect team inner tab
            Panel.ClickSectionAddButton("UNC Prospect Team", "Add team member");  // add a Prospect Team member
            foreach (string caption in teamMember.Keys)  //add the fields in the team member form
            {
                switch (caption)
                {
                    case "Team member":
                        Dialog.SetSearchList(Dialog.getXInput("ProspectTeamAddDataForm", "MEMBERID"), Dialog.getXInput("FundraiserSearch", "KEYNAME"), teamMember[caption]);
                        break;
                    case "Role":
                        Dialog.SetDropDown(Dialog.getXInput("ProspectTeamAddDataForm", "PROSPECTTEAMROLECODEID"), teamMember[caption]);
                        break;
                    case "Start date":
                        Dialog.SetTextField(Dialog.getXInput("ProspectTeamAddDataForm", "DATEFROM"), teamMember[caption]);
                        break;
                    case "End date":
                        Dialog.SetTextField(Dialog.getXInput("ProspectTeamAddDataForm", "DATETO"), teamMember[caption]);
                        break;
                    default:
                        throw new NotImplementedException("Field '" + caption + "' is not an implemented field for the 'Add a prospect team member' dialog.");
                }
            }
            Dialog.Save();
        }
        [When(@"I add a Prospect Request '(.*)'")]
        public void WhenIAddAProspectRequest(string prospectName)
        {
            try
            {
                prospectName += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive
                BBCRMHomePage.OpenProspectsFA(); // open prospects functional area
                Dialog.WaitClick("//button[contains(@class,'linkbutton')]/div[text()='Add a prospect research request']", 20);  //click on the research request link
                //set the fields on the request dialog form
                Dialog.SetTextField("//input[contains(@id,'_PROSPECTRESEARCHREQUESTPRIORITYCODEID_value')]", "No Meeting Planned");
                Dialog.SetTextField("//input[contains(@id,'_PROSPECTRESEARCHREQUESTTYPECODEID_value')]", "Research Profile");
                Dialog.SetTextField("//input[contains(@id,'_PROSPECTRESEARCHREQUESTREASONCODEID_value')]", "Development Officer Request");
                Dialog.SetSearchList(Dialog.getXInput("_ProspectResearchRequestAddForm", "_REQUESTEDBYID_value"), Dialog.getXInput("FundraiserSearch", "_KEYNAME_value"), "Root");
                string gridXPath = Dialog.getXGridCell("dataformdialog_", "CONSTITUENTS_value", 1, 2);
                Dialog.SetGridTextField(gridXPath, prospectName);
                Dialog.Save();

            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not save the interaction. " + ex.Message);
            }
        }

        [Then(@"a Prospect Request is Added for '(.*)'")]
        public void ThenAProspectRequestIsAdded(string prospectName)
        {
            try
            {
                prospectName += uniqueStamp;  //The unique stamp is a series of numbers added to the end to keep names distinctive
                BBCRMHomePage.OpenProspectsFA(); // open prospects functional area
                SearchProspect(prospectName); //search for the prospect
                //string for the XPath for the notification button
                string notificationString = "//div[@class='bbui-pages-summary-infobar']/button[contains(./text(), 'Notifications:') and contains(./text(), 'Click here for more information.')]";

                Panel.WaitClick(notificationString, 10); //click on the notification button
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not save the prospect request. " + ex.Message);
            }
        }
        [When(@"Add a Planned Gift")]
        public void WhenAddAPlannedGift(Table giftTable)
        {
            try
            {
                Panel.SelectTab("Planned Gifts");  // go to the planned gifts tab
                Panel.ClickSectionAddButton("Planned gifts");  //click on the add button to create a new planned gift

                // add values for three fields on the form.
                Dialog.SetTextField("//input[contains(@id,'_EXPECTEDGIFTAMOUNT_value')]", "$450.00");
                Dialog.SetCheckbox("//input[contains(@id,'_ISREVOCABLE_value')]", true);
                Dialog.SetTextField("//input[contains(@id,'_GIFTDATE_value')]", "11/17/2016");

                SetPlannedGiftRow(giftTable);  //set the items on the grid

                //click on the "distribute evenly" and the "save buttons
                Dialog.ClickButton("Distribute evenly", 50);
                Dialog.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a planned gift. " + ex.Message);
            }
        }
        [Then(@"the Planned Gift Is Added")]
        public void ThenThePlannedGiftIsAdded()
        {
            try
            {
                Panel.ClickSectionAddButton("Planned gift details", "Edit");  // just testing to see if the planned gift is there. If so, the edit button is visible.
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not determine that a planned gift was added. " + ex.Message);
            }
        }
        public void SetPlannedGiftRow(Table batchRows)
        {
            IDictionary<string, int> columnCaptionToIndex = new Dictionary<string, int>();
            int rowCount = 1;

            //get the captions on the prospect planned gift add form
            foreach (string caption in batchRows.Rows[0].Keys)
            {
                columnCaptionToIndex.Add(caption,
                    Dialog.GetDatalistColumnIndex(Dialog.getXGridHeaders("dataformdialog_", "_ProspectPlannedGiftAddForm"), caption));
            }

            //add the rows
            foreach (TableRow row in batchRows.Rows)
            {
                foreach (string caption in row.Keys)
                {
                    //Add if value provided using appropriate field set.  Use rowCount and column caption mapping when making XPath
                    if (row[caption] == string.Empty) continue;
                    string gridXPath = Dialog.getXGridCell("dataformdialog_", "_ProspectPlannedGiftAddForm", rowCount, columnCaptionToIndex[caption]);
                    string gridRowXPath = Dialog.getXGridRow("dataformdialog_", "_ProspectPlannedGiftAddForm", rowCount);
                    string value = row[caption];
                    switch (caption)
                    {
                        //add the values to the various columns (which have the following captions)
                        case "Designation":
                            Dialog.SetGridTextField(gridXPath, value);
                            break;
                        case "Amount":
                            Dialog.SetGridTextField(gridXPath, value);
                            break;

                        default:
                            throw new NotSupportedException(String.Format("Column '{0}' is not supported by the default AddPlanOutline method.  Additional implementation is required.", caption));
                    }
                }
                rowCount++;
            }

        }

    }
}
