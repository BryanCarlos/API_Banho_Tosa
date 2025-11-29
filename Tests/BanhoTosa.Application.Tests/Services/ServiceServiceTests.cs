using API_Banho_Tosa.Application.Common.Interfaces;
using API_Banho_Tosa.Application.Services.DTOs;
using API_Banho_Tosa.Application.Services.Services;
using API_Banho_Tosa.Domain.Common.Extensions;
using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.Enums;
using API_Banho_Tosa.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;
using TestsHelpers;

namespace BanhoTosa.Application.Tests.Services
{
    public class ServiceServiceTests
    {
        private readonly Mock<IServiceRepository> _serviceRepositoryMock;
        private readonly Mock<IPetRepository> _petRepositoryMock;
        private readonly Mock<IServiceStatusRepository> _serviceStatusRepositoryMock;
        private readonly Mock<IPaymentStatusRepository> _paymentStatusRepositoryMock;
        private readonly Mock<IServicePriceRepository> _servicePriceRepositoryMock;
        private readonly Mock<ICurrentUserService> _currentUserServiceMock;
        private readonly Mock<ILogger<ServiceService>> _loggerMock;

        private readonly IServiceService _sut;

        public ServiceServiceTests()
        {
            this._serviceRepositoryMock = new Mock<IServiceRepository>();
            this._servicePriceRepositoryMock = new Mock<IServicePriceRepository>();
            this._petRepositoryMock = new Mock<IPetRepository>();
            this._serviceStatusRepositoryMock = new Mock<IServiceStatusRepository>();
            this._paymentStatusRepositoryMock = new Mock<IPaymentStatusRepository>();
            this._servicePriceRepositoryMock = new Mock<IServicePriceRepository>();
            this._currentUserServiceMock = new Mock<ICurrentUserService>();
            this._loggerMock = new Mock<ILogger<ServiceService>>();

            this._currentUserServiceMock.Setup(x => x.UserId).Returns(Guid.CreateVersion7());
            this._currentUserServiceMock.Setup(x => x.Username).Returns("TestUser");

            this._sut = new ServiceService(
                this._serviceRepositoryMock.Object, this._petRepositoryMock.Object,
                this._serviceStatusRepositoryMock.Object, this._paymentStatusRepositoryMock.Object,
                this._servicePriceRepositoryMock.Object, this._currentUserServiceMock.Object, this._loggerMock.Object
            );
        }

        #region CreateServiceAsync tests

        [Fact]
        public async Task CreateServiceAsync_WithItemsAndDiscount_ShouldCalculateTotalsCorrectly()
        {
            // Arrange
            var animalTypeId = 1;
            var animalType = AnimalType.Create("Cachorro").SetProperty(nameof(AnimalType.Id), animalTypeId);

            var petSizeId = 1;
            var petSize = PetSize.Create("Pequeno").SetProperty(nameof(PetSize.Id), petSizeId);

            var breedId = 1;
            var breed = Breed.Create("Labrador", 1)
                .SetProperty(nameof(Breed.Id), breedId)
                .SetProperty(nameof(Breed.AnimalType), animalType);

            var petId = Guid.CreateVersion7();
            var pet = Pet.Create("Rex", 1, petSizeId, DateTime.UtcNow)
                .SetProperty(nameof(Pet.Id), petId)
                .SetProperty(nameof(Pet.Breed), breed)
                .SetProperty(nameof(Pet.PetSize), petSize);

            _petRepositoryMock
                .Setup(repo => repo.GetPetByIdAsync(petId))
                .ReturnsAsync(pet);

            var serviceId1 = 10;
            decimal price1 = 50.00m;
            
            var serviceId2 = 20;
            decimal price2 = 30.00m;

            var serviceDate = DateTime.UtcNow.AddDays(1);
            var servicesIds = new[] { serviceId1, serviceId2 };

            var request = new CreateServiceRequest(
                PetId: petId,
                ServiceDate: serviceDate,
                AvailableServicesId: servicesIds,
                DiscountValue: 10.00m,
                AdditionalCharges: 5.00m);

            _servicePriceRepositoryMock
                .Setup(repo => repo.GetServicePriceByCompositeKeyAsync(serviceId1, petSizeId))
                .ReturnsAsync(ServicePrice.Create(serviceId1, petSizeId, price1));

            _servicePriceRepositoryMock
                .Setup(repo => repo.GetServicePriceByCompositeKeyAsync(serviceId2, petSizeId))
                .ReturnsAsync(ServicePrice.Create(serviceId2, petSizeId, price2));

            _serviceStatusRepositoryMock.Setup(r => r.GetServiceStatusByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(ServiceStatus.Create("Agendado"));
            _paymentStatusRepositoryMock.Setup(r => r.GetPaymentStatusByIdAsync(It.IsAny<int>()))
                .ReturnsAsync(PaymentStatus.Create("Pendente"));

            // Act
            var result = await _sut.CreateServiceAsync(request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(80.00m, result.Subtotal);
            Assert.Equal(75.00m, result.Total);

            _serviceRepositoryMock.Verify(
                repo => repo.AddService(It.IsAny<Service>()),
                Times.Once()
            );
        }

        [Fact]
        public async Task CreateServiceAsync_WhenPetDoesntExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            var petId = Guid.CreateVersion7();

            _petRepositoryMock
                .Setup(repo => repo.GetPetByIdAsync(petId))
                .ReturnsAsync((Pet?)null);

            var serviceDate = DateTime.UtcNow.AddDays(1);
            var servicesIds = new[] { 10, 20 };

            var request = new CreateServiceRequest(
                PetId: petId,
                ServiceDate: serviceDate,
                AvailableServicesId: servicesIds,
                DiscountValue: 10.00m,
                AdditionalCharges: 5.00m);

            // Act
            var action = async () => await _sut.CreateServiceAsync(request);

            // Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(action);
            Assert.Contains("Pet doesn't exist", exception.Message, StringComparison.OrdinalIgnoreCase);

            _petRepositoryMock.Verify(
                repo => repo.GetPetByIdAsync(petId),
                Times.Once()
            );

            _serviceStatusRepositoryMock.Verify(
                repo => repo.GetServiceStatusByIdAsync(It.IsAny<int>()),
                Times.Never()
            );

            _paymentStatusRepositoryMock.Verify(
                repo => repo.GetPaymentStatusByIdAsync(It.IsAny<int>()),
                Times.Never()
            );

            _serviceRepositoryMock.Verify(
                repo => repo.AddService(It.IsAny<Service>()),
                Times.Never()
            );
        }

        [Fact]
        public async Task CreateServiceAsync_WhenPriceIsNotSet_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var petSizeId = 1;
            var petSize = PetSize.Create("Pequeno").SetProperty(nameof(PetSize.Id), petSizeId);

            var petId = Guid.CreateVersion7();
            var pet = Pet.Create("Rex", breedId: 1, petSizeId: 1, DateTime.UtcNow.AddDays(-1))
                .SetProperty(nameof(Pet.Id), petId)
                .SetProperty(nameof(Pet.PetSize), petSize);

            var serviceId1 = 10;
            var serviceId2 = 20;

            var serviceDate = DateTime.UtcNow.AddDays(1);
            var servicesIds = new[] { serviceId1, serviceId2 };

            var request = new CreateServiceRequest(
                PetId: petId,
                ServiceDate: serviceDate,
                AvailableServicesId: servicesIds,
                DiscountValue: 10.00m,
                AdditionalCharges: 5.00m);


            _petRepositoryMock
                .Setup(repo => repo.GetPetByIdAsync(petId))
                .ReturnsAsync(pet);

            _servicePriceRepositoryMock
                .Setup(repo => repo.GetServicePriceByCompositeKeyAsync(serviceId1, petSizeId))
                .ReturnsAsync((ServicePrice?)null);

            _servicePriceRepositoryMock
                .Setup(repo => repo.GetServicePriceByCompositeKeyAsync(serviceId2, petSizeId))
                .ReturnsAsync((ServicePrice?)null);

            // Act
            var action = async () => await _sut.CreateServiceAsync(request);

            // Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(action);
            Assert.Contains("no price configured for the service", exception.Message, StringComparison.OrdinalIgnoreCase);

            _petRepositoryMock.Verify(
                repo => repo.GetPetByIdAsync(petId),
                Times.Once()
            );

            _servicePriceRepositoryMock.Verify(
                repo => repo.GetServicePriceByCompositeKeyAsync(It.IsAny<int>(), It.IsAny<int>()),
                Times.AtLeastOnce()
            );
        }

        #endregion

        #region UpdateServiceAsync tests

        [Fact]
        public async Task UpdateServiceAsync_WhenServiceDoesntExist_ShouldThrowKeyNotFoundException()
        {
            // Arrange
            _serviceRepositoryMock
                .Setup(repo => repo.GetServiceByUuidAsync(It.IsAny<Guid>()))
                .ReturnsAsync((Service?)null);

            var request = new UpdateServiceRequest(ServiceDate: DateTime.UtcNow.AddDays(2), new[] { 10, 20 });

            // Act
            var action = async () => await _sut.UpdateServiceAsync(It.IsAny<Guid>(), request);

            // Assert
            var exception = await Assert.ThrowsAsync<KeyNotFoundException>(action);
            Assert.Contains("Service doesn't exist", exception.Message, StringComparison.OrdinalIgnoreCase);

            _serviceRepositoryMock.Verify(
                repo => repo.GetServiceByUuidAsync(It.IsAny<Guid>()),
                Times.Once()
            );

            _serviceRepositoryMock.Verify(
                repo => repo.SaveChangesAsync(),
                Times.Never());
        }

        [Fact]
        public async Task UpdateServiceAsync_WhenServiceIsAlreadyPaid_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var serviceId = Guid.CreateVersion7();

            var availableServiceId = 10;
            var serviceItems = new List<ServiceItem>
            {
                ServiceItem.Create(availableServiceId, 15.00m, 1)
            };

            var service = Service.Create(serviceId, serviceDate: DateTime.UtcNow.AddDays(2), serviceItems).SetProperty(nameof(Service.PaymentStatusId), (int)PaymentStatusEnum.Pago);

            var request = new UpdateServiceRequest(ServiceDate: DateTime.UtcNow.AddDays(3), new[] { 10, 20 });

            _serviceRepositoryMock
                .Setup(repo => repo.GetServiceByUuidAsync(serviceId))
                .ReturnsAsync(service);

            // Act
            var action = async () => await _sut.UpdateServiceAsync(serviceId, request);

            // Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(action);
            Assert.Contains("service that has already been paid", exception.Message);
        }

        [Fact]
        public async Task UpdateServiceAsync_WhenServiceIsAlreadyCompleted_ShouldThrowInvalidOperationException()
        {
            // Arrange
            var serviceId = Guid.CreateVersion7();

            var availableServiceId = 10;
            var serviceItems = new List<ServiceItem>
            {
                ServiceItem.Create(availableServiceId, 15.00m, 1)
            };

            var service = Service.Create(serviceId, serviceDate: DateTime.UtcNow.AddDays(2), serviceItems).SetProperty(nameof(Service.ServiceStatusId), (int)ServiceStatusEnum.Concluido);

            var serviceDate = DateTime.UtcNow.AddDays(1);
            var servicesIds = new[] { 10, 20 };

            var request = new UpdateServiceRequest(serviceDate, servicesIds);

            _serviceRepositoryMock
                .Setup(repo => repo.GetServiceByUuidAsync(serviceId))
                .ReturnsAsync(service);

            // Act
            var action = async () => await _sut.UpdateServiceAsync(serviceId, request);

            // Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(action);
            Assert.Contains("not possible to change a service that has already been completed", exception.Message);

            _serviceRepositoryMock.Verify(
                repo => repo.GetServiceByUuidAsync(serviceId),
                Times.Once()
            );
        }

        [Fact]
        public async Task UpdateServiceAsync_WithNewItems_ShouldRecalculateTotalAndReplaceItems()
        {
            // Arrange
            var serviceId = Guid.CreateVersion7();

            var petSizeId = 1;
            var petSize = PetSize.Create("Medio").SetProperty(nameof(PetSize.Id), petSizeId);

            var animalTypeId = 1;
            var animalType = AnimalType.Create("Cachorro").SetProperty(nameof(AnimalType.Id), animalTypeId);

            var breedId = 1;
            var breed = Breed.Create("Shitzu", animalTypeId)
                .SetProperty(nameof(Breed.Id), breedId)
                .SetProperty(nameof(Breed.AnimalType), animalType);

            var petId = Guid.CreateVersion7();
            var pet = Pet.Create("Max", breedId, petSizeId, birthDate: null)
                .SetProperty(nameof(Pet.Id), petId)
                .SetProperty(nameof(Pet.PetSize), petSize)
                .SetProperty(nameof(Pet.Breed), breed);

            var serviceAvailableId1 = 50;
            var price1 = 35.50m;
            var serviceAvailableId2 = 100;
            var price2 = 79.90m;
            var servicesIds = new[] { serviceAvailableId1, serviceAvailableId2 };
            var request = new UpdateServiceRequest(DateTime.UtcNow.AddDays(4), servicesIds, DiscountValue: 0, AdditionalCharges: 10.00m);

            var availableServiceId1 = 10;
            var serviceItems = new List<ServiceItem>
            {
                ServiceItem.Create(availableServiceId1, 75.00m, 1)
            };

            var status = ServiceStatus.Create("Agendado", (int)ServiceStatusEnum.Agendado);
            var payment = PaymentStatus.Create("Pendente", (int)PaymentStatusEnum.Pendente);

            var service = Service.Create(Guid.CreateVersion7(), DateTime.UtcNow.AddDays(1), serviceItems)
                .SetProperty(nameof(Service.PaymentStatus), payment)
                .SetProperty(nameof(Service.ServiceStatus), status)
                .SetProperty(nameof(Service.Pet), pet);

            _serviceRepositoryMock
                .Setup(repo => repo.GetServiceByUuidAsync(serviceId))
                .ReturnsAsync(service);

            _serviceRepositoryMock
                .Setup(repo => repo.DeleteServiceItems(service.ServiceItems));

            _servicePriceRepositoryMock
                .Setup(repo => repo.GetServicePriceByCompositeKeyAsync(serviceAvailableId1, petSizeId))
                .ReturnsAsync(ServicePrice.Create(serviceAvailableId1, petSizeId, price1));

            _servicePriceRepositoryMock
                .Setup(repo => repo.GetServicePriceByCompositeKeyAsync(serviceAvailableId2, petSizeId))
                .ReturnsAsync(ServicePrice.Create(serviceAvailableId2, petSizeId, price2));

            // Act
            var result = await _sut.UpdateServiceAsync(serviceId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(115.40m, result.Subtotal);
            Assert.Equal(125.40m, result.Total);

            _serviceRepositoryMock.Verify(
                repo => repo.GetServiceByUuidAsync(serviceId),
                Times.Once()
            );

            _serviceRepositoryMock.Verify(
                repo => repo.DeleteServiceItems(service.ServiceItems),
                Times.Once()
            );

            _serviceRepositoryMock.Verify(
                repo => repo.SaveChangesAsync(),
                Times.Once()
            );
        }

        #endregion
    }
}
