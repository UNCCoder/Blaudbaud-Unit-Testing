Feature: A-Constituent

	In order to manage Constituent Records
	As a Blackbaud CRM user
	I want to add a constituent, and a spouse who have addresses and validate their addresses
	Then decease the spouse 
	Add a special code to a constituent


@Constituent
Scenario: B-Add Constituent and spouse
	Given I have logged into the BBCRM home page 
	When I add constituent "Testperson"
	| First name | Title   | Address           | City        | State | ZIP   | Gender | Address type      |
	| John       | Admiral | 400 W Franklin St | Chapel Hill | NC    | 27516 | Male   | Business Physical |
	Then constituent "Testperson" is created 

	When I add spouse "Testpersonspouse"
	| First name | Title | Address           | City        | State | ZIP   | Gender |  Address type      |
	| Mary       | Dean  | 400 W Franklin St | Chapel Hill | NC    | 27516 | Female |  Business Physical |
	And Add a Household
	| Related individual | Individual is the | Related individual is the | Start date |
	| Testperson         | Spouse/Partner    | Spouse/Partner            | 10/15/2015 |
	Then constituent "Testpersonspouse" is created 

	When I decease a spouse
	And I have opened the constituent search dialog
	And I search for "Testpersonspouse"
	And I remove a spouse
	Then household is dissolved

@Constituent
Scenario: C-Add Special Code
	Given I have logged into the BBCRM home page 
	When I add constituent "Testpersona"
	| First name | Title   | Address           | City        | State | ZIP   | Gender |  Address type     |
	| John       | Admiral | 400 W Franklin St | Chapel Hill | NC    | 27516 | Male   | Business Physical |
	Then constituent "Testpersona" is created 

	When I add a specialcode "017 - Caribbean-Jan 1977"
	Then the specialcode "017 - Caribbean-Jan 1977" has been created

	
@Constituent
 Scenario: A-Add Constituent 
	Given I have logged into the BBCRM home page 
	When I add constituent "Testing_Personc" 
 	Then constituent "Testing_Personc" is created

@Constituent
Scenario: D-Edit Constituent in batch
	Given I have logged into the BBCRM home page
	And constituent 'Gwendolyn A Rivert' exists

	When I add friend constituency to 'Gwendolyn A Rivert'
	|Date from|Date to|  
	And Save the constituency
	Then a friend constituency is added to 'Gwendolyn A Rivert'
 	| Date from | Date to | Description |
 	|           |         | Friend      | 

	When I start to add a batch with template "DEV-Checks Only" and description "Edit Gwendolyn R" for a payment
	| Account system                   | Constituent        | Date       | Amount  | GL post status | Designation |
	| UNC PeopleSoft Accounting System | Gwendolyn A Rivert | 11/11/2016 | $100.00 | Not posted     | 104327      |
	And Open the Batch
	| Batch template  | Description      |
	| DEV-Checks Only | Edit Gwendolyn R |

	And edit the selected constituent
	| Title | Birth date | State |
	| Admiral | 12/25/1990 | SC |
	And save the batch and commit it
	| Batch template  | Description      |
	| DEV-Checks Only | Edit Gwendolyn R |
	Then the batch commits without errors or exceptions and 1 record processed 

	
@Constituent
Scenario: E-Add Notification
	Given I have logged into the BBCRM home page
	And constituent 'Constituent_new' exists
	When I add a Note to 'Constituent_new'
	| Type   | Date     | Title     | Author             | Notes     |
	| Career |11/11/2015 | Test Note | Constituent_new | Test note |
	And add a notification to note 'Test Note'
	| Displays for |
	| All users    |
	Then the notification bar displays the Note 'Test Note'

@Constituent
Scenario: F-Search
	Given I have logged into the BBCRM home page
	And I have opened the constituent search dialog
	When I search for "Baltimore"
	Then The results should contain "Baltimore Carolina Club"

@Constituent
Scenario: I-Add Educational Involvements In Batch
	Given I have logged into the BBCRM home page
	# this will be for Kelli Testing's educational involvement
	And Start Import of Educational Involvement
	| Name                               | Batch template                           |
	| GAA Educational Involvement Import | UNC Educational Involvement Import Batch |
	And Edit Import Involvement Process 'EducationalInvolvementTestStaging.csv'
	| Name                               | Batch template                           |
	| GAA Educational Involvement Import | UNC Educational Involvement Import Batch |

@Constituent
Scenario: G-Constituent History
	Given I have logged into the BBCRM home page
	When I have opened the constituent search dialog
	#Routh
	And I seek LookupID '702375273'
	Then I open the 'History' tab

@Constituent
Scenario: H-Prospect Tile
	Given I have logged into the BBCRM home page
	When I have opened the constituent search dialog
	#Routh
	And I seek LookupID '702375273'
	Then I See if the Prospect Tile text 'Prospect Status' is Visible
