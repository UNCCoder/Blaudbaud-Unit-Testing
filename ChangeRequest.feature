Feature: H-ChangeRequest
	In order to test our new change request system
	As a Blackbaud CRM user
	I want to add and edit various types of change requests

	
@ChangeRequest
Scenario: A-Change Request Add
	Given I have logged into the BBCRM home page
	And I start an add change request for constituent
	When I input data into the change request for constituent 'Sharontest'
	| First name | Last name  | Title   | Gender | Address           | City        | State | ZIP   | Address type      | Reason for request |
	| Jill       | Sharontest | Admiral | Female | 400 W Franklin St | Chapel Hill | NC    | 27516 | Business Physical | My reason          |

@ChangeRequest
Scenario: B-Change Request Edit Indiv
	Given I have logged into the BBCRM home page	
	When I add constituent "Testings"
	| First name | Title   | Address           | City        | State | ZIP   | Gender | Address type      | 
	| John       | Admiral | 400 W Franklin St | Chapel Hill | NC    | 27516 | Male   | Business Physical |

	Then constituent "Testings" is created 

	Given An Edit Personal Info Change
	| Middle name | Nickname | 
	| Samuel      | Jack     |

	Then the Personal Change is Requested

	When I go to the UNC Change Management Page
	| Name     | Requested By   |
	| Testings | ad\bbtest1.gst |

	And I have opened the constituent search dialog
	And I search for "Testings" 

	#William is the new middle name
	Then constituent 'William' exists   


@ChangeRequest
Scenario: C-Change Request Edit Org
	Given I have logged into the BBCRM home page	
	And I add an organization
	| Name                 | Industry   |
	| TestOrgChangeRequest | Foundation |
	Then organization is created 

	Given An Edit Organization Info Change
	| No. of employees | No. of subsidiaries |
	| 250              | 2                   |
	 
	Then the Organization Change is Requested

	When I go to the UNC Change Management Page
	| Name                 | Requested By   |
	| TestOrgChangeRequest | ad\bbtest1.gst |

	And I have opened the constituent search dialog
	And I search for "TestOrgChangeRequest" 
	
	Then Organization Change Request is Completed for 'TestOrgChangeRequest'

@ChangeRequest
Scenario: D-Change Request New Address
	Given I have logged into the BBCRM home page	
	When I add constituent "Testpersonnewaddress"
	| First name | Title      | Address               | City        | State | ZIP   | Gender | Address type |
	| Jane       | Ambassador | 800 W Franklin Street | Chapel Hill | NC    | 27516 | Female | Home Mailing |

	Then constituent "Testpersonnewaddress" is created 

	Given a new Address Moved Change Request
	| Address Type      | State | City      |
	| Business Physical | NC    | Charlotte |

	Then the new address is requested

@ChangeRequest
Scenario: E-Change Request new phone and email
	Given I have logged into the BBCRM home page	
	When I add constituent "Testpersonnewphone"
	| First name | Title   | Address           | City        | State | ZIP   | Gender | Address type  |
	| Judy       | Captain | 600 W Franklin St | Chapel Hill | NC    | 27516 | Female | Home Physical |

	Then constituent "Testpersonnewphone" is created 


	Given a new Phone request
	| Phone Type | Number |
	| Mobile     | 222-2222 |


	And a new Email Request
	| Type     | Email Address |
	| Business | judyT@abc.org |

	Then the new phone and email info is requested.

#Scenario: F-Change Request Delete Existing Phone and Email
#	Given I have logged into the BBCRM home page	
#	When I add constituent "Testpersondeletephone"
#	| First name | Title      | Address               | City        | State | ZIP   | Gender | Address type | Phone type | Phone number | Email type | Email address             |
#	| Karen      | Ambassador | 800 W Franklin Street | Chapel Hill | NC    | 27516 | Female | Home Mailing | Home       | 222-2222     | Business   | testpersondelete@abc.com |
#
#	Then constituent "Testpersondeletephone" is created 
#
#	Given a delete Phone Request
#	| Delete? |
#	| y       |
#
#	And a delete Email Request
#	| Delete? |
#	| y       |
#
#	Then the delete phone request is saved

@ChangeRequest
Scenario: G-Change Request Edit Address
	Given I have logged into the BBCRM home page	
	When I add constituent "Testpersoneditaddress"
	| First name | Title | Address               | City        | State | ZIP   | Gender | Address type     |
	| Mike       | Dean  | 800 W Franklin Street | Chapel Hill | NC    | 27516 | Male   | Business Mailing |

	Then constituent "Testpersoneditaddress" is created 

	Given Edit Address Change Request
	| Address Type | State | City      |
	| Home Mailing | NC    | Charlotte |

	Then the edit address change is requested

	When I go to the UNC Change Management Page for editing
	| Name                  | Requested By   |
	| Testpersoneditaddress | ad\bbtest1.gst |

	And Save Edit Address
	| Type | Address Type | Primary? | City      |
	| Edit | Home Mailing | Yes      | Charlotte |
	
	Given I have opened the constituent search dialog
	When I search for "Testpersoneditaddress"
	Then Verify that Edit Address Change was Saved for 'Testpersoneditaddress'

@ChangeRequest
Scenario: H-Change Request existing phone and email
	Given I have logged into the BBCRM home page	
	When I add constituent "Testpersoneditphone"
	| First name | Title   | Address               | City        | State | ZIP   | Gender | Address type     | Phone type | Phone number | Email type | Email address            |
	| William    | Captain | 210 W Franklin Street | Chapel Hill | NC    | 27516 | Male   | Business Mailing | Mobile       | 444-4444     | Business   | testpersonedit@abcde.com |

	Then constituent "Testpersoneditphone" is created 

	Given Edit Phone Change Request
	| Phone Type | Number   |
	| Home       | 333-3333 |

	And Edit Email Change Request
	| Type     | Email Address |
	| Personal | JohnT@abc.org |

	Then the edit phone change is requested

