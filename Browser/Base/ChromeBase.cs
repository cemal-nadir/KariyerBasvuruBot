
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using WebDriverManager;
using WebDriverManager.DriverConfigs.Impl;
using WebDriverManager.Helpers;
using Browser.Helpers;
using System;

namespace Browser.Base
{
    public class ChromeBase : IBrowserBase
    {
        private readonly ChromeOptions _options;

        public ChromeBase()
        {
            new DriverManager().SetUpDriver(new ChromeConfig(), VersionResolveStrategy.MatchingBrowser);
            _options = new ChromeOptions();
        //    options.AddArgument("--headless");             
            _options.AddArguments("--enable-extensions");
            _options.AddArgument("no-sandbox");
            _options.AddArgument("--ignore-certificate-errors");
       //     options.AddUserProfilePreference("download.default_directory", Configs.appConfiguration.GetSection("GeneralSettings").GetSection("DownloadLocation").Value);
       //     options.AddUserProfilePreference("download.prompt_for_download", false);
            _options.AddArgument("disable-infobars");
            _options.AddArgument("--disable-setuid-sandbox");
            _options.AddArgument("--disable-gpu");
      //      options.AddArguments("--start-maximized");
            _options.AddArgument("--user-agent=Mozilla/5.0 (X11; Linux x86_64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/60.0.3112.50 Safari/537.36");
            _options.AddArguments("--window-size=1280,1024");
        }
        public IWebDriver GenerateBrowser(string applicationStartupPath)
        {
            var serviceLoc=ChromeHelper.FindReleaseService(applicationStartupPath);
            var service=ChromeDriverService.CreateDefaultService(serviceLoc);
            service.SuppressInitialDiagnosticInformation = true;
            service.HideCommandPromptWindow = true;
            var driver= new ChromeDriver(service, _options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            return driver;
        }
    }
}
