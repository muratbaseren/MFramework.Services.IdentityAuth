namespace MFramework.Services.Config
{
    public static class ConfigServiceWrapper 
    {
        public static IConfigService ConfigService { get; set; } = new DefaultConfigService();
    }
}