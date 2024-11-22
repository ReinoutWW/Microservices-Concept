using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using PlatformService.DTO;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices
{
    public class MessageBusClient : IMessageBusClient
    {
        private readonly IConfiguration _configuration;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        public MessageBusClient(IConfiguration configuration)
        {
            _configuration = configuration;
            var factory = new ConnectionFactory() 
            {
                HostName = configuration["RabbitMQHost"],
                Port = int.Parse(configuration["RabbitMQPort"])
            };

            try 
            {   
                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);

                _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown;

                Console.WriteLine("-- Connected to Message Bus");

            } catch(Exception ex) 
            {
                Console.WriteLine($"Could not connect to the Message Bus: {ex.Message}");
            }
        }


        public void PublishNewPlatform(PlatformPublishedDTO platformPublishedDTO)
        {
            var message = JsonSerializer.Serialize(platformPublishedDTO);
            SendMessage(message);
        }

        private void SendMessage(string message)
        {
            if(_connection.IsOpen)
            {
                Console.WriteLine("-- RabbitMQ Connection Open, sending message..");
                PublishMessage(message);
            } else 
            {
                // RW: Not production ready. You'd like to have smart services and dumb message brokers.
                // Should handle this scenario correctly. :)
                Console.WriteLine("-- RabbitMQ Connection is closed, not sending.");
            }
        }

        private void PublishMessage(string message) 
        {
            var body = Encoding.UTF8.GetBytes(message);

            _channel.BasicPublish(
                exchange: "trigger", 
                routingKey: "", 
                basicProperties: null, 
                body: body
            );

            Console.WriteLine($"-- We have sent {message}");
        }

        public void Dispose()
        {
            Console.WriteLine("-- Message Bus Disposed");
            if(_channel.IsOpen)
            {
                _channel.Close();
                _connection.Close();
            }
        }

        private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs shutdownEventArgs) 
        {
            Console.WriteLine("-- Connection shutodwn");
        }
    }
}