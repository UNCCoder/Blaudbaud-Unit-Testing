Feature: F-Analysis And Query
	In order to validate AdHoc Query functionality
	As a user of BBCRM
	I want to add, modify, and validate AdHoc Queries, Smart Queries and Do Other Anaysis

@Browser:Chrome
@AnalysisAndQuery
Scenario: B-Add Revenue Query
	Given I have logged into the BBCRM home page
	When I add ad-hoc query type 'Revenue'
	And filter by 'Revenue'
	And add filter field 'Amount'
	| FILTEROPERATOR | VALUE1 |
	| Greater than   | 30000  |
	And filter by 'Revenue\Constituent\Spouse'
	And add filter field 'Gender'
	| FILTEROPERATOR | COMBOVALUE |
	| Not equal to   | Male       |
	And add output fields
	| Path                | Field      |
	| Revenue             | Revenue ID |
	| Revenue\Constituent | Name       |
	And set save options
	| Name              | Description      | Suppress duplicate row | Create a selection? | Create a dynamic selection |
	| RevenueQuery      | some description | checked                | checked             | on                         |
	Then ad-hoc query 'RevenueQuery' is saved

@AnalysisAndQuery
Scenario: A-Add Constituent Query  
	Given I have logged into the BBCRM home page
	When I add ad-hoc query type 'Constituent'
	And filter by 'Constituent'
	And add filter field 'Name'
	| FILTEROPERATOR | VALUE1 |
	| Greater than   | Will  |
	And filter by 'Constituent'
	And add filter field 'Gender'
	| FILTEROPERATOR | COMBOVALUE |
	| Not equal to   | Male       |
	And add output fields
	| Path        | Field     |
	| Constituent | Lookup ID |
	| Constituent | Name      |
	And set save options
	| Name             | Description      | Suppress duplicate row | Create a selection? | Create a dynamic selection |
	| ConstituentQuery | some description | checked                | checked             | on                         |
	Then ad-hoc query 'ConstituentQuery' is saved

@AnalysisAndQuery
Scenario: C-Add Smart Query
	Given I have logged into the BBCRM home page
	When I add smart query type 'Constituent'
	Then smart query 'ALL FY14 Academic Donors Selection' is exported

@AnalysisAndQuery
Scenario: G-Verify Export Definition Editable
	Given I have logged into the BBCRM home page
	When I open a constituent export definition
	Then Verify it is editable

@AnalysisAndQuery		
Scenario: F-Download Large Export
	Given I have logged into the BBCRM home page
	When I add a Constituent Adhoc Query type
	And filter by 'Constituent'
	And filter by 'Constituent\Education (Primary)'
	And add filter field 'Class of'
	| FILTEROPERATOR | VALUE1 |
	| Equal to       | 2013   |
 	And set query save options 
	| Name                    | Description      | Suppress duplicate row | Create a selection? | Create a dynamic selection |
	| Automated Testing Query | some description | checked                | checked             | on                         |
	And Setup an Export Process
	And Edit the Export Process 'Automated Testing Query'
	And Start Process
	Then Download Export


@Reports
Scenario: E-Run UNC Grad School Event Planner
	Given I have logged into the BBCRM home page
	And Open the 'Analysis' Functional Area
	When I Go to the 'UNC Grad School Event Planner' Report
	And Choose '100 Main Street', 'Dallas', 'TX', '75226'
	Then I run the Report

@Reports
Scenario: D-Run UNC Event Bio Report
	Given I have logged into the BBCRM home page
	And Open the 'Constituent' Functional Area
	When I Go to the 'UNC Event Bio Report Task' Report
	And Choose a PID '704453786'
	Then I run the Report
