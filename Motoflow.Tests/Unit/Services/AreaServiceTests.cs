using Moq;
using Motoflow.Web.Models;
using Motoflow.Web.Models.DTOs;
using Motoflow.Web.Repositories;
using Motoflow.Web.Services;
using Motoflow.Web.Models.Common;

namespace Motoflow.Tests.Unit.Services
{
    public class AreaServiceTests
    {
        private readonly Mock<IAreaRepository> _mockRepository;
        private readonly AreaService _service;

        public AreaServiceTests()
        {
            _mockRepository = new Mock<IAreaRepository>();
            _service = new AreaService(_mockRepository.Object);
        }

        [Fact]
        public async Task GetPagedAreasAsync_ShouldReturnPagedResult()
        {
            // Arrange
            var areas = new List<Area>
            {
                new Area("A1", 1, 10) { Id = 1 },
                new Area("A2", 1, 20) { Id = 2 },
                new Area("A3", 1, 30) { Id = 3 }
            };

            _mockRepository
                .Setup(r => r.GetAllAsync())
                .ReturnsAsync(areas);

            var pagination = new PaginationQuery { Page = 1, PageSize = 2 };
            var baseUrl = "http://localhost";

            // Act
            var result = await _service.GetPagedAreasAsync(pagination, baseUrl);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Data.Count());
            Assert.Equal(3, result.TotalCount);
            Assert.Equal(2, result.TotalPages);
            Assert.NotNull(result.Links);
        }

        [Fact]
        public async Task CreateAreaAsync_WithValidData_ShouldCreateArea()
        {
            // Arrange
            var dto = new RequestAreaDTO
            {
                Identificador = "A1",
                PatioId = 1,
                CapacidadeMaxima = 50
            };

            _mockRepository
                .Setup(r => r.AddAsync(It.IsAny<Area>()))
                .Returns(Task.CompletedTask);

            var baseUrl = "http://localhost";

            // Act
            var result = await _service.CreateAreaAsync(dto, baseUrl);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("A1", result.Identificador);
            Assert.Equal(50, result.CapacidadeMaxima);
            
            _mockRepository.Verify(r => r.AddAsync(It.Is<Area>(a => 
                a.Identificador == "A1" && 
                a.CapacidadeMaxima == 50
            )), Times.Once);
        }

        [Fact]
        public async Task UpdateAreaAsync_WithValidData_ShouldUpdateArea()
        {
            // Arrange
            var area = new Area("A1", 1, 10) { Id = 1 };
            var dto = new RequestAreaDTO
            {
                Identificador = "A1-Updated",
                PatioId = 2,
                CapacidadeMaxima = 20
            };

            _mockRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(area);

            _mockRepository
                .Setup(r => r.UpdateAsync(It.IsAny<Area>()))
                .Returns(Task.CompletedTask);

            // Act
            await _service.UpdateAreaAsync(1, dto);

            // Assert
            _mockRepository.Verify(r => r.UpdateAsync(It.Is<Area>(a => 
                a.Identificador == "A1-Updated" && 
                a.CapacidadeMaxima == 20
            )), Times.Once);
        }

        [Fact]
        public async Task DeleteAreaAsync_WithExistingArea_ShouldReturnTrue()
        {
            // Arrange
            var area = new Area("A1", 1, 10) { Id = 1 };

            _mockRepository
                .Setup(r => r.GetByIdAsync(1))
                .ReturnsAsync(area);

            _mockRepository
                .Setup(r => r.DeleteAsync(1))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _service.DeleteAreaAsync(1);

            // Assert
            Assert.True(result);
            _mockRepository.Verify(r => r.DeleteAsync(1), Times.Once);
        }

        [Fact]
        public async Task DeleteAreaAsync_WithNonExistentArea_ShouldReturnFalse()
        {
            // Arrange
            _mockRepository
                .Setup(r => r.GetByIdAsync(999))
                .ReturnsAsync((Area?)null);

            // Act
            var result = await _service.DeleteAreaAsync(999);

            // Assert
            Assert.False(result);
            _mockRepository.Verify(r => r.DeleteAsync(It.IsAny<long>()), Times.Never);
        }
    }
}
