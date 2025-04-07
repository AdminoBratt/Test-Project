Feature: User Login
  As a user of the system
  I want to be able to log in with my credentials
  So that I can access features based on my role

  Scenario: Successful login as Staff user
    Given I am on the application homepage
    When I click on the login button
    And I enter "staff@mail.com" as email
    And I enter "staff" as password
    And I click the Login button
    Then I should be logged in successfully
    And I should see staff features for "forsakring" department
    And I should see my name "Staff" in the header

  Scenario: Successful login as Admin user
    Given I am on the application homepage
    When I click on the login button
    And I enter "maadridista@gmail.com" as email
    And I enter "abc123" as password
    And I click the Login button
    Then I should be logged in successfully
    And I should see admin dashboard in the nav-bar
    And I should see my name "KevinAdmin" in the header

  Scenario: Successful login as Super Admin user
    Given I am on the application homepage
    When I click on the login button
    And I enter "Lundstedtkevin@gmail.com" as email
    And I enter "abc123" as password
    And I click the Login button
    Then I should be logged in successfully
    And I should see admin dashboard in the nav-bar
    And I should see my name "KevinSuper" in the header

  Scenario: Failed login with non-existent user
    Given I am on the application homepage
    When I click on the login button
    And I enter "nonexistent@example.com" as email
    And I enter "wrongpassword" as password
    And I click the Login button
    Then I should see an error message
    And I should remain on the login modal

  Scenario: Failed login with incorrect password
    Given I am on the application homepage
    When I click on the login button
    And I enter "staff@mail.com" as email
    And I enter "wrongpassword" as password
    And I click the Login button
    Then I should see an error message
    And I should remain on the login modal

