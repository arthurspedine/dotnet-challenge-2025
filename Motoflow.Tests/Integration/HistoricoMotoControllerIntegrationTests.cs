using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Motoflow.Web.Data;
using Motoflow.Web.Models;
using Motoflow.Web.Models.DTOs;

namespace Motoflow.Tests.Integration
{
    /// <summary>
    /// Custom WebApplicationFactory para testes de integração
    /// </summary>
    public class MotoflowWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Define o ambiente como Test
            builder.UseEnvironment("Test");

            // Configura para usar InMemory database via appsettings
            builder.ConfigureAppConfiguration((context, config) =>
            {
                config.AddJsonFile("appsettings.Test.json", optional: false);
                
                // Garantir que UseInMemoryDatabase seja true
                config.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["UseInMemoryDatabase"] = "true"
                });
            });

            builder.ConfigureServices(services =>
            {
                // Build the service provider
                var sp = services.BuildServiceProvider();

                // Create a scope to seed the database
                using (var scope = sp.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<OracleDbContext>();

                    // Ensure the database is created
                    db.Database.EnsureDeleted();
                    db.Database.EnsureCreated();
                }
            });
        }
    }

    public class HistoricoMotoControllerIntegrationTests : IClassFixture<MotoflowWebApplicationFactory>
    {
        private readonly MotoflowWebApplicationFactory _factory;
        private readonly HttpClient _client;
        
        // JsonSerializerOptions configurado com JsonStringEnumConverter
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };

        public HistoricoMotoControllerIntegrationTests(MotoflowWebApplicationFactory factory)
        {
            _factory = factory;
            _client = _factory.CreateClient();
        }

        private async Task<string> GetAuthTokenAsync()
        {
            // Primeiro, cria um usuário de teste
            var registerDto = new RegisterRequestDTO
            {
                Username = $"testuser_{Guid.NewGuid()}",
                Email = $"test_{Guid.NewGuid()}@test.com",
                Password = "Test@123",
                ConfirmPassword = "Test@123"
            };

            var registerResponse = await _client.PostAsJsonAsync("/api/Auth/register", registerDto);
            
            if (!registerResponse.IsSuccessStatusCode)
            {
                var errorContent = await registerResponse.Content.ReadAsStringAsync();
                throw new Exception($"Failed to register: {errorContent}");
            }

            // Faz login
            var loginDto = new LoginRequestDTO
            {
                Email = registerDto.Email,
                Password = registerDto.Password
            };

            var loginResponse = await _client.PostAsJsonAsync("/api/Auth/login", loginDto);
            loginResponse.EnsureSuccessStatusCode();

            var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponseDTO>();
            return authResponse?.Token ?? throw new Exception("Token not received");
        }

        private async Task<(long patioId, long areaId)> CreateTestPatioAndAreaAsync(string token)
        {
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Cria um pátio
            var patioDto = new PatioDTO
            {
                Nome = $"Pátio Teste {Guid.NewGuid()}",
                Localizacao = "Localização Teste"
            };

            var patioResponse = await _client.PostAsJsonAsync("/api/Patio", patioDto);
            patioResponse.EnsureSuccessStatusCode();
            var patioResult = await patioResponse.Content.ReadFromJsonAsync<PatioDTO>();

            // Cria uma área
            var areaDto = new RequestAreaDTO
            {
                Identificador = $"A{Guid.NewGuid().ToString()[..4]}",
                PatioId = patioResult!.Id,
                CapacidadeMaxima = 10
            };

            var areaResponse = await _client.PostAsJsonAsync("/api/Area", areaDto);
            areaResponse.EnsureSuccessStatusCode();
            var areaResult = await areaResponse.Content.ReadFromJsonAsync<AreaDTO>();

            return (patioResult.Id, areaResult!.Id);
        }

        [Fact]
        public async Task PostHistorico_WithValidData_ShouldReturnCreated()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            var (patioId, areaId) = await CreateTestPatioAndAreaAsync(token);

            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var historicoDto = new CreateHistoricoMotoDTO
            {
                Moto = new CreateMotoDTO
                {
                    Type = MotoType.Pop,
                    Placa = $"TEST{DateTime.Now.Ticks % 10000}"
                },
                AreaId = areaId,
                ObservacaoEntrada = "Teste de integração"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/HistoricoMoto", historicoDto);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            
            var result = await response.Content.ReadFromJsonAsync<HistoricoMotoDTO>(JsonOptions);
            Assert.NotNull(result);
            Assert.Equal(areaId, result.Area?.Id);
            Assert.Null(result.DataSaida);
        }

        [Fact]
        public async Task PostHistorico_WithNonExistentArea_ShouldReturnNotFound()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            var historicoDto = new CreateHistoricoMotoDTO
            {
                Moto = new CreateMotoDTO
                {
                    Type = MotoType.Pop,
                    Placa = "TEST9999"
                },
                AreaId = 99999, // ID inexistente
                ObservacaoEntrada = "Teste de integração"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/HistoricoMoto", historicoDto);

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task PutHistorico_WithValidSaida_ShouldReturnOk()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            var (patioId, areaId) = await CreateTestPatioAndAreaAsync(token);

            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Cria histórico
            var createDto = new CreateHistoricoMotoDTO
            {
                Moto = new CreateMotoDTO
                {
                    Type = MotoType.Pop,
                    Placa = $"TEST{DateTime.Now.Ticks % 10000}"
                },
                AreaId = areaId,
                ObservacaoEntrada = "Entrada teste"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/HistoricoMoto", createDto);
            createResponse.EnsureSuccessStatusCode();
            var createdHistorico = await createResponse.Content.ReadFromJsonAsync<HistoricoMotoDTO>(JsonOptions);

            // Act - Registra saída
            var updateDto = new UpdateHistoricoMotoDTO
            {
                ObservacaoSaida = "Saída teste"
            };

            var updateResponse = await _client.PutAsJsonAsync(
                $"/api/HistoricoMoto/{createdHistorico!.Id}", 
                updateDto
            );

            // Assert
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            
            var result = await updateResponse.Content.ReadFromJsonAsync<HistoricoMotoDTO>(JsonOptions);
            Assert.NotNull(result);
            Assert.NotNull(result.DataSaida);
            Assert.Equal("Saída teste", result.ObservacaoSaida);
        }

        [Fact]
        public async Task GetHistoricos_ShouldReturnPagedResult()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            
            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Act
            var response = await _client.GetAsync("/api/HistoricoMoto?page=1&pageSize=10");

            // Assert
            response.EnsureSuccessStatusCode();
            
            var content = await response.Content.ReadAsStringAsync();
            Assert.NotNull(content);
            Assert.Contains("\"data\"", content);
            Assert.Contains("\"totalCount\"", content);
        }

        [Fact]
        public async Task DeleteHistorico_WithExistingId_ShouldReturnNoContent()
        {
            // Arrange
            var token = await GetAuthTokenAsync();
            var (patioId, areaId) = await CreateTestPatioAndAreaAsync(token);

            _client.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Cria histórico
            var createDto = new CreateHistoricoMotoDTO
            {
                Moto = new CreateMotoDTO
                {
                    Type = MotoType.Pop,
                    Placa = $"TEST{DateTime.Now.Ticks % 10000}"
                },
                AreaId = areaId,
                ObservacaoEntrada = "Para deletar"
            };

            var createResponse = await _client.PostAsJsonAsync("/api/HistoricoMoto", createDto);
            var createdHistorico = await createResponse.Content.ReadFromJsonAsync<HistoricoMotoDTO>(JsonOptions);

            // Act
            var deleteResponse = await _client.DeleteAsync($"/api/HistoricoMoto/{createdHistorico!.Id}");

            // Assert
            Assert.Equal(HttpStatusCode.NoContent, deleteResponse.StatusCode);
        }
    }
}
