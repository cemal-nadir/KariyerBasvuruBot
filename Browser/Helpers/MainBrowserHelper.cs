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
        public static bool PageIsLoad(this IWebDriver driver)
        {
            try
            {
                return (bool)((IJavaScriptExecutor)driver).ExecuteScript("return jQuery.active == 0 && document.readyState === 'complete'");
            }
            catch (JavaScriptException ex)
            {
                if (ex.Message.Contains("javascript error: jQuery is not defined"))
                {
                    return (bool)((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState === 'complete'");
                }
                else
                    throw new Exception("Javascript çalıştırılırken bir hata meydana geldi " + ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Sebebi Bilinmeyen Bir Hata Oluştu " + ex.Message);
            }

        }
        public static Task WaitPageLoad(this IWebDriver driver, int second)
        {
            return Task.Run(() =>
            {
                var cacheSecond = second;
                try
                {
                    while (second > 0 && !(bool)((IJavaScriptExecutor)driver).ExecuteScript("return jQuery.active == 0 && document.readyState === 'complete'"))
                    {
                        Thread.Sleep(1000);
                        second--;
                    }
                    if (!(bool)((IJavaScriptExecutor)driver).ExecuteScript("return jQuery.active == 0 && document.readyState === 'complete'"))
                        throw new Exception(cacheSecond + " Saniye Beklendi Sayfa Yüklenmedi !");
                }
                catch (JavaScriptException ex)
                {
                    if (ex.Message.Contains("javascript error: jQuery is not defined"))
                    {
                        while (second > 0 && !(bool)((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState === 'complete'"))
                        {
                            Thread.Sleep(1000);
                            second--;
                        }
                        if (!(bool)((IJavaScriptExecutor)driver).ExecuteScript("return document.readyState === 'complete'"))
                            throw new Exception(cacheSecond + " Saniye Beklendi Sayfa Yüklenmedi !");
                    }
                }
                catch (Exception ex)
                {
                    throw new Exception("Sebebi Bilinmeyen Bir Hata Oluştu " + ex.Message);
                }

            });
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
    }
}
