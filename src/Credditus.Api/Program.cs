using BankAccounts;
using Credditus.Api.Features;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IValidator<Address>, AddressValidator>();
builder.Services.AddScoped<IAddressMapper, AddressMapper>();
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddKafkaConsumer<string, Address, AddressCreatedHandler>(p =>
{
    p.Topic = "address";
    p.GroupId = "address_group";
    p.BootstrapServers = "localhost:9092";
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGroup("/countries").GroupCountries();

app.MapPost("/Addresses", ([FromBody] Address address,
    [FromServices] IValidator<Address> validator,
    [FromServices] IAddressMapper mapper,
    [FromServices] IAddressService addressService
    ) =>
{
    var validationResult = validator.Validate(address);
    if (validationResult.IsValid)
    {
        var countryDto = mapper.Map(new List<Address> { address });
        return Results.CreatedAtRoute("countryById", new
        {
            Id = addressService.CreateOrUpdate(countryDto)
        });
    }
    return Results.ValidationProblem(validationResult.ToDictionary(), statusCode: (int)HttpStatusCode.BadRequest);
});
app.MapPut("/Addresses/{addressId:int}", ([FromRoute] int addressId, [FromForm] Address address) =>
{
    return Results.NoContent();
});

app
.MapPost("/producer", (string msg) =>
{
    var kafka = new ProducerKafka();
    return kafka.SendMessageByKafka();
})
.WithName("Producer")
.WithOpenApi();




app.MapGroup("/accounts")
    .GroupBankAccount();
app.MapGroup("/transactions")
    .GroupTransaction();
app.Run();
