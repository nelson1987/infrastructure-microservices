using FluentAssertions;
namespace TransferenciaInterna.UnitTests;
public class TransferenciaInternaTests
{

    [Fact]
    public async Task Dado_Request_Preenchido_Confirmar_Valido_Com_Sucesso()
    {
        var transferencia = new TransferenciaInterna(StatusTransferenciaInterna.Aberto);
        transferencia.ChangeStatus(TriggerTransferenciaInterna.Comercial);
        transferencia.Status.Should().Be(StatusTransferenciaInterna.Comercial);
    }
}

public static class TransferenciaInternaStateMachine
{

    private static readonly IReadOnlyDictionary<(StatusTransferenciaInterna currentState, TriggerTransferenciaInterna), StatusTransferenciaInterna> States =
        new Dictionary<(StatusTransferenciaInterna currentState, TriggerTransferenciaInterna), StatusTransferenciaInterna>
        {
            {(StatusTransferenciaInterna.Aberto,TriggerTransferenciaInterna.Comercial),StatusTransferenciaInterna.Comercial},
            {(StatusTransferenciaInterna.Comercial, TriggerTransferenciaInterna.Diretoria),StatusTransferenciaInterna.Diretoria} ,
            {(StatusTransferenciaInterna.Comercial, TriggerTransferenciaInterna.Documentacao),StatusTransferenciaInterna.Documentacao} ,
            {(StatusTransferenciaInterna.Comercial, TriggerTransferenciaInterna.Contratos),StatusTransferenciaInterna.Contratos } ,
            {(StatusTransferenciaInterna.Comercial, TriggerTransferenciaInterna.Cancelamento),StatusTransferenciaInterna.Cancelamento} ,
            {(StatusTransferenciaInterna.Diretoria, TriggerTransferenciaInterna.Mesa),StatusTransferenciaInterna.Mesa } ,
            {(StatusTransferenciaInterna.Diretoria, TriggerTransferenciaInterna.Cancelamento),StatusTransferenciaInterna.Cancelamento} ,
            {(StatusTransferenciaInterna.Documentacao, TriggerTransferenciaInterna.Comercial),StatusTransferenciaInterna.Comercial } ,
            {(StatusTransferenciaInterna.Contratos, TriggerTransferenciaInterna.Mesa),StatusTransferenciaInterna.Mesa } ,
            {(StatusTransferenciaInterna.Mesa, TriggerTransferenciaInterna.Saida),StatusTransferenciaInterna.Saida }
        };
    //public static Result<StatusTransferenciaInterna> GetNextState(StatusTransferenciaInterna currentState, TriggerTransferenciaInterna trigger)
    public static StatusTransferenciaInterna GetNextState(StatusTransferenciaInterna currentState, TriggerTransferenciaInterna trigger)
    {
        if (States.TryGetValue((currentState, trigger), out var newState))
            return newState;

        //return Result.Fail($"The current negotiation state of the Credit Note is {currentState}, " +
        //                   $"which prevents the requested action {trigger} from being executed");
        throw new Exception($"The current negotiation state of the Credit Note is {currentState}, " +
                           $"which prevents the requested action {trigger} from being executed");
    }
}

public class TransferenciaInterna
{
    public TransferenciaInterna(StatusTransferenciaInterna status)
    {
        Status = status;
    }

    public StatusTransferenciaInterna Status { get; set; }
    public void ChangeStatus(TriggerTransferenciaInterna trigger)
    {
        var novoStatus = TransferenciaInternaStateMachine.GetNextState(Status, trigger);
        Status = novoStatus;
    }
}
public enum StatusTransferenciaInterna
{
    Aberto = 1,
    Comercial = 2,
    Diretoria = 3,
    Documentacao = 4,
    Contratos = 5,
    Cancelamento = 6,
    Mesa = 7,
    Saida = 8
}
public enum TriggerTransferenciaInterna
{
    Comercial = 1,
    Diretoria = 2,
    Documentacao = 3,
    Contratos = 4,
    Cancelamento = 5,
    Mesa = 6,
    Saida = 7
}