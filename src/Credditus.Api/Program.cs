using System;
using System.Threading;
using System.Threading.Tasks;
using System.Text;
using Newtonsoft.Json;
using Confluent.Kafka;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddKafkaConsumer<string, User, UserCreatedHandler>(p =>
{
    p.Topic = "users";
    p.GroupId = "users_group";
    p.BootstrapServers = "localhost:9092";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app
.MapPost("/producer", (string msg) =>
{
    var kafka = new ProducerKafka();
    return kafka.SendMessageByKafka(msg);
})
.WithName("Producer")
.WithOpenApi();

app
.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

public class ProducerKafka
{
    public string SendMessageByKafka(string message)
    {
        var config = new ProducerConfig { BootstrapServers = "localhost:9092" };

        using (var producer = new ProducerBuilder<Null, string>(config).Build())
        {
            try
            {
                var sendResult = producer
                                    .ProduceAsync("teste", new Message<Null, string> { Value = message })
                                        .GetAwaiter()
                                            .GetResult();

                return $"Mensagem '{sendResult.Value}' de '{sendResult.TopicPartitionOffset}'";
            }
            catch (ProduceException<Null, string> e)
            {
                Console.WriteLine($"Delivery failed: {e.Error.Reason}");
            }
        }

        return string.Empty;
    }
}


record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}



public class UserCreatedHandler : IKafkaHandler<string, User>
{
    public async Task HandleAsync(string key, User value)
    {
        Console.WriteLine($"Key: {key}, Value: {value}");
    }
}
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Address { get; set; }
}
public static class RegisterServiceExtensions
{
    public static IServiceCollection AddKafkaConsumer<Tk, Tv, THandler>(this IServiceCollection services,
        Action<KafkaConsumerConfig<Tk, Tv>> configAction) where THandler : class, IKafkaHandler<Tk, Tv>
    {
        services.AddScoped<IKafkaHandler<Tk, Tv>, THandler>();

        services.AddHostedService<BackGroundKafkaConsumer<Tk, Tv>>();

        services.Configure(configAction);

        return services;
    }
}
public class KafkaConsumerConfig<Tk, Tv> : ConsumerConfig
{
    public string Topic { get; set; }
    public KafkaConsumerConfig()
    {
        AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
        EnableAutoOffsetStore = false;
    }
}
public interface IKafkaHandler<Tk, Tv>
{
    Task HandleAsync(Tk key, Tv value);
}
internal sealed class KafkaDeserializer<T> : IDeserializer<T>
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (typeof(T) == typeof(Null))
        {
            if (data.Length > 0)
                throw new ArgumentException("The data is null not null.");
            return default;
        }

        if (typeof(T) == typeof(Ignore))
            return default;

        var dataJson = Encoding.UTF8.GetString(data);

        return JsonConvert.DeserializeObject<T>(dataJson);
    }
}
public class BackGroundKafkaConsumer<TK, TV> : BackgroundService
{
    private readonly KafkaConsumerConfig<TK, TV> _config;
    private IKafkaHandler<TK, TV> _handler;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public BackGroundKafkaConsumer(IOptions<KafkaConsumerConfig<TK, TV>> config,
        IServiceScopeFactory serviceScopeFactory)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _config = config.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using (var scope = _serviceScopeFactory.CreateScope())
        {
            _handler = scope.ServiceProvider.GetRequiredService<IKafkaHandler<TK, TV>>();

            var builder = new ConsumerBuilder<TK, TV>(_config).SetValueDeserializer(new KafkaDeserializer<TV>());

            using (IConsumer<TK, TV> consumer = builder.Build())
            {
                consumer.Subscribe(_config.Topic);

                while (!stoppingToken.IsCancellationRequested)
                {
                    var result = consumer.Consume(TimeSpan.FromMilliseconds(1000));

                    if (result != null)
                    {
                        await _handler.HandleAsync(result.Message.Key, result.Message.Value);

                        consumer.Commit(result);

                        consumer.StoreOffset(result);
                    }
                }
            }
        }
    }
}