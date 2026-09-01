using br.com.fiap.cloudgames.Catalog.Application.Abstractions;
using br.com.fiap.cloudgames.Catalog.Application.Consumers;
using br.com.fiap.cloudgames.Catalog.Application.Handlers;
using br.com.fiap.cloudgames.Catalog.Application.Publishers;
using br.com.fiap.cloudgames.Catalog.Application.UnitsOfWork;
using br.com.fiap.cloudgames.Catalog.Application.UseCases.Game.CreateGame;
using br.com.fiap.cloudgames.Catalog.Application.UseCases.Game.RetrieveGame;
using br.com.fiap.cloudgames.Catalog.Application.UseCases.Game.UpdateGame;
using br.com.fiap.cloudgames.Catalog.Application.UseCases.Library.RetrieveLibrary;
using br.com.fiap.cloudgames.Catalog.Application.UseCases.Order.CancelOrder;
using br.com.fiap.cloudgames.Catalog.Application.UseCases.Order.CompleteOrder;
using br.com.fiap.cloudgames.Catalog.Application.UseCases.Order.CreateOrder;
using br.com.fiap.cloudgames.Catalog.Domain.Repositories;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Cache;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Config;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Identity;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Messagging;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Messagging.Consumers;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Messaging.Publishers;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Persistence;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Persistence.MongoDB.Configurations;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Persistence.MongoDB.Repositories;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Persistence.Relational.Context;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Persistence.Relational.Repositories;
using br.com.fiap.cloudgames.Catalog.Infrastructure.Persistence.Relational.Repositories.Cached;
using br.com.fiap.cloudgames.Catalog.WebAPI;
using br.com.fiap.cloudgames.Catalog.WebAPI.Middlewares;
using br.com.fiap.cloudgames.Catalog.WebAPI.Setup;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using Prometheus;
using StackExchange.Redis;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Logging
builder.Logging.ClearProviders();
builder.Logging.AddSimpleConsole(options =>
{
    options.IncludeScopes = true;
    options.SingleLine = true;
    options.TimestampFormat = "yyyy-MM-ddTHH:mm:ss.fffzzz ";
});

//Settings
builder.Services.Configure<JwtTokenSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("RabbitMQ"));
builder.Services.Configure<RedisSettings>(builder.Configuration.GetSection("Redis"));
builder.Services.Configure<MongoDBSettings>(builder.Configuration.GetSection("MongoDB"));


//Add Db Context
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default")), ServiceLifetime.Scoped );

//RedisConnection
var redisConnectionString = builder.Configuration["Redis:connectionString"]
    ?? throw new InvalidOperationException("Redis Settins is not configured");

builder.Services.AddSingleton<IConnectionMultiplexer>(ConnectionMultiplexer.Connect(redisConnectionString));
builder.Services.AddScoped<IDatabase>(provider =>
{
    var multiplexer = provider.GetRequiredService<IConnectionMultiplexer>();
    return multiplexer.GetDatabase();
});

//Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
    .AddJwtBearer("Bearer", options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,

            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"])),
            NameClaimType = JwtRegisteredClaimNames.Name,
            RoleClaimType = ClaimTypes.Role
        };
    });


builder.Services.AddHttpContextAccessor();

//Authorization
builder.Services.AddAuthorization();
builder.Services.AddScoped<ICurrentUser, JwtCurrentUser>();

//Cache Provider
builder.Services.AddScoped<ICacheProvider, RedisCacheProvider>();


//MongoDB
builder.Services.AddSingleton<IMongoClient>(sp =>
    new MongoClient(builder.Configuration["MongoDB:ConnectionString"]));

builder.Services.AddScoped<IMongoDatabase>(sp => {
    var client = sp.GetRequiredService<IMongoClient>();
    return client.GetDatabase(builder.Configuration["MongoDB:DatabaseName"]);
});

BsonSerializer.RegisterSerializer(new GuidSerializer(GuidRepresentation.Standard));

LibraryMongoMap.Configure();

//Relational Repositories
builder.Services.AddScoped<GameRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();

//Mongo Repositories
builder.Services.AddScoped<ILibraryRepository, LibraryRepository>();

//Cached Repositories
builder.Services.AddScoped<IGameRepository>(privider => 
    new CachedGameRepository(
        privider.GetRequiredService<ICacheProvider>(),
        privider.GetRequiredService<GameRepository>()
    ));

//UnitOfWork
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//Messaging
builder.Services.AddSingleton<RabbitMqConnection>();
builder.Services.AddScoped<IOrderCreatedEventPublisher, OrderCreatedEventPublisher>();
builder.Services.AddScoped<IPaymentProcessedEventConsumer, PaymentProcessedEventConsumer>();

//Handlers
builder.Services.AddScoped<PaymentProcessedEventHandler>();

//UseCases
builder.Services.AddScoped<CreateGameUseCase>();
builder.Services.AddScoped<RetrieveGameUseCase>();
builder.Services.AddScoped<UpdateGameUseCase>();
builder.Services.AddScoped<RetrieveLibraryUseCase>();
builder.Services.AddScoped<CancelOrderUseCase>();
builder.Services.AddScoped<CompleteOrderUseCase>();
builder.Services.AddScoped<CreateOrderUseCase>();

builder.Services.AddControllers();

// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new()
    {
        Title = "CatalogAPI (FCG)",
        Version = "v1",
        Description = "Game Catalog and Library API "
    });
});

// Add Worker
builder.Services.AddHostedService<Worker>();

var app = builder.Build();

//Run Migrations
await app.InitializeDatabaseAsync();

app.UseRouting();

app.UseRequestLoggingMiddleware();
app.UseErrorHandlingMiddleware();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.UseMetricServer();
app.UseHttpMetrics();

app.MapControllers();

app.Run();
