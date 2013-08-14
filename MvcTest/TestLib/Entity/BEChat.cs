using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SignalR;
using SignalR.Hubs;

namespace TestLib.Entity
{
    [HubName("LeoHub")]
    public class BEChat : Hub
    {
        public void sendMessage(string msg)
        {
            Clients.addMessage(msg);
        }
    }
}
