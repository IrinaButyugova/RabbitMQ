using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace HeadersExchangeProducer
{
    internal class Program
    {
        private static int _count = 0;
        static void Main(string[] args)
        {

            var usdAbroadHeaders = new Dictionary<string, object>();
            usdAbroadHeaders.Add("currencu", "USD");
            usdAbroadHeaders.Add("transfer", "Abroad");
            Task.Run(CreateTask(12000, usdAbroadHeaders));

            var usdInternalHeaders = new Dictionary<string, object>();
            usdInternalHeaders.Add("currencu", "USD");
            usdInternalHeaders.Add("transfer", "Internal");
            Task.Run(CreateTask(10000, usdInternalHeaders));

            var eurInternalHeaders = new Dictionary<string, object>();
            eurInternalHeaders.Add("currencu", "EUR");
            eurInternalHeaders.Add("transfer", "Internal");
            Task.Run(CreateTask(8000, eurInternalHeaders));

            Console.ReadKey();
        }

        static Func<Task> CreateTask(int timeToSleepTo, Dictionary<string, object> headers)
        {
            return () =>
            {
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
                            channel.ExchangeDeclare("headers-exchange", ExchangeType.Headers);
                            var properties = channel.CreateBasicProperties();
                            properties.Headers = headers;

                            var message = $"{_count} sent with headers [currencu: {headers["currencu"]}] [transfer: {headers["transfer"]}]";
                            var body = Encoding.UTF8.GetBytes(message);

                            channel.BasicPublish("headers-exchange", String.Empty, properties, body);

                            _count = Interlocked.Increment(ref _count);
                            Console.WriteLine($"{_count} {headers["currencu"]} transfer from {headers["transfer"]}");
                        }
                    }
                }
            };
        }
    }
}
