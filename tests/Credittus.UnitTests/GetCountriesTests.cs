namespace Credittus.UnitTests;

public class GetCountriesTests
{
    private readonly ICountryMapper _countryMapper;
    private readonly ICountryService _countryService;
    private readonly Fixture _fixture;
    public GetCountriesTests()
    {
        _countryMapper = Substitute.For<ICountryMapper>();
        _countryService = Substitute.For<ICountryService>();
        _fixture = new Fixture();
    }
    [Fact]
    public async Task When_GetCountries_Receives_NullPagingParametersAnd_GetAllAsyncMethodReturnsCountries_ShouldFillUpDefaultPagingParametersAndReturnCountries
    {
        //Arrange
        //Act
        //Assert
    }
}