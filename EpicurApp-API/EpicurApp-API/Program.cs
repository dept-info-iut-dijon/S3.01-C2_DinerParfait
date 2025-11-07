using EpicurApp.Logic.Services;
using EpicurApp_API.DAO;
using EpicurAPP_Partage.Interfaces;
using EpicurAppLogic.Services;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IPlatDAO, PlatDAO>();
builder.Services.AddScoped<AllergeneDAO>();
builder.Services.AddScoped<IClientDAO, ClientDAO>();
builder.Services.AddScoped<IMenuDAO, MenuDAO>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IMenuService, MenuService>();

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
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
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();
app.Run();