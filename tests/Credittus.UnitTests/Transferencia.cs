
using FluentAssertions;
using FluentValidation;
using FluentValidation.TestHelper;
using FluentValidation.Results;
using FluentResults;
using AutoFixture.AutoMoq;
using AutoFixture;
using FluentResults.Extensions.FluentAssertions;

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
        var request = new TransferenciaRequest(parte, contraParte, tipo, debito);
        request.ChangeValor(valor);

        request.Parte.Should().Be("");
        request.ContraParte.Should().Be("");
        request.Valor.Should().Be(1.00M);
    }
}
public class TransferenciaRequestHandlerTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly TransferenciaRequest _request;
    private readonly TransferenciaRequestHandler _validator;
    public TransferenciaRequestHandlerTests()
    {
        var parte = "";
        var contraParte = "";
        var valor = 1.00M;
        var tipo = TipoTransacao.TransferenciaInterna;
        var debito = true;
        _request = new TransferenciaRequest(parte, contraParte, tipo, debito);
        _request.ChangeValor(valor);
        _validator = _fixture.Create<TransferenciaRequestHandler>();
    }

    [Fact]
    public async Task Dado_Request_Valido_Executa_validator_Com_Sucesso()
    {
        var result = await _validator.Handle(_request, CancellationToken.None);
        //result.Status.Should().Be(200);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Dado_Request_Invalido_Executa_validator_Sem_Sucesso()
    {
        _request.ChangeValor(0);
        var result = await _validator.Handle(_request, CancellationToken.None);
        result.Should().BeSuccess();
    }
}

public class TransferenciaRequestValidatorTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly TransferenciaRequest _request;
    private readonly IValidator<TransferenciaRequest> _validator;

    public TransferenciaRequestValidatorTests()
    {
        // var parte = "";
        // var contraParte = "";
        // var valor = 1.00M;
        // var tipo = TipoTransacao.TransferenciaInterna;
        // var debito = true;
        //_request = new TransferenciaRequest(parte, contraParte, tipo, debito);
        //
        _validator = _fixture.Create<TransferenciaRequestValidator>();
        _request = _fixture.Build<TransferenciaRequest>()
            .Create();
        _request.ChangeValor(1);
    }

    [Fact]
    public async Task Dado_Request_Preenchido_Confirmar_Valido_Com_Sucesso()
    {
        _validator
            .TestValidate(_request)
            .ShouldNotHaveAnyValidationErrors();
    }
}

public class TransferenciaRequestHandler
{
    private readonly IValidator<TransferenciaRequest> _validator;

    public TransferenciaRequestHandler(IValidator<TransferenciaRequest> validator)
    {
        _validator = validator;
    }

    public async Task<Result> Handle(TransferenciaRequest request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);

        if (!validationResult.IsValid)
        {
            return validationResult.ToFailResult();
        }
        return Result.Ok();
    }
}
public static class ValidationResultExtensions
{
    public static Result ToFailResult(this ValidationResult validationResult)
    {
        var errors = validationResult.Errors.Select(x => new FluentResults.Error(x.ErrorMessage)
            .WithMetadata(nameof(x.PropertyName), x.PropertyName)
            .WithMetadata(nameof(x.AttemptedValue), x.AttemptedValue));
        //Melhorar
        return Result.Fail(errors.First());
    }
    // public static Result Fail(this Result validationResult, IEnumerable<IError> errors)
    // {
    //     if (errors == null)
    //     {
    //         throw new ArgumentNullException("errors", "The list of errors cannot be null");
    //     }

    //     Result result = new Result();
    //     result.WithErrors(errors);
    //     return result;
    // }
}
public class TransferenciaRequestValidator : AbstractValidator<TransferenciaRequest>
{
    public TransferenciaRequestValidator()
    {
        RuleFor(x => x.Valor).GreaterThan(0);
    }
}


public class ObjectMapperTests
{
    [Fact]
    public void ValidateMappingConfigurationTest()
    {
        var mapper = ObjectMapper.Mapper;

        mapper.ConfigurationProvider.AssertConfigurationIsValid();
    }
}