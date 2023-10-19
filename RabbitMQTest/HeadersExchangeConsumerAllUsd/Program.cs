using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeadersExchangeConsumerAllUsd
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var counter = 0;

            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("headers-exchange", ExchangeType.Headers);
                var headers = new Dictionary<string, object>();
                headers.Add("currencu", "USD");

                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queueName, "headers-exchange", String.Empty, headers);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, e) =>
                {
                    var body = e.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());

                    counter++;

                    Console.WriteLine($"Received message: {message}");
                    Console.WriteLine($"Total USD transefers: {counter}");
                };

                channel.BasicConsume(queueName, true, consumer);

                Console.WriteLine($"Subscribed to the queue '{queueName}'");
                Console.WriteLine($"Counts all USD transfers");
                Console.ReadLine();
            }
        }
    }
}
