Feature: E-Prospect
	In order to validate Propect functionality
	As a user of BBCRM
	I want to add, modify, and validate Prospects 

@Prospect
Scenario: A-Add Prospect
	Given I have logged into the BBCRM home page
	And constituent 'Constituent_473330' exists
	When I add prospect constituency to 'Constituent_473330'
	|Date from|Date to|
	Then a prospect constituency is added to 'Constituent_473330'
	| Date from | Date to | Description |
	|           |         | Prospect    |

@Prospect
Scenario: B-Add Fundraiser
	Given I have logged into the BBCRM home page
	And prospect 'Prospect_ABC' exists
	And fundraiser 'Fundraiser_ABC' exists
	When I add team member to 'Prospect_ABC'
	| Team member    | Role         |
	| Fundraiser_ABC | Collaborator |
	And add Prospect Manager 'Fundraiser_ABC' 

@Prospect
Scenario: D-Add Major Giving Plan Outline
	Given I have logged into the BBCRM home page
	When I add plan outline "MajorGivingPlan_P" to major giving setup
	| Objective             | Fundraiser role  | Stage                | Days from start | Contact method |
	| Clearance to Approach | Prospect manager | Qualification        | 7               |                |
	| Prepare Ask           | Primary manager  | Cultivation          | 20              |                |
	| Explore Inclination   | Primary manager  | Cultivation          | 60              |                |
	| Make Ask              | Primary manager  | Proposal Development | 90              |                |
	Then the plan outline "MajorGivingPlan_P" is created with "4" steps

@Prospect
Scenario: C-Add Plan And Opportunity
	Given I have logged into the BBCRM home page
	And prospect 'Prospect_G' exists
	And fundraiser 'Fundraiser_G' exists
	When I Add a Plan to a Prospect 'Prospect_G' With Fundraiser 'Fundraiser_G' and Plan 'Plan_G'
	When I add plan outline "Commitment_G" to prospect
	| Objective             | Owner        | Stage                | Status    | Contact method | Expected date |
	| Clearance to Approach | Fundraiser_G | Qualification        | Pending   | Electronic     | 10/17/2016    |
	| Prepare Ask           | Fundraiser_G | Cultivation          | Pending   | Mail           | 10/18/2016    |
	| Explore Inclination   | Fundraiser_G | Cultivation          | Pending   | Mail           | 10/19/2016    |
	| Make Ask              | Fundraiser_G | Proposal Development | Planned   | Task           | 10/20/2016    |
	And Add an Opportunity to a Prospect 'Prospect_G' With Fundraiser 'Fundraiser_G' for Plan 'Plan_G'
	| Designation | Funding Method | Amount  |
	| 000001      | Gift           | $300.00 |      
	Then the Plan and Opportunity Is Added

@Prospect
Scenario: I-Plan Steps Future Date Error
	Given I have logged into the BBCRM home page
	And prospect 'Prospect_A' exists
	And fundraiser 'Fundraiser_A' exists
	When I Add a Plan to a Prospect 'Prospect_A' With Fundraiser 'Fundraiser_A' and Plan 'Plan_A'
	And I add plan outline "Commitment_A" to prospect
	| Objective             | Expected date | Owner        | Stage         | Status    | Contact method | Actual date |
	| Clearance to Approach | 10/16/2016    | Fundraiser_A | Qualification | Pending   | Electronic     |             |
	| Prepare Ask           | 10/18/2016    | Fundraiser_A | Cultivation   | Completed | Mail           | 12/18/2025  |
	Then the Plan 'Plan_A' is added with plan type 'Commitment' for 'future'
	| Plan name | Start date |
	| 'Plan_A'  |            |

@Prospect
Scenario: H-Plan Interactions Future Date Error
	Given I have logged into the BBCRM home page
	And prospect 'Prospect_B' exists
	And fundraiser 'Fundraiser_B' exists
	When I Add a Plan to a Prospect 'Prospect_B' With Fundraiser 'Fundraiser_B' and Plan 'Plan_B'
	And I add plan outline "Commitment_B" to prospect
	| Objective             | Expected date | Owner        | Stage         | Status    | Contact method | Actual date |
	| Clearance to Approach | 10/16/2015    | Fundraiser_B | Qualification | Pending   | Electronic     |             |
	| Prepare Ask           | 10/18/2015    | Fundraiser_B | Cultivation   | Completed | Mail           | 11/16/2015  |
	And Add Contact Report 'Plan_B'
    | Status  | Owner        | Objective             |
    | Pending | Fundraiser_B | Clearance to Approach |
	Then the Plan Interactions have been saved

@Prospect
Scenario: E-Add Prospect Research Request
	Given I have logged into the BBCRM home page
	And prospect 'Prospect_C' exists
	When I add a Prospect Request 'Prospect_C'
	Then a Prospect Request is Added for 'Prospect_C'


@Prospect
Scenario: G-Cancelled Status Error
	Given I have logged into the BBCRM home page
	And prospect 'Prospect_D' exists
	And fundraiser 'Fundraiser_D' exists
	When I Add a Plan to a Prospect 'Prospect_D' With Fundraiser 'Fundraiser_D' and Plan 'Plan_D'
	And I add plan outline "Commitment_D" to prospect
	| Objective             | Expected date | Owner        | Stage         | Status    | Contact method |
	| Clearance to Approach | 10/16/2015    | Fundraiser_D | Qualification | Pending   | Electronic     |
	| Prepare Ask           | 10/18/2015    | Fundraiser_D | Cultivation   | Cancelled | Mail           |
	Then the Plan 'Plan_D' is added with plan type 'Commitment' for 'cancelled'
	| Plan name | Start date |
	| 'Plan_D'  |            |

@Prospect
Scenario: F-Add Planned Gift
	Given I have logged into the BBCRM home page
	And prospect 'Prospect_E' exists
	And fundraiser 'Fundraiser_E' exists
	When I Add a Plan to a Planned-Gift Prospect 'Prospect_E' With Fundraiser 'Fundraiser_E' and Plan 'Plan_E'
	When I add plan outline "PlannedGift_E" to prospect
	| Objective             | Owner        | Stage                | Status    | Contact method | Expected date |
	| Clearance to Approach | Fundraiser_E | Qualification        | Pending   | Electronic     | 10/17/2016    |
	| Prepare Ask           | Fundraiser_E | Cultivation          | Pending   | Mail           | 10/18/2016    |
	And Add an Opportunity to a Prospect 'Prospect_E' With Fundraiser 'Fundraiser_E' for Plan 'Plan_E'
	| Designation | Funding Method |
	| 000001      | Planned Gift   |  
	And Add a Planned Gift
	| Designation | Amount  |
	| 000001      | $450.00 | 
	Then the Planned Gift Is Added

@Prospect
Scenario: J-AddInteractioninBatch
	Given I have logged into the BBCRM home page
	And Start Import of Interactions
	| Name                         | Event Template    |
	| ALL-Interaction Batch Import | Interaction Batch |
	And Edit Import Interaction Process 'Med FY16 Endowment Stewardship Interactions (2-21-17).csv'