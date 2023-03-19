using OpenQA.Selenium;

namespace Browser.Base
{
    public interface IBrowserBase
    {
        public IWebDriver GenerateBrowser(string applicationStartupPath);
    }
}
