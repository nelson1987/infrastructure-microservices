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