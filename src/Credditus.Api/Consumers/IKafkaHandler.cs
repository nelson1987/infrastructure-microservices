public interface IKafkaHandler<Tk, Tv>
{
    Task Handle(Tk key, Tv value);
}
