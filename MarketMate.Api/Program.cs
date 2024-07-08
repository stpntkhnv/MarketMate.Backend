using MarketMate.Domain.Settings;
using MarketMate.Application.Services;
using MarketMate.Infrastructure.Extensions;
using MarketMate.Application.Extensions;
using MarketMate.Utilities.Extensions;
using MarketMate.Api.Extensions;
using Google.Cloud.Firestore;
using MarketMate.Domain.Abstractions.Repositories;
using MarketMate.Domain.Abstractions.Services;
using MarketMate.Infrastructure.Repositories;
using MarketMate.Application.Mapping;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApiServices();
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);
builder.Services.AddUtilityServices();


System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

ConfigureVkApi();
ConfigureCors();
ConfigureFirebase();
ConfigureMappings();
ConfigureLogging();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.UseCors("AllowAll");
app.UseExceptionHandler();

app.Run();

void ConfigureCors()
{
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("AllowAll",
            builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                       .AllowAnyMethod();
            });
    });
}

void ConfigureFirebase()
{
    var firebaseSettings = builder.Configuration.GetSection("FirebaseSettings").Get<FirebaseSettings>();

    builder.Services.AddSingleton(firebaseSettings
        ?? throw new InvalidOperationException("Firebase settings are not configured properly."));

    var serviceAccountKeyPath = Path.Combine(Directory.GetCurrentDirectory(), firebaseSettings.ServiceAccountKeyFilename);
    Environment.SetEnvironmentVariable(firebaseSettings.GoogleApplicationCredentialsEnvironmentVariableName, serviceAccountKeyPath);

    builder.Services.AddSingleton<IFirestoreService>(provider =>
        new FirestoreService(firebaseSettings.ProjectId));

    builder.Services.AddSingleton(sp =>
    {
        return FirestoreDb.Create(firebaseSettings.ProjectId);
    });

    builder.Services.AddScoped(typeof(IRepository<>), typeof(FirestoreRepositoryBase<>));
}

void ConfigureVkApi()
{
    var settings = builder.Configuration.GetSection("VkApiSettings").Get<VkApiSettings>();
    builder.Services.AddSingleton<VkApiSettings>(settings);
}


void ConfigureMappings()
{
    MappingConfigurator.RegisterMappings();
}

void ConfigureLogging()
{
    Log.Logger = new LoggerConfiguration()
        .ReadFrom.Configuration(builder.Configuration)
        .CreateLogger();
}