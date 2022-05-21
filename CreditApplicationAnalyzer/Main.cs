using CreditApplicationAnalyzer;

using Newtonsoft.Json;

using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using SharedLib.DTO.Application;
using SharedLib.MongoDB.Implementations;

using System.Text;

Console.WriteLine("[INFO] setting up config!");
var config = Startup.LoadConfiguration();
Console.WriteLine("[INFO] setting up logger!");
var logger = Startup.CreateSerilog();

Console.WriteLine("[INFO] ready!");

Main();

void Main()
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
        consumer.Received += ProcessMessage;

        channel.BasicConsume(queue: config.GetSection("QueueName").Value,
                             autoAck: true,
                             consumer: consumer);

        Console.WriteLine("Waiting for messages");
        Console.WriteLine(" Press [any] to stop the service.");
        Console.ReadKey();
    }

    Console.WriteLine(" Press [any] to exit.");
    Console.ReadLine();
}

void ProcessMessage(object? model, BasicDeliverEventArgs eventArgs)
{
    var body = eventArgs.Body.ToArray();
    var message = Encoding.UTF8.GetString(body);
    Console.WriteLine($"[x] Received {message}");
    var obj = GetObjectFromJSONString(message);
    SendCreditApplicationToDB(obj);
}

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
        logger.Warning($"Message {ex.Message}, \n {ex.StackTrace}");
    }
    catch (Exception ex)
    {
        logger.Fatal($"Message {ex.Message}, \n {ex.StackTrace}");
    }

    return result;
}

void SendCreditApplicationToDB(CreditApplication creditApplication)
{
    try
    {
        MongoDBAccessor<CreditApplication>.GetMongoCollection(config.GetSection("MongoDB:DBName").Value,
                                                        config.GetSection("MongoDB:CollectionName").Value)
        .InsertOneAsync(creditApplication);
    }
    catch (Exception e)
    {
        logger.Fatal(e, $"Ex while writing to DB");
    }
}