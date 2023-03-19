using Core.Signatures;
using System.Collections.Generic;

namespace Core.Configs.Kariyer
{
    public class KariyerConfig: IPrepareConfig
    {
        public IEnumerable<string> Countries { get; set; }
        public IEnumerable<string> Cities { get; set; }
        public IEnumerable<string> Counties { get; set; }
        public bool OnlyRemoteJobs { get; set; }
        public string Date { get; set; }
        public bool FirstTimePublished { get; set; }
        public bool JobsForYou { get; set; }
        public bool SavedJobs { get; set; }
        public bool FollowedCompaniesJobs { get; set; }
        public bool ViewedJobs { get; set; }
        public IEnumerable<string> Sectors { get; set; }
        public IEnumerable<string> PositionLevels { get; set; }
        public IEnumerable<string> Departments { get; set; }
        public IEnumerable<string> WorkingTypes { get; set; }
        public IEnumerable<string> EducationLevels { get; set; }
        public IEnumerable<string> Positions { get; set; }
        public IEnumerable<string> CompanyProperties { get; set; }
        public IEnumerable<string> JobLanguages { get; set; }
        public string ExpeirenceTime { get; set; }
        public string DisabilityJobs { get; set; }

        public IEnumerable<string> Words { get; set; }

    }
}
