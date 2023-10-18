using RabbitMQ.Client;
using System;
using System.Text;

namespace DefaultExchangeProducer
{
    class Program 
    {
        static void Main(string[] args)
        {
            var counter = 0;

            while(true)
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
                        channel.QueueDeclare("dev-queue", false, false, false, null);

                        var message = $"Message from publisher N {counter}";
                        var body = Encoding.UTF8.GetBytes(message);

                        channel.BasicPublish("", "dev-queue", body: body);

                        Console.WriteLine($"Message is sent to default Exchange {counter++}");
                    }
                }
            }
        }
    }
}
