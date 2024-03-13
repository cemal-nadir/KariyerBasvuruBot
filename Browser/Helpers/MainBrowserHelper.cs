using OpenQA.Selenium;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Browser.Helpers
{
    public static class MainBrowserHelper
    {
        public static void OpenNewTab(this IWebDriver driver)
        {
            ((IJavaScriptExecutor)driver).ExecuteScript("window.open();");
            driver.SwitchTo().Window(driver.WindowHandles.Last());
        }
        public static void SwitchTab(this IWebDriver driver,int tabNumber)
        {
            driver.SwitchTo().Window(driver.WindowHandles[tabNumber]);
        }
        public static void SwitchFirstTab(this IWebDriver driver)
        {
            driver.SwitchTo().Window(driver.WindowHandles.First());
        }
        public static void SwitchLastTab(this IWebDriver driver)
        {
            driver.SwitchTo().Window(driver.WindowHandles.Last());
        }
        public static void CloseCurrentTab(this IWebDriver driver)
        {
            driver.Close();
        }
        public static void ClickWithJs(this IWebDriver webDriver,IWebElement webElement)
        {

            var executor = (IJavaScriptExecutor)webDriver;
            executor.ExecuteScript("arguments[0].click();", webElement);
        }
        public static void PressEsc(this IWebDriver driver)
        {
            driver.FindElement(By.XPath("//body")).SendKeys(Keys.Escape);
        }
        public static string GetXPath(this IWebDriver driver, IWebElement element)
        {
            return (string)((IJavaScriptExecutor)driver).ExecuteScript("gPt=function(c){if(c.id!==''){return'id(\"'+c.id+'\")'}if(c===document.body){return c.tagName}var a=0;var e=c.parentNode.childNodes;for(var b=0;b<e.length;b++){var d=e[b];if(d===c){return gPt(c.parentNode)+'/'+c.tagName+'['+(a+1)+']'}if(d.nodeType===1&&d.tagName===c.tagName){a++}}};return gPt(arguments[0]).toLowerCase();", element);
        }

        public static void MoveElement(this IWebDriver driver, IWebElement element)
        {
            IJavaScriptExecutor js = (IJavaScriptExecutor)driver;
            js.ExecuteScript("arguments[0].scrollIntoView(true);", element);

            int yOffset = -100; // Scroll yapılacak yükseklik
            js.ExecuteScript("window.scrollTo(0, arguments[0].getBoundingClientRect().top + window.pageYOffset + " + yOffset + ")", element);


            //js.ExecuteScript("window.scrollBy(0, -30);");
        }
    }
}
