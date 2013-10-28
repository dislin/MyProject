using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Messaging;

namespace MvcTest.Controllers
{
    public class TestMsmqController : Controller
    {
        //
        // GET: /TestMsmq/

        public ActionResult testmsmq1()
        {
            ViewBag.Msg = ReceiveMessage();
            return View(ViewBag);
        }

        public ActionResult TestSendMsg1()
        {
            string msg = "this is a test at " + DateTime.Now.ToString();
            SendMessage(msg);
            return View();
        }

        private const string msg_queue = @".\Private$\mytest_queue";
        private MessageQueue _queue;
        private void SendMessage(string msg) {
            _queue = new MessageQueue(msg_queue);
            Message message = new Message();
            message.Body = msg;
            message.Label = "LeoMsmq at " + DateTime.Now.ToString();
            _queue.Send(message);
        }
        private string ReceiveMessage(){
            try {
                MessageQueue queue = new MessageQueue(msg_queue);
                if (!IsEmpty(queue))
                {
                    Message message = queue.Receive();
                    message.Formatter = new XmlMessageFormatter(new String[] { "System.String,mscorlib" });
                    return message.Body.ToString();
                }
                else
                {
                    return "No Message";
                }
            }
            catch {
                return "No Message";
            }
        }
        private bool IsEmpty(MessageQueue queue) {
            bool isempty = true;
            try {
                Message message = queue.Peek(new TimeSpan(0));
                isempty = false;
            }
            catch {
                isempty = true;
            }
            return isempty;
        }
    }
}
