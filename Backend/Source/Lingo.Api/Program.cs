using Lingo.Api.Authorization;
using Lingo.Api.Authorization.Contracts;
using Lingo.Api.Filters;
using Lingo.AppLogic;
using Lingo.AppLogic.Contracts;
using Lingo.Domain;
using Lingo.Infrastructure.Storage;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Reflection;
using System.Text;
using Lingo.Api.Converters;
using Lingo.Common;
using Lingo.Domain.Card;
using Lingo.Domain.Card.Contracts;
using Lingo.Domain.Contracts;
using Lingo.Domain.Puzzle;
using Lingo.Domain.Puzzle.Contracts;
using Microsoft.AspNetCore.Mvc.Formatters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSingleton(provider =>
                new LingoExceptionFilterAttribute(provider.GetRequiredService<ILogger<Program>>()));

builder.Services.AddControllers(options =>
{
    var onlyAuthenticatedUsersPolicy = new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
                    .RequireAuthenticatedUser().Build();
    options.Filters.Add(new AuthorizeFilter(onlyAuthenticatedUsersPolicy));
    options.Filters.AddService<LingoExceptionFilterAttribute>();

    var jsonOutputFormatter = options.OutputFormatters.OfType<SystemTextJsonOutputFormatter>().First();
    jsonOutputFormatter.SerializerOptions.Converters.Add(new TwoDimensionalArrayJsonConverter());
    jsonOutputFormatter.SerializerOptions.Converters.Add(new PuzzleJsonConverter());
    jsonOutputFormatter.SerializerOptions.Converters.Add(new BallPitJsonConverter());
    jsonOutputFormatter.SerializerOptions.Converters.Add(new PlayerJsonConverter());
});

builder.Services.AddCors();

builder.Services.AddEndpointsApiExplorer(); // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Lingo API",
        Description = "REST API for online LINGO"
    });

    // Use XML documentation
    string xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"; //api project
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    xmlFilename = $"{typeof(IGame).Assembly.GetName().Name}.xml"; //domain layer
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

    xmlFilename = $"{typeof(IGameService).Assembly.GetName().Name}.xml"; //logic layer
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));


    // Enable bearer token authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Copy 'Bearer ' + valid token into field. You can retrieve a bearer token via '/api/authentication/token'"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });

    options.EnableAnnotations(true, true);
});

IConfiguration configuration = builder.Configuration;
var tokenSettings = new TokenSettings();
configuration.Bind("Token", tokenSettings);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidIssuer = tokenSettings.Issuer,
        ValidAudience = tokenSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(tokenSettings.Key)),
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy(AppConstants.QuizmastersOnlyPolicyName,
        new AuthorizationPolicyBuilder(JwtBearerDefaults.AuthenticationScheme)
            .RequireRole(AppConstants.QuizmasterRoleName).Build());
});

builder.Services.AddDbContext<LingoDbContext>(options =>
{
    string connectionString = configuration.GetConnectionString("LingoDbConnection");
    options.UseSqlServer(connectionString).EnableSensitiveDataLogging();
});
builder.Services.AddScoped<DatabaseSeeder>();

builder.Services.AddIdentity<User, IdentityRole<Guid>>(options =>
{
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    options.Lockout.MaxFailedAccessAttempts = 8;
    options.Lockout.AllowedForNewUsers = true;

    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireDigit = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.Password.RequiredLength = 5;

    options.SignIn.RequireConfirmedEmail = false;
    options.SignIn.RequireConfirmedPhoneNumber = false;
})
.AddEntityFrameworkStores<LingoDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddAutoMapper(typeof(Program));
builder.Services.AddSingleton<ITokenFactory>(new JwtTokenFactory(tokenSettings));
builder.Services.AddScoped<IUserRepository, UserDbRepository>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddSingleton<IPuzzleService, PuzzleService>();
builder.Services.AddSingleton<IGameFactory, GameFactory>();
builder.Services.AddSingleton<IPuzzleFactory, PuzzleFactory>();
builder.Services.AddSingleton<ILingoCardFactory, LingoCardFactory>();
builder.Services.AddSingleton(typeof(ILingoCardFactory), typeof(LingoCardFactory));
builder.Services.AddSingleton<IGameRepository, InMemoryGameRepository>();
builder.Services.AddSingleton<IWordDictionaryRepository, InMemoryWordDictionaryRepository>();
builder.Services.AddSingleton<IRankingStrategy, RankingStrategy>();

var app = builder.Build();

//Create and seed database
var scope = app.Services.CreateScope();
LingoDbContext context = scope.ServiceProvider.GetRequiredService<LingoDbContext>();
context.Database.EnsureCreated();

DatabaseSeeder seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
await seeder.Seed();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(policyBuilder => policyBuilder.AllowAnyOrigin().AllowAnyHeader());
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
