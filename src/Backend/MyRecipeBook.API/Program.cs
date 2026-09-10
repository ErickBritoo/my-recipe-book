using System.Globalization;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Mime;
using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MyRecipeBook.API.Converts;
using MyRecipeBook.API.Filters;
using MyRecipeBook.API.Token;
using MyRecipeBook.Application;
using MyRecipeBook.Communication.Responses;
using MyRecipeBook.Domain.Repositories.User;
using MyRecipeBook.Domain.Security.Tokens;
using MyRecipeBook.Exceptions;
using MyRecipeBook.Infrastructure;
using MyRecipeBook.Infrastructure.Migrations;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new StringConvert());
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Meu Livro de Receitas" });
    
    // Cria a opção autenticação, adiciona o botão cadeado no swagger para envio do token
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization", 
        Description = "Enter only your access token. Swagger will add 'Bearer' automatically",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
    });
    
    // Vincula a definição de segurança criada anteriormente aos endpoints da API
    options.AddSecurityRequirement(openApiDocument => new OpenApiSecurityRequirement()
    {
        {new OpenApiSecuritySchemeReference("Bearer", openApiDocument), []}
    });
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAcessTokenProvider, HttpContextTokenProvider>();

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new List<CultureInfo>
        { new CultureInfo("en"), new CultureInfo("pt-BR"), new CultureInfo("es") };

    options.DefaultRequestCulture = new RequestCulture("en");

    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;

    options.RequestCultureProviders = [new AcceptLanguageHeaderRequestCultureProvider()];
});

builder.Services.AddMvc(options => { options.Filters.Add<ExceptionsFilter>(); });

builder.Services.AddRouting(options => options.LowercaseUrls = true);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(jwtOptions =>
{
    var signatureKey = builder.Configuration.GetValue<string>("Jwt:SigningKey")!;

    jwtOptions.TokenValidationParameters = new TokenValidationParameters()
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signatureKey)),
        ValidateAudience = false, // Identifica quem vai receber token, não usaremos
        ValidateIssuer = false, // Identifica quem vai emitiu token, não usaremos
        ValidateLifetime = true, // Verifica se o token expirou
        ClockSkew = TimeSpan.Zero, // Tolerância para expiração do token
    };

    jwtOptions.Events = new JwtBearerEvents()
    {
        OnTokenValidated = async context =>
        {
            var subject = context.Principal?.FindFirstValue(JwtRegisteredClaimNames.Sub) ??
                          context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);

            if (Guid.TryParse(subject, out var userId) == false)
            {
                context.Fail("Invalid token subject");
                return;
            }

            var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserReadOnlyRepository>();

            var userExists = await userRepository.ExistActiveUserWithId(userId);

            if (userExists == false)
            {
                context.Fail("User not found or inactive");
            }
        },
        OnChallenge = async context =>
        {
            context.HandleResponse();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = MediaTypeNames.Application.Json;

            var response = context.AuthenticateFailure switch
            {
                null => new ResponseErrorJson(ResourceMessagesExceptions.VALIDATION_ACESS_TOKEN_REQUIRED),
                SecurityTokenExpiredException => new ResponseErrorJson(ResourceMessagesExceptions.TOKEN_TIME_EXPIRED,
                    acessTokenExpired: true),
                _ => new ResponseErrorJson(ResourceMessagesExceptions.VALIDATION_RESOURCE_ACESS_DENIED)
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    };
});

var app = builder.Build();

var localization = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>();
app.UseRequestLocalization(localization.Value);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();

    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

await ExecuteMigrations();

app.Run();
return;

async Task ExecuteMigrations()
{
    await using var scope = app.Services.CreateAsyncScope();

    await DatabaseMigration.ExecuteMigrations(scope.ServiceProvider);
}