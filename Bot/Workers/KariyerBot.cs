using System;
using System.Collections.Generic;
using System.Linq;
using Bot.Abstract;
using Browser.Base;
using Core.Configs.Kariyer;
using System.Threading;
using System.Threading.Tasks;
using Bot.Concrete;
using Core.Bot;

namespace Bot.Workers
{
    public class KariyerBot : IWorker<KariyerConfig, KariyerApplyJobConfig>
    {
        private readonly IBrowserBase _browserBase;
        private IKariyerPage _pageService;

        public KariyerBot()
        {
            _browserBase = new ChromeBase();
        }


        public IEnumerable<KariyerApplyJobConfig> ApplyJobConfigs { get; set; }

        public KariyerConfig Config { get; set; }
        public string ApplicationStartPath { get; set; }

        public void StartBrowser()
        {
            if (string.IsNullOrEmpty(ApplicationStartPath))
                throw new Exception($"{nameof(ApplicationStartPath)} can not be null");

            _pageService = new KariyerPageService(_browserBase.GenerateBrowser(ApplicationStartPath));
        }
        public async Task StartBot()
        {
            if (ApplyJobConfigs is null || !ApplyJobConfigs.Any())
                throw new Exception($"{nameof(ApplyJobConfigs)} can not be null");
            if (Config is null)
                throw new Exception($"{nameof(Config)} can not be null");


            await _pageService.PreparePage(Config);

            bool pageIsLast;
            do
            {
                Thread.Sleep(5000);

                var allJobs = await _pageService.GetJobLinks();

                var filteredJobs = await _pageService.CheckJobsForMe(allJobs);

                await _pageService.ApplyJobs(filteredJobs, ApplyJobConfigs);

                await _pageService.NextPage();

                pageIsLast = await _pageService.CheckPageIsLast();

            }
            while (!pageIsLast);


        }

        public async Task StopBot()
        {
            await Task.Run(() =>
            {
                _pageService.Dispose();

            });
        }

        public bool CheckLogin()
        {
            CheckFields();
            return _pageService.CheckIsLogged();
        }

        private void CheckFields()
        {
            if (string.IsNullOrEmpty(ApplicationStartPath))
                throw new Exception($"{nameof(ApplicationStartPath)} can not be null");
            if (ApplyJobConfigs is null || !ApplyJobConfigs.Any())
                throw new Exception($"{nameof(ApplyJobConfigs)} can not be null");
            if (Config is null)
                throw new Exception($"{nameof(Config)} can not be null");

        }

    }
}
