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
            // Use the login helper to log in
            await LogIn.LoginAsync(_page);
            
            // Verify we're logged in by checking for dashboard elements
            await _page.WaitForSelectorAsync("h2:text('Admin')", new() { Timeout = 10000 });
        }

        [When(@"I navigate to the update password page")]
        public async Task WhenINavigateToTheUpdatePasswordPage()
        {
            // Instead of directly navigating to the URL, click on the "Update password" link in the navigation
            Console.WriteLine("Looking for 'Update password' link in navigation...");
            
            // Try to find the link by its href attribute and text content
            var updatePasswordLink = await _page.QuerySelectorAsync("a[href='/staff/update-user']");
            
            if (updatePasswordLink == null)
            {
                // If not found by href, try to find by text content
                updatePasswordLink = await _page.QuerySelectorAsync("a:has-text('Update password')");
            }
            
            if (updatePasswordLink == null)
            {
                // Try with Swedish text
                updatePasswordLink = await _page.QuerySelectorAsync("a:has-text('Uppdatera lösenord')");
            }
            
            if (updatePasswordLink != null)
            {
                Console.WriteLine("Found 'Update password' link, clicking it...");
                await updatePasswordLink.ClickAsync();
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                
                Console.WriteLine($"Current URL after clicking: {_page.Url}");
            }
            else
            {
                Console.WriteLine("WARNING: Could not find 'Update password' link in navigation");
                Console.WriteLine("Taking screenshot of the current page to help debug...");
                
                // Take a screenshot to help debug
                await _page.ScreenshotAsync(new PageScreenshotOptions 
                { 
                    Path = "navigation-page.png",
                    FullPage = true
                });
                
                // Print all links on the page to help debug
                var allLinks = await _page.QuerySelectorAllAsync("a");
                Console.WriteLine($"Found {allLinks.Count} links on the page:");
                
                foreach (var link in allLinks)
                {
                    var href = await link.GetAttributeAsync("href");
                    var text = await link.TextContentAsync();
                    Console.WriteLine($"Link: href='{href}', text='{text}'");
                }
                
                // As a fallback, try direct navigation
                Console.WriteLine("Falling back to direct navigation...");
                await _page.GotoAsync("http://localhost:3001/staff/update-user");
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
            
            // Check if we're on the right page by looking for input fields
            var inputs = await _page.QuerySelectorAllAsync("input");
            Console.WriteLine($"Found {inputs.Count} input elements on the page");
            
            // Take a screenshot of the update password page
            await _page.ScreenshotAsync(new PageScreenshotOptions 
            { 
                Path = "update-password-page.png",
                FullPage = true
            });
            
            Console.WriteLine("Screenshot saved as update-password-page.png");
        }

        [When(@"I enter ""(.*)"" as the new username")]
        public async Task WhenIEnterAsTheNewUsername(string username)
        {
            // Try to find the input by name first
            var input = await _page.QuerySelectorAsync("input[name='firstName']");
            
            if (input == null)
            {
                // If not found by name, try to find by placeholder
                input = await _page.QuerySelectorAsync("input[placeholder*='användarnamn' i]");
            }
            
            if (input != null)
            {
                await input.FillAsync(username);
                Console.WriteLine($"Entered username: {username}");
            }
            else
            {
                Console.WriteLine("WARNING: Could not find username input field");
                Assert.True(false, "Username input field not found");
            }
        }

        [When(@"I enter ""(.*)"" as the new password")]
        public async Task WhenIEnterAsTheNewPassword(string password)
        {
            // Try to find the first password input
            var inputs = await _page.QuerySelectorAllAsync("input[type='password']");
            
            if (inputs.Count > 0)
            {
                await inputs[0].FillAsync(password);
                Console.WriteLine($"Entered password: {password}");
            }
            else
            {
                Console.WriteLine("WARNING: Could not find password input field");
                Assert.True(false, "Password input field not found");
            }
        }

        [When(@"I enter ""(.*)"" as the confirm password")]
        public async Task WhenIEnterAsTheConfirmPassword(string confirmPassword)
        {
            // Try to find the second password input
            var inputs = await _page.QuerySelectorAllAsync("input[type='password']");
            
            if (inputs.Count > 1)
            {
                await inputs[1].FillAsync(confirmPassword);
                Console.WriteLine($"Entered confirm password: {confirmPassword}");
            }
            else
            {
                Console.WriteLine("WARNING: Could not find confirm password input field");
                Assert.True(false, "Confirm password input field not found");
            }
        }

        [When(@"I click the Update button")]
        public async Task WhenIClickTheUpdateButton()
        {
            // Wait for any previous operations to complete
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            // Try to find the button by class first
            var button = await _page.QuerySelectorAsync("button.bla");
            
            if (button == null)
            {
                // If not found by class, try to find by text content
                button = await _page.QuerySelectorAsync("button:has-text('Uppdatera')");
            }
            
            if (button != null)
            {
                await button.ClickAsync();
                Console.WriteLine("Clicked the update button");
                
                // Wait for the request to complete
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
            else
            {
                Console.WriteLine("WARNING: Could not find update button");
                
                // Take a screenshot to help debug
                await _page.ScreenshotAsync(new PageScreenshotOptions 
                { 
                    Path = "update-button-not-found.png",
                    FullPage = true
                });
                
                Assert.True(false, "Update button not found");
            }
        }

        [Then(@"I should see a password mismatch error")]
        public async Task ThenIShouldSeeAPasswordMismatchError()
        {
            // Wait for any error message to appear
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            
            // Try to find error message by class
            var errorMessage = await _page.QuerySelectorAsync(".error-message");
            
            if (errorMessage == null)
            {
                // If not found by class, try to find by text content
                errorMessage = await _page.QuerySelectorAsync("div:has-text('matchar inte')");
            }
            
            if (errorMessage != null)
            {
                var messageText = await errorMessage.TextContentAsync();
                Console.WriteLine($"Found error message: {messageText}");
                Assert.Contains("matchar inte", messageText);
            }
            else
            {
                // Take a screenshot to help debug
                await _page.ScreenshotAsync(new PageScreenshotOptions 
                { 
                    Path = "error-message-not-found.png",
                    FullPage = true
                });
                
                Console.WriteLine("ERROR: Could not find error message. Screenshot saved as error-message-not-found.png");
                Assert.True(false, "Error message not found");
            }
        }

        [Then(@"I should see a password update success message")]
public async Task ThenIShouldSeeAPasswordUpdateSuccessMessage()
{
    // Wait for the success message to appear with a longer timeout
    try
    {
        // First wait for network activity to complete
        await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        
        // Then wait for the success message with a specific timeout
        Console.WriteLine("Waiting for success message to appear...");
        await _page.WaitForSelectorAsync(
            "div:has-text('framgångsrikt'), .success-message", 
            new() { Timeout = 5000 }
        );
        
        // Now try to find the success message
        var successMessage = await _page.QuerySelectorAsync(".success-message");
        
        if (successMessage == null)
        {
            // If not found by class, try to find by text content
            successMessage = await _page.QuerySelectorAsync("div:has-text('framgångsrikt')");
        }
        
        if (successMessage != null)
        {
            var messageText = await successMessage.TextContentAsync();
            Console.WriteLine($"Found success message: {messageText}");
            Assert.Contains("framgångsrikt", messageText);
        }
        else
        {
            // Take a screenshot to help debug
            await _page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = "success-message-not-found.png",
                FullPage = true
            });
            
            Console.WriteLine("ERROR: Could not find success message. Screenshot saved as success-message-not-found.png");
            Assert.True(false, "Success message not found");
        }
    }
    catch (TimeoutException ex)
    {
        // Take a screenshot if we time out waiting for the message
        await _page.ScreenshotAsync(new PageScreenshotOptions
        {
            Path = "success-message-timeout.png",
            FullPage = true
        });
        
        Console.WriteLine($"ERROR: Timed out waiting for success message: {ex.Message}");
        Console.WriteLine("Screenshot saved as success-message-timeout.png");
        
        // Print the page content to help debug
        var pageContent = await _page.ContentAsync();
        Console.WriteLine("Page content at time of failure:");
        Console.WriteLine(pageContent.Substring(0, Math.Min(500, pageContent.Length)) + "...");
        
        Assert.True(false, "Timed out waiting for success message");
    }
}

        }
    }

