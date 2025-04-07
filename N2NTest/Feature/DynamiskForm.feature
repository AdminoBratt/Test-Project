Feature: Contact Customer Service Form
As a customer
I want to fill out the appropriate contact form
So that I can get help with my specific issue

    Scenario: Submit Telecom service request
        Given I am on the customer service contact form page
        When I select "Tele/Bredband" as the company type
        And I fill in my personal information
        And I select "Bredband" as the service type
        And I select "Tekniskt problem" as the issue type
        And I enter a detailed message
        And I submit the form
        Then I should see a success message

    Scenario: Submit Car repair service request
        Given I am on the customer service contact form page
        When I select "Fordonsservice" as the company type
        And I fill in my personal information
        And I enter "ABC123" as the registration number
        And I select "Garantiärende" as the issue type
        And I enter a detailed message
        And I submit the form
        Then I should see a success message

    Scenario: Submit Insurance request
        Given I am on the customer service contact form page
        When I select "Försäkringsärenden" as the company type
        And I fill in my personal information
        And I select "Hemförsäkring" as the insurance type
        And I select "Fakturafrågor" as the issue type
        And I enter a detailed message
        And I submit the form
        Then I should see a success message

    Scenario: Verify form validation
        Given I am on the customer service contact form page
        When I select "Tele/Bredband" as the company type
        And I submit the form without filling required fields
        Then I should see validation errors