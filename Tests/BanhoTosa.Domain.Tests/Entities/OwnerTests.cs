using API_Banho_Tosa.Domain.Entities;
using API_Banho_Tosa.Domain.ValueObjects;

namespace BanhoTosa.Domain.Tests.Entities
{
    public class OwnerTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void UpdateName_WithInvalidNullOrWhitespaceName_ShouldThrowArgumentNullException(string invalidName)
        {
            // Arrange
            var owner = Owner.Create(name: "Old name");

            // Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                owner.UpdateName(invalidName);
            });
        }

        [Fact]
        public void UpdateName_WithInvalidLength_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var owner = Owner.Create(name: "Old name");

            // Act
            string newName = new string('a', 256);

            // Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                owner.UpdateName(newName);
            });
        }

        [Fact]
        public void UpdateName_WithValidContent_ShouldChangeNameProperty()
        {
            // Arrange
            var owner = Owner.Create(name: "Old name");
            string expectedNewName = "New name correctly updated";

            // Act
            owner.UpdateName(expectedNewName);

            // Assert
            Assert.Equal(expectedNewName, owner.Name);
        }

        [Fact]
        public void UpdatePhone_WithNullContent_ShouldChangeValidPhoneToNullPhone()
        {
            // Arrange
            var validPhone = PhoneNumber.Create("(41) 98765-4321");
            var owner = Owner.Create(name: "Owner name", phone: validPhone);

            // Assert
            Assert.NotNull(owner.Phone);

            // Act
            owner.UpdatePhone(null);

            // Assert
            Assert.Null(owner.Phone);
        }

        [Fact]
        public void UpdateAddress_WithInvalidLength_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var owner = Owner.Create(name: "Owner name", address: "Owner address");

            // Act
            string invalidAddress = new string('a', 501);

            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                owner.UpdateAddress(invalidAddress);
            });
        }

        [Fact]
        public void UpdateAddress_WithValidContent_ShouldChangeAddressProperty()
        {
            // Arrange
            var owner = Owner.Create(name: "Owner name", address: "Owner old address");
            string expectedNewAddress = "New address correctly updated";

            // Act
            owner.UpdateAddress(expectedNewAddress);

            // Assert
            Assert.Equal(expectedNewAddress, owner.Address);
        }

        [Fact]
        public void UpdateAddress_WithNullContent_ShouldChangeValidAddressToNullAddress()
        {
            // Arrange
            var owner = Owner.Create(name: "Owner name", address: "Owner old address");

            // Assert
            Assert.NotNull(owner.Address);

            // Act
            owner.UpdateAddress(null);

            // Assert
            Assert.Null(owner.Address);
        }
    }
}
