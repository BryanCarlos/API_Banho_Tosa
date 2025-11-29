using API_Banho_Tosa.Application.AnimalTypes.Services;
using API_Banho_Tosa.Application.Breeds.DTOs;
using API_Banho_Tosa.Application.Breeds.Services;
using API_Banho_Tosa.Application.Common.Interfaces;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace BanhoTosa.Application.Tests.Breeds
{
    public class BreedServiceTests
    {
        private readonly BreedService _sut;
        private readonly Mock<IBreedRepository> _breedRepositoryMock;
        private readonly Mock<IAnimalTypeRepository> _animalTypeRepositoryMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<ILogger<BreedService>> _loggerMock;

        public BreedServiceTests()
        {
            this._breedRepositoryMock = new Mock<IBreedRepository>();
            this._animalTypeRepositoryMock = new Mock<IAnimalTypeRepository>();

            this._currentUserServiceMock = new Mock<ICurrentUserService>();
            this._loggerMock = new Mock<ILogger<BreedService>>();

            this._currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.CreateVersion7());
            this._currentUserServiceMock.Setup(x => x.Username).Returns("TestUser");

            this._sut = new BreedService(this._breedRepositoryMock.Object, this._animalTypeRepositoryMock.Object, this._currentUserServiceMock.Object, this._loggerMock.Object);
        }

        [Fact]
        public async Task CreateBreedAsync_WithValidDto_ShouldCreateAndStoreBreed()
        {
            // Arrange
            string breedName = "Test";

            var requestDto = new CreateBreedRequest()
            {
                Name = breedName,
                AnimalTypeId = 1
            };

            var animalTypeReturn = AnimalType.Create(name: "Dog");
            animalTypeReturn.GetType().GetProperty(nameof(AnimalType.Id))!.SetValue(animalTypeReturn, 1);

            _animalTypeRepositoryMock
                .Setup(repo => repo.GetAnimalTypeByIdAsync(requestDto.AnimalTypeId.Value))
                .ReturnsAsync(animalTypeReturn);

            _breedRepositoryMock
                .Setup(repo => repo.InsertBreedAsync(It.IsAny<Breed>()))
                .Returns(Task.CompletedTask);

            var breedWithDetails = Breed.Create(breedName, 1);
            breedWithDetails.GetType().GetProperty(nameof(Breed.Id))!.SetValue(breedWithDetails, 1);
            breedWithDetails.GetType().GetProperty(nameof(Breed.AnimalType))!.SetValue(breedWithDetails, animalTypeReturn);

            _breedRepositoryMock
                .Setup(repo => repo.GetBreedByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(breedWithDetails);

            // Act
            var response = await _sut.CreateBreedAsync(requestDto);

            // Assert
            _breedRepositoryMock
                .Verify(
                    repo => repo.SaveChangesAsync(), 
                    Times.Once()
                );

            Assert.NotNull(response);
            Assert.Equal(1, response.Id);
            Assert.Equal(requestDto.Name, response.Name);
            Assert.Equal(animalTypeReturn.Id, response.AnimalType.Id);
        }

        [Fact]
        public async Task CreateBreedAsync_WhenAnimalTypeNotExists_ShouldThrowArgumentException()
        {
            // Arrange
            int animalTypeId = 1;

            _animalTypeRepositoryMock
                .Setup(repo => repo.GetAnimalTypeByIdAsync(animalTypeId))
                .ReturnsAsync((AnimalType?)null);

            var request = new CreateBreedRequest()
            {
                Name = "Test",
                AnimalTypeId = animalTypeId
            };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentException>(() => _sut.CreateBreedAsync(request));

            Assert.Equal($"The animal type with ID {animalTypeId} doesn't exists.", exception.Message);
        }

        [Fact]
        public async Task CreateBreedAsync_WhenInsertedBreedIsNotFound_ShouldThrowInvalidOperationException()
        {
            // Arrange
            int animalTypeId = 1;

            var request = new CreateBreedRequest()
            {
                Name = "Test",
                AnimalTypeId = animalTypeId
            };

            var animalTypeToReturn = AnimalType.Create("Dog");

            _animalTypeRepositoryMock
                .Setup(repo => repo.GetAnimalTypeByIdAsync(animalTypeId))
                .ReturnsAsync(animalTypeToReturn);

            _breedRepositoryMock
                .Setup(repo => repo.InsertBreedAsync(It.IsAny<Breed>()))
                .Returns(Task.CompletedTask);

            _breedRepositoryMock
                .Setup(repo => repo.GetBreedByIdAsync(It.IsAny<int>()))
                .ReturnsAsync((Breed?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _sut.CreateBreedAsync(request));

            Assert.Equal("Failed to fetch newly created breed.", exception.Message);
        }

        [Fact]
        public async Task DeleteBreedByIdAsync_ShouldDeleteBreed()
        {
            // Arrange
            int breedId = 1;

            var breedToDelete = Breed.Create(name: "Test", animalTypeId: 1);

            _breedRepositoryMock
                .Setup(repo => repo.GetBreedByIdAsync(breedId))
                .ReturnsAsync(breedToDelete);

            _breedRepositoryMock
                .Setup(repo => repo.DeleteBreed(breedToDelete));

            // Act
            await _sut.DeleteBreedByIdAsync(breedId);

            // Assert
            _breedRepositoryMock
                .Verify(
                    repo => repo.DeleteBreed(breedToDelete),
                    Times.Once
                );

            _breedRepositoryMock
                .Verify(
                    repo => repo.SaveChangesAsync(),
                    Times.Once
                );
        }

        [Fact]
        public async Task DeleteBreedByIdAsync_WhenBreedNotExists_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            int breedId = 1;

            _breedRepositoryMock
                .Setup(repo => repo.GetBreedByIdAsync(breedId))
                .ReturnsAsync((Breed?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.DeleteBreedByIdAsync(breedId));

            Assert.Equal($"Breed with ID {breedId} not found.", exception.Message);
        }

        [Fact]
        public async Task GetBreedByIdAsync_WhenBreedExists_ShouldReturnBreedResponse()
        {
            // Arrange
            int breedId = 1;

            string breedName = "Test";
            string animalTypeName = "Dog";

            var animalTypeReturn = AnimalType.Create(animalTypeName);
            animalTypeReturn.GetType().GetProperty(nameof(AnimalType.Id))!.SetValue(animalTypeReturn, 1);

            var breedWithDetails = Breed.Create(breedName, 1);
            breedWithDetails.GetType().GetProperty(nameof(Breed.Id))!.SetValue(breedWithDetails, 1);
            breedWithDetails.GetType().GetProperty(nameof(Breed.AnimalType))!.SetValue(breedWithDetails, animalTypeReturn);

            _breedRepositoryMock
                .Setup(repo => repo.GetBreedByIdAsync(breedId))
                .ReturnsAsync(breedWithDetails);

            // Act
            var response = await _sut.GetBreedByIdAsync(breedId);

            // Assert
            Assert.NotNull(response);
            Assert.Equal(breedId, response.Id);
            Assert.Equal(breedName, response.Name);

            Assert.Equal(1, response.AnimalType.Id);
            Assert.Equal(animalTypeName, response.AnimalType.Name);
        }

        [Fact]
        public async Task GetBreedByIdAsync_WhenBreedNotExists_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            int breedId = 1;

            _breedRepositoryMock
                .Setup(repo => repo.GetBreedByIdAsync(breedId))
                .ReturnsAsync((Breed?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.GetBreedByIdAsync(breedId));

            Assert.Equal($"Breed with ID {breedId} not found.", exception.Message);
        }

        [Fact]
        public async Task SearchBreedsAsync_WithoutQueryParams_ShouldReturnRegisteredBreeds()
        {
            // Arrange
            var searchParams = new SearchBreedRequest();

            var breeds = GetSampleBreeds();

            _breedRepositoryMock
                .Setup(repo => repo.SearchBreedsAsync(searchParams))
                .ReturnsAsync(breeds);

            // Act
            var response = await _sut.SearchBreedsAsync(searchParams);

            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Assert.Equal(10, response.Count);
        }

        [Fact]
        public async Task SearchBreedsAsync_WithBreedIdParam_ShouldReturnASingleRecord()
        {
            // Arrange
            int breedIdToSearch = 4;

            var searchParams = new SearchBreedRequest(Id: breedIdToSearch);

            var breeds = GetSampleBreeds();
            var breedsToReturn = breeds.Where(b => b.Id == breedIdToSearch).ToList();

            _breedRepositoryMock
                .Setup(repo => repo.SearchBreedsAsync(searchParams))
                .ReturnsAsync(breedsToReturn);

            // Act
            var response = await _sut.SearchBreedsAsync(searchParams);

            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Assert.Single(response);

            var breed = response.First();
            Assert.Equal("Poodle", breed.Name);
        }

        [Fact]
        public async Task SearchBreedsAsync_WithBreedNameParam_ShouldReturnASingleRecord()
        {
            // Arrange
            string breedName = "maine";

            var searchParams = new SearchBreedRequest(Name: breedName);

            var breeds = GetSampleBreeds();
            var breedsToReturn = breeds.Where(b => b.Name.Contains(breedName, StringComparison.OrdinalIgnoreCase)).ToList();

            _breedRepositoryMock
                .Setup(repo => repo.SearchBreedsAsync(searchParams))
                .ReturnsAsync(breedsToReturn);

            // Act
            var response = await _sut.SearchBreedsAsync(searchParams);

            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Assert.Single(response);

            var breed = response.First();
            Assert.Equal("Maine Coon", breed.Name);
        }

        [Fact]
        public async Task UpdateBreedAsync_WithNewData_ShouldUpdateBreed()
        {
            // Arrange
            int animalTypeId = 1;

            var animalType = AnimalType.Create(name: "Animal");
            animalType.GetType().GetProperty(nameof(AnimalType.Id))!.SetValue(animalType, animalTypeId);

            string oldName = "Old name";
            int breedId = 2;

            var breedToReturn = Breed.Create(oldName, animalTypeId);
            breedToReturn.GetType().GetProperty(nameof(Breed.Id))!.SetValue(breedToReturn, breedId);
            breedToReturn.GetType().GetProperty(nameof(Breed.AnimalType))!.SetValue(breedToReturn, animalType);

            string newName = "New name";

            _breedRepositoryMock
                .Setup(repo => repo.GetBreedByIdAsync(breedId))
                .ReturnsAsync(breedToReturn);

            var updateRequest = new UpdateBreedRequest(Name: newName);

            // Act
            var response = await _sut.UpdateBreedAsync(breedId, updateRequest);

            // Assert
            _breedRepositoryMock
                .Verify(
                    repo => repo.SaveChangesAsync(),
                    Times.Once
                );

            Assert.Equal(newName, breedToReturn.Name);
            Assert.NotEqual(breedToReturn.CreatedAt, breedToReturn.UpdatedAt);

            Assert.NotNull(response);
            Assert.Equal(breedId, response.Id);
            Assert.Equal(newName, response.Name);
        }

        [Fact]
        public async Task UpdateBreedAsync_WhenBreedNotExists_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            int breedId = 2;

            string newName = "New name";
            var updateRequest = new UpdateBreedRequest(Name: newName);

            _breedRepositoryMock
                .Setup(repo => repo.GetBreedByIdAsync(breedId))
                .ReturnsAsync((Breed?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(() => _sut.UpdateBreedAsync(breedId, updateRequest));

            Assert.Equal($"Breed with ID {breedId} not found.", exception.Message);
        }

        [Fact]
        public async Task GetBreedsByAnimalTypeIdAsync_WithValidAnimalTypeId_ShouldReturnFilteredBreedList()
        {
            // Arrange
            int animalTypeId = 2;

            var breeds = GetSampleBreeds();
            var breedsToReturn = breeds.Where(b => b.AnimalTypeId == animalTypeId).ToList();

            _breedRepositoryMock
                .Setup(repo => repo.GetBreedsByAnimalTypeIdAsync(animalTypeId))
                .ReturnsAsync(breedsToReturn);

            // Act
            var response = await _sut.GetBreedsByAnimalTypeIdAsync(animalTypeId);

            // Assert
            Assert.NotNull(response);
            Assert.NotEmpty(response);
            Assert.Equal(5, response.Count);
        }

        [Fact]
        public async Task GetBreedsByAnimalTypeIdAsync_WithInvalidAnimalTypeId_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            int animalTypeId = 0;

            _breedRepositoryMock
                .Setup(repo => repo.GetBreedsByAnimalTypeIdAsync(animalTypeId))
                .ReturnsAsync(It.IsAny<IReadOnlyCollection<Breed>>());

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() =>  _sut.GetBreedsByAnimalTypeIdAsync(animalTypeId));

            Assert.Equal("Specified argument was out of the range of valid values. (Parameter 'animalTypeId')", exception.Message);
        }

        private IReadOnlyCollection<Breed> GetSampleBreeds()
        {
            int dogAnimalId = 1;
            int catAnimalId = 2;

            string dogTypeName = "Dog";
            string catTypeName = "Cat";

            var dogAnimalType = AnimalType.Create(dogTypeName);
            dogAnimalType.GetType().GetProperty(nameof(AnimalType.Id))!.SetValue(dogAnimalType, dogAnimalId);

            var catAnimalType = AnimalType.Create(catTypeName);
            catAnimalType.GetType().GetProperty(nameof(AnimalType.Id))!.SetValue(catAnimalType, catAnimalId);

            var breed1 = Breed.Create("Pinscher", dogAnimalId);
            breed1.GetType().GetProperty(nameof(Breed.Id))!.SetValue(breed1, 1);
            breed1.GetType().GetProperty(nameof(Breed.AnimalType))!.SetValue(breed1, dogAnimalType);

            var breed2 = Breed.Create("Shih Tzu", dogAnimalId);
            breed2.GetType().GetProperty(nameof(Breed.Id))!.SetValue(breed2, 2);
            breed2.GetType().GetProperty(nameof(Breed.AnimalType))!.SetValue(breed2, dogAnimalType);

            var breed3 = Breed.Create("Golden Retriever", dogAnimalId);
            breed3.GetType().GetProperty(nameof(Breed.Id))!.SetValue(breed3, 3);
            breed3.GetType().GetProperty(nameof(Breed.AnimalType))!.SetValue(breed3, dogAnimalType);

            var breed4 = Breed.Create("Poodle", dogAnimalId);
            breed4.GetType().GetProperty(nameof(Breed.Id))!.SetValue(breed4, 4);
            breed4.GetType().GetProperty(nameof(Breed.AnimalType))!.SetValue(breed4, dogAnimalType);

            var breed5 = Breed.Create("Buldogue Francês", dogAnimalId);
            breed5.GetType().GetProperty(nameof(Breed.Id))!.SetValue(breed5, 5);
            breed5.GetType().GetProperty(nameof(Breed.AnimalType))!.SetValue(breed5, dogAnimalType);

            // Raças de Gato
            var breed6 = Breed.Create("Siamês", catAnimalId);
            breed6.GetType().GetProperty(nameof(Breed.Id))!.SetValue(breed6, 6);
            breed6.GetType().GetProperty(nameof(Breed.AnimalType))!.SetValue(breed6, catAnimalType);

            var breed7 = Breed.Create("Persa", catAnimalId);
            breed7.GetType().GetProperty(nameof(Breed.Id))!.SetValue(breed7, 7);
            breed7.GetType().GetProperty(nameof(Breed.AnimalType))!.SetValue(breed7, catAnimalType);

            var breed8 = Breed.Create("Maine Coon", catAnimalId);
            breed8.GetType().GetProperty(nameof(Breed.Id))!.SetValue(breed8, 8);
            breed8.GetType().GetProperty(nameof(Breed.AnimalType))!.SetValue(breed8, catAnimalType);

            var breed9 = Breed.Create("Sphynx", catAnimalId);
            breed9.GetType().GetProperty(nameof(Breed.Id))!.SetValue(breed9, 9);
            breed9.GetType().GetProperty(nameof(Breed.AnimalType))!.SetValue(breed9, catAnimalType);

            var breed10 = Breed.Create("Angorá", catAnimalId);
            breed10.GetType().GetProperty(nameof(Breed.Id))!.SetValue(breed10, 10);
            breed10.GetType().GetProperty(nameof(Breed.AnimalType))!.SetValue(breed10, catAnimalType);


            // --- Retorno da Coleção Completa ---
            return new List<Breed>()
            {
                breed1, breed2, breed3, breed4, breed5,
                breed6, breed7, breed8, breed9, breed10
            };
        }
    }
}
