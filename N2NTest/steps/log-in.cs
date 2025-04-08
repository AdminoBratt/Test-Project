using Microsoft.Playwright;
using TechTalk.SpecFlow;

namespace E2ETesting.Steps
{
    [Binding]
    public class LoginSteps
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

        [Given(@"I am on the application homepage")]
        public async Task GivenIAmOnTheApplicationHomepage()
        {
            await _page.GotoAsync("http://localhost:3001/staff/login");
        }

        [When(@"I click on the login button")]
        public async Task WhenIClickOnTheLoginButton()
        {
            await _page.ClickAsync(".staff-login-button");
        }

        [When(@"I enter ""(.*)"" as email")]
        public async Task WhenIEnterAsEmail(string email)
        {
            await _page.FillAsync("input[type='text']", email);
        }

        [When(@"I enter ""(.*)"" as password")]
        public async Task WhenIEnterAsPassword(string password)
        {
            await _page.FillAsync("input[type='password']", password);
        }

        [When(@"I click the Login button")]
        public async Task WhenIClickTheLoginButton()
        {
            await _page.ClickAsync("button[type='submit']");
        }

        [Then(@"I should be logged in successfully")]
        public async Task ThenIShouldBeLoggedInSuccessfully()
        {
            await _page.WaitForURLAsync("**/dashboard");
            Assert.Contains("dashboard", _page.Url);
        }

        [Then(@"I should see staff features for ""(.*)"" department")]
        public async Task ThenIShouldSeeStaffFeaturesForDepartment(string department)
        {
            var element = await _page.WaitForSelectorAsync($"span.company-name:text('{department}')");
            Assert.NotNull(element);
        }



        [Then(@"I should see my name ""(.*)"" in the header")]
        public async Task ThenIShouldSeeMyNameInTheHeader(string name)
        {
            var nameElement = await _page.WaitForSelectorAsync(".user-name");
            var text = await nameElement.TextContentAsync();
            Assert.Equal(name, text);
        }

        [Then(@"I should see admin dashboard in the nav-bar")]
        public async Task ThenIShouldSeeAdminDashboardInTheNavBar()
        {
            var element = await _page.WaitForSelectorAsync("h2:text('Admin')");
            Assert.NotNull(element);
        }


        [Then(@"I should see an error message")]
        public async Task ThenIShouldSeeAnErrorMessage()
        {
            var errorElement = await _page.WaitForSelectorAsync(".error-message");
            Assert.NotNull(errorElement);
        }

        [Then(@"I should remain on the login modal")]
        public async Task ThenIShouldRemainOnTheLoginModal()
        {
            Assert.Contains("login", _page.Url);
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            await _browser?.CloseAsync();
            _playwright?.Dispose();
        }
    }
}
