using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DirectExchangeProducer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Task.Run(CreateTask(12000, "error"));
            Task.Run(CreateTask(10000, "info"));
            Task.Run(CreateTask(8000, "warning"));

            Console.ReadKey();
        }

        static Func<Task> CreateTask(int timeToSleepTo, string routingKey)
        {
            return () =>
            {
                var counter = 0;

                while (true)
                {
                    int timeToSleep = new Random().Next(1000, timeToSleepTo);
                    Thread.Sleep(timeToSleep);

                    var factory = new ConnectionFactory()
                    {
                        HostName = "localhost"
                    };

                    using (var connection = factory.CreateConnection())
                    {
                        using (var channel = connection.CreateModel())
                        {
                            channel.ExchangeDeclare("direct-logs", ExchangeType.Direct);

                            var message = $"Message type [{routingKey}] from publisher N {counter}";
                            var body = Encoding.UTF8.GetBytes(message);

                            channel.BasicPublish("direct-logs", routingKey, body: body);

                            Console.WriteLine($"Message type [{routingKey}] is sent to Direct Exchange {counter++}");
                        }
                    }
                }
            };
        }
    }
}
