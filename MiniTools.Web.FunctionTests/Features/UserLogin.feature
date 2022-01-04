Feature: UserLogin

Web page to let user authenticate their credentials and log into web application.

@tag1
Scenario: Log in with valid credentials
	Given a set of valid credentials
	When user log in with credentials
	Then user will be logged in
