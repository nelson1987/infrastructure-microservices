public class AddressCreatedHandler : IKafkaHandler<string, Address>
{
    public async Task Handle(string key, Address value)
    {
        Console.WriteLine($"Key: {key}, Value: {value.Name}");
    }
}
