using API_Banho_Tosa.Domain.ValueObjects;

namespace BanhoTosa.Domain.Tests.ValueObjects
{
    public class PhoneNumberTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void Create_WithNullContent_ShouldThrowArgumentNullException(string invalidNumber)
        {
            Assert.Throws<ArgumentNullException>(() =>
            {
                PhoneNumber.Create(invalidNumber);
            });
        }

        [Fact]
        public void Create_WithInvalidLength_ShouldThrowArgumentException()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                PhoneNumber.Create("123");
            });
        }

        [Fact]
        public void Create_WithValidNumber_ShouldCreateInstanceSuccessfully()
        {
            // Arrange
            string inputPhoneNumber = "(41) 98765-4321";
            string expectedPhoneNumber = "41987654321";

            // Act
            PhoneNumber phone = PhoneNumber.Create(inputPhoneNumber);

            // Assert
            Assert.NotNull(phone);
            Assert.Equal(expectedPhoneNumber, phone.Value);
        }
    }
}
