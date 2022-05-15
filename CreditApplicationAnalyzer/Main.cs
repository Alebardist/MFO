using CreditApplicationAnalyzer;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using System.Text;
using SharedLib.MongoDB.Implementations;
using CreditApplicationsAnalyzer.DTO;
using Newtonsoft.Json;

Console.WriteLine("[INFO] setting up config!");
var config = Startup.LoadConfiguration();
Console.WriteLine("[INFO] setting up logger!");
var logger = Startup.CreateSerilog();

Console.WriteLine("[INFO] ready!");

Ack();

void Ack()
{
    var factory = new ConnectionFactory() { HostName = config.GetSection("RabbitMQHost").Value };
    using (var connection = factory.CreateConnection())
    using (var channel = connection.CreateModel())
    {
        channel.QueueDeclare(queue: config.GetSection("QueueName").Value,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);
        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            Console.WriteLine($"[x] Received {message}");
            var obj = GetObjectFromJSONString(message);
            SendCreditApplicationToDB(obj);
        };
        channel.BasicConsume(queue: config.GetSection("QueueName").Value,
                             autoAck: true,
                             consumer: consumer);

        Console.WriteLine("Waiting for messages");
        Console.ReadLine();
    }
}

Console.WriteLine(" Press [any] to exit.");
Console.ReadLine();

CreditApplication GetObjectFromJSONString(string jsonString)
{
    CreditApplication result = new CreditApplication();

    try
    {
        result = JsonConvert.DeserializeObject<CreditApplication>(jsonString);
        if (result == null)
        {
            throw new NullReferenceException($"{nameof(result)} is null");
        }
    }
    catch (NullReferenceException ex)
    {
        logger.Warning(ex.Message);
    }
    catch (Exception ex)
    {
        logger.Error(ex.Message);
    }

    return result;
}

void SendCreditApplicationToDB(CreditApplication creditApplication)
{
    MongoDBAccessor<CreditApplication>.GetMongoCollection(config.GetSection("MongoDB:DBName").Value, 
                                                        config.GetSection("MongoDB:CollectionName").Value)
        .InsertOneAsync(creditApplication);
}