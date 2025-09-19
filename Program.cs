using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Motoflow REST API", 
        Version = "v1",
        Description = @"API RESTful para gestão de motos em pátios utilizando .NET 8.

## Domínio de Negócio

O **Motoflow** é um sistema de gestão de motos em pátios que permite:
- **Pátios**: Locais físicos onde as motos são armazenadas
- **Áreas**: Divisões dentro dos pátios com capacidade limitada
- **Histórico de Motos**: Registro de entrada e saída de motos nas áreas

### Justificativa das Entidades:
1. **Pátio**: Representa o local físico (ex: Pátio Central, Pátio Norte)
2. **Área**: Subdivisões dos pátios para organização (ex: Área A1, B2) 
3. **HistoricoMoto**: Controla movimentação das motos entre áreas

## Recursos da API

- ✅ **CRUD Completo** para todas as entidades
- ✅ **Paginação** em todos os endpoints de listagem
- ✅ **HATEOAS** (Hypermedia as the Engine of Application State)
- ✅ **Status Codes HTTP** adequados
- ✅ **Documentação OpenAPI** completa
- ✅ **Validação de dados** com Data Annotations"
    });

    // Include XML comments for better documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }
    
    // Configure response examples
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "Exemplo de autenticação JWT (não implementado nesta versão)",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
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