using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace signalrMVC
{
    [HubName("chatHub")]
    public class ChatHub : Hub
    {

        //宣告靜態類別，來儲存上線清單
        public static class UserHandler
        {
            public static Dictionary<string, string> ConnectedIds = new Dictionary<string, string>();
        }




        /// <summary>
        /// 某人加入聊天室
        /// </summary>
        /// <param name="name">使用者名稱</param>
        public void UserConnected(string name)
        {
            var id = Context.ConnectionId;

            name = HttpUtility.HtmlEncode(name); // ***重要*** 進行編碼，防止XSS攻擊
                                                 // 或是.net4.5以上的 AntiXssEncoder.HtmlEncode https://dotblogs.com.tw/mantou1201/2013/05/23/104729
                                                 //新增目前使用者至上線清單

            if (UserHandler.ConnectedIds.Count(x => x.Key == id) == 0 )
            {
                UserHandler.ConnectedIds.Add(id, name);

                string message = "<span style='color:blue;'>歡迎" + name + "加入聊天室!</span>";
                //發送訊息給除了自己的其他使用者 自己不會看到歡迎訊息(其他人才會觸發且看到)
                Clients.Others.hello(message); //自己上線的訊息新增到別人的對話框內
                Clients.Caller.hello("你加入了聊天室"); //自己上線顯示的上線訊息

                //顯示新增的人到上線清單內(其他人才會觸發且看到)
                Clients.Others.addList(id, name);
            }

            //初始化，自己取得所有的上線清單
            Clients.Caller.getList(UserHandler.ConnectedIds.Select(p => new { id = p.Key, name = p.Value }).ToList());
        }



        /// <summary>
        /// 發送訊息給所有人
        /// </summary>
        /// <param name="name">使用者名稱</param>
        /// <param name="message">訊息內容</param>
        public void Send(string message)
        {
            message = HttpUtility.HtmlEncode(message);
            var name = UserHandler.ConnectedIds.Where(p => p.Key == Context.ConnectionId).FirstOrDefault().Value; //用Context.ConnectionId(自己的ID)去找自己的名稱
            message = name + "說：" + message;
            Clients.All.sendAll(message);
        }

        /// <summary>
        /// 發送訊息至特定使用者
        /// </summary>
        /// <param name="ToId">傳送特定使用者的ConnectionId</param>
        /// <param name="message">訊息內容</param>
        public void SendToSomeOne(string ToId, string message)
        {
            var ContentMsg = HttpUtility.HtmlEncode(message);
            //---傳給別人看
            var name = UserHandler.ConnectedIds.Where(p => p.Key == Context.ConnectionId).FirstOrDefault().Value;//用Context.ConnectionId(自己的ID)去找自己的名稱
            message = $"<span style='color:#b158aa'>{name}悄悄的對你說：{ContentMsg}</span>";
            Clients.Client(ToId).sendToSomeOne(message); //傳給特定的使用者


            //---傳給自己看
            name = UserHandler.ConnectedIds.Where(p => p.Key == ToId).FirstOrDefault().Value;//用ToId去找被傳送地特定使用者的名稱
            message = $"<span style='color:#b158aa'>你對{name}悄悄的說：{ContentMsg}</span>";
            Clients.Client(Context.ConnectionId).sendToSomeOneMe(message); //傳給自己
        }



        //當使用者斷線時呼叫
        public override Task OnDisconnected(bool stopCalled)
        {
            if (stopCalled)
            {
                var item = UserHandler.ConnectedIds.FirstOrDefault(x => x.Key == Context.ConnectionId);

                Clients.Others.hello(
                    "<span style='color:red;'>" +
                    item.Value + 
                    " 離開了聊天室。</span>");

                UserHandler.ConnectedIds.Remove(item.Key);

                //當使用者離開時，移除在清單內的 ConnectionId
                Clients.All.removeList(item.Key);
            }

            return base.OnDisconnected(stopCalled);
        }


    }
}