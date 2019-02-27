Feature: 1_Preliminary
	In order to have certain items created before other tests
	As a user of BBCRM
	I will need to do these first

@Preliminary
Scenario: A-Create Test Event
	Given I have logged into the BBCRM home page	

	When I search for event 'UDO FY17 - Other test event'
	| Location           | Start date | Name                        |
	| Ackland Art Museum | 11/15/2016 | UDO FY17 - Other test event |
	And I add registration options to event 'UDO FY17 - Other test event '
    | Registration type | Name       |
    | Individual        | Individual | 

	Then event "UDO FY17 - Other test event" has registration options
	| Registration type | Name       |
	| Individual        | Individual |

@Preliminary
Scenario: B-Create Organization
	Given I have logged into the BBCRM home page
	And I have opened the constituent search dialog
	When I search for 'Testmatchingcompany' Organization
	Then organization 'Testmatchingcompany' exists

#@Preliminary
#Scenario: C-Create Testing Constituent
#	Given I have logged into the BBCRM home page
#	When I add test constituent "Kelli" Lastname "Testing"
#	| First name | Title   | Address type      | Address           | City        | State | ZIP   | Gender |
#	| Kelli      | Admiral | Business Physical | 400 W Franklin St | Chapel Hill | NC    | 27516 | Female |
#	Then constituent "Testing" is created 

@Preliminary
Scenario: C-Verify that Marketing Selection Exists
	Given I have logged into the BBCRM home page
	When I search for a marketing selection 'ACK FY15 Spring Annual Giving Additions Selection' 
	Then the Marketing selection 'ACK FY15 Spring Annual Giving Additions Selection' exists

@Preliminary
Scenario: D-Create Exclusion Selection
	Given I have logged into the BBCRM home page
	When I search for a exclusion selection 'ACK FY15 Spring Annual Giving Exclusions' 
	When I add selection ad-hoc query type 'Constituent'
	And filter by 'Constituent'
	And add filter field 'Lookup ID'
	| FILTEROPERATOR | VALUE1    |
	| Equal to       | 703880143 |
	And set save options for Exclusion
	| Name                                     | Description                              | Suppress duplicate row | 
	| ACK FY15 Spring Annual Giving Exclusions | ACK FY15 Spring Annual Giving Exclusions | checked                | 
	Then the Marketing selection 'ACK FY15 Spring Annual Giving Exclusions' exists

@Preliminary
Scenario: : E-Import Mapping Template for Educational Involvement
	Given I have logged into the BBCRM home page
	And Import Mapping File 'EducationalInvolvementTestStaging.csv', 'DEV-GAAEducationalImport'
	| Name                     | Batch template                           |
	| DEV-GAAEducationalImport | UNC Educational Involvement Import Batch |
	Then the mappingfile 'DEV-GAAEducationalImport' exists

@Preliminary
Scenario: : F-Import Mapping Template for Interactions
	Given I have logged into the BBCRM home page
	And Import Mapping File 'Med FY16 Endowment Stewardship Interactions (2-21-17).csv', 'CAS-InteractionImport'
	| Name                  | Batch template    |
	| CAS-InteractionImport | Interaction Batch |
	Then the mappingfile 'CAS-InteractionImport' exists

