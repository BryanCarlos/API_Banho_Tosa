using API_Banho_Tosa.Application.AnimalTypes.DTOs;
using API_Banho_Tosa.Application.AnimalTypes.Services;
using API_Banho_Tosa.Application.Common.Interfaces;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace BanhoTosa.Application.Tests.AnimalTypes
{
    public class AnimalTypeServiceTests
    {
        private readonly AnimalTypeService _sut;
        private readonly Mock<IAnimalTypeRepository> _animalTypeRepositoryMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<ILogger<AnimalTypeService>> _loggerMock;

        public AnimalTypeServiceTests()
        {
            this._animalTypeRepositoryMock = new Mock<IAnimalTypeRepository>();
            this._currentUserServiceMock = new Mock<ICurrentUserService>();
            this._loggerMock = new Mock<ILogger<AnimalTypeService>>();

            this._currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.CreateVersion7());
            this._currentUserServiceMock.Setup(x => x.Username).Returns("TestUser");

            this._sut = new AnimalTypeService(
                this._animalTypeRepositoryMock.Object,
                this._currentUserServiceMock.Object,
                this._loggerMock.Object);
        }

        [Fact]
        public async Task CreateAnimalTypeAsync_WithValidInputs_ShouldCreateAnimalType()
        {
            // Arrange
            string typeName = "Cachorro";
            var animalTypeRequest = new AnimalTypeRequest(typeName);

            _animalTypeRepositoryMock
                .Setup(repo => repo.InsertAnimalTypeAsync(It.IsAny<AnimalType>()))
                // Callback's called when the mocked method's called
                .Callback((AnimalType at) => 
                {
                    // Here, we simulate database setting an ID to the object
                    // the service created. Since the ID setter is private, we use Reflection
                    at.GetType().GetProperty(nameof(AnimalType.Id))!.SetValue(at, 1);
                });

            _animalTypeRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var response = await _sut.CreateAnimalTypeAsync(animalTypeRequest);

            // Assert
            _animalTypeRepositoryMock.Verify(
                repo => repo.InsertAnimalTypeAsync(It.IsAny<AnimalType>()),
                Times.Once()
            );

            _animalTypeRepositoryMock.Verify(
                repo => repo.SaveChangesAsync(),
                Times.Once()
            );

            Assert.NotNull(response);
            Assert.True(response.Id > 0);
            Assert.Equal(typeName, response.Name);
        }

        [Fact]
        public async Task DeleteAnimalTypeAsync_ShouldDeleteRegister()
        {
            // Arrange
            int idToDelete = 67;

            var animalToDelete = AnimalType.Create(name: "Animal");
            animalToDelete.GetType().GetProperty(nameof(AnimalType.Id))!.SetValue(animalToDelete, idToDelete);

            _animalTypeRepositoryMock
                .Setup(repo => repo.GetAnimalTypeByIdAsync(idToDelete))
                .ReturnsAsync(animalToDelete);

            // Act
            await _sut.DeleteAnimalTypeAsync(idToDelete);

            // Assert
            _animalTypeRepositoryMock.Verify(
                repo => repo.GetAnimalTypeByIdAsync(idToDelete),
                Times.Once()
            );

            _animalTypeRepositoryMock.Verify(
                repo => repo.DeleteAnimalType(animalToDelete),
                Times.Once()
            );

            _animalTypeRepositoryMock.Verify(
                repo => repo.SaveChangesAsync(),
                Times.Once()
            );
        }

        [Fact]
        public async Task DeleteAnimalTypeAsync_WhenAnimalTypeNotExists_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            int idToDelete = 123;

            _animalTypeRepositoryMock
                .Setup(repo => repo.GetAnimalTypeByIdAsync(idToDelete))
                .ReturnsAsync((AnimalType?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteAnimalTypeAsync(idToDelete));

            Assert.Equal($"Animal type with ID {idToDelete} not found.", exception.Message);
        }

        [Fact]
        public async Task GetAnimalTypeByIdAsync_ShouldReturnValidResponse()
        {
            // Arrange
            string typeName = "Animal";
            var animalTypeToReturn = AnimalType.Create(name: typeName);

            _animalTypeRepositoryMock
                .Setup(repo => repo.GetAnimalTypeByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(animalTypeToReturn);

            // Act
            var response = await _sut.GetAnimalTypeByIdAsync(id: It.IsAny<int>());

            // Assert
            Assert.NotNull(response);
            Assert.Equal(typeName, response.Name);
        }

        [Fact]
        public async Task GetAnimalTypeByIdAsync_WhenAnimalTypeNotExists_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            int idToDelete = 65;

            _animalTypeRepositoryMock
                .Setup(repo => repo.GetAnimalTypeByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((AnimalType?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetAnimalTypeByIdAsync(idToDelete));

            Assert.Equal($"Animal type with ID {idToDelete} not found.", exception.Message);
        }

        [Fact]
        public async Task SearchAnimalTypesAsync_WithoutQueryParams_ShouldReturnRegisteredAnimalTypes()
        {
            // Arrange
            var filter = new SearchAnimalTypeRequest();

            var typesToReturn = GetSampleAnimalTypes();

            _animalTypeRepositoryMock
                .Setup(repo => repo.SearchAnimalTypesAsync(filter))
                .ReturnsAsync(typesToReturn);

            // Act
            var response = await _sut.SearchAnimalTypesAsync(filter);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(3, response.Count());
        }

        [Fact]
        public async Task SearchAnimalTypesAsync_WithNameQueryParam_ShouldReturnRegisteredAnimalTypes()
        {
            // Arrange
            var registeredTypes = GetSampleAnimalTypes();
            var animalsReturned = registeredTypes.Where(at => at.Name.Contains("gato", StringComparison.OrdinalIgnoreCase));

            var firstAnimalReturned = animalsReturned.First();

            var searchParams = new SearchAnimalTypeRequest(Id: null, Name: "gato");

            _animalTypeRepositoryMock
                .Setup(repo => repo.SearchAnimalTypesAsync(searchParams))
                .ReturnsAsync(animalsReturned);

            // Act
            var response = await _sut.SearchAnimalTypesAsync(searchParams);

            // Assert
            Assert.NotNull(response);
            Assert.Single(response);

            var firstAnimalResponse = response.First();
            Assert.Equal("Gato", firstAnimalResponse.Name);
        }

        [Fact]
        public async Task SearchAnimalTypesAsync_WithIdQueryParam_ShouldReturnRegisteredAnimalTypes()
        {
            // Arrange
            var registeredTypes = GetSampleAnimalTypes();
            var animalsReturned = registeredTypes.Where(at => at.Id == 2);

            var firstAnimalReturned = animalsReturned.First();

            var searchParams = new SearchAnimalTypeRequest(Id: 2, Name: null);

            _animalTypeRepositoryMock
                .Setup(repo => repo.SearchAnimalTypesAsync(searchParams))
                .ReturnsAsync(animalsReturned);

            // Act
            var response = await _sut.SearchAnimalTypesAsync(searchParams);

            // Assert
            Assert.NotNull(response);
            Assert.Single(response);

            var firstAnimalResponse = response.First();
            Assert.Equal(firstAnimalReturned.Name, firstAnimalResponse.Name);
        }

        [Fact]
        public async Task SearchAnimalTypesAsync_WithIdAndNameQueryParams_ShouldReturnRegisteredAnimalTypes()
        {
            // Arrange
            var registeredTypes = GetSampleAnimalTypes();
            var animalsReturned = registeredTypes.Where(at => at.Id == 2 && at.Name.Contains("coelho", StringComparison.OrdinalIgnoreCase));

            var firstAnimalReturned = animalsReturned.First();

            var searchParams = new SearchAnimalTypeRequest(Id: 2, Name: "coel");

            _animalTypeRepositoryMock
                .Setup(repo => repo.SearchAnimalTypesAsync(searchParams))
                .ReturnsAsync(animalsReturned);

            // Act
            var response = await _sut.SearchAnimalTypesAsync(searchParams);

            // Assert
            Assert.NotNull(response);
            Assert.Single(response);

            var firstAnimalResponse = response.First();
            Assert.Equal(firstAnimalReturned.Name, firstAnimalResponse.Name);
        }

        [Fact]
        public async Task UpdateAnimalTypeAsync_WhenTypeExists_ShouldUpdateAndSaveChanges()
        {
            // Arrange
            int idToUpdate = 1;
            string oldName = "Old name";
            string newName = "New name";
            var animalTypeToUpdate = AnimalType.Create(oldName);
            animalTypeToUpdate.GetType().GetProperty(nameof(AnimalType.Id))!.SetValue(animalTypeToUpdate, idToUpdate);

            var animalTypeUpdateDto = new AnimalTypeRequest(newName);

            _animalTypeRepositoryMock
                .Setup(repo => repo.GetAnimalTypeByIdAsync(idToUpdate))
                .ReturnsAsync(animalTypeToUpdate);

            // Act
            var response = await _sut.UpdateAnimalTypeAsync(idToUpdate, animalTypeUpdateDto);

            // Assert
            Assert.Equal(newName, animalTypeToUpdate.Name);

            _animalTypeRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once());

            Assert.NotNull(response);
            Assert.Equal(newName, response.Name);
        }

        [Fact]
        public async Task UpdateAnimalTypeAsync_WhenAnimalTypeNotExists_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            int id = 9432;

            _animalTypeRepositoryMock
                .Setup(repo => repo.GetAnimalTypeByIdAsync(id))
                .ReturnsAsync((AnimalType?)null);

            // Act & Assert
            var exception = await  Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateAnimalTypeAsync(id, It.IsAny<AnimalTypeRequest>()));

            Assert.Equal($"Animal type with ID {id} not found.", exception.Message);
        }

        private IEnumerable<AnimalType> GetSampleAnimalTypes()
        {
            var type1 = AnimalType.Create(name: "Cachorro");
            type1.GetType().GetProperty(nameof(AnimalType.Id))!.SetValue(type1, 1);

            var type2 = AnimalType.Create(name: "Coelho");
            type2.GetType().GetProperty(nameof(AnimalType.Id))!.SetValue(type2, 2);

            var type3 = AnimalType.Create(name: "Gato");
            type3.GetType().GetProperty(nameof(AnimalType.Id))!.SetValue(type3, 3);

            return new List<AnimalType>() { type1, type2, type3 };
        }
    }
}
