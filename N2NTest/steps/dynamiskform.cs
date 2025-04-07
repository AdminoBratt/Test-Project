using Microsoft.Playwright;
using TechTalk.SpecFlow;
using Xunit;

namespace E2ETesting.Steps;

[Binding]
public class ContactFormSteps
{
    // SETUP:
    private IPlaywright _playwright;
    private IBrowser _browser;
    private IBrowserContext _context;
    private IPage _page;
    private string _baseUrl = "http://localhost:3001";

    [BeforeScenario]
    public async Task Setup()
    {
        _playwright = await Playwright.CreateAsync();
        _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = true, SlowMo = 0 });
        _context = await _browser.NewContextAsync();
        _page = await _context.NewPageAsync();
    }

    [AfterScenario]
    public async Task TearDown()
    {
        await _context.CloseAsync();
        await _browser.CloseAsync();
        _playwright.Dispose();
    }

    // STEPS:
    [Given(@"I am on the customer service contact form page")]
    public async Task GivenIAmOnTheCustomerServiceContactFormPage()
    {
        await _page.GotoAsync($"{_baseUrl}");
        await _page.WaitForSelectorAsync(".dynamisk-form-container");
    }

    [When(@"I select ""(.*)"" as the company type")]
    public async Task WhenISelectAsTheCompanyType(string companyType)
    {
        await _page.SelectOptionAsync("select[name='companyType']", companyType);
        // Allow time for dynamic fields to load
        await _page.WaitForTimeoutAsync(200);
    }

    [When(@"I fill in my personal information")]
    public async Task WhenIFillInMyPersonalInformation()
    {
        await _page.FillAsync("input[name='firstName']", "Test Person");
        await _page.FillAsync("input[name='email']", "test@example.com");
    }

    [When(@"I select ""(.*)"" as the service type")]
    public async Task WhenISelectAsTheServiceType(string serviceType)
    {
        await _page.SelectOptionAsync("select[name='serviceType']", serviceType);
    }

    [When(@"I select ""(.*)"" as the issue type")]
    public async Task WhenISelectAsTheIssueType(string issueType)
    {
        await _page.SelectOptionAsync("select[name='issueType']", issueType);
    }

    [When(@"I enter ""(.*)"" as the registration number")]
    public async Task WhenIEnterAsTheRegistrationNumber(string regNumber)
    {
        await _page.FillAsync("input[name='registrationNumber']", regNumber);
    }

    [When(@"I select ""(.*)"" as the insurance type")]
    public async Task WhenISelectAsTheInsuranceType(string insuranceType)
    {
        await _page.SelectOptionAsync("select[name='insuranceType']", insuranceType);
    }

    [When(@"I enter a detailed message")]
    public async Task WhenIEnterADetailedMessage()
    {
        await _page.FillAsync("textarea[name='message']", "This is a test message describing my issue in detail. Please contact me as soon as possible.");
    }

    [When(@"I submit the form")]
    public async Task WhenISubmitTheForm()
    {
        await _page.ClickAsync("button.dynamisk-form-button");
        
        // Wait for form submission to complete
        await _page.WaitForSelectorAsync(".dynamisk-message", new() { State = WaitForSelectorState.Visible });
    }

    [When(@"I submit the form without filling required fields")]
    public async Task WhenISubmitTheFormWithoutFillingRequiredFields()
    {
        // Clear any filled fields first
        await _page.EvaluateAsync("document.querySelector('input[name=\"firstName\"]').value = ''");
        await _page.EvaluateAsync("document.querySelector('input[name=\"email\"]').value = ''");
        
        await _page.ClickAsync("button.dynamisk-form-button");
    }

    [Then(@"I should see a success message")]
    public async Task ThenIShouldSeeASuccessMessage()
    {
        var messageElement = await _page.WaitForSelectorAsync(".dynamisk-message:not(.error)");
        Assert.NotNull(messageElement);
        
        // Verify the message is not an error
        var classAttribute = await messageElement.GetAttributeAsync("class");
        Assert.DoesNotContain("error", classAttribute);
        
        // Optional: Verify specific success message text
        var messageText = await messageElement.TextContentAsync();
        Assert.NotEmpty(messageText);
    }

    [Then(@"I should see validation errors")]
    public async Task ThenIShouldSeeValidationErrors()
    {
        // Check for HTML5 validation errors
        var isValid = await _page.EvaluateAsync<bool>("() => document.querySelector('form').checkValidity()");
        Assert.False(isValid);
        
        // Alternative: Check for visible error message if your form shows custom errors
        var errorElement = await _page.QuerySelectorAsync(".dynamisk-message.error");
        if (errorElement != null)
        {
            var errorText = await errorElement.TextContentAsync();
            Assert.NotEmpty(errorText);
        }
    }

    // Helper method to wait for element to disappear (replaces wait timeout)
    private async Task WaitForElementToDisappear(string selector, int timeoutMs = 5000)
    {
        var startTime = DateTime.Now;
        while ((DateTime.Now - startTime).TotalMilliseconds < timeoutMs)
        {
            var element = await _page.QuerySelectorAsync(selector);
            if (element == null)
                return;
            
            await _page.WaitForTimeoutAsync(100);
        }
        
        Assert.Fail($"Element {selector} did not disappear within {timeoutMs}ms");
    }
}
    