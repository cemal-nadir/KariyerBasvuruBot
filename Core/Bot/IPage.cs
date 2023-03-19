using Core.Signatures;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Core.Bot
{
    public interface IPage<in TConfig, in TApc>: IDisposable where TConfig:IPrepareConfig where TApc:IApplyJobConfig
    {
        public bool CheckIsLogged();
        public Task PreparePage(TConfig config);
        public Task NextPage();
        public Task<IEnumerable<string>> CheckJobsForMe(IEnumerable<string> jobsLinks);
        public Task ApplyJobs(IEnumerable<string> jobLinks, IEnumerable<TApc> applyJobConfigs);
        public Task<IEnumerable<string>> GetJobLinks();
        public Task<bool> CheckPageIsLast();

    }
}
