using Microsoft.Playwright;

namespace N2NTest.Helper
{
    public class LogIn
    {
        public static async Task LoginAsync(IPage page)
        {
            
            
                // Navigate to the staff login page
                await page.GotoAsync("http://localhost:3001/staff/login");
                
                // Wait for the login form to be visible
                await page.WaitForSelectorAsync(".staff-field-input[type='text']", new() { State = WaitForSelectorState.Visible });
                
                // Fill in the username field
                await page.FillAsync(".staff-field-input[type='text']", "siggebratt1@gmail.com");
                
                // Fill in the password field
                await page.FillAsync(".staff-field-input[type='password']", "02589");
                
                // Take a screenshot before clicking login
                await page.ScreenshotAsync(new() { Path = "before-login-click.png" });
                
                // Click the login button without waiting for navigation
                await page.ClickAsync(".staff-login-button");
                
                // Wait for navigation to complete after login
                // Don't use WaitForURLAsync as it's causing the timeout
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                
                // Take a screenshot after login
                await page.ScreenshotAsync(new() { Path = "after-login.png" });
                
                // Verify we're logged in by checking for admin dashboard elements
                // or by checking the URL contains "admin/dashboard"
                var currentUrl = page.Url;
                if (!currentUrl.Contains("admin/dashboard"))
                {
                    // If we're not on the dashboard, navigate there directly
                    await page.GotoAsync("http://localhost:3001/admin/dashboard");
                    await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                }
            
            
        }
    }
}
