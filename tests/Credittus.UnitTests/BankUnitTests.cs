using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using FluentValidation.Results;
using FluentResults;
using AutoFixture.AutoMoq;
using AutoFixture;
using FluentResults.Extensions.FluentAssertions;
using AutoMapper;
using Moq;

namespace Credittus.UnitTests;
/*
Comandos de Usuario
Criar Conta
Cancelar Conta
Acessar Extrato
Realizar Transferencia
Imprimir Comprovante
*/
/// <summary>
/// Criar Conta
/// </summary>
/// <param name="Documento"></param>
/// <param name="Email"></param>
public record CriaContaCommand(string Documento, string Email);
public class CriaContaCommandTests
{
    [Fact]
    public async Task Quando_Comando_CriaConta_For_Valida()
    {
        var parte = "";
        var contraParte = "";
        var request = new CriaContaCommand(parte, contraParte);
        
        request.Documento.Should().Be("");
        request.Email.Should().Be("");
    }
}
/// <summary>
/// Cancelar Conta
/// </summary>
/// <param name="Conta">Número da Conta</param>
/// <param name="Documento">Cpf/Cnpj do Cliente</param>
/// <param name="Email">Email do Cliente</param>
public record CancelaContaCommand(string Conta, string Documento, string Email);

public class CancelaContaCommandTests
{
    [Fact]
    public async Task Quando_Comando_CancelaConta_For_Valida()
    {
        var parte = "";
        var contraParte = "";
        var email = "";
        var request = new CancelaContaCommand(parte, contraParte, email);

        request.Conta.Should().Be("");        
        request.Documento.Should().Be("");
        request.Email.Should().Be("");
    }
}
/// <summary>
/// Acessar Extrato
/// </summary>
/// <param name="Conta">Número da Conta</param>
public record BuscaExtratoQuery(string Conta);

public class BuscaExtratoQueryTests
{
    [Fact]
    public async Task Quando_Comando_BuscaExtrato_For_Valida()
    {
        var parte = "";
        var request = new BuscaExtratoQuery(parte);

        request.Conta.Should().Be("");
    }
}


/// <summary>
/// Realizar Transferencia
/// </summary>
/// <param name="Debitante">Número da Conta Debitante</param>
/// <param name="Creditada">Número da Conta Creditada</param>
/// <param name="Valor">Valor a ser transferido</param>
public record CriaTransferenciaCommand(string Debitante, string Creditada, decimal Valor);

public class CriaTransferenciaCommandTests
{
    [Fact]
    public async Task Quando_Comando_CriaTransferencia_For_Valida()
    {
        var parte = "";
        var contraParte = "";
        var email = 1.00M;
        var request = new CriaTransferenciaCommand(parte, contraParte, email);

        request.Debitante.Should().Be("");
        request.Creditada.Should().Be("");
        request.Valor.Should().Be(1.00M);
    }
}

/// <summary>
/// Imprimir Comprovante
/// </summary>
/// <param name="Conta">Número da Conta</param>
/// <param name="IdMovimentacao">Identificador da Movimentação</param>
public record BuscaComprovanteQuery(string Conta, Guid IdMovimentacao);

public class BuscaComprovanteQueryTests
{
    [Fact]
    public async Task Quando_Comando_BuscaComprovante_For_Valida()
    {
        var parte = "";
        var email = Guid.NewGuid();
        var request = new BuscaComprovanteQuery(parte, email);

        request.Conta.Should().Be("");
        request.IdMovimentacao.Should().Be(Guid.NewGuid());
    }
}