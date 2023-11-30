
using AutoMapper;
using BankAccounts.CreateAccount;
using FluentValidation;

namespace BankAccounts
{
    public static class BankAccountEndpoints 
    { 
        public static RouteGroupBuilder GroupBankAccount(this RouteGroupBuilder group)
        {            
            group.MapGet("/", () => "");
            return group;
        }
    }
    
    public interface IBankAccountRepository
    {
        Task<BankAccount> Create(BankAccount entity);
    }
    public class BankAccount
    {
        public required string Name { get; set; }
    }
    public class BankAccountMapper : Profile
    {
        public BankAccountMapper()
        {
            CreateMap<BankAccount, CreateBankAccountRequest>()
            .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Name));
            
            CreateMap<CreateBankAccountResponse, BankAccount>()
            .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Name));
        }
    }
}

namespace BankAccounts.CreateAccount
{
    public record  CreateBankAccountRequest(string Name);
    public class CreateBankAccountValidator : AbstractValidator<CreateBankAccountRequest>
    {
        public CreateBankAccountValidator()
        {
            RuleFor(x => x.Name).NotEmpty();
        }
    }

    public record CreateBankAccountResponse(string Name);
    public class CreateBankAccountHandler {
        private readonly IBankAccountRepository _bankAccountService;

        public CreateBankAccountHandler(IBankAccountRepository bankAccountService)
        {
            _bankAccountService = bankAccountService;
        }

        public async Task<IResult> Handle(CreateBankAccountRequest request, CancellationToken cancellationToken)
        {
            var criado = await _bankAccountService.Create(request.MapTo<BankAccount>());
            return Results.Ok(criado.MapTo<CreateBankAccountResponse>() );
        }
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
