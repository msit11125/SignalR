using System;
using System.Threading.Tasks;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(singnalrTest.Startup))]

namespace singnalrTest
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            //1 加入owin類別
            // 如需如何設定應用程式的詳細資訊，請參閱  http://go.microsoft.com/fwlink/?LinkID=316888
            app.MapSignalR();
            
        }
    }
}
