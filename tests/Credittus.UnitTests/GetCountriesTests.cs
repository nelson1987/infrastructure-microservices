using AspNetCore8MinimalApis.Endpoints;
using AutoFixture;
using ExpectedObjects;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;

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
    public async Task When_GetCountries_Receives_NullPagingParametersAnd_GetAllAsyncMethodReturnsCountries_ShouldFillUpDefaultPagingParametersAndReturnCountries()
    {
        // Arrange
        int? pageIndex = null;
        int? pageSize = null;
        var expectedPaging = new PagingDto(PageIndex: 1, PageSize: 10).ToExpectedObject();
        var countries = _fixture.CreateMany<CountryDto>(2).ToList();
        var expectedCountries = countries.ToExpectedObject();
        var mappedCountries = _fixture.CreateMany<Country>(2).ToList();
        var expectedMappedCountries = mappedCountries.ToExpectedObject();
        _countryService.GetAllAsync(Arg.Any<PagingDto>()).Returns(x => countries);
        _countryMapper.Map(Arg.Any<List<CountryDto>>()).Returns(x => mappedCountries);
        // Act
        var result = (await CountryEndpoints.GetCountries(pageIndex, pageSize, _countryMapper, _countryService)) as Ok<List<Country>>;
        // Assert
        expectedMappedCountries.Should().Be(result!.Value);
    }
}