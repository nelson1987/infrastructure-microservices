
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
public class TransferenciaRequestTests
{
    [Fact]
    public async Task Quando_Transferencia_For_Valida()
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
    private readonly CancellationToken _cancellationToken = CancellationToken.None;
    private readonly IValidator<TransferenciaRequest> _validator;
    private readonly TransferenciaRequest _request;
    private readonly TransferenciaRequestHandler _handler;
    public TransferenciaRequestHandlerTests()
    {
        var parte = "";
        var contraParte = "";
        var valor = 1.00M;
        var tipo = TipoTransacao.TransferenciaInterna;
        var debito = true;
        _request = new TransferenciaRequest(parte, contraParte, tipo, debito);
        _request.ChangeValor(valor);
        
        Transferencia transferencia = _fixture
                .Build<Transferencia>()
                .Create();

        _fixture.Freeze<Mock<ITransferenciaService>>()
            .Setup(x => x.Create(_request.Parte,_request.ContraParte, _request.Valor, _cancellationToken))
            .ReturnsAsync(transferencia);

        _fixture.Freeze<Mock<IValidator<TransferenciaRequest>>>()
            .Setup(x => x.Validate(_request))
            .Returns(new ValidationResult());

        _handler = _fixture.Create<TransferenciaRequestHandler>();
    }

    [Fact]
    public async Task Dado_Request_Valido_Executa_validator_Com_Sucesso()
    {
        var result = await _handler.Handle(_request, _cancellationToken);
        
        _fixture.Freeze<Mock<ITransferenciaService>>()
                .Verify(x => x.Create(_request.Parte,_request.ContraParte, _request.Valor, _cancellationToken)
                , Times.Once());
                
        result.Should().BeSuccess();
    }

    [Fact]
    public async Task Dado_Request_Invalido_Executa_validator_Sem_Sucesso()
    {
            _fixture.Freeze<Mock<IValidator<TransferenciaRequest>>>()
                    .Setup(x => x.Validate(_request))
                    .Returns(new ValidationResult(new[] { new ValidationFailure("prop", "error") }));
        //_validator.Should().NotBeNull();    
        var result = await _handler.Handle(_request, _cancellationToken);

        result.Should().BeFailure();

        // result.Should().BeOfType<BadRequestObjectResult>()
        //         .Which.Value.Should().BeOfType<SerializableError>()
        //         .Which.Should().BeEquivalentTo(new Dictionary<string, object> { { "prop", new[] { "error" } } });
        
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
    private readonly ITransferenciaService _service;

    public TransferenciaRequestHandler(IValidator<TransferenciaRequest> validator, ITransferenciaService service)
    {
        _validator = validator;
        _service = service;
    }

    public async Task<Result> Handle(TransferenciaRequest request, CancellationToken cancellationToken)
    {
        var validationResult = _validator.Validate(request);

        if (!validationResult.IsValid)
        {
            return validationResult.ToFailResult();
        }
        await _service.Create(request.Parte, request.ContraParte, request.Valor, cancellationToken);
        return Result.Ok();
    }
}
public interface ITransferenciaService
{
    Task<Transferencia> Create(string debitante, string creditante, decimal valor, CancellationToken cancellationToken);
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

public class Transferencia
{
    public decimal Valor { get; set; }
    public string Parte { get; set; }
    public string ContraParte { get; set; }
    public TipoTransacao Tipo { get; set; }
    public bool Debito { get; set; }
}
public class TransferenciaRequestMapper : Profile
{
    public TransferenciaRequestMapper()
    {
        CreateMap<Transferencia, TransferenciaRequest>();
    }
}

public static class ObjectMapper
{
    private static readonly Lazy<IMapper> Lazy = new(() =>
    {
        var config = new MapperConfiguration(cfg => cfg.AddMaps(typeof(ObjectMapper).Assembly));
        return config.CreateMapper();
    });

    public static IMapper Mapper => Lazy.Value;

    public static T MapTo<T>(this object source) => Mapper.Map<T>(source);
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