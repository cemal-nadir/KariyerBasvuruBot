using Core.Configs.Kariyer;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace App.Helpers
{
    public static class MainHelper
    {
        public static void SaveKariyerConfigs(this KariyerConfig kariyerConfig)
        {
            var json = JsonConvert.SerializeObject(kariyerConfig, Formatting.Indented);
            File.WriteAllText(Application.StartupPath + "\\configs.json", json, Encoding.UTF8);
        }
        public static void SaveKariyerApplyConfigs(this IEnumerable<KariyerApplyJobConfig> kariyerapplyConfigs)
        {
            var json = JsonConvert.SerializeObject(kariyerapplyConfigs, Formatting.Indented);
            File.WriteAllText(Application.StartupPath + "\\applyconfigs.json", json, Encoding.UTF8);
        }
        public static KariyerConfig GetSavedKariyerConfig()
        {

            if (!File.Exists(Application.StartupPath + "\\configs.json"))
            {
                var kariyerConfig = new KariyerConfig();
                kariyerConfig.SaveKariyerConfigs();

            }

            using var r = new StreamReader(Application.StartupPath + "\\configs.json");
            var json = r.ReadToEnd();
            var model = JsonConvert.DeserializeObject<KariyerConfig>(json);
            return model;
        }
        public static IEnumerable<KariyerApplyJobConfig> GetSavedKariyerApplyConfig()
        {
            if (!File.Exists(Application.StartupPath + "\\applyconfigs.json"))
            {
                var kariyerApplyConfig = new List<KariyerApplyJobConfig>();
                kariyerApplyConfig.SaveKariyerApplyConfigs();

            }

            using var r = new StreamReader(Application.StartupPath + "\\applyconfigs.json");
            var json = r.ReadToEnd();
            var model = JsonConvert.DeserializeObject<IEnumerable<KariyerApplyJobConfig>>(json);
            return model;
        }
    }
}
