public class AddressCreatedHandler : IKafkaHandler<string, Address>
{
    public void Handle(string key, Address value)
    {
        if (string.IsNullOrWhiteSpace(key)) throw new ArgumentNullException(nameof(key));
        Console.WriteLine($"Key: {key}, Value: {value.Name}");
    }

    Task IKafkaHandler<string, Address>.Handle(string key, Address value)
    {
        throw new NotImplementedException();
    }
}
