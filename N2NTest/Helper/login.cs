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
                } catch (TimeoutException) {
                    // Continue even on timeout
                }
                
                // Check for error messages
                var errorElement = await page.QuerySelectorAsync(".error-message");
                if (errorElement != null)
                {
                    var errorText = await errorElement.TextContentAsync();
                    Console.WriteLine($"Login error detected: {errorText}");
                }
                
                // Check if we're on the dashboard
                var currentUrl = page.Url;
                
                if (!currentUrl.Contains("admin/dashboard"))
                {
                    // If not on dashboard, try navigating directly
                    await page.GotoAsync("http://localhost:3001/admin/dashboard");
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                    
                    // Check URL again after direct navigation
                    currentUrl = page.Url;
                    
                    // If we're redirected back to login, login likely failed
                    if (currentUrl.Contains("staff/login"))
                    {
                        Console.WriteLine("Redirected back to login page - login appears to have failed");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception during login: {ex.Message}");
                throw;
            }
        }
    }
}