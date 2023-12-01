using System.Net;
using Asp.Versioning;
using BankAccounts;
using Credditus.Api.Configs;
using Credditus.Api.Features;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new HeaderApiVersionReader("apiversion");
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VV"; // Formats the version as follow: "'v'major[.minor]"
    options.SubstituteApiVersionInUrl = true;
})
    .EnableApiVersionBinding();
builder.Services.AddSwaggerGen(
  options =>
{
    //    options.EnableAnnotations();
    options.AddXmlComments();
}
);
builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, SwaggerConfigurationsOptions>();
builder.Services.AddScoped<IValidator<Address>, AddressValidator>();
builder.Services.AddScoped<IAddressMapper, AddressMapper>();
builder.Services.AddScoped<IAddressService, AddressService>();
/*
builder.Services.AddKafkaConsumer<string, Address, AddressCreatedHandler>(p =>
{
    p.Topic = "address";
    p.GroupId = "address_group";
    p.BootstrapServers = "localhost:9092";
});
*/
var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseSwagger()
.UseSwaggerUI(c =>
{
    // Workaround, hardcoding versions to be displayed in Swagger
    c.SwaggerEndpoint($"/swagger/v1.0/swagger.json", "Version 1.0");
    c.SwaggerEndpoint($"/swagger/v2.0/swagger.json", "Version 2.0");
    // Not working correctly in ASP.NET Core 8 preview
    //foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions.Reverse())
    //{
    // c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json",
    // description.GroupName.ToUpperInvariant());
    //}
});


app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGroup("/countries").GroupCountries();

app.MapPost("/v{version:apiVersion}/Addresses", ([FromBody] Address address,
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
}).WithOpenApi(operation =>
{
    //OpenApiGenerator
    operation.Summary = "This is a summary";
    operation.Description = "This is a description";
    operation.OperationId = "GetTodos";
    operation.Tags = new List<OpenApiTag> { new() { Name = "Todos" } };

    OpenApiRequestBody body = operation.RequestBody;
    body.Required = true;
    body.Description = "Descricao Request";
    //var jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

    //Type = parameter.Type.Name,
    //Example = new OpenApiString(JsonSerializer.Serialize(parameter.Example,
    //    jsonSerializerOptions))
    //operation.RequestBody.Content["application/json"] = new OpenApiMediaType()
    //{
    //    Schema = new OpenApiSchema()
    //    {
    //        Type = typeof(Address).Name,
    //        Example = new OpenApiString(JsonSerializer.Serialize(new Address()
    //        {
    //            Description = "Descricao",
    //            FlagUri = "Flag",
    //            Name = "Nome"
    //        },
    //    jsonSerializerOptions))
    //        //    Properties = new Dictionary<string, OpenApiSchema>
    //        //{
    //        //    { "grant_type", new OpenApiSchema() { Type = typeof(string).ToString() } },
    //        //    { "scope", new OpenApiSchema() { Type = typeof(string).ToString() } },
    //        //    { "username", new OpenApiSchema() { Type = typeof(string).ToString() } },
    //        //    { "password", new OpenApiSchema() { Type = typeof(string).ToString() } }
    //        //}
    //    }
    //};
    //var jsonSerializerOptions = new JsonSerializerOptions { WriteIndented = true };

    //Type = parameter.Type.Name,
    //Example = new OpenApiString(JsonSerializer.Serialize(parameter.Example,
    //    jsonSerializerOptions))
    return operation;
});
//app.MapPut("/Addresses/{addressId:int}", ([FromRoute] int addressId, [FromForm] Address address) =>
//{
//    return Results.NoContent();
//});

app
.MapPost("/producer", (string msg) =>
{
    var kafka = new ProducerKafka();
    return kafka.SendMessageByKafka();
})
.WithName("Producer")
.WithOpenApi(operation =>
{
    operation.Summary = "This is a summary";
    operation.Description = "This is a description";
    operation.OperationId = "ProduzirMensagem";
    operation.Tags = new List<OpenApiTag> { new() { Name = "Todos" } };
    OpenApiParameter parameter = operation.Parameters[0];
    parameter.Description = "Mensagem a ser enviado no evento";
    return operation;
}); ;

var versionSet = app.NewApiVersionSet()
 .HasApiVersion(new ApiVersion(1.0))
 .HasApiVersion(new ApiVersion(2.0))
 .Build();
app.MapGet("/version", () => "Hello version 1").
WithApiVersionSet(versionSet).MapToApiVersion(1.0);
app.MapGet("/version", () => "Hello version 2").
WithApiVersionSet(versionSet).MapToApiVersion(2.0);
app.MapGet("/version2only", () => "Hello version 2 only").
WithApiVersionSet(versionSet).MapToApiVersion(2.0);
app.MapGet("/versionneutral", () => "Hello neutral version").
WithApiVersionSet(versionSet).IsApiVersionNeutral();




app.MapGroup("/accounts")
    .GroupBankAccount();
app.MapGroup("/transactions")
    .GroupTransaction();
app.Run();
