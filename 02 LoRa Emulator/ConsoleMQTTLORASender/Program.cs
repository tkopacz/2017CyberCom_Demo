using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace ConsoleMQTTLORASender
{
    public class TKMessage
    {
        public DateTime dt { get; set; }
        public string macAddress { get; set; }
        public double val1 { get; set; }
        public double val2 { get; set; }
    }
    class Program
    {
        static void Main(string[] args)
        {
            int cnt=0;
            MqttClient client = new MqttClient("localhost");
            client.MqttMsgPublished += Client_MqttMsgPublished;
            client.Connect(Guid.NewGuid().ToString());
            if (!client.IsConnected) Console.WriteLine("Not Connected!");
            string[] mac = new string[] { "01:02:03:04:05:06", "01:02:03:04:05:07", "01:02:03:04:05:08" };
            Random rnd = new Random();
            while (true)
            {
                TKMessage msg = new TKMessage {
                    dt = DateTime.Now,
                    val1 = rnd.NextDouble(),
                    val2 = rnd.NextDouble(),
                    macAddress = mac[rnd.Next(mac.Length)]
                };
                client.Publish("lora/events",Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(msg)),MqttMsgBase.QOS_LEVEL_AT_LEAST_ONCE, false);
                Thread.Sleep(100);
                if (cnt++ % 50 == 0) Console.WriteLine(cnt);
                if (cnt > 200) break;
            }
            Console.WriteLine("End");
            Console.ReadLine();
        }

        private static void Client_MqttMsgPublished(object sender, MqttMsgPublishedEventArgs e)
        {
            //Internal confirmation
        }
    }
}
