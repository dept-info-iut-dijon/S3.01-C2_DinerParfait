using EpicurApp_API.DAO;
using EpicurApp_API.Configuration;
using EpicurAppLogic.Interfaces;
using EpicurAppLogic.Services;
using EpicurApp_API.Data;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Enregistrement de la configuration centralisée de la base de données
builder.Services.AddSingleton<DatabaseConfiguration>();

// Enregistrement des DAO
builder.Services.AddScoped<IIngredientDAO, IngredientDAO>();
builder.Services.AddScoped<IPlatDAO, PlatDAO>();
builder.Services.AddScoped<IAllergeneDAO, AllergeneDAO>();
builder.Services.AddScoped<IClientDAO, ClientDAO>();
builder.Services.AddScoped<IMenuDAO, MenuDAO>();
builder.Services.AddScoped<IRepasDAO, RepasDAO>();
builder.Services.AddScoped<IIdeePlatDAO, IdeePlatDAO>();
builder.Services.AddScoped<ServiceDAO>();
builder.Services.AddScoped<ReservationDAO>();
builder.Services.AddScoped<IUtilisateurDAO, UtilisateurDAO>();
builder.Services.AddScoped<IRestaurantDAO, RestaurantDAO>();

// Enregistrement des services
builder.Services.AddScoped<IPlatService, PlatService>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IAllergeneService, AllergeneService>();
builder.Services.AddScoped<IIdeePlatService, IdeePlatService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuration CORS pour permettre les requêtes depuis le navigateur
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

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

// Activer CORS
app.UseCors("AllowAll");

app.UseAuthorization();
app.MapControllers();
app.Run();