namespace MFramework.Services.IdentityAuth.Migrations
{
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
