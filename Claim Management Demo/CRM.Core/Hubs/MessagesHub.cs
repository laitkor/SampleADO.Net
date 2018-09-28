using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using System.Configuration;
using Microsoft.AspNet.SignalR.Hubs;

namespace CRM.Core.Hubs
{
    public class MessagesHub : Hub
    {
        private static string conString = ConfigurationManager.ConnectionStrings["CRMConnectionString"].ToString();
        public void Hello()
        {

            Clients.All.hello();
        }
        [HubMethodName("sendMessages")]
        public static void SendMessages()
        {
            IHubContext context = GlobalHost.ConnectionManager.GetHubContext<MessagesHub>();
            context.Clients.All.updateMessages();
        }
    }
}
