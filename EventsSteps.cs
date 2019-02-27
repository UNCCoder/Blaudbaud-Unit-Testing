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
    public class EventsSteps : BaseSteps
    {

        [Given(@"Location ""(.*)"" exists")]
        public void GivenLocationExists(string locationName)
        {
            try
            {
                //Open events functional area and then add a location
                locationName += uniqueStamp;   // the unique stamp is a series of numbers to keep names different from each other
                 BBCRMHomePage.OpenEventsFA();  // open event functional area
                EventsFunctionalArea.Locations();
                LocationsPanel.AddLocation(locationName);  //add a location

            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not find the specified event location. " + ex.Message);
            }
        }

        [When(@"I add events")]
        public void WhenIAddEvents(Table eventsToAdd)
        {
            try
            {
                BBCRMHomePage.OpenEventsFA();  // open event functional area
                Panel.CollapseSection("Event calendar", "CalendarViewForm");
                foreach (var eventToAdd in eventsToAdd.Rows)
                {
                    if (eventToAdd["Name"].Substring(0, 3) != "UDO") 
                    { 
                        eventToAdd["Name"] += uniqueStamp;   // the unique stamp is a series of numbers to keep names different from each other
                    }
                    EventsFunctionalArea.AddEvent(eventToAdd);  // add an event
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add an event. " + ex.Message);
            }
          
        }

        [When(@"I add coordinator '(.*)' to event '(.*)'")]
        public void WhenIAddCoordinatorToEvent(string constituentLastName, string eventName)
        {
            try
            {
                constituentLastName += uniqueStamp;   // the unique stamp is a series of numbers to keep names different from each other
                eventName += uniqueStamp;   // the unique stamp is a series of numbers to keep names different from each other
                BBCRMHomePage.OpenEventsFA();  // open event functional area
                EventsFunctionalArea.EventSearch(eventName);  // search for an event
                EventPanel.AddCoordinator(constituentLastName);  // add a coordinator
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a coordinator to an event. " + ex.Message);
            }
        }
        [Then(@"an event exists with the name '(.*)', start date '(.*)'")]
        public void ThenAnEventExistsWithTheNameStartDate(string eventName, string startDate)
        {
            try
            {
                if (eventName.Substring(0, 3) != "UDO")
                {
                    eventName += uniqueStamp;   // the unique stamp is a series of numbers to keep names different from each other
                }
                if (!EventPanel.IsEventName(eventName))
                    throw new ArgumentException("Current event does not have the name " + eventName);
                if (!EventPanel.IsStartDate(startDate))
                    throw new ArgumentException("Current event does not have the start date " + startDate);
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not find an event with a particular name and start date. " + ex.Message);
            }
        }

        [Given(@"an event exists")]
        public void GivenAnEventExists(Table events)
        {
            try
            {
                foreach (var e in events.Rows)
                {
                    e["Name"] += uniqueStamp;   // the unique stamp is a series of numbers to keep names different from each other
                    BBCRMHomePage.OpenEventsFA();  // open event functional area
                    EventsFunctionalArea.AddEvent(e);  // Add an event
                 }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not find that a certain event exists. " + ex.Message);
            }

        }

        [When(@"I add a registration option to event '(.*)'")]
        public void WhenIAddARegistrationOptionToEvent(string eventName, Table registrationOptions)
        {
            try
            {
                eventName += uniqueStamp;   // the unique stamp is a series of numbers to keep names different from each other
                GetEventPanel(eventName);  // open event panel
                foreach (var option in registrationOptions.Rows)
                {
                    EventPanel.AddRegistrationOption(option);  //add a registration option
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a registration option. " + ex.Message);
            }
        }

        [Then(@"event ""(.*)"" has registration option")]
        public void ThenEventHasRegistrationOption(string eventName, Table registrationOptions)
        {
            eventName += uniqueStamp;   // the unique stamp is a series of numbers to keep names different from each other
            GetEventPanel(eventName);  // open event panel  
            foreach (var option in registrationOptions.Rows)
            {
                if (!EventPanel.RegistrationOptionExists(option))
                    throw new ArgumentException("'" + option +
                                                "' was not found on the Registration options datalist for event " +
                                                eventName + ".");
            }
        }

        [Given(@"event '(.*)' has registration option")]
        public void GivenEventHasRegistrationOption(string eventName, Table registrationOptions)
        {
            try
            {
                WhenIAddARegistrationOptionToEvent(eventName, registrationOptions);  // add registration option
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not find the given registration option. " + ex.Message);
            }
        }


        [When(@"I add registrant '(.*)' to event '(.*)'")]
        public void WhenIAddRegistrantToEvent(string registrant, string eventName, Table registrations)
        {
            try
            {
                registrant += uniqueStamp;   // the unique stamp is a series of numbers to keep names different from each other
                eventName += uniqueStamp;   // the unique stamp is a series of numbers to keep names different from each other
                BBCRMHomePage.OpenEventsFA();  // open event functional area
                Panel.CollapseSection("Event calendar", "CalendarViewForm");
                EventsFunctionalArea.EventSearch(eventName);  // search for an event
                EventPanel.AddRegistrant();
                RegistrantDialog.SetRegistrant(registrant);

                foreach (var registrantRow in registrations.Rows)  // find all the registrants in the table
                {
                    if (registrantRow.ContainsKey("Registrant") && !string.IsNullOrEmpty(registrantRow["Registrant"])
                        && registrantRow["Registrant"] != "(Unnamed guest)") registrantRow["Registrant"] += uniqueStamp;   // the unique stamp is a series of numbers to keep names different from each other
                }

                RegistrantDialog.SetRegistrants(registrations);
                Dialog.Save();
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not add a registrant to an event. " + ex.Message);
            }
        }

        [Then(@"registrant record '(.*)' is created for event '(.*)' with (.*) guest")]
        public void ThenRegistrantRecordIsCreatedForEventWithGuest(string registrant, string eventName, int numGuests)
        {
            registrant += uniqueStamp;   // the unique stamp is a series of numbers to keep names different from each other
            eventName += uniqueStamp;   // the unique stamp is a series of numbers to keep names different from each other
            if (!RegistrantPanel.IsRegistrant(registrant))
                throw new ArgumentException(String.Format("'{0}' is not the name of the registrant", registrant));
            if (!RegistrantPanel.IsEvent(eventName))
                throw new ArgumentException(String.Format("'{0}' is not the event for the registrant", eventName));
            if (!RegistrantPanel.IsNumGuests(numGuests))
                throw new ArgumentException(String.Format("'{0}' is not the number of guests for the registrant",
                    numGuests));
        }
        [When(@"I Create an Invitation '(.*)'")]
        public void WhenICreateAnInvitation(string eventName)
        {
            string invitationName;
            string xTab;

            eventName += uniqueStamp;
            BBCRMHomePage.OpenEventsFA();

            EventsFunctionalArea.EventSearch(eventName);  //search for an event
            invitationName = "Inv" + eventName;
            string caption = "General";
            EventPanel.OpenTab("Invitations");  //open the invitations tab
            EventPanel.ClickSectionAddButton("Invitations"); // add an invitation
            xTab = getXInnerTab(caption);
            EventPanel.getXTab(xTab);  

            string xEmailString;
            xEmailString = Dialog.getXInput("InvitationAddForm", "_NAME_value");
            xTab =  getXInnerTab(caption);
            Dialog.WaitClick(xTab, 10); //open the general subtab in the popup
            Dialog.SetTextField(xEmailString, invitationName);  // enter the invitation name
            Dialog.SetTextField("//input[contains(@id, '_MAILDATE_value')]", DateTime.Today.ToShortDateString()); //add in the date
            xEmailString = Dialog.getXInput("PackageSearch", "_NAME_value");

            caption = "Processing Options";
            xTab = getXInnerTab(caption);
            Dialog.WaitClick(xTab, 10);
            Dialog.SetCheckbox("//input[contains(@id, '_CHANNELCODE_1')]", true);  //set the "send through email" radio button to true
            // enter email package name
            Dialog.SetSearchList(Dialog.getXInput("InvitationAddForm", "_EMAILPACKAGEID_value"), xEmailString, "UDO FY16 Carolina Connection Email Package");
            Dialog.Save();
        }

        public static string getXInnerTab(string caption)
        {
            return String.Format("//div[contains(@id,'InvitationAddForm')]//ul[contains(@class,'x-tab-strip')]//span[text()='{0}']", caption);
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
        [When(@"Run The invitation '(.*)' with Alumni '(.*)'")]
        public void WhenRunTheInvitationWithAlumni(string eventName, string alumniName){
        
            try {
            eventName += uniqueStamp;
            alumniName += uniqueStamp;
            EventPanel.WaitClick(EventPanel.getXDatalistRowLinkByActionAndCaption("", eventName), 10); //click on the invitation name under the Invitation tab
            EventPanel.ClickSectionAddButton("Invitees"); // select the invitee add button
            EventPanel.WaitClick(getXMenuItem("Constituent"), 10); // select to add a constituent
            Dialog.SetSearchList(Dialog.getXInput("dataformdialog", "_CONSTITUENTID_value"), Dialog.getXInput("ConstituentSearch", "_KEYNAME_value"), alumniName); // add the constituent in the search box
            Dialog.Save();
            string invitationSend;
            invitationSend = "Send Inv" + eventName + " to " + eventName + " invitees";
            string xWaitText = "//div[text()='" + invitationSend + "']";
            EventPanel.WaitClick(xWaitText, 10); // send the invitation
            Dialog.ClickButton("Start", 50); // press the start button
            
            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not run the invitation. " + ex.Message);
            }
        }
        protected string getXMenuItem(string caption = "")
        {
            return String.Format("//div[contains(@class,'x-menu') and contains(@style,'visibility: visible')]//span[./text()='{0}' and @class='x-menu-item-text']", caption);
        }
        /// <param name="caption">The caption of the button.</param> 
        public string getXButtonEdit(string caption = "")
        {
            //determine the xml to select the edit button
            string FormattedString = "";
            FormattedString = String.Format("//*[./text()=\"{0}\" and contains(@class,\"x-btn-text\")]", caption);
            return FormattedString;
        }

        [Given(@"Start Import of Registrants")]
        public void GivenStartImportOfRegistrants(Table registrants)
        {
            try
            {
                string getXSearchInput = "//div[contains(@class,'bbui-pages-contentcontainer') and not(contains(@class,'hide'))]//input[@placeholder='Search']";
                BBCRMHomePage.OpenFunctionalArea("Administration"); //open the admin functional area
                Dialog.WaitClick("//div[contains(h3,'Tools')]//button[contains(@class,'linkbutton')]/div[text()='Import']", 15);

                var registrant = registrants.Rows[0];
                if (registrant.ContainsKey("Name") && !String.IsNullOrEmpty(registrant["Name"]))
                {
                    Dialog.SetTextField(getXSearchInput, registrant["Name"]);
                    Dialog.GetDisplayedElement(getXSearchInput).SendKeys(Keys.Tab);
                }
                System.Threading.Thread.Sleep(4000); //pause because otherwise the code runs so rapidly that the next line is not invoked in time
                string xSelectedRow = "//div[contains(@id,'bbui-gen-pagecontainer')]//div[contains(@id, '')]//div[@class='x-grid3-body']/div[1]//td[2]";
                xSelectedRow = "//div[contains(@id,'ext-gen')]/div/table/tbody/tr[1]/td[3]/div/a";
               // Dialog.WaitClick(Dialog.getXGridRowSelector("bbui-gen-pagecontainer", "", 1), 6);  //click on the selected row
                Dialog.WaitClick(xSelectedRow, 15);

            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not start the import of registrants. " + ex.Message);
            }

        }

        [Given(@"Edit Import Process '(.*)'")]
        public void GivenEditImportProcess(string fileName)
        {
            try
            {
                Dialog.WaitClick("//button[contains(@class,'bbui-linkbutton')]/div[text()='Edit process']", 15);  // click on the Edit Process link
                Dialog.OpenTab("Configure import file"); // open the configure import file tab
                Dialog.SetDropDown("//input[contains(@id, '_IMPORTSOURCEID_value')]", "Default network directory"); // use default network directory
                Dialog.SetDropDown("//input[contains(@id, '_FILE_value')]", fileName); // use the selected file
                Dialog.OpenTab("Map fields");
                Dialog.SetCheckbox("//input[contains(@id, '_MAPPINGTYPECODEID_0')]", true);  // Use manual mapping
                Dialog.SetDropDown("//input[contains(@id, '_DELIMITEROPTION_value')]", "Comma"); // use a comma delimiter
                Dialog.ClickButton("Auto-map");
                Dialog.OpenTab("Set options");
                Dialog.SetCheckbox("//input[contains(@id, '_BATCHCOMMITOPTION_2')]", true);  // Commit batches if they have no batch exceptions
                Dialog.Save();
                Dialog.WaitClick("//button[contains(@class,'bbui-linkbutton')]/div[text()='Start process']", 15); // start the importing
                System.Threading.Thread.Sleep(4000); 
            }

            catch (Exception ex)
            {
                throw new Exception("Error: could not edit the import process. " + ex.Message);
            }
        }
        [Given(@"search for event '(.*)'")]
        public void GivenSearchForEvent(string eventName)
        {
            try
            {
                BBCRMHomePage.OpenEventsFA();  // open event functional area
                EventsFunctionalArea.EventSearch(eventName);  // search for an event
                EventPanel.OpenTab("Registrations");
                String xRegistrantName = "//input[contains(@id,'_CONSTITUENTNAME_value')]";
                Panel.SetTextField(xRegistrantName, "Crispin"); // search for the Kelli Crispin registrant with the "Crispin" char string
                Panel.GetDisplayedElement(xRegistrantName).SendKeys(Keys.Enter);
                string xSelectedRow;
                xSelectedRow = "//div[contains(@id,'ext-gen')]/div/table/tbody/tr[1]/td[4]/div/a";
                Dialog.WaitClick(xSelectedRow, 15);

                //click on the link to remove a registrant 
                Dialog.WaitClick("//button[contains(@class,'linkbutton')]/div[text()='Delete registrant']", 10);
                                    //click on the link to remove a spouse 
               
                Dialog.Yes();

            }
            catch (Exception ex)
            {
                throw new Exception("Error: could not search for the event. " + ex.Message);
            }

        }
        [When(@"I add a task to event '(.*)'")]
        public void WhenIAddATaskToEvent(string eventName, Table tasks)
        {
            eventName += uniqueStamp;
            GetEventPanel(eventName);
            foreach (var task in tasks.Rows)
            {
                if (task.Keys.Contains("Owner") && task["Owner"] != string.Empty)
                    task["Owner"] = task["Owner"] + uniqueStamp;
                EventPanel.AddTaskDialog();
                TaskDialog.SetFields(task);
                Dialog.Save();
            }
        }

        [When(@"add reminder to task '(.*)' on event '(.*)'")]
        public void WhenAddReminderToTaskOnEvent(string taskName, string eventName, Table reminders)
        {
            eventName += uniqueStamp;
            GetEventPanel(eventName);
            EventPanel.EditTask(taskName);
            TaskDialog.SetReminders(reminders);
            Dialog.Save();
        }

        [Then(@"reminder '(.*)' exists for task '(.*)'")]
        public void ThenReminderExistsForTask(string reminder, string task)
        {
            if (!EventPanel.ReminderExists(task, reminder))
                throw new ArgumentException(String.Format("Reminder '{0}' does not exist for task '{1}'", reminder, task));
        }
    }
}

