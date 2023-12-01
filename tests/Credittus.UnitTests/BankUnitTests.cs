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
using Microsoft.AspNetCore.Http;

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

public class CriaContaCommandHandler
{
    public async Task<Result> Handle(CriaContaCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
public class CriaContaCommandHandlerTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly CriaContaCommandHandler _handler;
    private readonly CriaContaCommand _request;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    public CriaContaCommandHandlerTests()
    {
        _request = _fixture.Build<CriaContaCommand>()
                    .Create();
        _handler = _fixture.Create<CriaContaCommandHandler>();
    }

    [Fact]
    public async Task Init()
    {
        var result = await _handler.Handle(_request, _cancellationToken);
        result.Should().BeSuccess();
    }
}

public class CriaContaCommandValidator : AbstractValidator<CriaContaCommand>
{
    public CriaContaCommandValidator()
    {
        RuleFor(x => x.Documento).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
    }
}
public class CriaContaCommandValidatorTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly CriaContaCommand _request;
    private readonly IValidator<CriaContaCommand> _validator;

    public CriaContaCommandValidatorTests()
    {
        _validator = _fixture.Create<CriaContaCommandValidator>();
        _request = _fixture.Build<CriaContaCommand>()
            .Create();
    }

    [Fact]
    public async Task Dado_Request_Preenchido_Confirmar_Valido_Com_Sucesso()
    {
        _validator
            .TestValidate(_request)
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Dado_Request_Preenchido_Confirmar_InValido_Com_Sucesso()
    {
        _validator
            .TestValidate(_request with { Documento = string.Empty })
            .ShouldHaveValidationErrorFor(x => x.Documento)
            .Only();
    }
}

public class CriaContaCommandMappings : Profile
{
    public CriaContaCommandMappings()
    {
        //CreateMap<CriaContaCommand, Source>();
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

public class CancelaContaCommandHandler
{
    public async Task<Result> Handle(CancelaContaCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
public class CancelaContaCommandHandlerTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly CancelaContaCommandHandler _handler;
    private readonly CancelaContaCommand _request;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    public CancelaContaCommandHandlerTests()
    {
        _request = _fixture.Build<CancelaContaCommand>()
                    .Create();
        _handler = _fixture.Create<CancelaContaCommandHandler>();
    }

    [Fact]
    public async Task Init()
    {
        var result = await _handler.Handle(_request, _cancellationToken);
        result.Should().BeSuccess();
    }
}

public class CancelaContaCommandValidator : AbstractValidator<CancelaContaCommand>
{
    public CancelaContaCommandValidator()
    {
        RuleFor(x => x.Conta).NotEmpty();
        RuleFor(x => x.Documento).NotEmpty();
        RuleFor(x => x.Email).NotEmpty();
    }
}
public class CancelaContaCommandValidatorTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly CancelaContaCommand _request;
    private readonly IValidator<CancelaContaCommand> _validator;

    public CancelaContaCommandValidatorTests()
    {
        _validator = _fixture.Create<CancelaContaCommandValidator>();
        _request = _fixture.Build<CancelaContaCommand>()
            .Create();
    }

    [Fact]
    public async Task Dado_Request_Preenchido_Confirmar_Valido_Com_Sucesso()
    {
        _validator
            .TestValidate(_request)
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Dado_Request_Preenchido_Confirmar_InValido_Com_Sucesso()
    {
        _validator
            .TestValidate(_request with { Conta = string.Empty })
            .ShouldHaveValidationErrorFor(x => x.Conta)
            .Only();
    }
}

public class CancelaContaCommandMappings : Profile
{
    public CancelaContaCommandMappings()
    {
        //CreateMap<CancelaContaCommand, Source>();
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

public class BuscaExtratoQueryHandler
{
    public async Task<Result> Handle(BuscaExtratoQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
public class BuscaExtratoQueryHandlerTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly BuscaExtratoQueryHandler _handler;
    private readonly BuscaExtratoQuery _request;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    public BuscaExtratoQueryHandlerTests()
    {
        _request = _fixture.Build<BuscaExtratoQuery>()
                    .Create();
        _handler = _fixture.Create<BuscaExtratoQueryHandler>();
    }

    [Fact]
    public async Task Init()
    {
        var result = await _handler.Handle(_request, _cancellationToken);
        result.Should().BeSuccess();
    }
}

public class BuscaExtratoQueryValidator : AbstractValidator<BuscaExtratoQuery>
{
    public BuscaExtratoQueryValidator()
    {
        RuleFor(x => x.Conta).NotEmpty();
    }
}
public class BuscaExtratoQueryValidatorTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly BuscaExtratoQuery _request;
    private readonly IValidator<BuscaExtratoQuery> _validator;

    public BuscaExtratoQueryValidatorTests()
    {
        _validator = _fixture.Create<BuscaExtratoQueryValidator>();
        _request = _fixture.Build<BuscaExtratoQuery>()
            .Create();
    }

    [Fact]
    public async Task Dado_Request_Preenchido_Confirmar_Valido_Com_Sucesso()
    {
        _validator
            .TestValidate(_request)
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Dado_Request_Preenchido_Confirmar_InValido_Com_Sucesso()
    {
        _validator
            .TestValidate(_request with { Conta = string.Empty })
            .ShouldHaveValidationErrorFor(x => x.Conta)
            .Only();
    }
}

public class BuscaExtratoQueryMappings : Profile
{
    public BuscaExtratoQueryMappings()
    {
        //CreateMap<BuscaExtratoQuery, Source>();
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

public class CriaTransferenciaCommandHandler
{
    public async Task<Result> Handle(CriaTransferenciaCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
public class CriaTransferenciaCommandHandlerTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly CriaTransferenciaCommandHandler _handler;
    private readonly CriaTransferenciaCommand _request;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    public CriaTransferenciaCommandHandlerTests()
    {
        _request = _fixture.Build<CriaTransferenciaCommand>()
                    .Create();
        _handler = _fixture.Create<CriaTransferenciaCommandHandler>();
    }

    [Fact]
    public async Task Init()
    {
        var result = await _handler.Handle(_request, _cancellationToken);
        result.Should().BeSuccess();
    }
}

public class CriaTransferenciaCommandValidator : AbstractValidator<CriaTransferenciaCommand>
{
    public CriaTransferenciaCommandValidator()
    {
        RuleFor(x => x.Debitante).NotEmpty();
        RuleFor(x => x.Creditada).NotEmpty();
        RuleFor(x => x.Valor).NotEmpty();
    }
}
public class CriaTransferenciaCommandValidatorTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly CriaTransferenciaCommand _request;
    private readonly IValidator<CriaTransferenciaCommand> _validator;

    public CriaTransferenciaCommandValidatorTests()
    {
        _validator = _fixture.Create<CriaTransferenciaCommandValidator>();
        _request = _fixture.Build<CriaTransferenciaCommand>()
            .Create();
    }

    [Fact]
    public async Task Dado_Request_Preenchido_Confirmar_Valido_Com_Sucesso()
    {
        _validator
            .TestValidate(_request)
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Dado_Request_Preenchido_Confirmar_InValido_Com_Sucesso()
    {
        _validator
            .TestValidate(_request with { Debitante = string.Empty })
            .ShouldHaveValidationErrorFor(x => x.Debitante)
            .Only();
    }
}

public class CriaTransferenciaCommandMappings : Profile
{
    public CriaTransferenciaCommandMappings()
    {
        //CreateMap<CriaTransferenciaCommand, Source>();
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

public class BuscaComprovanteQueryHandler
{
    public async Task<Result> Handle(BuscaComprovanteQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
public class BuscaComprovanteQueryHandlerTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly BuscaComprovanteQueryHandler _handler;
    private readonly BuscaComprovanteQuery _request;
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    public BuscaComprovanteQueryHandlerTests()
    {
        _request = _fixture.Build<BuscaComprovanteQuery>()
                    .Create();
        _handler = _fixture.Create<BuscaComprovanteQueryHandler>();
    }

    [Fact]
    public async Task Init()
    {
        var result = await _handler.Handle(_request, _cancellationToken);
        result.Should().BeSuccess();
    }
}

public class BuscaComprovanteQueryValidator : AbstractValidator<BuscaComprovanteQuery>
{
    public BuscaComprovanteQueryValidator()
    {
        RuleFor(x => x.Conta).NotEmpty();
        RuleFor(x => x.IdMovimentacao).NotEmpty();
    }
}
public class BuscaComprovanteQueryValidatorTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly BuscaComprovanteQuery _request;
    private readonly IValidator<BuscaComprovanteQuery> _validator;

    public BuscaComprovanteQueryValidatorTests()
    {
        _validator = _fixture.Create<BuscaComprovanteQueryValidator>();
        _request = _fixture.Build<BuscaComprovanteQuery>()
            .Create();
    }

    [Fact]
    public async Task Dado_Request_Preenchido_Confirmar_Valido_Com_Sucesso()
    {
        _validator
            .TestValidate(_request)
            .ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task Dado_Request_Preenchido_Confirmar_InValido_Com_Sucesso()
    {
        _validator
            .TestValidate(_request with { Conta = string.Empty })
            .ShouldHaveValidationErrorFor(x => x.Conta)
            .Only();
    }
}

public class BuscaComprovanteQueryMappings : Profile
{
    public BuscaComprovanteQueryMappings()
    {
        //CreateMap<BuscaComprovanteQuery, Source>();
    }
}
