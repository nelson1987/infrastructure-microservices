using FluentValidation;
using FluentValidation.Results;
using System.Text.RegularExpressions;

namespace Credditus.Api.Features;
public class AddressValidator : AbstractValidator<Address>
{
    public AddressValidator()
    {
        RuleFor(x => x.Name)
        .NotEmpty().WithMessage("{PropertyName} is required")
 .Custom((name, context) =>
 {
     Regex rg = new Regex("<.*?>"); // Matches HTML tags
     if (rg.Matches(name).Count > 0)
     {
         // Raises an error
         context.AddFailure(
         new ValidationFailure("Name", "The parameter has invalid content")
         );
     }
 });
        RuleFor(x => x.FlagUri)
 .NotEmpty()
 .WithMessage("{PropertyName} is required")
 .Matches("^(https:\\/\\/.)[-a-zA-Z0-9@:%._\\+~#=]{ 2,256}\\.[a-z]{ 2,6}\\b([-a - zA - Z0 - 9@:% _\\+.~#?&//=]*)$")
 .WithMessage("{PropertyName} must match an HTTPS URL");
    }
}
public interface IAddressMapper
{
    List<AddressDto> Map(List<Address> countries);
}
public class AddressMapper : IAddressMapper
{
    public List<AddressDto> Map(List<Address> countries)
    {
        throw new NotImplementedException();
    }
}
public interface IAddressService
{
    Task<List<Address>> GetAllAsync(PagingDto pagging);
}
public record PagingDto(int PageIndex, int PageSize);
public record AddressDto();
public static class CountryEndpoints
{
    public static RouteGroupBuilder GroupCountries(this
RouteGroupBuilder group)
    {
        var countries = new string[]
        {
 "France",
 "Canada",
 "USA"
        };
        var languages = new Dictionary<string, List<string>>()
 {
 { "France", new List<string> { "french" } },
 { "Canada", new List<string> { "french","english" } },
 { "USA", new List<string> { "english","spanish" } }
 };
        group.MapGet("/", () => countries);
        group.MapGet("/{id}", (int id) => countries[id]);
        group.MapGet("/{id}/languages", (int id) =>
        {
            var country = countries[id];
            return languages[country];
        });
        return group;
    }
    public static async Task<IResult> GetAddresses(
       int? pageIndex,
       int? pageSize,
       IAddressMapper mapper,
       IAddressService countryService)
    {
        var paging = new PagingDto(pageIndex ?? 1,
                                    pageSize ?? 10);
        var countries = await countryService.GetAllAsync(paging);
        return Results.Ok(mapper.Map(countries));
    }
}