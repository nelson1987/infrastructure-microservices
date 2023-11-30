public class TransacaoService
{
    private readonly ITransacaoRepository _repository;

    public TransacaoService(ITransacaoRepository repository)
    {
        _repository = repository;
    }

    public async Task CriarTransacao(string debitante, string creditante, TipoTransacao tipo)
    {
        debitante = "a1";
        creditante = "b4";
        tipo = TipoTransacao.TransferenciaInterna;
        var transacaoDebito = new Transacao()
        {
            Parte = debitante,
            ContraParte = creditante,
            Tipo = tipo,
            Debito = true
        };
        var transacaoCredito = new Transacao()
        {
            Parte = creditante,
            ContraParte = debitante,
            Tipo = tipo,
            Debito = true
        };
        await _repository.Create(transacaoCredito);
        await _repository.Create(transacaoDebito);
    }
}
public interface ITransacaoRepository
{
    Task Create(Transacao transacao);
}
public class Transacao
{
    public string Parte { get; internal set; }
    public string ContraParte { get; internal set; }
    public decimal Valor { get; internal set; }
    public TipoTransacao Tipo { get; internal set; }
    public bool Debito { get; internal set; }
}
public enum TipoTransacao
{
    TransferenciaInterna = 1
}
//GerarExtrato
//CriarConta
//CriarTransacao
public record TransferenciaRequest(string Parte, string ContraParte, decimal Valor, TipoTransacao Tipo, bool Debito);