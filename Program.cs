using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore.InMemory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Add DbContext configuration
var raceconnectionString = builder.Configuration.GetConnectionString("RaceConnection");
builder.Services.AddDbContext<RaceDbContext>(options =>
    options.UseSqlite(raceconnectionString));

var driverconnectionString = builder.Configuration.GetConnectionString("DriverConnection");
builder.Services.AddDbContext<DriverDbContext>(options =>
    options.UseSqlite(driverconnectionString));

var teamconnectionString = builder.Configuration.GetConnectionString("TeamConnection");
builder.Services.AddDbContext<TeamDbContext>(options =>
    options.UseSqlite(teamconnectionString));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
