using ApiControllerWithFastendpoints.Authentication;
using FastEndpoints;
using FastEndpoints.Swagger;
using NSwag;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseKestrel(options => options.AllowSynchronousIO = true);

// Add services to the container.
builder.Services.AddOptions();
builder.Services.AddFastEndpoints();
builder.Services.AddMvc()
                    .AddJsonOptions(options =>
                    {
                        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = ApiKeyAuthenticationOptions.DefaultScheme;
    options.DefaultChallengeScheme = ApiKeyAuthenticationOptions.DefaultScheme;
})
.AddApiKeySupport(options => { });


static OpenApiSecurityScheme ApiKeyAuth()
{
    return new()
    {
        Type = OpenApiSecuritySchemeType.ApiKey,
        In = OpenApiSecurityApiKeyLocation.Header,
        Name = "x-api-key"
    };
}

builder.Services
    .AddSwaggerDoc(addJWTBearerAuth: false, shortSchemaNames: true, maxEndpointVersion: 2, settings: (Action<NSwag.Generation.AspNetCore.AspNetCoreOpenApiDocumentGeneratorSettings>)(s =>
    {
        s.DocumentName = "Legacy";
        s.Title = "API";
        s.Version = "v2";
        s.AddAuth(ApiKeyAuthenticationOptions.DefaultScheme, ApiKeyAuth());
    }))
    .AddSwaggerDoc(addJWTBearerAuth: false, shortSchemaNames: true, minEndpointVersion: 3, maxEndpointVersion: 3, excludeNonFastEndpoints: true, settings: s =>
    {
        s.DocumentName = "v3";
        s.Title = "API";
        s.Version = "v3";
        s.AddAuth(ApiKeyAuthenticationOptions.DefaultScheme, ApiKeyAuth());
    });


var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseHsts();
}
app.UsePathBase("/sample-api");

app.UseFastEndpoints(c =>
{
    c.Versioning.Prefix = "v";
    c.Versioning.DefaultVersion = 2;
});

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});
app.UseHttpsRedirection();
//app.UseSwaggerUi3(c =>
//{
//    c.DocExpansion = "list";
//});

app.UseSwaggerGen();

app.Run();
