
using FluentAssertions;
using Credditus.Api.Features;
using Microsoft.AspNetCore.Http;

namespace Credittus.UnitTests;
public class TransferenciaRequestTests
{
    [Fact]
    public async Task Quando_Transferencia_For_Pedida()
    {
        var parte = "";
        var contraParte = "";
        var valor = 1.00M;
        var tipo = TipoTransacao.TransferenciaInterna;
        var debito = true;
        var request = new TransferenciaRequest(parte, contraParte, valor, tipo, debito);

        request.Parte.Should().Be("");
        request.ContraParte.Should().Be("");
        request.Valor.Should().Be(1.00M);
    }
}
public class TransferenciaRequestHandlerTests
{
    private readonly TransferenciaRequest _request;

    public TransferenciaRequestHandlerTests()
    {
        var parte = "";
        var contraParte = "";
        var valor = 1.00M;
        var tipo = TipoTransacao.TransferenciaInterna;
        var debito = true;
        _request = new TransferenciaRequest(parte, contraParte, valor, tipo, debito);
    }

    [Fact]
    public async Task Dado_Request_Valido_Executar_Handler_Com_Sucesso()
    {
        var handler = new TransferenciaRequestHandler();
        var result = handler.Handle(_request, CancellationToken.None);
        result.Status.Should().Be(200);
    }
}

public class TransferenciaRequestHandler
{
    public async Task<IResult> Handle(TransferenciaRequest request, CancellationToken cancellationToken)
    {

        return Results.Ok();
    }
}