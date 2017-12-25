using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace singnalrTest
{
    [HubName("HitCountHub")]
    public class HitCountHub : Hub
    {
        static int _hitCount = 0;
        public void RecordHit()
        {
            _hitCount += 1;
            //onRecordHit 是一個動態的Method，可自行命名
            Clients.All.onRecordHit(_hitCount);
        }
        public override Task OnDisconnected(bool stopCalled)
        {
            //5 Client 斷線後將 Count的數字減1並且通知人
            _hitCount -= 1;
            Clients.All.onRecordHit(_hitCount);
            return base.OnDisconnected(stopCalled);
        }
    }
}