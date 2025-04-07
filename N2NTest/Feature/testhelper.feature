Feature: Staff Login
As a staff member
I want to be able to log in to the admin dashboard
So that I can access administrative features

    Scenario: Successful login to admin dashboard
        Given I am on the staff login page
        When I enter valid staff credentials
        And I click the login button
        Then I should be redirected to the admin dashboard