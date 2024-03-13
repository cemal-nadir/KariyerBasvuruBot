#nullable enable
using Bot.Abstract;
using Bot.Extensions;
using Bot.Helpers;
using Browser.Helpers;
using Core.Configs;
using Core.Configs.Kariyer;
using Core.Enums;
using Core.Helpers;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using Keys = OpenQA.Selenium.Keys;

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

        public async Task ApplyJobs(IEnumerable<string> jobLinks, IEnumerable<KariyerApplyJobConfig> applyJobConfigs)
        {
            await Task.Run(() =>
            {
                var applyJobConfigsList = applyJobConfigs.ToList();
                var jobApplyLinks = (from jobLink in jobLinks
                                     let jobId = jobLink.Split('-').Last()
                                     select UrlConfigs.KariyerApplyJobUrl + jobId).ToList();
                if (!jobApplyLinks.Any()) return;
                _webDriver.OpenNewTab();

                foreach (var jobApplyLink in jobApplyLinks)
                {
                    JobState? jobState = null;
                    KariyerApplyJobConfig? missingConfig = null;
                    var retryCount = 3;
                    do
                    {
                        if (jobState is JobState.NeedAnswer && missingConfig != null)
                        {

                            if (MessageBox.Show(
                                    missingConfig.Question +
                                    " - Alanı belirlenmemiş. Lütfen alanı belirleyin. Belirlemek için Evet\'e İlanı Atlamak İçin Hayır\'a Basın ",
                                    "Uyarı", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) is DialogResult.No)
                            {
                                break;
                            }

                            var value = string.Empty;
                            while (FormHelper.InputBox(missingConfig.Question + " alanını belirleyin", missingConfig.Answer, ref value) == DialogResult.Cancel)
                            {
                            }

                            missingConfig.Answer = value;
                            var currentConfig = MainHelper.GetSavedKariyerApplyConfig().ToList();
                            var config = missingConfig;

                            if (currentConfig.FirstOrDefault(x => x.Question == config.Question) != null)
                                currentConfig.RemoveAll(x => x.Question == config.Question);

                            currentConfig.Add(missingConfig);
                            currentConfig.SaveKariyerApplyConfigs();
                            applyJobConfigsList = currentConfig;
                        }
                        else if (jobState is JobState.Skipped) break;


                        var response = CheckMissingAndApply(applyJobConfigsList, jobApplyLink);
                        jobState = response.Item2;
                        missingConfig = response.Item1;
                        if (jobState is JobState.Retry)
                        {
                            retryCount--;
                        }
                        if (retryCount < 1)
                        {
                            jobState = JobState.Skipped;
                        }
                    } while (jobState is JobState.NeedAnswer or JobState.Retry);


                }
                _webDriver.CloseCurrentTab();
                _webDriver.SwitchFirstTab();
            });
        }

        private (KariyerApplyJobConfig?, JobState) CheckMissingAndApply(List<KariyerApplyJobConfig> currentConfigs,
            string jobLink)
        {
            try
            {
                _webDriver.Navigate().GoToUrl(jobLink);

                if (_webDriver.Url != jobLink) return (null, JobState.Skipped);

                var toggleCvButton = _webDriver.FindElement(By.CssSelector(
                    "#application > div > section.application-form.section.mb-0 > div:nth-child(1) > div > div > div > div"));
                toggleCvButton.Click();
                var resumeSelectButton = _webDriver.FindElement(By.ClassName("select-resume"));
                resumeSelectButton.Click();
                Thread.Sleep(500);
                var cvOptions = _webDriver.FindElement(By.ClassName("resume-list")).FindElements(By.TagName("li"))
                    ;
                var selectCv = cvOptions.FirstOrDefault(x =>
                    x.Text[..^11] == currentConfigs
                        .FirstOrDefault(y => y.Question == "cv")?.Answer);

                if (selectCv is null)
                    return (new KariyerApplyJobConfig()
                    {
                        Question = "cv",
                        Answer = "Kariyer.Netde yer alan cv adınızı eksiksiz girin"
                    }, JobState.NeedAnswer);

                selectCv.Click();
                var continueButton = _webDriver.FindElements(By.TagName("button")).FirstOrDefault(x => x.Text == "Devam Et");
                continueButton?.Click();
                var coverLetterBox = _webDriver.FindElement(By.ClassName("cover-letter-none-box"));
                coverLetterBox.Click();
                var coverLetterSelector = _webDriver.FindElement(By.ClassName("multiselect__select"));
                coverLetterSelector.Click();
                Thread.Sleep(500);
                var coverLetterOptions = _webDriver
                    .FindElements(By.ClassName("multiselect__element"));
                var selectCoverLetter = coverLetterOptions.FirstOrDefault(x =>
                    x.FindElement(By.TagName("span")).FindElement(By.TagName("span")).Text == currentConfigs
                        .FirstOrDefault(y => y.Question == "coverLetter")?.Answer);

                if (selectCoverLetter is null)
                    return (new KariyerApplyJobConfig()
                    {
                        Question = "coverLetter",
                        Answer = "Kariyer.Netde yer alan Ön Yazı adınızı eksiksiz girin"
                    }, JobState.NeedAnswer);


                selectCoverLetter?.Click();

                var input = _webDriver.FindElement(By.CssSelector("#name"));

                input.Click();
                input.SendKeys(Keys.Control + "a");
                input.SendKeys(currentConfigs
                    .FirstOrDefault(y => y.Question == "coverLetter")?.Answer);

                var selectButton = _webDriver.FindElements(By.TagName("button"))
                    .FirstOrDefault(x => x.Text is "Seç" or "Kaydet");
                _webDriver.ClickWithJs(selectButton);

                continueButton = _webDriver.FindElements(By.TagName("button")).FirstOrDefault(x => x.Text == "Devam Et");
                if (continueButton is null)
                {
                    var completeButtonFirst = _webDriver.FindElements(By.TagName("button")).FirstOrDefault(x => x.Text == "Başvurunu Tamamla");
                    completeButtonFirst?.Click();
                    return (null, JobState.Applied);
                }
                continueButton?.Click();


                var companyQuestions = _webDriver.FindElements(By.ClassName("company-question-item"));
                if (companyQuestions != null && companyQuestions.Any())
                    foreach (var companyQuestion in companyQuestions)
                    {
                        _webDriver.MoveElement(companyQuestion);

                        var questionName = companyQuestion.FindElement(By.ClassName("question-label"));
                        var answer = currentConfigs.FirstOrDefault(x => x.Question == questionName.Text);
                        if (answer is null)
                            return (new KariyerApplyJobConfig()
                            {
                                Question = questionName.Text,
                                Answer = "Bu birden fazla cevabın yer alabileceği bir select sorusuysa örneğin şehir seçiciler aralara boşluk koymadan şehirleri yazın örneğin: İstanbul,İzmir,Antalya"
                            }, JobState.NeedAnswer);

                        switch (GetQuestionType(companyQuestion))
                        {
                            case QuestionType.Text:
                                {
                                    var textInput = companyQuestion.FindElement(By.TagName("input"));
                                    _webDriver.MoveElement(textInput);

                                    textInput.Click();
                                    textInput.SendKeys(answer.Answer);
                                    break;
                                }

                            case QuestionType.Select:
                                {
                                    var selectElement = companyQuestion.FindElement(By.ClassName("knet-select"));
                                    _webDriver.MoveElement(selectElement);

                                    selectElement.Click();
                                    Thread.Sleep(1000);
                                    var options = selectElement.FindElements(By.ClassName("multiselect__element"));

                                    //Multi Answer
                                    if (answer.Answer.Contains(','))
                                    {
                                        var choices = answer.Answer.Split(',');
                                        var isFounded = false;
                                        foreach (var choice in choices)
                                        {
                                            var select = options.FirstOrDefault(x => x.Text.Trim() == choice);
                                            if (select is null) continue;
                                            _webDriver.MoveElement(select);

                                            select.Click();
                                            isFounded = true;
                                        }

                                        if (!isFounded)
                                            return (new KariyerApplyJobConfig()
                                            {
                                                Question = questionName.Text,
                                                Answer =
                                                    $"Virgül ile ayrılan bütün seçenekler denendi ama cevap seçim kutusunda yok cevaplarınızı gözden geçirin ve kaydedin. Eski cevap : {answer.Answer}"
                                            }, JobState.NeedAnswer);

                                    }
                                    else
                                    {
                                        var select = options.FirstOrDefault(x => x.Text.Trim() == answer.Answer);
                                        if (select is null) return (new KariyerApplyJobConfig()
                                        {
                                            Question = questionName.Text,
                                            Answer =
                                                $"Bütün seçenekler denendi ama cevap seçim kutusunda yok cevaplarınızı gözden geçirin ve kaydedin. Eski cevap : {answer.Answer}"
                                        }, JobState.NeedAnswer);

                                        _webDriver.MoveElement(select);

                                        select.Click();
                                    }

                                    break;
                                }
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                if (_webDriver.PageSource.Contains("consent-input"))
                {
                    var checkBoxes = _webDriver.FindElements(By.ClassName("consent-input"));
                    if (checkBoxes != null && checkBoxes.Any())
                        foreach (var checkbox in checkBoxes)
                        {
                            var innerCheckBoxes = checkbox.FindElements(By.TagName("input"))
                                .Where(x => x.GetAttribute("type") == "checkbox");
                            foreach (var innerCheckBox in innerCheckBoxes)
                            {
                                _webDriver.MoveElement(innerCheckBox);

                                _webDriver.ClickWithJs(innerCheckBox);
                            }
                        }
                }
                
                var completeButton = _webDriver.FindElements(By.TagName("button")).FirstOrDefault(x => x.Text == "Başvurunu Tamamla");
                _webDriver.MoveElement(completeButton);

                completeButton?.Click();

                WebDriverWait wait = new WebDriverWait(_webDriver, TimeSpan.FromSeconds(10));
                wait.Until(d => d.PageSource.Contains("Başvurunu ilettik!"));



                return (null, JobState.Applied);
            }
            catch (Exception ex)
            {
                return (null, JobState.Retry);
            }


        }

        private QuestionType GetQuestionType(IWebElement element)
        {
            try
            {
                var selectElement = element.FindElement(By.ClassName("knet-select"));
                if (selectElement != null) return QuestionType.Select;
            }
            catch { }

            try
            {
                if (element.FindElement(By.TagName("input")).GetAttribute("type") is "text")
                    return QuestionType.Text;
            }
            catch
            {
                throw new Exception("Question Type Unknown");
            }
            throw new Exception("Question Type Unknown");
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
                   var noResult= _webDriver.FindElement(By.ClassName("no-result"));
                   return noResult != null;
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

                if (_kariyerConfig is { OnSite: true })
                    _webDriver.FilterCheckBox("İş Yerinde");
                if (_kariyerConfig is { RemoteJob: true })
                    _webDriver.FilterCheckBox("Uzaktan / Remote");
                if (_kariyerConfig is { HybridJob: true })
                    _webDriver.FilterCheckBox("Hibrit");

                _webDriver.Filter(_kariyerConfig?.Date, collapseId: "Tarih");

                if (_kariyerConfig is { FirstTimePublished: true })
                    _webDriver.FilterCheckBox("İlk kez yayınlananlar");

                if (_kariyerConfig is { JobsForYou: true })
                    _webDriver.FilterCheckBox("Sana Uygun İlanlar");

                if (_kariyerConfig is { SavedJobs: true })
                    _webDriver.FilterCheckBox("Kaydettiğin İlanlar");

                if (_kariyerConfig is { FollowedCompaniesJobs: true })
                    _webDriver.FilterCheckBox("Takip Ettiğin Şirketin İlanları");

                if (_kariyerConfig is { ViewedJobs: true })
                    _webDriver.FilterCheckBox("İncelediğin İlanlar");


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
            });
        }
        public Task NextPage()
        {
            return Task.Run(() =>
            {
                var current = _webDriver.Url;
                if (current is null) throw new Exception();
                if (current.Contains("&cp"))
                {
                    var number = current.Substring(current.IndexOf("&cp=", StringComparison.Ordinal) + 4);
                    number = number.Substring(0, number.IndexOf('&'));

                    var numericNumber = int.Parse(number);
                    current = current.Replace($"&cp={numericNumber}", $"&cp={numericNumber + 1}");
                }
                else
                {
                    current += $"&cp=2";
                }
                _webDriver.Navigate().GoToUrl(current);
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
