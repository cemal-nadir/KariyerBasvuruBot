using Core.Configs.Kariyer;
using Newtonsoft.Json;
using System.Text;

namespace Bot.Helpers
{
    public static class MainHelper
    {
        public static string AppPath;
        public static void SaveKariyerConfigs(this KariyerConfig kariyerConfig)
        {
            var json = JsonConvert.SerializeObject(kariyerConfig, Formatting.Indented);
            File.WriteAllText(AppPath + "\\configs.json", json, Encoding.UTF8);
        }
        public static void SaveKariyerApplyConfigs(this IEnumerable<KariyerApplyJobConfig> kariyerapplyConfigs)
        {
            var json = JsonConvert.SerializeObject(kariyerapplyConfigs, Formatting.Indented);
            File.WriteAllText(AppPath + "\\applyconfigs.json", json, Encoding.UTF8);
        }
        public static KariyerConfig GetSavedKariyerConfig()
        {

            if (!File.Exists(AppPath + "\\configs.json"))
            {
                var kariyerConfig = new KariyerConfig();
                kariyerConfig.SaveKariyerConfigs();

            }

            using var r = new StreamReader(AppPath + "\\configs.json");
            var json = r.ReadToEnd();
            var model = JsonConvert.DeserializeObject<KariyerConfig>(json);
            return model;
        }
        public static IEnumerable<KariyerApplyJobConfig> GetSavedKariyerApplyConfig()
        {
            if (!File.Exists(AppPath + "\\applyconfigs.json"))
            {
                var kariyerApplyConfig = new List<KariyerApplyJobConfig>();
                kariyerApplyConfig.SaveKariyerApplyConfigs();

            }

            using var r = new StreamReader(AppPath + "\\applyconfigs.json");
            var json = r.ReadToEnd();
            var model = JsonConvert.DeserializeObject<IEnumerable<KariyerApplyJobConfig>>(json);
            return model;
        }
    }
}
