using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
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

## Autenticação

Para acessar os endpoints protegidos:
1. Registre-se em `/api/auth/register` ou faça login em `/api/auth/login`
2. Copie o token JWT retornado
3. Clique no botão **Authorize** (🔒) no topo desta página
4. Digite: `{seu_token}`
5. Clique em **Authorize** e feche o modal"
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

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Motoflow API V1");
        c.RoutePrefix = string.Empty; // Swagger na raiz
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
