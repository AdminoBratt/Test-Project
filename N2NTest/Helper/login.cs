using Microsoft.Playwright;
using System;
using System.Threading.Tasks;

namespace N2NTest.Helper
{
    public class LogIn
    {
        public static async Task LoginAsync(IPage page)
        {
            try
            {
                // Navigate to the staff login page
                await page.GotoAsync("http://localhost:3001/staff/login");
                
                // Wait for the login form to be visible
                await page.WaitForSelectorAsync(".staff-field-input[type='text']", new() { State = WaitForSelectorState.Visible });
                
                // Clear fields first to ensure clean input
                await page.FillAsync(".staff-field-input[type='text']", "");
                await page.FillAsync(".staff-field-input[type='password']", "");
                
                // Fill in the username field
                await page.FillAsync(".staff-field-input[type='text']", "siggebratt1@gmail.com");
                
                // Fill in the password field
                await page.FillAsync(".staff-field-input[type='password']", "02589");
                
                // Take a screenshot before clicking login
                await page.ScreenshotAsync(new() { Path = "before-login-click.png" });
                
                // Set up a task to wait for navigation
                var navigationPromise = page.WaitForNavigationAsync(new() { 
                    Timeout = 30000,  // 30 seconds timeout
                    WaitUntil = WaitUntilState.NetworkIdle
                });
                
                // Click the login button
                await page.ClickAsync("button.staff-login-button");
                
                // Wait for navigation to complete
                try {
                    await navigationPromise;
                    Console.WriteLine("Navigation completed successfully");
                } catch (TimeoutException) {
                    Console.WriteLine("Navigation timeout occurred - proceeding anyway");
                    // Continue even on timeout
                }
                
                // Take a screenshot after login attempt
                await page.ScreenshotAsync(new() { Path = "after-login-click.png" });
                
                // Check for error messages
                var errorElement = await page.QuerySelectorAsync(".error-message");
                if (errorElement != null)
                {
                    var errorText = await errorElement.TextContentAsync();
                    Console.WriteLine($"Login error detected: {errorText}");
                }
                
                // Check if we're on the dashboard
                var currentUrl = page.Url;
                Console.WriteLine($"Current URL after login attempt: {currentUrl}");
                
                if (!currentUrl.Contains("admin/dashboard"))
                {
                    Console.WriteLine("Not on dashboard, attempting direct navigation...");
                    
                    // If not on dashboard, try navigating directly
                    await page.GotoAsync("http://localhost:3001/admin/dashboard");
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    
                    // Check URL again after direct navigation
                    currentUrl = page.Url;
                    Console.WriteLine($"URL after direct navigation: {currentUrl}");
                    
                    // If we're redirected back to login, login likely failed
                    if (currentUrl.Contains("staff/login"))
                    {
                        Console.WriteLine("Redirected back to login page - login appears to have failed");
                        await page.ScreenshotAsync(new() { Path = "login-failure.png" });
                    }
                }
                
                // Final screenshot to show current state
                await page.ScreenshotAsync(new() { Path = "final-state.png" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during login: {ex.Message}");
                await page.ScreenshotAsync(new() { Path = "login-exception.png" });
                throw;
            }
        }
    }
}