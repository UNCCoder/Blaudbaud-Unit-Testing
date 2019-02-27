using System;
using Blackbaud.UAT.Core.Base;
using Blackbaud.UAT.Core.Crm;
using TechTalk.SpecFlow;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using Blackbaud.UAT.Core.Crm.Dialogs;
using Blackbaud.UAT.Core.Crm.Panels;

namespace UnitTestProject
{
    [Binding]
    public class ConstituentSteps: BaseSteps
    {

        [Given(@"constituent '(.*)' exists")]
        public void GivenConstituentExists(string lastName)
        {
            //add the constituent with the specified last name
            WhenIAddConstituent(lastName);
        }
        [When(@"I add constituent ""(.*)""")]
        public void WhenIAddConstituent(string lastName)
        {
            try
            {
                if (lastName != "Testing")
                {
                    //add the constituent with the specified last name
                    lastName += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other
                }
                BBCRMHomePage.OpenConstituentsFA();  //Open constituent functional area
               // add an individual with the last name given
                ConstituentsFunctionalArea.AddAnIndividual();
                IndividualDialog.SetLastName(lastName);
                IndividualDialog.SetDropDown("//input[contains(@id,'_EMAILADDRESS_EMAILADDRESSTYPECODEID_value')]", "Business");
                IndividualDialog.SetTextField("//input[contains(@id,'_EMAILADDRESS_EMAILADDRESS_value')]", "testperson@test.com");
                IndividualDialog.SetTextField("//input[contains(@id,'_BIRTHDATE_value')]", "11/18/1952");
                IndividualDialog.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add constituent with the specified last name. " + ex.Message);
            }
        }
        [When(@"I add constituent")]
        public void WhenIAddConstituent(Table batchRows)
        {
            try {
                BBCRMHomePage.OpenConstituentsFA();  //Open constituent functional area
                string lastName = "";

                if (batchRows.RowCount != 1)
                    throw new ArgumentException("Only provide one row to select.");

                var batchRow = batchRows.Rows[0];  // select only the first row of the batch
                if (batchRow.Keys.Contains("Last name") && (!String.IsNullOrEmpty(batchRow["Last name"])))
                {
                    batchRow["Last name"] += uniqueStamp;
                    lastName = batchRow["Last name"];
                    //Add an individual constituent
                    ConstituentsFunctionalArea.AddAnIndividual(batchRow, timeout:30);
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not search for specified name. " + ex.Message);
            }
        }
        [When(@"I add constituent ""(.*)""")]
        public void WhenIAddConstituent(string lastName, Table batchRows )
        {
            try
            {
                if (lastName != "Testing")
                {
                    lastName += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other
                }
                if (batchRows.RowCount != 1)
                    throw new ArgumentException("Only provide one row to select.");
                var batchRow = batchRows.Rows[0];  // select only the first row of the batch

                BBCRMHomePage.OpenConstituentsFA();  //Open constituent functional area

                ConstituentsFunctionalArea.AddAnIndividual();
                IndividualDialog.SetLastName(lastName);
                IndividualDialog.SetIndividualFields(batchRow);
                IndividualDialog.ClickButton("Validate", 50);
                IndividualDialog.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a constituent with the specified last name. " + ex.Message);
            }
        }
        [When(@"I add spouse ""(.*)""")]
        public void WhenIAddSpouse(string lastName, Table batchRows)
        {
            try
            {
                lastName += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other
                if (batchRows.RowCount != 1)
                    throw new ArgumentException("Only provide one row to select.");
                var batchRow = batchRows.Rows[0];  // select only the first row of the batch

                BBCRMHomePage.OpenConstituentsFA();  //Open constituent functional area
                ConstituentsFunctionalArea.AddAnIndividual(); // add an individual
                IndividualDialog.SetLastName(lastName);
                IndividualDialog.SetIndividualFields(batchRow); //set the individual's fields according to the batch parameter
                IndividualDialog.ClickButton("Validate", 50); //validate the address
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a spouse. " + ex.Message);
            }
   
        }

        [When(@"Add a Household")]
        public void WhenAddAHousehold(Table batchRows)
        {
            try
            {
                string lastName = "";
                if (batchRows.RowCount != 1)
                    throw new ArgumentException("Only provide one row to select.");
                var batchRow = batchRows.Rows[0];  // select only the first row of the batch
                if (batchRow.Keys.Contains("Related individual") && (!String.IsNullOrEmpty(batchRow["Related individual"])))
                {
                    batchRow["Related individual"] += uniqueStamp; //select a spouse from constituents
                    lastName = batchRow["Related individual"];
                }

                IndividualDialog.SetHouseholdFields(batchRow);  //Set the fields setting up a household
                IndividualDialog.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a household. " + ex.Message);
            }
        }

        [When(@"I remove a spouse")]
        public void WhenIRemoveASpouse()
        {
            try
            {
                //click on the link to remove a spouse 
                Dialog.WaitClick("//button[contains(@class,'linkbutton')]/div[text()='Remove spouse']", 10);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not remove a spouse. " + ex.Message);
            }
        }
        
        [Then(@"household is dissolved")]
        public void ThenHouseholdIsDissolved()
        {
            try { 
                //dissolve a household because of death and set one of the spouses to be living and ther other deceased
                Dialog.SetTextField("//input[contains(@id,'_RELATIONSHIPTYPECODEID_value')]", "Living Spouse/Partner");
                Dialog.SetTextField("//input[contains(@id,'_RECIPROCALTYPECODEID_value')]", "Deceased Spouse/Partner");
                Dialog.SetTextField("//input[contains(@id,'_REMOVEDATE_value')]", "10/15/2016");
                Dialog.SetTextField("//input[contains(@id,'_DISSOLVEREASONCODEID_value')]", "Death");
                Dialog.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not dissolve the household. " + ex.Message);
            }
        }
        [When(@"I add a specialcode ""(.*)""")]
        public void WhenIAddASpecialcode(string SpecialCode) {

            try {
                //add a special code by selecting the special code tab and pressing the add button
                string formName = "UNCConstituentSpecialCodeDataFormAddDataForm";
                ConstituentPanel.SelectTab("UNC Special Codes");
                ConstituentPanel.ClickButton("Add");

                //enter a special code and a start date
                Dialog.SetTextField(Dialog.getXInput(formName, "_SPECIALCODEID_value"), SpecialCode);
                Dialog.SetTextField(Dialog.getXInput(formName, "_DATE_value"), "12/11/2015");
            }
            catch (Exception ex)  {
                throw new Exception ("Error: could not add special code. " + ex.Message);
            }
        }

        [Then(@"the specialcode ""(.*)"" has been created")]
        public void ThenTheSpecialcodeHasBeenCreated(string SpecialCode)
        {
            //save the special code.
            Dialog.Save();
        }
        [Given(@"constituent exists")]
        public void GivenConstituentExists(Table constituents)
        {
            try  {
                //find each constituent in a batch and add the unique stamp to the last name
                foreach (var constituent in constituents.Rows)   {
                    if (constituent.ContainsKey("Last name") && !string.IsNullOrEmpty(constituent["Last name"]))
                        constituent["Last name"] += uniqueStamp;
                    BBCRMHomePage.OpenConstituentsFA();  //Open constituent functional area
                    ConstituentsFunctionalArea.AddAnIndividual(constituent, timeout: 30);
                }
            }
            catch (Exception ex)   {
                throw new Exception("Error: could not find the given constituent. " + ex.Message);
            }
        }

        [When(@"Save the constituency")]
        public void WhenSaveTheConstituency()
        {
            try  {
                Dialog.Save();  //save the constituent
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not save the constituency. " + ex.Message);
            }

        }

        [When(@"I add friend constituency to '(.*)'")]
        public void WhenIAddFriendConstituencyTo(string lastName, Table friends)
        {
            try
            {
                lastName += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other
                BBCRMHomePage.OpenConstituentsFA();  //Open constituent functional area
                ConstituentsFunctionalArea.ConstituentSearch(lastName); // search by last name for a constituent
                string formName = "ConstituencyAddForm";
                if (friends == null || friends.Rows.Count == 0) // if there is only one row
                {
                    ConstituentPanel.AddConstituency("User-defined");  //click the user-defined link
                    Dialog.OK();
                    Dialog.SetTextField(Dialog.getXInput(formName, "_CONSTITUENCYCODEID_value"), "Friend"); // ad a friend constituency
                }
                else
                {
                    foreach (var friend in friends.Rows)  //add the friend constituency to several
                    {
                        ConstituentPanel.AddConstituency("Friend", friend);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add the friend constituency. " + ex.Message);
            }
        }
        private void GetConstituentPanel(string lastName)
        {
            try
            {
                if (!ConstituentPanel.IsLastName(lastName))
                {
                    BBCRMHomePage.OpenConstituentsFA();  //Open constituent functional area
                    ConstituentsFunctionalArea.ConstituentSearch(lastName); // search by last name for a constituent
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not open the constituent panel. " + ex.Message);
            }
        }

        [Then(@"a friend constituency is added to '(.*)'")]
        public void ThenAFriendConstituencyIsAddedTo(string lastName, Table friends)
        {
            try
            {
                lastName += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other
                GetConstituentPanel(lastName); // open constituent panel for the one with the selected lastname

                foreach (var friend in friends.Rows)
                {
                    if (!ConstituentPanel.ConstituencyExists(friend))
                        throw new ArgumentException(String.Format("Constituent page '{0}' does not have constituent as friend '{1}'",
                            lastName, friend));
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not verify the addition of a friend constituency. " + ex.Message);
            }
        }

        [Then(@"constituent '(.*)', '(.*)' exists")]
        public void ThenConstituentExists(string firstname, string lastname)
        {
            BBCRMHomePage.OpenConstituentsFA();  //Open constituent functional area
        }
        [Given(@"staff constituent ""(.*)"" exists")]
        public void GivenStaffConstituentExists(string lastName)
        {
            try
            {
                lastName += uniqueStamp;  // the unique stamp is a series of numbers to keep names different from each other
                BBCRMHomePage.OpenConstituentsFA(); // open the constituents functional area
                ConstituentsFunctionalArea.AddAnIndividual();  // add a new constituent
                IndividualDialog.SetLastName(lastName);
                IndividualDialog.Save();
                ConstituentPanel.AddConstituency("Staff"); // add the staff constituency
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Could not add a staff constituency. " + ex.Message);
            }
        }
        public bool CoordinatorExists(string coordinatorName)
        {
            try
            {
                EventPanel.SelectTab("Tasks/Coordinators");

                IDictionary<string, string> rowValues = new Dictionary<string, string>();
                rowValues.Add("Coordinator", coordinatorName);
                return EventPanel.SectionDatalistRowExists(rowValues, "Coordinators");
            }
            catch (Exception ex)
            {
                throw new Exception("Error: Could not add a coordinator for an event. " + ex.Message);
            }
        }

        [Then(@"'(.*)' is a coordinator for event '(.*)'")]
        public void ThenIsACoordinatorForEvent(string coordinatorName, string eventName)
        {
            try
            {
                coordinatorName += uniqueStamp;   // the unique stamp is a series of numbers to keep names different from each other
                eventName += uniqueStamp;   // the unique stamp is a series of numbers to keep names different from each other

                CoordinatorExists(coordinatorName);
            }

            catch (Exception ex)
            {
                throw new Exception("Error: could not find that a particular coordinator exists. " + ex.Message);
            }
        }

        [Then(@"constituent ""(.*)"" is created")]
        public void ThenConstituentIsCreated(string lastName)
        {
            //verify that the constituent is created with the specified last name
            if (lastName != "Testing")
            {
                lastName += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other
            }
            if (!ConstituentPanel.IsLastName(lastName))
                throw new ArgumentException("Current constituent page does not have the last name " + lastName);
        }
        [When(@"edit the selected constituent")]
        public void WhenEditTheSelectedConstituent(Table fieldMappings)
        {
            try
            {
                //find just one specific constituent to edit. Save.
                if (fieldMappings.RowCount != 1) throw new ArgumentException("Only provide one row of field values");
                EnhancedRevenueBatchDialog.EditConstituent(fieldMappings.Rows[0]);
                IndividualDialog.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not edit selected constituent. " + ex.Message);
            }
        }
        [When(@"I add a Note to '(.*)'")]
        public void WhenIAddANoteTo(string lastName, Table notes)
        {
            lastName += uniqueStamp;
            GetConstituentPanel(lastName);
            foreach (var note in notes.Rows)
            {
                if (note.Keys.Contains("Author")) note["Author"] = note["Author"] + uniqueStamp;
                ConstituentPanel.AddNote(note);
            }
        }

        [When(@"add a notification to note '(.*)'")]
        public void WhenAddANotificationToNote(string noteTitle, Table notifications)
        {
            foreach (var notification in notifications.Rows)
            {
                if (notification.Keys.Contains("Selection"))
                    notification["Selection"] = notification["Selection"] + uniqueStamp;
                ConstituentPanel.AddNotification(noteTitle, notification);
            }
        }

        [Then(@"the notification bar displays the Note '(.*)'")]
        public void ThenTheNotificationBarDisplaysTheNote(string noteTitle)
        {
            if (!ConstituentPanel.NotificationExists(noteTitle))
                throw new ArgumentException(
                    String.Format("Current constituent does not have a notification displayed for note '{0}'", noteTitle));
        }
        [Then(@"I open the '(.*)' tab")]
        public void ThenIOpenTheTab(string tabName)
        {
            Panel.SelectTab(tabName);
            Panel.ClickButton("View report");
            System.Threading.Thread.Sleep(8000); 
        }
        [When(@"Open the Batch")]
        public void WhenOpenTheBatch(Table batchRows)
        {
            try
            {
                if (batchRows.RowCount != 1) throw new ArgumentException("Only provide one row to select.");
                BBCRMHomePage.OpenRevenueFA();  //Open revenue functional area
                RevenueFunctionalArea.BatchEntry();
                var batchRow = batchRows.Rows[0];  // select only the first row of the batch
                if (batchRow.ContainsKey("Description") && batchRow["Description"] != null &&
                    batchRow["Description"] != string.Empty)
                    batchRow["Description"] += uniqueStamp;  //The unique stamp is a series of numbers to keep constituents different from each other


                BatchEntryPanel.SelectUncommittedBatch(batchRow); //select uncommitted batch according to the table parameter
                if (BatchEntryPanel.UncommittedBatchExists(batchRow))  // if the selected batch exists
                {
                    System.Threading.Thread.Sleep(2000); //pause because otherwise the code runs so rapidly that the next line is not invoked in time
                    BatchEntryPanel.SelectSectionDatalistRow(batchRow, "Uncommitted batches");
                    BatchEntryPanel.ClickButton("Edit batch");
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

        [Then(@"I See if the Prospect Tile text '(.*)' is Visible")]
        public void ThenISeeIfTheProspectTileTextIsVisible(string prospectText)
        {
            try
            {
                string VisibleText;
                //find out about the prospect tile 
                string XPathTile = "//div[contains(@class, 'bbui-pages-contentcontainer') and not(contains (@class, 'x-hide-display'))]//div[contains(@id, '_ConstituentSummaryProspectTileViewDataForm')]//a[contains(@id,'_PROSPECTLINK_action')]";
                IWebElement statusElement = Panel.GetDisplayedElement(XPathTile, 25);

                VisibleText = statusElement.Text;  //Does the prospect tile have the correct text?
                if (prospectText != VisibleText)
                {
                    throw new Exception("Error: The prospect tile had the incorrect text or is not visible.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not view the prospect tile. " + ex.Message);
            }
        }

        [Given(@"Start Import of Educational Involvement")]
        public void GivenStartImportOfEducationalInvolvement(Table involvementTable)
        {
            try
            {
                string getXSearchInput = "//div[contains(@class,'bbui-pages-contentcontainer') and not(contains(@class,'hide'))]//input[@placeholder='Search']";
                BBCRMHomePage.OpenFunctionalArea("Administration"); //open the admin functional area and then click on import button
                Dialog.WaitClick("//div[contains(h3,'Tools')]//button[contains(@class,'linkbutton')]/div[text()='Import']", 15);

                var involvementRow = involvementTable.Rows[0];
                if (involvementRow.ContainsKey("Name") && !String.IsNullOrEmpty(involvementRow["Name"])) // if there is a name (not blank)
                {
                    Dialog.SetTextField(getXSearchInput, involvementRow["Name"]);
                    Dialog.GetDisplayedElement(getXSearchInput).SendKeys(Keys.Tab);
                }
                Panel.SelectSectionDatalistRow(involvementRow, "Import processes");  //find the correct row which has the correct general correspondence process
                Panel.WaitClick(Panel.getXSelectedDatalistRowButton("Edit"));
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not start import of educational involvement. " + ex.Message);
            }
        }

        [Given(@"Edit Import Involvement Process '(.*)'")]
        public void GivenEditImportInvolvementProcess(string fileName, Table involvementTable)
        {
            var involvementRow = involvementTable.Rows[0];

            try
            {
                Dialog.OpenTab("Configure import file"); // open the configure import file tab
                Dialog.SetDropDown("//input[contains(@id, '_IMPORTSOURCEID_value')]", "Default network directory"); // use default network directory
                Dialog.SetDropDown("//input[contains(@id, '_FILE_value')]", fileName); // use the selected file
                Dialog.OpenTab("Map fields");
                Dialog.OpenTab("Map fields");
                Dialog.SetCheckbox("//input[contains(@id, '_MAPPINGTYPECODEID_1')]", true);  // Use file mapping template
                Dialog.SetDropDown("//input[contains(@id, '_IMPORTFILETEMPLATEID_value')]", "DEV-GAAEducationalImport (delimited file)"); // use a comma delimited file
                Dialog.OpenTab("Set options");
                Dialog.SetCheckbox("//input[contains(@id, '_BATCHCOMMITOPTION_2')]", true);  // Commit batches if they have no batch exceptions
                Dialog.Save();
                System.Threading.Thread.Sleep(4000); //pause because otherwise the code runs so rapidly that the next line is not invoked in time

                Panel.SelectSectionDatalistRow(involvementRow, "Import processes");  //find the correct row which has the correct general correspondence process

                Panel.WaitClick(Panel.getXSelectedDatalistRowButton("Start import"), 35);
                System.Threading.Thread.Sleep(4000);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not start import of educational involvement. " + ex.Message);
            }
        }

        [When(@"I decease a spouse")]
        public void WhenIDeceaseASpouse()
        {
            ConstituentPanel.SelectInfoTab();  // Go to the personal info tab
            string xPathdeceased = "//td[contains(@class,'x-toolbar-cell') and not (contains(@class,'x-hide-display'))]//table[contains(@id,'bbui-gen-tbaraction-')and not(contains(@class,'hide'))]/tbody/tr[2]/td[2]/em/button[./text() = 'Mark deceased']";
            ConstituentPanel.WaitClick(xPathdeceased);
            Dialog.SetTextField("//div[contains(@id, 'IndividualMarkDeceasedEditForm2')]//input[contains(@id, '_DECEASEDDATE_value')]", "10/15/2016");
            Dialog.Save();
        }

    }

}
