using EpicurApp.Logic.Services;
using EpicurApp_API.DAO;
using EpicurAPP_Partage.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<IPlatDAO, PlatDAO>();
builder.Services.AddScoped<AllergeneDAO>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IClientDAO, ClientDAO>();
builder.Services.AddScoped<IMenuDAO, MenuDAO>();
builder.Services.AddScoped<IClientService, ClientService>();
builder.Services.AddScoped<IMenuService, MenuService>();

var app = builder.Build();

try
{
    DatabaseInitializer.Initialize(app.Configuration);
    app.Logger.LogInformation("Base de données initialisée avec succès");
}
catch (Exception ex)
{
    app.Logger.LogError(ex, "Erreur lors de l'initialisation de la base de données");
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
