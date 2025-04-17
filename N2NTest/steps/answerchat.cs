using Microsoft.Playwright;
using TechTalk.SpecFlow;
using N2NTest.Helper;

namespace N2NTest.StepDefinitions
{
    [Binding]
    public class AdminChatSteps
    {
        // SETUP:
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IBrowserContext _context;
        private IPage _page;
        private string _baseUrl = "http://localhost:3001";
        private string _testMessage;

        [BeforeScenario]
        public async Task BeforeScenario()
        {
            _playwright = await Playwright.CreateAsync();
            _browser = await _playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                SlowMo = 100
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

        [Given(@"I am logged in as an admin")]
        public async Task GivenIAmLoggedInAsAnAdmin()
        {
            await LogIn.LoginAsync(_page);
            
            await _page.WaitForURLAsync("**/admin/dashboard", new PageWaitForURLOptions { Timeout = 30000 });
        }

        [When(@"I navigate to the admin dashboard")]
        public async Task WhenINavigateToTheAdminDashboard()
        {
            if (!_page.Url.Contains("/admin/dashboard"))
            {
                await _page.GotoAsync($"{_baseUrl}/admin/dashboard");
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            }
        }

        [When(@"I switch to the tickets view")]
        public async Task WhenISwitchToTheTicketsView()
        {
            await _page.ClickAsync("button.toggle-button:text('Ärenden')");
            
            await _page.WaitForSelectorAsync("table.data-table", new PageWaitForSelectorOptions 
            { 
                State = WaitForSelectorState.Visible 
            });
        }

        [When(@"I open a chat for the first available ticket")]
        public async Task WhenIOpenAChatForTheFirstAvailableTicket()
        {
            var chatLink = await _page.WaitForSelectorAsync("a:text('Open Chat')", new PageWaitForSelectorOptions 
            { 
                State = WaitForSelectorState.Visible 
            });
            
            if (chatLink == null)
            {
                throw new System.Exception("No chat links found in the tickets table");
            }
            
            await chatLink.ClickAsync();
            
            await _page.WaitForSelectorAsync(".chat-modal__container", new PageWaitForSelectorOptions 
            { 
                State = WaitForSelectorState.Visible, 
                Timeout = 10000 
            });
        }

        [When(@"I send a message ""(.*)""")]
        public async Task WhenISendAMessage(string message)
        {
            _testMessage = message;
            
            await _page.FillAsync(".chat-modal__input-field", message);
            
            await _page.ClickAsync(".chat-modal__send-button");
            
            await _page.WaitForTimeoutAsync(2000);
        }

        [Then(@"I should see my message in the chat window")]
        public async Task ThenIShouldSeeMyMessageInTheChatWindow()
        {
            var messageSelector = $"p.chat-modal__message-text:text(\"{_testMessage}\")";
            
            var messageElement = await _page.WaitForSelectorAsync(messageSelector, new PageWaitForSelectorOptions 
            { 
                State = WaitForSelectorState.Visible,
                Timeout = 15000  
            });
            
            Assert.NotNull(messageElement);
            
            var messageText = await messageElement.TextContentAsync();
            Assert.Contains(_testMessage, messageText);
            
        }
    }
}
