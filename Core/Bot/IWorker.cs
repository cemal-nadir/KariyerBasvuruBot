using System.Collections.Generic;
using System.Threading.Tasks;
using Core.Signatures;

namespace Core.Bot
{
    public interface IWorker<TConfig,TApc> where TConfig: IPrepareConfig where TApc:IApplyJobConfig
    {
        public TConfig Config { get;set; }
        public IEnumerable<TApc> ApplyJobConfigs { get;set; }
        public string ApplicationStartPath { get; set; }
        public Task StartBot();
        public void StartBrowser();
        public Task StopBot();
        public bool CheckLogin();
    }
}
