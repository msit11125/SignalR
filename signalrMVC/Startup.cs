using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;

[assembly:OwinStartup(typeof(signalrMVC.Startup))]
namespace signalrMVC
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = "ApplicationCookie",
                //這邊設定User在沒有登入的情況下會導到哪個Controller的Action裡面
                LoginPath = new PathString("/Home/Chat")
            });

            app.MapSignalR();
        }
    }
}