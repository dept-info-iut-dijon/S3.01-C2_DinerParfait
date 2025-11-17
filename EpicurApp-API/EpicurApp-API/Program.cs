using EpicurApp_API.Configuration;
using EpicurApp_API.DAO;
using EpicurAppLogic.Interfaces;
using EpicurAppLogic.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

// Configuration de la base de données (singleton)
builder.Services.AddSingleton<DatabaseConfiguration>();

// Enregistrement des DAO
builder.Services.AddScoped<IIngredientDAO, IngredientDAO>();
builder.Services.AddScoped<IngredientDAO>();
builder.Services.AddScoped<IPlatDAO, PlatDAO>();
builder.Services.AddScoped<PlatDAO>();
builder.Services.AddScoped<IAllergeneDAO, AllergeneDAO>();
builder.Services.AddScoped<AllergeneDAO>();
builder.Services.AddScoped<IClientDAO, ClientDAO>();
builder.Services.AddScoped<IMenuDAO, MenuDAO>();

// Enregistrement des services
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IMenuService, MenuService>();
builder.Services.AddScoped<IAllergeneService, AllergeneService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

WebApplication app = builder.Build();

// Initialisation de la base de données
try
{
    var dbConfig = app.Services.GetRequiredService<DatabaseConfiguration>();
    DatabaseInitializer.Initialize(dbConfig);
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
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();
app.Run();