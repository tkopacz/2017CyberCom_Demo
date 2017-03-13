using Microsoft.Azure.IoT.Gateway;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using uPLibrary.Networking.M2Mqtt;
using uPLibrary.Networking.M2Mqtt.Messages;

namespace ModuleMQTTReader
{
    public class TKMessage
    {
        public DateTime dt { get; set; }
        public string macAddress { get; set; }
        public double val1 { get; set; }
        public double val2 { get; set; }
    }
    public class MQTTReader : IGatewayModule, IGatewayModuleStart
    {
        Broker m_broker;
        private MqttClient m_mqtt;

        public void Create(Broker broker, byte[] configuration)
        {
            Console.WriteLine("CREATE");
            m_broker = broker;
        }

        public void Destroy()
        {
        }

        public void Receive(Message received_message)
        {
        }

        public void Start()
        {
            Console.WriteLine("Starting");
            m_mqtt = new MqttClient("localhost");
            m_mqtt.MqttMsgPublishReceived += M_mqtt_MqttMsgPublishReceived;
            m_mqtt.Subscribe(new string[] { "lora/events/#" }, new byte[] { MqttMsgBase.QOS_LEVEL_AT_MOST_ONCE });
            m_mqtt.Connect(Guid.NewGuid().ToString());
            if (!m_mqtt.IsConnected) throw new ArgumentException("No MQTT?");
            Console.WriteLine("Started");

        }

        private void M_mqtt_MqttMsgPublishReceived(object sender, uPLibrary.Networking.M2Mqtt.Messages.MqttMsgPublishEventArgs e)
        {
            var msg = JsonConvert.DeserializeObject<TKMessage>(Encoding.UTF8.GetString(e.Message));
            Dictionary<string, string> msgProp = new Dictionary<string, string>();

            msgProp.Add("source", "tktelemetry");
            msgProp.Add("macAddress", msg.macAddress);

            Message messageToPublish = new Message(e.Message, msgProp);
            this.m_broker.Publish(messageToPublish);

        }
    }
}
