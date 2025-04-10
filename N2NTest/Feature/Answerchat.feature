Feature: Admin Chat Functionality
As an admin user
I want to access and respond to customer tickets
So that I can provide customer support

    Scenario: Admin can view tickets and send messages in chat
        Given I am logged in as an admin
        When I navigate to the admin dashboard
        And I switch to the tickets view
        And I open a chat for the first available ticket
        And I send a message "This is a test response from admin"
        Then I should see my message in the chat window