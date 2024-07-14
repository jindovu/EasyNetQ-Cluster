using EasyNetQ;
using Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var serviceCollection = new ServiceCollection();
serviceCollection.AddLogging(builder => builder
    .ClearProviders()
    .AddProvider(new CustomConsoleLoggerProvider()));

var rabbitMqHost = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "10.1.1.1";
var rabbitMqHostCluster = Environment.GetEnvironmentVariable("RABBITMQ_HOST") ?? "10.1.1.2";
var rabbitMqUser = Environment.GetEnvironmentVariable("RABBITMQ_USER") ?? "admin";
var rabbitMqPass = Environment.GetEnvironmentVariable("RABBITMQ_PASS") ?? "rabbitAdmin@1111";
var connectionString = $"host={rabbitMqHost}:5672,host={rabbitMqHostCluster}:5672;username={rabbitMqUser};password={rabbitMqPass}";

serviceCollection.AddEasyNetQ(connectionString).UseSystemTextJson();
using var provider = serviceCollection.BuildServiceProvider();

var loggerFactory = provider.GetRequiredService<ILoggerFactory>();
var logger = loggerFactory.CreateLogger<Program>();

logger.LogInformation("Starting subscriber application.");

IBus bus = provider.GetRequiredService<IBus>();
bus.PubSub.Subscribe<TextMessage>("test", HandleTextMessage);

void HandleTextMessage(TextMessage textMessage)
{
    Thread.Sleep(100);
    logger.LogInformation("Got message: {0}", textMessage.Text);
}


Console.ReadLine();

