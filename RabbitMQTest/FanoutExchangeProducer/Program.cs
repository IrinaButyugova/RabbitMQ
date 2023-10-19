using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace FanoutExchangeProducer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var random = new Random();

            while (true)
            {
                int timeToSleep = new Random().Next(1000, 3000);
                Thread.Sleep(timeToSleep);

                var factory = new ConnectionFactory()
                {
                    HostName = "localhost"
                };

                using (var connection = factory.CreateConnection())
                {
                    using (var channel = connection.CreateModel())
                    {
                        channel.ExchangeDeclare("notifier", ExchangeType.Fanout);

                        var moneyCount = random.Next(1000, 10000);
                        var message = $"Payment received for the amount of {moneyCount}";
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish("notifier", "", body: body);

                        Console.WriteLine($"Payment received for the amount of {moneyCount}.\n Notifying by 'notifier' Exchange");
                    }
                }
            }
        }
    }
}
