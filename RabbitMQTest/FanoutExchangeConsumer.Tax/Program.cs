using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FanoutExchangeConsumer.Tax
{
    internal class Program
    {
        private static double _totalHold;

        static void Main(string[] args)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost"
            };

            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.ExchangeDeclare("notifier", ExchangeType.Fanout);
                var queueName = channel.QueueDeclare().QueueName;
                channel.QueueBind(queueName, "notifier", String.Empty);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (sender, e) =>
                {
                    var body = e.Body;
                    var message = Encoding.UTF8.GetString(body.ToArray());

                    var payment = GetPayment(message);
                    _totalHold += payment * 0.01;

                    Console.WriteLine($"Payment received for the amount of {payment}");
                    Console.WriteLine($"{_totalHold} held from this person");
                };

                channel.BasicConsume(queueName, true, consumer);

                Console.WriteLine($"Subscribed to the queue '{queueName}'");
                Console.WriteLine($"Listening");
                Console.ReadLine();
            }
        }

        private static int GetPayment(string message)
        {
            var messageStrs = message.Split(' ');
            if (int.TryParse(messageStrs[messageStrs.Length - 1], out var result))
            {
                return result;
            }

            return 0;
        }
    }
}
