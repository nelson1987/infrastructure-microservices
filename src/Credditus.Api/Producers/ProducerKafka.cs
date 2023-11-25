using Confluent.Kafka;
using Newtonsoft.Json;

public class ProducerKafka
{
    public string SendMessageByKafka()
    {
        Address user = new() { Description = "Address", Name = "Email", FlagUri = "" };
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
