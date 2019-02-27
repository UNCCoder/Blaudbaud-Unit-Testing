Feature: B-Revenue
	In order to validate Revenue functionality
	As a user of BBCRM
	I want to add, modify, and validate Pledges and Batches

@Revenue
Scenario: C-Add pledge and payment
	Given I have logged into the BBCRM home page
	And constituent 'Paul Grandee' exists

	When I add friend constituency to 'Paul Grandee' 
	|Date from|Date to|  
	And Save the constituency
	Then a friend constituency is added to 'Paul Grandee' 
 	| Date from | Date to | Description |
 	|           |         | Friend      | 

	When I add a pledge
	| Constituent  | Amount  | Designations | Date       | Frequency | Starting on | No. installments | Post status |
	| Paul Grandee | $300.00 | 104327       | 11/11/2017 | Monthly   | 10/11/2018  | 3                | Not posted  |
	Then a pledge exists for constituent "Paul Grandee" with amount "$300.00"

	When I start to add a batch with template "DEV-Checks Only" and description "Batch for Paul Grandee" for a payment
	| Account system                   | Constituent  | Date       | Amount  | GL post status | Designation |
	| UNC PeopleSoft Accounting System | Paul Grandee | 11/11/2015 | $100.00 | Not posted     | 104327      |

@Revenue
Scenario: B-Add pledge
	Given I have logged into the BBCRM home page
	And constituent 'Paul Symons' exists

	When I add friend constituency to 'Paul Symons' 
	|Date from|Date to|  
	And Save the constituency
	Then a friend constituency is added to 'Paul Symons' 
 	| Date from | Date to | Description |
 	|           |         | Friend      | 

	When I add a pledge
	| Constituent | Amount  | Designations | Date     | Frequency | Starting on | No. installments  | Post status |
	| Paul Symons | $300.00 | 104327       |11/11/2015 | Monthly   | 1/1/2020    | 3                 | Not posted |
	Then a pledge exists for constituent "Paul Symons" with amount "$300.00"

@Revenue
Scenario: A-Add Pledge With Installments
	Given I have logged into the BBCRM home page
	And constituent 'Art Garfunkel' exists
	And designation exists '204367'
	And designation exists '252501'

	When I add friend constituency to 'Art Garfunkel'
	|Date from|Date to|  
	And Save the constituency
	Then a friend constituency is added to 'Art Garfunkel'
 	| Date from | Date to | Description |
 	|           |         | Friend      | 

	When I start to add a pledge
	| Constituent   | Amount  | Designations | Date       | Frequency | Starting on | No. installments | Post status |
	| Art Garfunkel | $300.00 | 104327       | 11/11/2017 | Monthly   | 10/11/2018  | 5                | Not posted  |
	And split the pledge designations evenly
	| Designation | Amount  |
	| 204367      | $300.00 |
	| 252501       | $0.00   |
	And save the pledge
	Then a pledge exists with designations
	| Designation | Amount  | Balance |
	| 204367      | $150.00 | $150.00 |
	| 252501       | $150.00 | $150.00 |

@Revenue
Scenario: D-Enter Revenue Batch
	Given I have logged into the BBCRM home page
	And constituent 'Cam Newton' exists

	When I add friend constituency to 'Cam Newton' 
	|Date from|Date to|  
	And Save the constituency
	Then a friend constituency is added to 'Cam Newton' 
 	| Date from | Date to | Description |
 	|           |         | Friend      | 

	When I start to add a batch with template "DEV-Checks Only" and description "Batch for Cam Newton" for a payment
	| Account system                   | Constituent | Date       | Amount  | GL post status | Designation |
	| UNC PeopleSoft Accounting System | Cam Newton  | 11/11/2015 | $150.00 | Not posted     | 104327      |
	| UNC PeopleSoft Accounting System | Cam Newton  | 11/11/2015 | $50.00  | Not posted     | 252501      |

	When I commit the batch
	| Batch template  | Description          |
	| DEV-Checks Only | Batch for Cam Newton |
	Then the batch commits without errors or exceptions and 2 record processed

@Revenue
Scenario: E-Add Matching Gift and Payment
	Given I have logged into the BBCRM home page
	And constituent 'John Tested' exists
	And Add a Matching Company 'Testmatchingcompany' as Employer

	When I add friend constituency to 'John Tested' 
	|Date from|Date to|  
	And Save the constituency
	Then a friend constituency is added to 'John Tested' 
 	| Date from | Date to | Description |
 	|           |         | Friend      | 

	And I start to add a payment
	| Constituent | Amount  | Date       |
	| John Tested | $100.00 | 11/11/2015 |
	And add applications to the payment
	| Application | Applied | Designation |
	| Donation    | $100.00 | 000001      |
	And save the the payment

	Then I add a matching gift for company 'Testmatchingcompany'
#	
	When I start to add a batch with template "DEV-Checks Only" and description "Matching Gift for John Tested" for a payment
	| Account system                   | Constituent         | Date       | GL post status | Designation | Amount  |
	| UNC PeopleSoft Accounting System | John Tested		 | 11/11/2017 | Not posted     | 000001      | $100.00 |

	When I commit the batch
	| Batch template  | Description                   |
	| DEV-Checks Only | Matching Gift for John Tested |
	Then the batch commits without errors or exceptions and 1 record processed

@Revenue
Scenario: F-Run receipt process
	Given I have logged into the BBCRM home page
	And constituent exists
	| Last name       | Address type | Country       | Address               | City       | State | ZIP   | Email type | Email address           |
	| Norman TESTHopewell | Other        | United States | 1990 Daniel Island Dr | Charleston | SC    | 29407 | Other      | testemail@blackbaud.com |

	When I add friend constituency to 'Norman TESTHopewell' 
	|Date from|Date to|  
	And Save the constituency
	Then a friend constituency is added to 'Norman TESTHopewell' 
 	| Date from | Date to | Description |
 	|           |         | Friend      | 

	And I start to add a payment
	| Constituent     | Amount  | Date       |
	| Norman TESTHopewell | $100.00 | 11/11/2015 |
	And add applications to the payment
	| Application | Applied | Designation |
	| Donation    | $100.00 | 00001     |
	And save the the payment
	When I create a receipt process
	| Name            | Output format        | Mark revenue 'Receipted' when process completes |
	| Receipt Process | Email receipt output | true                                            |
	And run receipt process
	| Name            | Output format        |
	| Receipt Process | Email receipt output |
