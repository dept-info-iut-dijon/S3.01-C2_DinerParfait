using EpicurApp_API.DAO;
using EpicurApp_API.Configuration;
using EpicurAppLogic.Interfaces;
using EpicurAppLogic.Services;
using EpicurApp_API.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Enregistrement de la configuration centralisée de la base de données
builder.Services.AddSingleton<DatabaseConfiguration>();

// Enregistrement des DAO
builder.Services.AddScoped<IPlatDAO, PlatDAO>();
builder.Services.AddScoped<AllergeneDAO>();
builder.Services.AddScoped<IClientDAO, ClientDAO>();
builder.Services.AddScoped<IMenuDAO, MenuDAO>();
builder.Services.AddScoped<IRepasDAO, RepasDAO>();
builder.Services.AddScoped<IdeePlatDAO>();


// Enregistrement des services
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IMenuService, MenuService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNamingPolicy = null; // Utiliser PascalCase au lieu de camelCase
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

try
{
    DatabaseInitializer.Initialize(app.Configuration);
    app.Logger.LogInformation("Base de données initialisée avec succès");
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Erreur lors de l'initialisation de la base de données");
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();