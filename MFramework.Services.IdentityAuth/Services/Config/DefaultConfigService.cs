using System;
using System.Configuration;

namespace MFramework.Services.Config
{
    public class DefaultConfigService : IConfigService
    {
        public T GetAppSettingsValue<T>(string key)
        {
            string strValue = ConfigurationManager.AppSettings[key];
            return (T)Convert.ChangeType(strValue, typeof(T));
        }

        public string GetConnectionString(string name)
        {
            return ConfigurationManager.ConnectionStrings[name]?.ConnectionString ?? string.Empty;
        }
    }
}
