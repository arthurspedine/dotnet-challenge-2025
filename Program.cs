using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Motoflow.Data;
using Motoflow.Repositories;
using Motoflow.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<OracleDbContext>(options =>
    options.UseOracle(builder.Configuration.GetConnectionString("OracleConnection")));
builder.Services.AddScoped<PatioService>();
builder.Services.AddScoped<PatioRepository>();
builder.Services.AddScoped<AreaService>();
builder.Services.AddScoped<AreaRepository>();
builder.Services.AddScoped<HistoricoMotoRepository>();
builder.Services.AddScoped<HistoricoMotoService>();
builder.Services.AddScoped<MotoRepository>();
builder.Services.AddScoped<MotoService>();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Motoflow REST API", Version = "v1" });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();