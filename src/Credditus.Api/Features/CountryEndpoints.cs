namespace AspNetCore8MinimalApis.Endpoints;
public class Country { }
public interface ICountryMapper
{
    List<Country> Map(List<CountryDto> countries);
}
public interface ICountryService
{
    Task<List<CountryDto>> GetAllAsync(PagingDto pagging);
}
public record PagingDto(int PageIndex, int PageSize);
public record CountryDto();
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
    public static async Task<IResult> GetCountries(
       int? pageIndex,
       int? pageSize,
       ICountryMapper mapper,
       ICountryService countryService)
    {
        var paging = new PagingDto(pageIndex ?? 1,
                                    pageSize ?? 10);
        var countries = await countryService.GetAllAsync(paging);
        return Results.Ok(mapper.Map(countries));
    }
}