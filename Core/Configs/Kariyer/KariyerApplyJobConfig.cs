using Core.Signatures;

namespace Core.Configs.Kariyer
{
    public class KariyerApplyJobConfig:IApplyJobConfig
    {
        public string Question { get; set; }
        public string Answer { get; set; }
    }
}
