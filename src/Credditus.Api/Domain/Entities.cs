public class TransacaoService
{
    private readonly ITransacaoRepository _repository;

    public TransacaoService(ITransacaoRepository repository)
    {
        _repository = repository;
    }

    public async Task CriarTransacao()
    {
        var transacao = new Transacao()
        {
            Parte = "a1",
            ContraParte = "b4",
            Tipo = TipoTransacao.TransferenciaInterna
        };
        await _repository.Create(transacao);
    }
}
public interface ITransacaoRepository
{
    Task Create(Transacao transacao);
}
public class Transacao
{
    public string Parte { get; set; }
    public string ContraParte { get; set; }
    public TipoTransacao Tipo { get; set; }
}
public enum TipoTransacao
{
    TransferenciaInterna = 1
}
//GerarExtrato
//CriarConta
//CriarTransacao