using System;
using System.Threading.Tasks;
using Microsoft.Playwright;
using TechTalk.SpecFlow;
using Xunit;
using N2NTest.Helper;

namespace N2NTest.StepDefinitions
{
    [Binding]
    public class UpdatePasswordStepDefinitions
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

        [Given(@"I am logged in to the application")]
        public async Task GivenIAmLoggedInToTheApplication()
        {
            await LogIn.LoginAsync(_page);
            await _page.WaitForSelectorAsync("h2:text('Admin')", new() { Timeout = 10000 });
        }

        [When(@"I navigate to the update password page")]
        public async Task WhenINavigateToTheUpdatePasswordPage()
        {
            var updatePasswordLink = await _page.QuerySelectorAsync("a[href='/staff/update-user']");
            
            if (updatePasswordLink == null)
                updatePasswordLink = await _page.QuerySelectorAsync("a:has-text('Update password')");
            
            if (updatePasswordLink == null)
                updatePasswordLink = await _page.QuerySelectorAsync("a:has-text('Uppdatera lösenord')");
            
            if (updatePasswordLink != null)
            {
                await updatePasswordLink.ClickAsync();
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
            else
            {
                await _page.GotoAsync("http://localhost:3001/staff/update-user");
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
        }

        [When(@"I enter ""(.*)"" as the new username")]
        public async Task WhenIEnterAsTheNewUsername(string username)
        {
            var input = await _page.QuerySelectorAsync("input[name='firstName']");
            
            if (input == null)
                input = await _page.QuerySelectorAsync("input[placeholder*='användarnamn' i]");
            
            if (input != null)
                await input.FillAsync(username);
            else
                Assert.True(false, "Username input field not found");
        }

        [When(@"I enter ""(.*)"" as the new password")]
        public async Task WhenIEnterAsTheNewPassword(string password)
        {
            var inputs = await _page.QuerySelectorAllAsync("input[type='password']");
            
            if (inputs.Count > 0)
                await inputs[0].FillAsync(password);
            else
                Assert.True(false, "Password input field not found");
        }

        [When(@"I enter ""(.*)"" as the confirm password")]
        public async Task WhenIEnterAsTheConfirmPassword(string confirmPassword)
        {
            var inputs = await _page.QuerySelectorAllAsync("input[type='password']");
            
            if (inputs.Count > 1)
                await inputs[1].FillAsync(confirmPassword);
            else
                Assert.True(false, "Confirm password input field not found");
        }

        [When(@"I click the Update button")]
        public async Task WhenIClickTheUpdateButton()
        {
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            var button = await _page.QuerySelectorAsync("button.bla");
            
            if (button == null)
                button = await _page.QuerySelectorAsync("button:has-text('Uppdatera')");
            
            if (button != null)
            {
                await button.ClickAsync();
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
            else
                Assert.True(false, "Update button not found");
        }

        [Then(@"I should see a password mismatch error")]
        public async Task ThenIShouldSeeAPasswordMismatchError()
        {
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            var errorMessage = await _page.QuerySelectorAsync(".error-message");
            
            if (errorMessage == null)
                errorMessage = await _page.QuerySelectorAsync("div:has-text('matchar inte')");
            
            if (errorMessage != null)
            {
                var messageText = await errorMessage.TextContentAsync();
                Assert.Contains("matchar inte", messageText);
            }
            else
                Assert.True(false, "Error message not found");
        }

        [Then(@"I should see a password update success message")]
        public async Task ThenIShouldSeeAPasswordUpdateSuccessMessage()
        {
            try
            {
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                
                await _page.WaitForSelectorAsync(
                    "div:has-text('framgångsrikt'), .success-message", 
                    new() { Timeout = 5000 }
                );
                
                var successMessage = await _page.QuerySelectorAsync(".success-message");
                
                if (successMessage == null)
                    successMessage = await _page.QuerySelectorAsync("div:has-text('framgångsrikt')");
                
                if (successMessage != null)
                {
                    var messageText = await successMessage.TextContentAsync();
                    Assert.Contains("framgångsrikt", messageText);
                }
                else
                    Assert.True(false, "Success message not found");
            }
            catch (TimeoutException)
            {
                Assert.True(false, "Timed out waiting for success message");
            }
        }
    }
}