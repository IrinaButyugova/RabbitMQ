using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TopicExchangeConsumerRed
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            using (var connection = factory.CreateConnection())

            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("topic-logs", ExchangeType.Topic);
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queueName, "topic-logs", "*.red.#");

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, e) =>
                {
                    var body = e.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());
                    Console.WriteLine($"Receive message: {message}");
                };

                channel.BasicConsume(queueName, true, consumer);

                Console.WriteLine($"Subscribed to the queue {queueName}");
                Console.WriteLine($"Subscribed to [*.red.#]");
                Console.ReadLine();
            }
        }
    }
}
