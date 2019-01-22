namespace MFramework.Services.Config
{
    public interface IConfigService
    {
        T GetAppSettingsValue<T>(string key);
        string GetConnectionString(string name);
    }
}
