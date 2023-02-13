using BenchmarkTest.Interfaces;
using BenchmarkTest.Models;
using BenchmarkTest.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Dependency Injection for DbContext
builder.Services.AddDbContext<BenchmarkTestContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("dbConnection")));

// Dependency injection for all the repositories used in the project
builder.Services.AddTransient<GameInterface, GameRepository>();
builder.Services.AddTransient<UserInterface, UserRepository>();
builder.Services.AddTransient<GameScoreInterface, GameScoreRepository>();
builder.Services.AddTransient<RollInterface, RollRepository>();

// Cache setup
builder.Services.AddMemoryCache();

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
