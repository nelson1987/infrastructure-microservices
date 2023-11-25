﻿using Confluent.Kafka;
using Microsoft.Extensions.Options;

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
