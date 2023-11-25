namespace AspNetCore8MinimalApis.Endpoints;
public class Country{}
public interface ICountryMapper {
    PagingDto Map(IReadOnlyCollection<Country> countries);
 }
public interface ICountryService { 
    Task<IReadOnlyCollection<Country>> GetAllAsync(PagingDto pagging);
}
public record PagingDto(int PageIndex, int PageSize);
public static class CountryEndpoints
{
    public static async Task<IResult> GetCountries(
       int? pageIndex,
       int? pageSize,
       ICountryMapper mapper,
       ICountryService countryService)
    {
        var paging = new PagingDto(pageIndex.HasValue ? pageIndex.Value : 1,
                                    pageSize.HasValue ? pageSize.Value : 10);
        var countries = await countryService.GetAllAsync(paging);
        return Results.Ok(mapper.Map(countries));
    }
}