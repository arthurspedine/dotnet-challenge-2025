using Moq;
using Motoflow.Web.Models;
using Motoflow.Web.Models.DTOs;
using Motoflow.Web.Repositories;
using Motoflow.Web.Services;
using Motoflow.Web.Models.Common;

namespace Motoflow.Tests.Unit.Services
{
    public class HistoricoMotoServiceTests
    {
        private readonly Mock<IHistoricoMotoRepository> _mockHistoricoRepository;
        private readonly Mock<MotoService> _mockMotoService;
        private readonly Mock<AreaService> _mockAreaService;
        private readonly HistoricoMotoService _service;

        public HistoricoMotoServiceTests()
        {
            _mockHistoricoRepository = new Mock<IHistoricoMotoRepository>();
            _mockMotoService = new Mock<MotoService>(MockBehavior.Strict, null!);
            _mockAreaService = new Mock<AreaService>(MockBehavior.Strict, null!);
            
            _service = new HistoricoMotoService(
                _mockHistoricoRepository.Object,
                _mockMotoService.Object,
                _mockAreaService.Object
            );
        }

        [Fact]
        public async Task GetHistoricoByIdAsync_WithExistingId_ShouldReturnHistorico()
        {
            // Arrange
            var historico = new HistoricoMoto
            {
                Id = 1,
                MotoId = 1,
                AreaId = 1,
                DataEntrada = DateTime.Now,
                ObservacaoEntrada = "Entrada OK"
            };

            _mockHistoricoRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(historico);

            // Act
            var result = await _service.GetHistoricoByIdAsync(1);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(1, result.Id);
            Assert.Equal("Entrada OK", result.ObservacaoEntrada);
        }

        [Fact]
        public async Task GetHistoricoByIdAsync_WithNonExistentId_ShouldReturnNull()
        {
            // Arrange
            _mockHistoricoRepository
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((HistoricoMoto?)null);

            // Act
            var result = await _service.GetHistoricoByIdAsync(999);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task DeleteHistoricoAsync_WithExistingHistorico_ShouldReturnTrue()
        {
            // Arrange
            var historico = new HistoricoMoto
            {
                Id = 1,
                MotoId = 1,
                AreaId = 1,
                DataEntrada = DateTime.Now,
                ObservacaoEntrada = "Entrada OK"
            };

            _mockHistoricoRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(historico);

            _mockHistoricoRepository
                .Setup(r => r.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteHistoricoAsync(1);

            // Assert
            Assert.True(result);
            _mockHistoricoRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteHistoricoAsync_WithNonExistentHistorico_ShouldReturnFalse()
        {
            // Arrange
            _mockHistoricoRepository
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((HistoricoMoto?)null);

            // Act
            var result = await _service.DeleteHistoricoAsync(999);

            // Assert
            Assert.False(result);
            _mockHistoricoRepository.Verify(r => r.DeleteAsync(It.IsAny<long>()), Times.Never);
        }

        [Fact]
        public async Task GetAllHistoricosAsync_ShouldReturnPagedResult()
        {
            // Arrange
            var pagination = new PaginationQuery { Page = 1, PageSize = 10 };
            var historicos = new List<HistoricoMoto>
            {
                new() { Id = 1, MotoId = 1, AreaId = 1, DataEntrada = DateTime.Now, ObservacaoEntrada = "Teste 1" },
                new() { Id = 2, MotoId = 2, AreaId = 1, DataEntrada = DateTime.Now, ObservacaoEntrada = "Teste 2" }
            };

            var pagedResult = new PagedResult<HistoricoMoto>(historicos, 1, 10, 2);

            _mockHistoricoRepository
                .Setup(r => r.GetAllPagedAsync(pagination))
                .ReturnsAsync(pagedResult);

            // Act
            var result = await _service.GetAllHistoricosAsync(pagination);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.TotalCount);
            Assert.Equal(2, result.Data.Count());
        }
    }
}
