﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Browser.Helpers;
using Core.Configs;
using OpenQA.Selenium;

namespace Bot.Extensions
{
    public static class KariyerExtension
    {
        #region İhtiyaç Dahilinde Kullanılacaklar
        /// <summary>
        /// Kariyer Giriş Ekranını Açar
        /// </summary>
        /// <param name="driver"></param>
        public static void GoLoginPage(this IWebDriver driver)
        {
            driver.Navigate().GoToUrl(UrlConfigs.KariyerLoginUrl);
            driver.WaitPageLoad(20);
            Thread.Sleep(5000);
        }
        /// <summary>
        /// Kariyer İş Arama Ekranını Açar
        /// </summary>
        /// <param name="driver"></param>
        public static void GoJobSearchScreen(this IWebDriver driver)
        {
            driver.Navigate().GoToUrl(UrlConfigs.KariyerJobSearchUrl);
        }
        /// <summary>
        /// Kariyer'e Giriş Yapılıp Yapılmadığını Kontrol Eder
        /// </summary>
        /// <param name="driver"></param>
        /// <returns></returns>
        public static bool CheckIsLogged(this IWebDriver driver)
        {
            while (string.IsNullOrEmpty(driver.PageSource))
            {
                Thread.Sleep(100);
            }
            return driver.PageSource.Contains("profile-url");
        }
        /// <summary>
        /// Kariyer İş Filtrelemelerini Uygulamak İçin Basılması Gereken Butona Basar
        /// </summary>
        /// <param name="driver"></param>
        public static void ClickApplyFilterButton(this IWebDriver driver)
        {

            var element = driver.FindElement(By.XPath("//button[@data-test='apply-button']"));
            driver.ClickWithJs(element);
            driver.WaitPageLoad(20).Wait();
        }

        public static void CheckAndCloseNotification(this IWebDriver driver)
        {
            try
            {
                var notificationDiv = driver.FindElement(By.CssSelector("body > div.vl-notifier-container"));
                driver.ClickWithJs(notificationDiv.FindElement(By.ClassName("vl-close")));
                driver.WaitPageLoad(20).Wait();
                Thread.Sleep(2000);


            }
            catch
            {
                // ignored
            }
        }
        public static void CheckAndCloseCookiePolicy(this IWebDriver driver)
        {
            try
            {
                var notificationDiv = driver.FindElement(By.CssSelector("body > div.cookie-policy"));
                var closeButton = notificationDiv.FindElement(By.ClassName("cookie-policy-close"));
                driver.ClickWithJs(closeButton);
                driver.WaitPageLoad(20).Wait();
                Thread.Sleep(2000);


            }
            catch
            {
                // ignored
            }
        }

        public static void OpenCollapse(this IWebDriver driver, string collapseId)
        {
            var elements = driver.FindElements(By.ClassName("k-collapse"));
            foreach (var element in elements)
            {
                if (element.GetAttribute("collapse-id") != collapseId &&
                    element.GetAttribute("collapseid") != collapseId) continue;
                try
                {
                    var aTag = element.FindElement(By.TagName("a"));
                    driver.ClickWithJs(aTag);
                    Thread.Sleep(1000);
                }
                catch
                {
                    // ignored
                }
            }
            driver.WaitPageLoad(20).Wait();
        }
        #endregion

        #region Filtreleme İşlemleri

        #region Çoklu Seçim Filtreleme

        /// <summary>
        /// ComboBox Benzeri Çoklu Seçimler İçin Filtreleme
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="selectedTexts">Seçim Listesi</param>
        /// <param name="dataTestValue"></param>
        /// <param name="collapseId"></param>
        public static void Filter(this IWebDriver driver, IEnumerable<string> selectedTexts, string dataTestValue = null, string collapseId = null)
        {
            var addedCache = new List<string>();
            var filterSection = driver.GetFilterSection(collapseId);


            if (selectedTexts == null) return;
            foreach (var text in selectedTexts)
            {
                var elements = filterSection.FindElements(By.XPath("//*[contains(text(),'" + text + "')]"))
                    .Where(x => x.Text.ToLower(System.Globalization.CultureInfo.CreateSpecificCulture("tr")).Replace(" ", "") ==
                                text.ToLower(System.Globalization.CultureInfo.CreateSpecificCulture("tr")).Replace(" ", ""));
                foreach (var element in elements)
                {
                    if (element.GetAttribute("data-test") != dataTestValue || addedCache.Any(x => x == element.Text)) continue;
                    try
                    {
                        driver.ClickWithJs(element);
                        driver.WaitPageLoad(20).Wait();
                        Thread.Sleep(1000);
                        addedCache.Add(element.Text);
                    }
                    catch
                    {
                        // ignored
                    }
                }
                driver.WaitPageLoad(20).Wait();
            }
        }


        /// <summary>
        /// ComboBox Benzeri Çoklu Seçimler İçin Filtreleme
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="selectedTexts">Seçim Listesi</param>
        /// <param name="placeholderText">Metin Input Placeholder'ı</param>
        /// <param name="dataTestValue"></param>
        /// <param name="collapseId"></param>
        public static void FilterSearchable(this IWebDriver driver, string placeholderText, IEnumerable<string> selectedTexts, string dataTestValue = null, string collapseId = null)
        {
            var addedCache = new List<string>();
            var filterSection = driver.GetFilterSection(collapseId);
            var inputBoxSection = filterSection.FindElement(By.XPath("//input[@placeholder='" + placeholderText + "']"));

            if (selectedTexts == null) return;
            foreach (var text in selectedTexts)
            {
                driver.ClickWithJs(inputBoxSection);

                driver.WaitPageLoad(20).Wait();
                Thread.Sleep(1000);
                inputBoxSection.Clear();
                inputBoxSection.SendKeys(text);
                driver.WaitPageLoad(20).Wait();
                Thread.Sleep(1000);
                var elements = filterSection.FindElements(By.XPath("//*[contains(text(),'" + text + "')]"))
                    .Where(x => x.Text.ToLower(System.Globalization.CultureInfo.CreateSpecificCulture("tr")).Replace(" ", "") ==
                                text.ToLower(System.Globalization.CultureInfo.CreateSpecificCulture("tr")).Replace(" ", ""));
                foreach (var element in elements)
                {
                    if (element.GetAttribute("data-test") != dataTestValue ||
                        addedCache.Any(x => x == element.Text)) continue;
                    try
                    {
                        driver.ClickWithJs(element);
                        driver.WaitPageLoad(20);
                        Thread.Sleep(1000);
                        addedCache.Add(element.Text);
                    }
                    catch
                    {
                        // ignored
                    }
                }
                driver.WaitPageLoad(20).Wait();
                Thread.Sleep(1000);
            }
        }
        #endregion

        #region Tekil Seçim Filtreleme
        /// <summary>
        /// Uzaktan Çalışma İlan Filtreleme
        /// </summary>
        /// <param name="driver"></param>
        public static void FilterRemoteJobs(this IWebDriver driver)
        {
            var filterSection = driver.GetFilterSection();
            driver.ClickWithJs(filterSection.FindElement(By.XPath("//*[contains(text(),'Uzaktan Çalışma')]")));
        }
        /// <summary>
        /// Başvurduğum İlanları Gösterme Filtrelemesi
        /// </summary>
        /// <param name="driver"></param>
        public static void FilterHideAppliedJobs(this IWebDriver driver)
        {
            var filterSection = driver.GetFilterSection();
            driver.ClickWithJs(filterSection.FindElement(By.XPath("//*[contains(text(),'Başvurduğum ilanları gösterme')]")));
        }

        /// <summary>
        /// Radio, Tekil Checkbox Benzeri Tekil Seçimler İçin Filtreleme
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="selectedText">Seçim Metni</param>
        /// <param name="dataTestValue"></param>
        /// <param name="collapseId"></param>
        public static void Filter(this IWebDriver driver, string selectedText, string dataTestValue = null, string collapseId = null)
        {
            var filterSection = driver.GetFilterSection(collapseId);
            var elements = filterSection.FindElements(By.XPath("//*[contains(text(),'" + selectedText + "')]"))
                  .Where(x => x.Text.ToLower(System.Globalization.CultureInfo.CreateSpecificCulture("tr")).Replace(" ", "") ==
                        selectedText.ToLower(System.Globalization.CultureInfo.CreateSpecificCulture("tr")).Replace(" ", ""));

            if (selectedText == null) return;
            foreach (var element in elements)
            {
                if (element.GetAttribute("data-test") != dataTestValue) continue;
                try
                {
                    driver.ClickWithJs(element);
                    driver.WaitPageLoad(20).Wait();
                    Thread.Sleep(1000);
                }
                catch
                {
                    // ignored
                }
            }
        }
        #endregion

        #endregion




        private static IWebElement GetFilterSection(this IWebDriver driver, string collapseId = null)
        {
            Thread.Sleep(3000);
            if (collapseId is null)
                return driver.FindElement(By.Id("filter-section"));

            var elements = driver.FindElements(By.ClassName("k-collapse"));
            foreach (var element in elements)
            {
                if (element.GetAttribute("collapse-id") == collapseId || element.GetAttribute("collapseid") == collapseId)
                    return element;
            }
            return driver.FindElement(By.Id("filter-section"));
        }
    }
}
