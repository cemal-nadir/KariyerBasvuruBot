using Core.Configs.Kariyer;
using Newtonsoft.Json;

namespace Core.Helpers
{
    public static class CustomException
    {
        public static KariyerApplyJobConfig ExtractKariyerApplyJobConfigFromException(string exceptionMessage)
        {
            var jsonText = exceptionMessage.Split('|')[1];
            return JsonConvert.DeserializeObject<KariyerApplyJobConfig>(jsonText);

        }
     

    }

}
