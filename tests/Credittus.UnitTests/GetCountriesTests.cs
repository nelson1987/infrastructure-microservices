using AutoFixture;
using Credditus.Api.Features;
using ExpectedObjects;
using FluentAssertions;
using Microsoft.AspNetCore.Http.HttpResults;
using NSubstitute;

namespace Credittus.UnitTests;

public class GetCountriesTests
{
    private readonly IAddressMapper _countryMapper;
    private readonly IAddressService _countryService;
    private readonly Fixture _fixture;
    public GetCountriesTests()
    {
        _countryMapper = Substitute.For<IAddressMapper>();
        _countryService = Substitute.For<IAddressService>();
        _fixture = new Fixture();
    }
    [Fact]
    public async Task When_GetCountries_Receives_NullPagingParametersAnd_GetAllAsyncMethodReturnsCountries_ShouldFillUpDefaultPagingParametersAndReturnCountries()
    {
        // Arrange
        int? pageIndex = null;
        int? pageSize = null;
        var expectedPaging = new PagingDto(PageIndex: 1, PageSize: 10).ToExpectedObject();
        var countries = _fixture.CreateMany<Address>(2).ToList();
        var expectedCountries = countries.ToExpectedObject();
        var mappedCountries = _fixture.CreateMany<AddressDto>(2).ToList();
        var expectedMappedCountries = mappedCountries.ToExpectedObject();
        _countryService.GetAllAsync(Arg.Any<PagingDto>()).Returns(x => countries);
        _countryMapper.Map(Arg.Any<List<Address>>()).Returns(x => mappedCountries);
        // Act
        var result = (await CountryEndpoints.GetAddresses(pageIndex, pageSize, _countryMapper, _countryService)) as Ok<List<Address>>;
        // Assert
        expectedMappedCountries.Should().Be(result!.Value);
    }
}