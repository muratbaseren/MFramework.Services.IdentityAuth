﻿using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(MFramework.Services.IdentityAuth.Startup))]
namespace MFramework.Services.IdentityAuth
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
