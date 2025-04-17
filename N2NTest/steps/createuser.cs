using Microsoft.Playwright;
using TechTalk.SpecFlow;
using N2NTest.Helper;

namespace E2ETesting.Steps
{
    [Binding]
    [Scope(Feature = "Admin Create User")] 
    public class AdminCreateUserSteps
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
            _browser = await _playwright.Chromium.LaunchAsync(new() { Headless = false, SlowMo = 100 });
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
        [Given(@"I am logged in as an admin")]
        public async Task GivenIAmLoggedInAsAnAdmin()
        {
            await LogIn.LoginAsync(_page);
            
            await _page.WaitForURLAsync("**/admin/dashboard", new() { Timeout = 30000 });
        }

        [Given(@"I navigate to the create user page")]
        public async Task GivenINavigateToTheCreateUserPage()
        {
            try
            {
                await _page.GotoAsync($"{_baseUrl}/admin/create-user");
                
                var createUserForm = await _page.QuerySelectorAsync("form .admin-login:text('Skapa användare')");
                
                if (createUserForm == null)
                {
                    var createUserLink = await _page.QuerySelectorAsync("a[href='/admin/create-user']");
                    if (createUserLink != null)
                    {
                        await createUserLink.ClickAsync();
                        await _page.WaitForSelectorAsync("form .admin-login:text('Skapa användare')", new() { Timeout = 5000 });
                    }
                    else
                    {
                        throw new Exception("Could not find the Create User link in the navigation");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error navigating to create user page: {ex.Message}");
                throw;
            }
        }

        [When(@"I fill in the email field with ""(.*)""")]
        public async Task WhenIFillInTheEmailFieldWith(string email)
        {
            await _page.FillAsync("input[name='email']", email);
        }

        [When(@"I fill in the username field with ""(.*)""")]
        public async Task WhenIFillInTheUsernameFieldWith(string username)
        {
            await _page.FillAsync("input[name='firstName']", username);
        }

        [When(@"I fill in the password field with ""(.*)""")]
        public async Task WhenIFillInThePasswordFieldWith(string password)
        {
            await _page.FillAsync("input[name='password']", password);
        }

        [When(@"I select ""(.*)"" as the company")]
        public async Task WhenISelectAsTheCompany(string company)
        {
            await _page.SelectOptionAsync("select[name='company']", company);
        }

        [When(@"I select ""(.*)"" as the role")]
        public async Task WhenISelectAsTheRole(string role)
        {
            await _page.SelectOptionAsync("select[name='role']", role);
        }

        [When(@"I submit the create user form")]
        public async Task WhenISubmitTheCreateUserForm()
        {
            await _page.ClickAsync("button.bla");
    
            await _page.WaitForSelectorAsync("div[class*='message']", 
                new() { State = WaitForSelectorState.Visible, Timeout = 30000 });
        }


        [When(@"I fill in all required fields with valid data")]
        public async Task WhenIFillInAllRequiredFieldsWithValidData()
        {
            string randomEmail = $"test{DateTime.Now.Ticks}@example.com";
            
            await _page.FillAsync("input[name='email']", randomEmail);
            await _page.FillAsync("input[name='firstName']", "Test User");
            await _page.FillAsync("input[name='password']", "Password123!");
            await _page.SelectOptionAsync("select[name='company']", "fordon");
            await _page.SelectOptionAsync("select[name='role']", "staff");
        }

        [Then(@"I should see a success message")]
        public async Task ThenIShouldSeeASuccessMessage()
        {
            var successMessage = await _page.WaitForSelectorAsync("div.success-message", 
                new() { State = WaitForSelectorState.Visible, Timeout = 30000 });
    
            Assert.NotNull(successMessage);
    
            var messageText = await successMessage.TextContentAsync();
            Assert.Contains("framgångsrikt", messageText);
        }


        [Then(@"the form should be reset")]
        public async Task ThenTheFormShouldBeReset()
        {
            var emailValue = await _page.InputValueAsync("input[name='email']");
            var firstNameValue = await _page.InputValueAsync("input[name='firstName']");
            var passwordValue = await _page.InputValueAsync("input[name='password']");
            
            Assert.Equal("", emailValue);
            Assert.Equal("", firstNameValue);
            Assert.Equal("", passwordValue);
        }

        [Then(@"I should see an error message")]
        public async Task ThenIShouldSeeAnErrorMessage()
        {
            var errorMessage = await _page.WaitForSelectorAsync("div[class*='error-message']");
            Assert.NotNull(errorMessage);
            
            var messageText = await errorMessage.TextContentAsync();
            Assert.NotEmpty(messageText);
        }
    }
}
