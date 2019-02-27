Feature: C-Marketing
	In order to validate Direct Marketing functionality
	As a user of BBCRM
	I want to add, modify, and validate Marketing processes

@Marketing
Scenario: A-Acknowledgement Process
	Given I have logged into the BBCRM home page	
	And Setup An Acknowledgement 'CAS FYXX 101501 Gifts to Acknowledge Assign Letters Process'
	And Run the Process 
	Then Export the Acknowledgement 'CAS FYXX 101501 Gifts to Acknowledge Process'

@Marketing`
Scenario: B-General Correspondence
	Given I have logged into the BBCRM home page	
	And Setup A General Correspondence 'UDO FY17 General Correspondence'
	And Run the Correspondence Process 'UDO FY17 General Correspondence'
	Then Export the Correspondence 
	
	When I have opened the constituent search dialog

	And Check for Correspondence Comment for LookupID '702615731' 
	| Communication          | Details                    |
	| General Correspondence | UDO-General Correspondence |

	Given Edit a General Correspondence Description 'UDO FY17 General Correspondence'
	| Name                            | Description |
	| UDO FY17 General Correspondence | Test Desc   |
	And Change Query and Exclusions
	And Run the Correspondence Process 'UDO FY17 General Correspondence'
	Then Export the Correspondence 
		
#Calculate, add a new exclusion selection and recalculate a marketing effort
@Marketing
Scenario: C-Create Export Recalculate Marketing Effort
	Given I have logged into the BBCRM home page
	And Add A Constituent Segment 'ACK FY15 TEST Segment'
	When I Create a Marketing Effort 'ACK FY15 TEST Marketing Effort'
	And Add Segment 'ACK FY15 TEST Segment' and Package 'CAF FY16 CYE Dec. 30 FY15 Donor Email Package' and List Segment 'ACK FY15 Art Lenders List Segment' To Effort
	| Format      | Value |
	| ACK         |       |
	| Acquisition |       |
	| FY          | 17    |
	And Calculate Segment Counts 'ACK FY15 TEST Marketing Effort'
	And An Exclusion 'ACK FY15 Spring Annual Giving Exclusions' is Added So Recalculate
	And Add Export definition '*****ALL FYXX BROKEN Default Email Marketing Effort Export Definition****' for 'ACK FY15 TEST Marketing Effort'
	Then the Marketing Effort 'ACK FY15 TEST Marketing Effort' has been recalculated
	