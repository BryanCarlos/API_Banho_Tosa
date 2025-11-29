using API_Banho_Tosa.Application.Breeds.Services;
using API_Banho_Tosa.Application.Common.Interfaces;
using API_Banho_Tosa.Application.Owners.DTOs;
using API_Banho_Tosa.Application.Owners.Mappers;
using API_Banho_Tosa.Application.Owners.Services;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Interfaces;
using API_Banho_Tosa.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Numerics;

namespace BanhoTosa.Application.Tests.Owners
{
    public class OwnerServiceTests
    {
        private readonly OwnerService _sut; // System Under Test
        private readonly Mock<IOwnerRepository> _ownerRepositoryMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<ILogger<OwnerService>> _loggerMock;

        public OwnerServiceTests()
        {
            this._ownerRepositoryMock = new Mock<IOwnerRepository>();
            this._currentUserServiceMock = new Mock<ICurrentUserService>();
            this._loggerMock = new Mock<ILogger<OwnerService>>();

            this._sut = new OwnerService(this._ownerRepositoryMock.Object, this._currentUserServiceMock.Object, this._loggerMock.Object);
        }

        [Fact]
        public async Task CreateOwnerAsync_WithValidData_ShouldCreateOwner()
        {
            // Arrange
            var ownerDto = new OwnerRequest(Name: "Owner test", Phone: "41 98765-4321", Address: "Rua Abc, 1234");

            _ownerRepositoryMock
                .Setup(repo => repo.InsertOwnerAsync(It.IsAny<Owner>())) // we use It.IsAny<Owner> to accepts any Owner object
                .Returns(Task.CompletedTask);

            _ownerRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(1); // SaveChangesAsync returns Task<int>, so here we use ReturnsAsync


            // Act
            var ownerResponse = await _sut.CreateOwnerAsync(ownerDto);

            // Assert
            // Here we verify if our mock methods were called correctly
            _ownerRepositoryMock.Verify(
                repo => repo.InsertOwnerAsync(It.IsAny<Owner>()),
                Times.Once
            );

            _ownerRepositoryMock.Verify(
                repo => repo.SaveChangesAsync(),
                Times.Once
            );

            Assert.NotNull(ownerResponse);
            Assert.Equal(ownerDto.Name, ownerResponse.Name);
        }

        [Fact]
        public async Task SearchOwnersAsync_WithoutParameters_ShouldReturnEveryActiveOwner()
        {
            // Arrange
            var ownersEntities = GetSampleOwners();
            var activeOwnersEntities = ownersEntities.Where(o => o.DeletedAt == null);

            var searchParams = new SearchOwnerRequest();

            _ownerRepositoryMock
                .Setup(repo => repo.SearchOwnersAsync(searchParams))
                .ReturnsAsync(activeOwnersEntities);

            var expectedDtos = activeOwnersEntities.Select(o => o.MapToResponse()).ToList();

            // Act
            var actualDtos = await _sut.SearchOwnersAsync(searchParams);

            // Assert
            Assert.Equal(4, actualDtos.Count());

            var expectedFirst = expectedDtos.First();
            var actualFirst = actualDtos.First();

            Assert.Equal(expectedFirst.Name, actualFirst.Name);
            Assert.Equal(expectedFirst.Phone, actualFirst.Phone);
        }

        [Fact]
        public async Task SearchOwnersAsync_WithParameters_ShouldReturnFilteredOwners()
        {
            // Arrange
            var allOwnersEntities = GetSampleOwners();

            var searchParams = new SearchOwnerRequest(Name: "Silva");

            var expectedOwners = allOwnersEntities.Where(o => o.Name.Contains("Silva", StringComparison.OrdinalIgnoreCase)).ToList();

            _ownerRepositoryMock
                .Setup(repo => repo.SearchOwnersAsync(searchParams))
                .ReturnsAsync(expectedOwners);

            // Act
            var filteredOwners = await _sut.SearchOwnersAsync(searchParams);

            // Assert
            Assert.Equal(2, filteredOwners.Count());

            var expectedFirst = expectedOwners.First();
            var filteredFirst = filteredOwners.First();

            Assert.All(filteredOwners, dto => Assert.Contains("Silva", dto.Name));
        }

        [Fact]
        public async Task GetOwnersFullInfo_ShouldReturnAllOwners_IncludingDeleted()
        {
            // Arrange
            var owners = GetSampleOwners();

            _ownerRepositoryMock
                .Setup(repo => repo.GetOwnersAsync())
                .ReturnsAsync(owners);

            // Act
            var ownersFullInfo = await _sut.GetOwnersFullInfo();

            // Assert
            Assert.Equal(5, ownersFullInfo.Count());
        }

        [Fact]
        public async Task GetOwnersFullInfo_ShouldCorrectlyMap_ActiveOwnerData()
        {
            // Arrange
            var owners = GetSampleOwners();
            var activeOwnerEntity = owners.First(o => o.Name.Contains("Bryan"));

            _ownerRepositoryMock
                .Setup(repo => repo.GetOwnersAsync())
                .ReturnsAsync(owners);

            // Act
            var ownersFullInfo = await _sut.GetOwnersFullInfo();

            // Assert
            var bryanDto = ownersFullInfo.FirstOrDefault(o => o.Name.Contains("Bryan"));

            Assert.NotNull(bryanDto);
            Assert.Equal(activeOwnerEntity.Uuid, bryanDto.Uuid);
            Assert.Null(bryanDto.DeletedAt);
            Assert.Equal(activeOwnerEntity.UpdatedAt, bryanDto.UpdatedAt);
        }

        [Fact]
        public async Task GetOwnersFullInfo_ShouldCorrectlyMap_DeletedOwnerData()
        {
            // Arrange
            var owners = GetSampleOwners();
            var deletedOwnerEntity = owners.First(o => o.Name == "Pedro Pascal");

            _ownerRepositoryMock
                .Setup(repo => repo.GetOwnersAsync())
                .ReturnsAsync(owners);

            // Act
            var result = await _sut.GetOwnersFullInfo();

            // Assert
            var pedroDto = result.FirstOrDefault(o => o.Name == "Pedro Pascal");

            Assert.NotNull(pedroDto);
            Assert.Equal(deletedOwnerEntity.Uuid, pedroDto.Uuid);
            Assert.NotNull(pedroDto.DeletedAt);
            Assert.Equal(deletedOwnerEntity.DeletedAt, pedroDto.DeletedAt);
        }

        [Fact]
        public async Task DeleteOwnerByUuid_ShouldDeleteActiveOwner()
        {
            // Arrange
            var owners = GetSampleOwners();
            var ownerToDelete = owners.First(o => o.Name.Contains("Andrew"));

            _ownerRepositoryMock
                .Setup(repo => repo.GetOwnerByUuidAsync(ownerToDelete.Uuid))
                .ReturnsAsync(ownerToDelete);

            // Act
            await _sut.DeleteOwnerByUuid(ownerToDelete.Uuid);

            // Assert
            Assert.NotNull(ownerToDelete.DeletedAt);

            _ownerRepositoryMock.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteOwnerByUuid_WhenOwnerIsAlreadyDeleted_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var owners = GetSampleOwners();
            var deletedOwner = owners.First(o => o.DeletedAt != null);

            _ownerRepositoryMock
                .Setup(repo => repo.GetOwnerByUuidAsync(deletedOwner.Uuid))
                .ReturnsAsync(deletedOwner);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>( 
                () => _sut.DeleteOwnerByUuid(deletedOwner.Uuid)
            );

            Assert.Equal("The owner has already been deleted.", exception.Message);
        }

        [Fact]
        public async Task DeleteOwnerByUuid_WhenOwnerIsNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var guidToDelete = Guid.NewGuid();

            _ownerRepositoryMock
                .Setup(repo => repo.GetOwnerByUuidAsync(guidToDelete))
                .ReturnsAsync((Owner?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.DeleteOwnerByUuid(guidToDelete)
            );

            Assert.Equal($"Owner with UUID {guidToDelete} not found.", exception.Message);
        }

        [Fact]
        public async Task GetOwnerByUuid_ShouldReturnExistentOwner()
        {
            // Arrange
            var owners = GetSampleOwners();
            var ownerToReturn = owners.First(o => o.Name.Contains("Ada"));

            _ownerRepositoryMock
                .Setup(repo => repo.GetOwnerByUuidAsync(ownerToReturn.Uuid))
                .ReturnsAsync(ownerToReturn);

            // Act
            var ownerResponse = await _sut.GetOwnerByUuid(ownerToReturn.Uuid);

            // Assert
            Assert.NotNull(ownerResponse);
            Assert.Equal(ownerToReturn.Uuid, ownerResponse.Uuid);
            Assert.Equal(ownerToReturn.Name, ownerResponse.Name);
            Assert.Equal(ownerToReturn.Address, ownerResponse.Address);
            Assert.Equal(ownerToReturn.Phone?.ToString(), ownerResponse.Phone);
        }

        [Fact]
        public async Task GetOwnerByUuid_WhenOwnerIsNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var guidToDontFind = Guid.NewGuid();

            _ownerRepositoryMock
                .Setup(repo => repo.GetOwnerByUuidAsync(guidToDontFind))
                .ReturnsAsync((Owner?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.GetOwnerByUuid(guidToDontFind)
            );

            Assert.Equal($"Owner with UUID {guidToDontFind} not found.", exception.Message);
        }

        [Fact]
        public async Task GetArchivedOwners_ShouldReturnDeletedOwners()
        {
            // Arrange
            var owners = GetSampleOwners();
            var deletedOwners = owners.Where(o => o.DeletedAt != null);

            _ownerRepositoryMock
                .Setup(repo => repo.GetArchivedOwnersAsync())
                .ReturnsAsync(deletedOwners);

            // Act
            var archivedOwners = await _sut.GetArchivedOwners();

            // Assert
            Assert.Single(archivedOwners);
        }

        [Fact]
        public async Task UpdateOwner_WithFullData_ShouldUpdateEntireEntity()
        {
            // Arrange
            var owners = GetSampleOwners();
            var ownerToUpdate = owners.First(o => o.Name.Contains("Bryan"));
            var originalUuid = ownerToUpdate.Uuid;

            var updatedName = "Bryan Carlos da Silva";
            var updatedPhone = "(41) 99999-8888";
            var ownerUpdatedData = new OwnerRequest(Name: updatedName, Address: ownerToUpdate.Address, Phone: updatedPhone);

            _ownerRepositoryMock
                .Setup(repo => repo.GetOwnerByUuidAsync(originalUuid))
                .ReturnsAsync(ownerToUpdate);

            _ownerRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var ownerResponse = await _sut.UpdateOwner(originalUuid, ownerUpdatedData);

            // Assert
            _ownerRepositoryMock
                .Verify(repo => repo.SaveChangesAsync(), Times.Once);

            Assert.Equal(updatedName, ownerToUpdate.Name);
            Assert.Equal(PhoneNumber.Clean(updatedPhone), ownerToUpdate.Phone?.ToString());

            Assert.NotNull(ownerResponse);
            Assert.Equal(originalUuid, ownerResponse.Uuid);
            Assert.Equal(updatedName, ownerResponse.Name);
            Assert.Equal(PhoneNumber.Clean(updatedPhone), ownerResponse.Phone);
        }

        [Fact]
        public async Task UpdateOwner_WhenDtoHasNullAddress_ShouldClearExistingAddress()
        {
            // Arrange
            var owners = GetSampleOwners();
            var ownerWithAddress = owners.First(o => o.Address != null);
            var originalUuid = ownerWithAddress.Uuid;
            var originalAddress = ownerWithAddress.Address;

            var updatedName = "New Owner Name";
            var updatedPhone = "(55) 94444-2222";

            var ownerUpdateData = new OwnerRequest(Name: updatedName, Phone: updatedPhone);

            _ownerRepositoryMock
                .Setup(repo => repo.GetOwnerByUuidAsync(originalUuid))
                .ReturnsAsync(ownerWithAddress);

            _ownerRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var updatedOwnerResponse = await _sut.UpdateOwner(originalUuid, ownerUpdateData);

            // Assert
            Assert.NotNull(updatedOwnerResponse);

            Assert.Equal(updatedName, ownerWithAddress.Name);
            Assert.Null(ownerWithAddress.Address);

            Assert.Equal(originalUuid, updatedOwnerResponse.Uuid);
            Assert.Equal(updatedName, updatedOwnerResponse.Name);
            Assert.Equal(PhoneNumber.Clean(updatedPhone), updatedOwnerResponse.Phone);
            Assert.NotNull(originalAddress);
            Assert.Null(ownerUpdateData.Address);
        }

        [Fact]
        public async Task UpdateOwner_WhenDtoHasNullPhone_ShouldClearExistingPhone()
        {
            // Arrange
            var owners = GetSampleOwners();
            var ownerWithPhone = owners.First(o => o.Phone != null);
            var originalUuid = ownerWithPhone.Uuid;
            var originalPhone = ownerWithPhone.Phone;

            var updatedName = "New Owner Name";
            var updatedAddress = "Rua Novo Endereco, 456";

            var ownerUpdateData = new OwnerRequest(Name: updatedName, Address: updatedAddress);

            _ownerRepositoryMock
                .Setup(repo => repo.GetOwnerByUuidAsync(originalUuid))
                .ReturnsAsync(ownerWithPhone);

            _ownerRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var updatedOwnerResponse = await _sut.UpdateOwner(originalUuid, ownerUpdateData);

            // Assert
            Assert.NotNull(updatedOwnerResponse);

            Assert.Equal(updatedName, ownerWithPhone.Name);
            Assert.Null(ownerWithPhone.Phone);

            Assert.Equal(originalUuid, updatedOwnerResponse.Uuid);
            Assert.Equal(updatedName, updatedOwnerResponse.Name);
            Assert.Equal(updatedAddress, updatedOwnerResponse.Address);
            Assert.NotNull(originalPhone);
            Assert.Null(updatedOwnerResponse.Phone);
        }

        [Fact]
        public async Task UpdateOwner_WhenDtoHasOnlyName_ShouldClearExistingPhoneAndAddress()
        {
            // Arrange
            var owners = GetSampleOwners();
            var ownerWithPhone = owners.First(o => o.Phone != null && o.Address != null);
            var originalUuid = ownerWithPhone.Uuid;
            var originalPhone = ownerWithPhone.Phone;
            var originalAddress = ownerWithPhone.Address;

            var updatedName = "New Owner Name";

            var ownerUpdateData = new OwnerRequest(Name: updatedName);

            _ownerRepositoryMock
                .Setup(repo => repo.GetOwnerByUuidAsync(originalUuid))
                .ReturnsAsync(ownerWithPhone);

            _ownerRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var updatedOwnerResponse = await _sut.UpdateOwner(originalUuid, ownerUpdateData);

            // Assert
            Assert.NotNull(updatedOwnerResponse);

            Assert.Equal(updatedName, ownerWithPhone.Name);
            Assert.Null(ownerWithPhone.Phone);

            Assert.Equal(originalUuid, updatedOwnerResponse.Uuid);
            Assert.Equal(updatedName, updatedOwnerResponse.Name);
            Assert.NotNull(originalAddress);
            Assert.Null(updatedOwnerResponse.Address);
            Assert.NotNull(originalPhone);
            Assert.Null(updatedOwnerResponse.Phone);
        }

        [Fact]
        public async Task UpdateOwner_WhenDtoHasNotRequiredNameField_ShouldThrowRequiredFieldException()
        {
            // Arrange
            var owners = GetSampleOwners();
            var firstOwner = owners.First();
            var originalUuid = firstOwner.Uuid;

            var ownerUpdateData = new OwnerRequest(Name: "");

            _ownerRepositoryMock
                .Setup(repo => repo.GetOwnerByUuidAsync(originalUuid))
                .ReturnsAsync(firstOwner);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<ArgumentNullException>(
                () => _sut.UpdateOwner(originalUuid, ownerUpdateData)
            );

            Assert.Equal("The name must be filled in. (Parameter 'name')", exception.Message);
        }

        [Fact]
        public async Task UpdateOwner_WhenOwnerIsNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var nonExistingOwnerUuid = Guid.NewGuid();

            _ownerRepositoryMock
                .Setup(repo => repo.GetOwnerByUuidAsync(nonExistingOwnerUuid))
                .ReturnsAsync((Owner?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.UpdateOwner(nonExistingOwnerUuid, It.IsAny<OwnerRequest>())
            );

            Assert.Equal($"Owner with UUID {nonExistingOwnerUuid} not found.", exception.Message);
        }

        [Fact]
        public async Task ReactivateOwner_WhenUserIsDeleted_ShouldReactivateSuccessfully()
        {
            // Arrange
            var owners = GetSampleOwners();
            var deletedOwner = owners.First(o => o.DeletedAt != null);
            var originalUuid = deletedOwner.Uuid;

            _ownerRepositoryMock
                .Setup(repo => repo.GetOwnerByUuidIgnoringFiltersAsync(originalUuid))
                .ReturnsAsync(deletedOwner);

            _ownerRepositoryMock
                .Setup(repo => repo.SaveChangesAsync())
                .ReturnsAsync(1);

            // Act
            var reactivatedOwnerResponse = await _sut.ReactivateOwner(originalUuid);

            // Assert
            Assert.NotNull(reactivatedOwnerResponse);

            Assert.Equal(originalUuid, reactivatedOwnerResponse.Uuid);
            Assert.Null(deletedOwner.DeletedAt);
        }

        [Fact]
        public async Task ReactivateOwner_WhenUserAlreadyDeleted_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var owners = GetSampleOwners();
            var activeOwner = owners.First(o => o.DeletedAt == null);
            var originalUuid = activeOwner.Uuid;

            _ownerRepositoryMock
                .Setup(repo => repo.GetOwnerByUuidIgnoringFiltersAsync(originalUuid))
                .ReturnsAsync(activeOwner);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _sut.ReactivateOwner(originalUuid)
            );

            Assert.Equal("The owner's already active.", exception.Message);
        }

        [Fact]
        public async Task ReactivateOwner_WhenOwnerIsNotFound_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var nonExistingOwnerUuid = Guid.NewGuid();

            _ownerRepositoryMock
                .Setup(repo => repo.GetOwnerByUuidAsync(nonExistingOwnerUuid))
                .ReturnsAsync((Owner?)null);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(
                () => _sut.ReactivateOwner(nonExistingOwnerUuid)
            );

            Assert.Equal($"Owner with UUID {nonExistingOwnerUuid} not found.", exception.Message);
        }

        private List<Owner> GetSampleOwners()
        {
            var owner1 = Owner.Create(name: "Bryan", phone: PhoneNumber.Create("41987654321"), address: "Rua Dom Pedro, 456");
            owner1.UpdateName("Bryan Silva");

            var owner2 = Owner.Create(name: "Pedro Pascal", address: "Rua Abc, 954");
            owner2.Delete();

            var owner3 = Owner.Create(name: "Andrew Stuart Tanenbaum", phone: PhoneNumber.Create("41988776655"));

            var owner4 = Owner.Create(name: "Ada Lovelace");

            var owner5 = Owner.Create(name: "Fulano da Silva", phone: PhoneNumber.Create("41913579012"), address: "Rua Teste, 12");

            return new List<Owner> { owner1, owner2, owner3, owner4, owner5 };
        }
    }
}