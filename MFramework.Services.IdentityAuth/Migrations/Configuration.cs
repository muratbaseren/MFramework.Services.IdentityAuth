namespace MFramework_Services_IdentityAuth.Migrations
{
    using MFramework.Services.IdentityAuth;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<IdentityContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(IdentityContext context)
        {
            IdentityContextInitializer.ApplyIdentityInitDataIsRequired(context);
        }
    }
}
