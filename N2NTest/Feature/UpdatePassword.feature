Feature: Update User Password
As a logged in user
I want to update my password
So that I can maintain my account security

    Scenario: Password mismatch error
        Given I am logged in to the application
        When I navigate to the update password page
        And I enter "NewUsername" as the new username
        And I enter "NewPassword123" as the new password
        And I enter "DifferentPassword" as the confirm password
        And I click the Update button
        Then I should see a password mismatch error
    
    Scenario: Successfully update user password
        Given I am logged in to the application
        When I navigate to the update password page
        And I enter "NewUsername" as the new username
        And I enter "NewPassword123" as the new password
        And I enter "NewPassword123" as the confirm password
        And I click the Update button
        Then I should see a password update success message