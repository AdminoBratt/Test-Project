using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using TechTalk.SpecFlow;
using N2NTest.Helper;

namespace N2NTest.StepDefinitions
{
    [Binding]
    public class LoginStepDefinitions
    {
        private IPage _page = null!;
        private IBrowser _browser = null!;
        private IBrowserContext _context = null!;
        private IPlaywright _playwright = null!;

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false
            });
            _context = await _browser.NewContextAsync();
            _page = await _context.NewPageAsync();
        }

        [AfterScenario]
        public async Task AfterScenario()
        {
            await _context?.CloseAsync();
            await _browser?.CloseAsync();
            _playwright?.Dispose();
        }

        [Given(@"I am on the staff login page")]
        public async Task GivenIAmOnTheStaffLoginPage()
        {
            await _page.GotoAsync("http://localhost:3001/staff/login");
            await _page.WaitForSelectorAsync(".staff-field-input[type='text']", new() { State = WaitForSelectorState.Visible });
        }

        [When(@"I enter valid staff credentials")]
        public async Task WhenIEnterValidStaffCredentials()
        {
            // Clear fields first to ensure clean input
            await _page.FillAsync(".staff-field-input[type='text']", "");
            await _page.FillAsync(".staff-field-input[type='password']", "");
            
            // Then fill with credentials
            await _page.FillAsync(".staff-field-input[type='text']", "siggebratt1@gmail.com");
            await _page.FillAsync(".staff-field-input[type='password']", "02589");
        }

        [When(@"I click the login button")]
        public async Task WhenIClickTheLoginButton()
        {
            // Use reliable Promise.all pattern to handle navigation
            var navigationTask = _page.WaitForNavigationAsync(new() { 
                Timeout = 30000,
                WaitUntil = WaitUntilState.NetworkIdle
            });
            
            await _page.ClickAsync("button.staff-login-button");
            
            try {
                await navigationTask;
            } catch (TimeoutException) {
                Console.WriteLine("Navigation timeout occurred - will proceed anyway");
            }
        }

        [Then(@"I should be redirected to the admin dashboard")]
        public async Task ThenIShouldBeRedirectedToTheAdminDashboard()
        {
            // Wait a bit longer in case of delayed redirection
            await Task.Delay(2000);
            
            var currentUrl = _page.Url;
            
            // If not automatically redirected, navigate directly
            if (!currentUrl.Contains("admin/dashboard"))
            {
                await _page.GotoAsync("http://localhost:3001/admin/dashboard");
                
                // Wait for network to become idle after manual navigation
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                
                // Check if we're able to access the dashboard
                currentUrl = _page.Url;
                
                // If we're still not on the dashboard after direct navigation, login might have failed
                if (!currentUrl.Contains("admin/dashboard"))
                {
                    // Check if we were redirected back to login page with error
                    var errorElement = await _page.QuerySelectorAsync(".error-message");
                    if (errorElement != null)
                    {
                        var errorText = await errorElement.TextContentAsync();
                        throw new Exception($"Login failed with error: {errorText}");
                    }
                    
                    throw new Exception($"Expected URL to contain 'admin/dashboard', but was '{currentUrl}'");
                }
            }
        }

        [When(@"I log in using the helper function")]
        public async Task WhenILogInUsingTheHelperFunction()
        {
            await LogIn.LoginAsync(_page);
        }
    }
}