using Core.Signatures;

namespace Core.Configs.Kariyer
{
    public class KariyerApplyJobConfig:IApplyJobConfig
    {
        public string SelectId { get; set; }
        public string SelectCaption { get; set; }
        public string OptionString { get; set; }
    }
}
