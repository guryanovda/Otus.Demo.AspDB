using System.Data.Common;
using System.Globalization;
using EFCore.NamingConventions.Internal;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using Npgsql;
using WebApplication2.Abstractions;
using WebApplication2.Domain;
using WebApplication2.Repos;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(x =>
{
    x.UseNpgsql(builder.Configuration.GetConnectionString("db"));
    x.UseSnakeCaseNamingConvention();
});

builder.Services.AddScoped(typeof(DbContext), typeof(DataContext));
builder.Services.AddScoped(typeof(DbConnection), (_) => new NpgsqlConnection(builder.Configuration.GetConnectionString("db")));
builder.Services.AddScoped(typeof(INameRewriter), (_) => new SnakeCaseNameRewriter(CultureInfo.CurrentCulture));
builder.Services.AddScoped(typeof(MongoUrl), (_) => new MongoUrl(builder.Configuration.GetConnectionString("mongo")));

builder.Services.AddScoped(typeof(IRepository<>), typeof(MongoRepository<>));

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
