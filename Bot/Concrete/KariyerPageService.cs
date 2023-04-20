using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Bot.Abstract;
using Bot.Extensions;
using Browser.Helpers;
using Core.Configs;
using Core.Configs.Kariyer;
using Newtonsoft.Json;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;

namespace Bot.Concrete
{
    public class KariyerPageService : IKariyerPage
    {
        private readonly IWebDriver _webDriver;
        private KariyerConfig _kariyerConfig;
        private readonly HttpClient _httpClient;
        private readonly Actions _actions;
        public KariyerPageService(IWebDriver webDriver)
        {
            _webDriver = webDriver;
            _httpClient = new HttpClient();
            _actions = new Actions(_webDriver);
        }

        public Task ApplyJobs(IEnumerable<string> jobLinks, IEnumerable<KariyerApplyJobConfig>  applyJobConfigs)
        {
            return Task.Run(() =>
            {
                
                var jobApplyLinks = (from jobLink in jobLinks
                                     let jobId = jobLink.Split('-').Last()
                                     select UrlConfigs.KariyerApplyJobUrl + jobId).ToList();
                if (!jobApplyLinks.Any()) return;
                _webDriver.OpenNewTab();

                foreach (var jobApplyLink in jobApplyLinks)
                {
                    _webDriver.Navigate().GoToUrl(jobApplyLink);
                    _webDriver.CheckAndCloseNotification();
                    _webDriver.CheckAndCloseCookiePolicy();
                    var canJobApplify = true;
                    if (_webDriver.Url != jobApplyLink) continue;

                    #region Select - Option
                    var selectDivs = _webDriver.FindElements(By.CssSelector("div[class*='select with-label']"));
                    foreach (var selectDiv in selectDivs)
                    {

                        var selectId = selectDiv.FindElement(By.TagName("select")).GetAttribute("id");
                        var selectCaption = selectDiv.FindElements(By.TagName("label")).FirstOrDefault(x => x.Text != null && !string.IsNullOrEmpty(x.Text))?.Text;
                        var settedSelect = applyJobConfigs.FirstOrDefault(x => x.SelectId == selectId);
                        if (settedSelect is null)
                            throw new Exception($"1|{JsonConvert.SerializeObject(new KariyerApplyJobConfig() { SelectCaption = selectCaption, SelectId = selectId, OptionString = "" })}");
                        _actions.MoveToElement(selectDiv).Build().Perform();
                        selectDiv.FindElements(By.TagName("span")).FirstOrDefault(x => x.GetAttribute("class") == "select2-arrow")?.Click();
                        _webDriver.WaitPageLoad(20).Wait();

                        var options = _webDriver.FindElements(By.CssSelector(".select2-results li.select2-result-selectable"));


                        var settedClick = options.FirstOrDefault(x => x.Text == settedSelect.OptionString);
                        if (settedClick is null)
                        {
                            canJobApplify = false;
                            break;
                                 
                        }
                                   
                        _actions.MoveToElement(settedClick).Build().Perform();
                        settedClick.Click();
                        _webDriver.WaitPageLoad(20).Wait();
                    }
                    #endregion

                    if (!canJobApplify) continue;
                    {
                        #region CheckBoxes
                        var checkBoxes = _webDriver.FindElements(By.ClassName("icheckbox"));

                        foreach (var checkBox in checkBoxes)
                        {
                            _actions.MoveToElement(checkBox).Build().Perform();
                            checkBox.Click();
                        }
                        #endregion
                        #region Textarea

                        var textDivs = _webDriver.FindElements(By.ClassName("textarea"));

                        foreach (var textDiv in textDivs)
                        {

                            var textId = textDiv.FindElement(By.TagName("textarea")).GetAttribute("id");
                            var textCaption = textDiv.FindElement(By.TagName("label")).GetAttribute("title");
                            var settedSelect = applyJobConfigs.FirstOrDefault(x => x.SelectId == textId);
                            if (settedSelect is null)
                                throw new Exception($"1|{JsonConvert.SerializeObject(new KariyerApplyJobConfig() { SelectCaption = textCaption, SelectId = textId, OptionString = "" })}");
                            _actions.MoveToElement(textDiv).Build().Perform();
                            textDiv.Click();
                            _webDriver.WaitPageLoad(20).Wait();

                            var textArea = textDiv.FindElement(By.TagName("textarea"));



                            textArea.SendKeys(settedSelect.OptionString);
                            _webDriver.WaitPageLoad(20).Wait();
                        }

                        #endregion
                        var btnElement = _webDriver.FindElement(By.CssSelector("#btnBasvuruTamamla"));
                        btnElement.Click();
                    }

                }
                _webDriver.CloseCurrentTab();
                _webDriver.SwitchFirstTab();

            });

        }

        public async Task<IEnumerable<string>> CheckJobsForMe(IEnumerable<string> jobsLinks)
        {
            if (_kariyerConfig.Words is null) return jobsLinks;
            var referableJobs = new List<string>();
            foreach (var jobLink in jobsLinks)
            {
                var resp = await _httpClient.GetStringAsync(jobLink);
                var isReferable = false;
                var wait = new object();
                Parallel.ForEach(_kariyerConfig.Words, word =>
                {
                    if (!resp.ToLower(System.Globalization.CultureInfo.CreateSpecificCulture("tr"))
                            .Contains(word.ToLower(System.Globalization.CultureInfo.CreateSpecificCulture("tr"))))
                        return;
                    lock (wait)
                    {
                        isReferable = true;
                    }
                });
                if (isReferable)
                    referableJobs.Add(jobLink);
            }
            return referableJobs;
        }

        public Task<bool> CheckPageIsLast()
        {
            return Task.Run(() =>
            {
                try
                {
                    _webDriver.FindElement(By.CssSelector("#__layout > div > div.content-wrapper.with-banner > div.jobs-list-container > div.k-skeleton-joblist.mt-lg-0 > div.clean-container-padding.container > div:nth-child(2) > div.col > div.list-items-wrapper > div.ad-pagination.bg-white.d-md-block.d-none > div > ul > li.page-item.disabled.tiny-padding.custom-next"));
                    return true;
                }
                catch
                {
                    return false;
                }
            });

        }

        public async Task<IEnumerable<string>> GetJobLinks()
        {
            return await Task.Run(() =>
            {

                List<string> jobLinks = new();
                var jobDivs = _webDriver.FindElements(By.ClassName("list-items"));
                try
                {
                    jobLinks.AddRange(jobDivs.Select(jobDiv => jobDiv.FindElement(By.TagName("a")).GetAttribute("href")));
                }
                catch
                {
                    Thread.Sleep(5000);
                    jobLinks.Clear();
                    jobDivs = _webDriver.FindElements(By.ClassName("list-items"));
                    jobLinks.AddRange(jobDivs.Select(jobDiv => jobDiv.FindElement(By.TagName("a")).GetAttribute("href")));
                }
              
          
                return jobLinks;
            });

        }

        public async Task PreparePage(KariyerConfig config)
        {
            _kariyerConfig = config;
            await Task.Run(() =>
            {

                _webDriver.GoJobSearchScreen();

                _webDriver.Filter("Türkiye", "country-title");

                _webDriver.Filter(_kariyerConfig?.Countries, "dropdown-item");

                _webDriver.Filter("Şehir seçin", "city-title");

                _webDriver.Filter(_kariyerConfig?.Cities, "checkbox-item-value");

                _webDriver.Filter("close", "close-icon");

                _webDriver.Filter("İlçe seçin", "district-title");

                _webDriver.Filter(_kariyerConfig?.Counties, "checkbox-item-value");

                _webDriver.Filter("close", "close-icon");

                _webDriver.Filter(_kariyerConfig?.Date, collapseId: "Tarih");

                if (_kariyerConfig is { OnlyRemoteJobs: true })
                    _webDriver.FilterRemoteJobs();

                if (_kariyerConfig is { FirstTimePublished: true })
                    _webDriver.Filter("İlk kez yayınlananlar", "checkbox-item-value");

                if (_kariyerConfig is { JobsForYou: true })
                    _webDriver.Filter("Sana Uygun İlanlar", "checkbox-item-value");

                if (_kariyerConfig is { SavedJobs: true })
                    _webDriver.Filter("Kaydettiğin İlanlar", "checkbox-item-value");

                if (_kariyerConfig is { FollowedCompaniesJobs: true })
                    _webDriver.Filter("Takip Ettiğin Şirketin İlanları", "checkbox-item-value");

                if (_kariyerConfig is { ViewedJobs: true })
                    _webDriver.Filter("İncelediğin İlanlar", "checkbox-item-value");


                if (_kariyerConfig != null)
                {
                    _webDriver.FilterSearchable("Sektör ara", _kariyerConfig.Sectors, "checkbox-item-value",
                        "Şirketin Sektörü");

                    _webDriver.Filter(_kariyerConfig.PositionLevels, "checkbox-item-value", "Pozisyon Seviyesi");

                    _webDriver.FilterSearchable("Departman ara", _kariyerConfig.Departments, "checkbox-item-value",
                        "Departman");

                    _webDriver.Filter(_kariyerConfig.WorkingTypes, "checkbox-item-value", "Çalışma Şekli");

                    _webDriver.Filter(_kariyerConfig.EducationLevels, "checkbox-item-value", "Eğitim Seviyesi");

                    _webDriver.OpenCollapse("Pozisyon");
                    _webDriver.FilterSearchable("Pozisyon ara", _kariyerConfig.Positions, "checkbox-item-value",
                        "Pozisyon");

                    _webDriver.OpenCollapse("Şirketin Özellikleri");
                    _webDriver.Filter(_kariyerConfig.CompanyProperties, "checkbox-item-value", "Şirketin Özellikleri");

                    _webDriver.OpenCollapse("İlan Dili");
                    _webDriver.Filter(_kariyerConfig.JobLanguages, "checkbox-item-value", "İlan Dili");

                    _webDriver.OpenCollapse("Deneyim Süresi");
                    _webDriver.Filter(_kariyerConfig.ExpeirenceTime, "Deneyim Süresi");

                    _webDriver.OpenCollapse("Engelli İlanı");
                    _webDriver.Filter(_kariyerConfig.DisabilityJobs, collapseId: "Engelli İlanı");
                }

                _webDriver.Filter("Başvurduğum ilanları gösterme", "checkbox-item-value");

                _webDriver.ClickApplyFilterButton();

                _webDriver.WaitPageLoad(20).Wait();

                Thread.Sleep(2000);

            });
        }
        public Task NextPage()
        {
            return Task.Run(() =>
            {
                try
                {
                    _webDriver.ClickWithJs(_webDriver.FindElement(By.CssSelector("#__layout > div > div.content-wrapper.with-banner > div.jobs-list-container > div.k-skeleton-joblist.mt-lg-0 > div.clean-container-padding.container > div:nth-child(2) > div.col > div.list-items-wrapper > div.ad-pagination.bg-white.d-md-block.d-none > div > ul > li.page-item.tiny-padding.custom-next.pr-0 > button")));
                    _webDriver.ClickWithJs(_webDriver.FindElement(By.CssSelector("#__layout > div > div.content-wrapper.with-banner > div.jobs-list-container > div.k-skeleton-joblist.mt-lg-0 > div.clean-container-padding.container > div:nth-child(2) > div.col > div.list-items-wrapper > div.ad-pagination.bg-white.d-md-block.d-none > div > ul > li.page-item.tiny-padding.custom-next > button")));
                    _webDriver.WaitPageLoad(20).Wait();
                    Thread.Sleep(3000);
                }
                catch
                {
                    try
                    {
                        _webDriver.ClickWithJs(_webDriver.FindElement(By.CssSelector("#__layout > div > div.content-wrapper.with-banner > div.jobs-list-container > div.k-skeleton-joblist.mt-lg-0 > div.clean-container-padding.container > div:nth-child(2) > div.col > div.list-items-wrapper > div.ad-pagination.bg-white.d-md-block.d-none > div > ul > li.page-item.tiny-padding.custom-next > button")));
                        _webDriver.WaitPageLoad(20).Wait();
                        Thread.Sleep(3000);
                    }
                    catch
                    {
                        // ignored
                    }
                }

            });

        }
        public bool CheckIsLogged()
        {

            _webDriver.GoLoginPage();
            return _webDriver.CheckIsLogged();

        }

        public void Dispose()
        {
            _webDriver?.Close();
            _webDriver?.Quit();
            _webDriver?.Dispose();
            _httpClient?.Dispose();
          
        }
    }
}
