using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using RabbitMQ.Client;

namespace TopicExchangeProducer
{
    internal class Program
    {
        private static readonly List<string> cars = new List<string>() { "BMW", "Audi", "Tesla", "Mercedes" };
        private static readonly List<string> colors = new List<string>() { "red", "white", "black" };
        private static readonly Random random = new Random();

        static void Main(string[] args)
        {
            var counter = 0;

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
                        channel.ExchangeDeclare("topic-logs", ExchangeType.Topic);

                        var routingKey = counter % 4 == 0 ? "Tesla.red.fast.ecological" : counter % 5 == 0 ? "Mercedes.exclusive.expensive.ecological" : GenerateRoutingKey();

                        var message = $"Message type [{routingKey}] from publisher N {counter}";
                        var body = Encoding.UTF8.GetBytes(message);
                        channel.BasicPublish("topic-logs", routingKey, body: body);

                        Console.WriteLine($"Message type [{routingKey}] is sent to Topic Exchange {counter++}");
                    }
                }
            }
        }

        private static string GenerateRoutingKey()
        {
            return $"{cars[random.Next(0, 3)]}.{colors[random.Next(0, 2)]}";
        }
    }
}
