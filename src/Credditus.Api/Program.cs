using AspNetCore8MinimalApis.Endpoints;
using Confluent.Kafka;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddKafkaConsumer<string, User, UserCreatedHandler>(p =>
{
    p.Topic = "teste";
    p.GroupId = "teste_group";
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

app.MapGroup("/countries").GroupCountries();

app.MapPost("/Addresses", ([FromBody] Address address) =>
{
    return Results.Ok();
});
app.MapPut("/Addresses/{addressId}", ([FromRoute] int addressId, [FromForm] Address address) =>
{
    return Results.NoContent();
});
app.MapGet("/provinces/{provinceId:int}",
    (int provinceId) =>
    $"ProvinceId {provinceId}");

app
.MapPost("/producer", (string msg) =>
{
    var kafka = new ProducerKafka();
    return kafka.SendMessageByKafka();
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
    public string SendMessageByKafka()
    {
        User user = new() { Address = "Address", Email = "Email", FirstName = "FirstName", LastName = "LastName", Password = "Password" };
        string message = JsonConvert.SerializeObject(user);

        var config = new ProducerConfig { BootstrapServers = "localhost:9092" };

        using (var producer = new ProducerBuilder<Null, string>(config).Build())
        {
            try
            {
                var sendResult = producer
                                    .ProduceAsync("teste", new Message<Null, string> { Value = message.ToString() })
                                        .GetAwaiter()
                                            .GetResult();

                return $"Mensagem '{sendResult.Value}' de '{sendResult.TopicPartitionOffset}' on Partition: {sendResult.Partition} with Offset: {sendResult.Offset}";
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
    public void Handle(string key, User value)
    {
        Console.WriteLine($"Key: {key}, Value: {value.Email}");
    }

    Task IKafkaHandler<string, User>.Handle(string key, User value)
    {
        throw new NotImplementedException();
    }
}

public class User
{
    public Guid Id { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Address { get; set; }
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
    public required string Topic { get; set; }
    public KafkaConsumerConfig()
    {
        AutoOffsetReset = Confluent.Kafka.AutoOffsetReset.Earliest;
        EnableAutoOffsetStore = false;
    }
}
public interface IKafkaHandler<Tk, Tv>
{
    Task Handle(Tk key, Tv value);
}
internal sealed class KafkaDeserializer<T> : IDeserializer<T>
{
    public T Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        if (typeof(T) == typeof(Null))
        {
            return data.Length <= 0
                ? default!
                : throw new ArgumentException("The data is null not null.");
        }

        if (typeof(T) == typeof(Ignore))
            return default!;

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
                        await _handler.Handle(result.Message.Key, result.Message.Value);

                        consumer.Commit(result);

                        consumer.StoreOffset(result);
                    }
                }
            }
        }
    }
}
public class Address
{
    [FromRoute]
    public int AddressId { get; set; }
    [FromForm]
    public int StreetNumber { get; set; }
    [FromForm]
    public required string StreetName { get; set; }
    [FromForm]
    public required string StreetType { get; set; }
    [FromForm]
    public required string City { get; set; }
    [FromForm]
    public required string Country { get; set; }
    [FromForm]
    public int PostalCode { get; set; }
};