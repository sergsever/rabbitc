using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

namespace rabbitc
{
    internal class Program
    {
        static private void Send(string message, IModel channel)
        {
            for (int i = 0; i < 5; i++)
            {
                Console.WriteLine("start send");
                byte[] body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "", routingKey: "test", body: body);
                Thread.Sleep(1000);
            }

        }

        private static string Message {  get; set; }
        static void Main(string[] args)
        {
            Console.WriteLine("rabbitmq");
            try
            {
                ConnectionFactory factory = new ConnectionFactory() { HostName = "rabbitmq", Port = 5672, UserName = "rabbit", Password = "rabbitMQ" };
                IConnection connection = factory.CreateConnection();
                IModel channel = connection.CreateModel();
                channel.QueueDeclare(queue: "test", durable: false);
                string message = "A test message";
                byte[] body = Encoding.UTF8.GetBytes(message);
                channel.BasicPublish(exchange: "", routingKey: "test", body: body);
                var consumer = new EventingBasicConsumer(channel);
                string fromcons = "";
                consumer.Received += (IModel, ea) =>
                {
                    var body = ea.Body.ToArray();
                    string message = Encoding.UTF8.GetString(body);
                    fromcons = message;
                    Console.WriteLine("received: " + message);
                };
                
                string messag = "test message";
                Thread sender = new Thread(() => Send(message, channel));
                channel.BasicConsume("", autoAck: true, consumer);

                sender.Start();
                sender.Join();
                Console.WriteLine("consumed: " + fromcons);
            } catch(Exception e)
            {
                Console.WriteLine("Ex: " + e.Message);
            }



        }
    }
}