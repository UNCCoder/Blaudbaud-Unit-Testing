Feature: D-Events
	In order to validate Events functionality
	As a user of BBCRM
	I want to add, modify, and validate Events

@Events
Scenario: A-Add Event
	Given I have logged into the BBCRM home page
	When I add events
	| Location           | Start date | Name         |
	| Ackland Art Museum | 1/5/2015   | Event_469108 |
	Then an event exists with the name 'Event_469108', start date '1/5/2015'

@Events
Scenario: E-Add multiple Registration Options
	Given I have logged into the BBCRM home page
	And an event exists
    | Name          | Start date |
    | Event1_ABC |11/11/2015   |
	And an event exists
    | Name          | Start date |
    | Event2_473412 |11/11/2015   |
	When I add a registration option to event 'Event1_ABC'
    | Registration type | Name  |
    | Alumni             | Adult |
    | Child             | Child |
	And I add a registration option to event 'Event1_ABC'
	| Registration type | Name       |
	| Couple            | Couple     |
	| Non-Member        | Non-Member |
	Then event "Event1_ABC" has registration option
	| Registration type | Name  |
	| Alumni            | Adult |
	| Child             | Child |


@Events
Scenario: C-Add Coordinator 
	Given I have logged into the BBCRM home page
	And an event exists
	| Name         | Start date |
	| Event1_DEF |11/11/2015   |
	And staff constituent "StaffTest" exists
	When I add coordinator 'StaffTest' to event 'Event1_DEF'
	Then 'StaffTest' is a coordinator for event 'Event1_DEF'

@Events
Scenario: D-Add Registration options 
	Given I have logged into the BBCRM home page
	And an event exists
    | Name         | Start date |
    | Event_G |11/11/2015   |
	When I add a registration option to event 'Event_G'
	| Registration type | Name       |
	| Alumni            | Full Price |
	Then event "Event_G" has registration option
	| Registration type | Name       |
	| Alumni            | Full Price |

@Events
Scenario: F-Add registrant 
	Given I have logged into the BBCRM home page
	And constituent 'Anonymous Alumnus' exists
	And an event exists
	| Name         | Start date |
	| Student TEST Event |11/11/2015   |
	And event 'Student TEST Event' has registration option
	| Registration type | Name      | Registration count | Registration fee |
	| Alumni            | Alumni +1 | 2                  | $20.00           |
	When I add registrant 'Anonymous Alumnus' to event 'Student TEST Event'
	| Registration option | Registrant        |
	| Alumni +1           | Anonymous Alumnus |
	Then registrant record 'Anonymous Alumnus' is created for event 'Student TEST Event' with 1 guest

@Events
Scenario: G-Run Invitation
	Given I have logged into the BBCRM home page
	And constituent 'Test Abc' exists
	And an event exists
	| Name         | Start date |
	| Student ABC Event |11/11/2015   |
	And event 'Student ABC Event' has registration option
	| Registration type | Name      | Registration count | Registration fee |
	| Alumni            | Alumni +1 | 2                  | $20.00           |
	When I add registrant 'Test Abc' to event 'Student ABC Event'
	| Registration option | Registrant |
	| Alumni +1           | Test Abc   |  
	When I Create an Invitation 'Student ABC Event'
	And Run The invitation 'Student ABC Event' with Alumni 'Test Abc'

@Events
Scenario: H-Import Registrants
	Given I have logged into the BBCRM home page
	And search for event 'UDO FY17 - Other test event'
	And Start Import of Registrants
	| Name                              | Event Template                             |
	| ALL-Event Registrant Batch Import | ALL-Event Registrant Import Batch Template |
	And Edit Import Process 'RegistrantListStaging.csv'


@Events
Scenario: B-Add Task
	Given I have logged into the BBCRM home page
	And an event exists
	| Name         | Start date |
	| Event_XYZ |11/11/2015   |
	And constituent 'Constituent_XYZ' exists
	When I add a task to event 'Event_XYZ'
	| Name             | Comment             | Owner           | Date due   |
	| Send Invitations | Prepare invitations | Constituent_XYZ | 12/11/2017 |
	And add reminder to task 'Send Invitations' on event 'Event_XYZ'
	| Name       | Date       |
	| Reminder 1 | 11/11/2017 |
	Then reminder 'Reminder 1 (11/11/2017)' exists for task 'Send Invitations'