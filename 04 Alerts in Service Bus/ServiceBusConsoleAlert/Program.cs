using Microsoft.ServiceBus.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ServiceBusConsoleAlert
{
    class Program
    {
        //Create topic importantiotmessages and subscription all (with default rule - 1=1)
        static SubscriptionClient m_clientAll = SubscriptionClient.Create("importantiotmessages", "all");
        static void Main(string[] args)
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken ct = cts.Token;
            Task.Run(() => processAll(ct));
            Console.WriteLine("Enter = End");
            Console.ReadLine();
            cts.Cancel(); //Cancel background processing
        }
        static async void processAll(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                var msg = await m_clientAll.ReceiveAsync(TimeSpan.FromSeconds(5));
                if (msg != null)
                {
                    Console.WriteLine($"ALERT: {msg.MessageId}");
                    await m_clientAll.CompleteAsync(msg.LockToken); //Yes, we processed "ALERT"
                }
            }
        }

    }
}
