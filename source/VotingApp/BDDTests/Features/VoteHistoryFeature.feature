﻿Feature: VoteHistoryFeature

A short summary of the feature
Background:
	Given the following users exist
	  | UserName   | Email                 | FirstName | LastName | Password  |
	  | sam2     | sam2@mail.com    | sam    | 2    | Password123! |
@tag1
Scenario Outline: Logged in user can see Vote History button
	Given I am a user with first name '<FirstName>'
	When I login
#	Then I am redirected to '<Url>'
	Then I am redirected to the '<Page>' page
	  And I can see the Vote History button in the navbar
	Examples:
	| FirstName | Page |
	| sam   | Home |

Scenario Outline:Non Logged in user can not see Vote History button
	Given I am on the '<Page>' page
	Then I cannot see the Vote History Button in the navbar
	Examples:
	| Page |
	| Home |

Scenario Outline:Non Logged that navigate to the vote history page will be taken to loggin
	Given I am on the '<Page>' page
		And I navigate to the Vote History page
	Then I will be redirect to the login page
	Examples:
	| Page |
	| Home |

Scenario Outline:Logged in users that navigate to the vote history page will be taken to the vote history page
	Given I am a user with first name '<FirstName>'
		And I login
	Then I am redirected to the '<Page>' page
		And I navigate to the Vote History page
	Then I will be redirected to the Vote History Page
	Examples:
	| FirstName | Page |
	| sam   | Home |