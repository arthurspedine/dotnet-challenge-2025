using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Motoflow.Web.Data;
using Motoflow.Web.Repositories;
using Motoflow.Web.Services;

// Método estático para configuração do banco de dados
static void ConfigureDatabase(IServiceCollection services, IConfiguration configuration)
{
    // Verifica se está em ambiente de testes (variável de ambiente ou configuração)
    var useInMemoryDatabase = configuration.GetValue<bool>("UseInMemoryDatabase", false);
    
    if (useInMemoryDatabase)
    {
        // Usa InMemory Database para testes
        services.AddDbContext<OracleDbContext>(options =>
            options.UseInMemoryDatabase("MotoflowTestDb"));
    }
    else
    {
        // Usa Oracle em produção/desenvolvimento
        services.AddDbContext<OracleDbContext>(options =>
            options.UseOracle(configuration.GetConnectionString("OracleConnection")));
    }
}

var builder = WebApplication.CreateBuilder(args);

// Database Configuration
ConfigureDatabase(builder.Services, builder.Configuration);

// Repository and Service Registration
builder.Services.AddScoped<PatioService>();
builder.Services.AddScoped<IPatioRepository, PatioRepository>();
builder.Services.AddScoped<AreaService>();
builder.Services.AddScoped<IAreaRepository, AreaRepository>();
builder.Services.AddScoped<IHistoricoMotoRepository, HistoricoMotoRepository>();
builder.Services.AddScoped<HistoricoMotoService>();
builder.Services.AddScoped<MotoRepository>();
builder.Services.AddScoped<MotoService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddSingleton<MLPredictionService>();

// JWT Authentication Configuration
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["SecretKey"] 
    ?? throw new InvalidOperationException("JWT SecretKey não configurada no appsettings.json");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false; // Em produção, defina como true
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ClockSkew = TimeSpan.Zero // Remove o delay padrão de 5 minutos
    };
    
    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
            {
                context.Response.Headers.Append("Token-Expired", "true");
            }
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

// Health Checks Configuration
builder.Services.AddHealthChecks()
    .AddDbContextCheck<OracleDbContext>(
        name: "database",
        tags: new[] { "db", "oracle" })
    .AddCheck("api", () => 
        Microsoft.Extensions.Diagnostics.HealthChecks.HealthCheckResult.Healthy("API está funcionando"), 
        tags: new[] { "api" });

// Controllers Configuration
builder.Services.AddControllers()
    .AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

builder.Services.AddEndpointsApiExplorer();

// Swagger Configuration with JWT Support
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo 
    { 
        Title = "Motoflow REST API - v1", 
        Version = "v1.0",
        Description = @"API RESTful para gestão de motos em pátios utilizando .NET 8.

## Versão 1.0

Esta é a versão inicial da API com funcionalidades básicas.

### Endpoints de Histórico (v1):
- `GET /api/v1.0/HistoricoMoto/moto/{motoId}` - Busca históricos **apenas por ID numérico** da moto

## Domínio de Negócio

O **Motoflow** é um sistema de gestão de motos em pátios que permite:
- **Pátios**: Locais físicos onde as motos são armazenadas
- **Áreas**: Divisões dentro dos pátios com capacidade limitada
- **Histórico de Motos**: Registro de entrada e saída de motos nas áreas

### Justificativa das Entidades:
1. **Pátio**: Representa o local físico (ex: Pátio Central, Pátio Norte)
2. **Área**: Subdivisões dos pátios para organização (ex: Área A1, B2) 
3. **HistoricoMoto**: Controla movimentação das motos entre áreas

## Autenticação

Para acessar os endpoints protegidos:
1. Registre-se em `/api/auth/register` ou faça login em `/api/auth/login`
2. Copie o token JWT retornado
3. Clique no botão **Authorize** (🔒) no topo desta página
4. Digite: `{seu_token}`
5. Clique em **Authorize** e feche o modal"
    });

    c.SwaggerDoc("v2", new OpenApiInfo 
    { 
        Title = "Motoflow REST API - v2", 
        Version = "v2.0",
        Description = @"API RESTful para gestão de motos em pátios utilizando .NET 8.

## Versão 2.0 - Melhorias

Esta versão adiciona funcionalidades aprimoradas de busca.

### 🆕 Novidades da v2:
- **Busca Flexível de Motos**: `GET /api/v2/HistoricoMoto/moto/{moto}`
  - Aceita **ID numérico** (ex: `123`)
  - Aceita **Placa** (ex: `ABC1234`)
  - Aceita **Chassi** (ex: `9BWZZZ377VT004251`)
  - Aceita **QR Code** (ex: `QR123456789`)

### Exemplos de Uso:
```
GET /api/v2/HistoricoMoto/moto/123          # Busca por ID
GET /api/v2/HistoricoMoto/moto/ABC1234      # Busca por Placa
GET /api/v2/HistoricoMoto/moto/9BWZZZ...    # Busca por Chassi
GET /api/v2/HistoricoMoto/moto/QR123456789  # Busca por QR Code
```

## Autenticação

Para acessar os endpoints protegidos:
1. Registre-se em `/api/v1/auth/register` ou faça login em `/api/v1/auth/login`
2. Copie o token JWT retornado
3. Clique no botão **Authorize** (🔒) no topo desta página
4. Digite: `{seu_token}`
5. Clique em **Authorize** e feche o modal"
    });

    // Configuração para separar endpoints por versão no Swagger
    c.DocInclusionPredicate((version, desc) =>
    {
        // Pega a versão da rota
        if (desc.RelativePath != null && desc.RelativePath.Contains("/v"))
        {
            var routeVersion = desc.RelativePath.Split('/')[1]; // ex: "v1" ou "v2"
            return routeVersion.StartsWith(version.Replace(".", ""));
        }
        
        return version == "v1"; // default para v1
    });

    // Include XML comments for better documentation
    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath);
    }

    // JWT Authentication in Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"Autenticação JWT - Cole apenas o token (sem 'Bearer').

O Swagger irá adicionar automaticamente o prefixo 'Bearer' ao token.

Exemplo: Cole apenas '12345abcdef'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List<string>()
        }
    });
});

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
    options.ApiVersionReader = Microsoft.AspNetCore.Mvc.Versioning.ApiVersionReader.Combine(
        new Microsoft.AspNetCore.Mvc.Versioning.UrlSegmentApiVersionReader()
    );
});

// API Versioning Configuration
builder.Services.AddVersionedApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";         // v1, v2
    options.SubstituteApiVersionInUrl = true;
});

var app = builder.Build();

var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        foreach (var description in provider.ApiVersionDescriptions)
        {
            c.SwaggerEndpoint($"/swagger/{description.GroupName}/swagger.json", $"Motoflow API v{description.ApiVersion}");
            c.RoutePrefix = string.Empty; // Swagger na raiz
        }
    });
}

// Health Checks Endpoint
app.MapHealthChecks("/health", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";
        
        var result = System.Text.Json.JsonSerializer.Serialize(new
        {
            status = report.Status.ToString(),
            timestamp = DateTime.UtcNow,
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                description = e.Value.Description,
                duration = e.Value.Duration.TotalMilliseconds,
                tags = e.Value.Tags
            })
        });
        
        await context.Response.WriteAsync(result);
    }
});

app.UseHttpsRedirection();

// Authentication & Authorization Middleware
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();

// Torna a classe Program acessível para testes
public partial class Program { }
