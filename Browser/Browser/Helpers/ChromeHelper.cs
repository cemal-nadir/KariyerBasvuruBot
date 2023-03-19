using System;
using System.IO;
using System.Linq;

namespace Browser.Helpers
{
    public abstract class ChromeHelper
    {
        public static string FindReleaseService(string applicationStartupPath)
        {
            try
            {
              
                var directories = new DirectoryInfo(applicationStartupPath + "Chrome").EnumerateDirectories()
                    .OrderByDescending(d => d.CreationTime)
                    .Select(d => d.Name)
                    .ToList();
                foreach (var serviceLocation in directories.Select(directory => Directory.GetDirectories(applicationStartupPath + "Chrome\\"+directory).FirstOrDefault()).Where(serviceLocation => serviceLocation != null))
                {
                    return serviceLocation;
                }

                throw new Exception("Servis Bulunamadı");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

        }
    }
}
