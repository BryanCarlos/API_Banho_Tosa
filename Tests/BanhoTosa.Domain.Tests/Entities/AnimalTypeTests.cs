using API_Banho_Tosa.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BanhoTosa.Domain.Tests.Entities
{
    public class AnimalTypeTests
    {
        [Fact]
        public void AnimalType_WithValidInputs_ShouldCreateNewAnimalTypeInstance()
        {
            // Arrange
            string type = "Cachorro";

            // Act
            var animalType = new AnimalType(type);

            // Assert
            Assert.NotNull(animalType);
            Assert.NotEmpty(animalType.Name);
            Assert.Equal(type, animalType.Name);
        }

        [Fact]
        public void UpdateName_WithValidInput_ShouldUpdateAnimalTypeName()
        {
            // Arrange
            string initialName = "Gato";
            string updatedName = "Coelho";

            DateTime now = DateTime.UtcNow;

            var animalType = new AnimalType(initialName);

            // Act
            animalType.UpdateName(updatedName);

            // Assert
            Assert.InRange(animalType.UpdatedAt, now.Subtract(TimeSpan.FromMilliseconds(500)), now.AddMilliseconds(500));
            Assert.NotEqual(initialName, animalType.Name);
            Assert.Equal(updatedName, animalType.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void UpdateName_WithNullOrInvalidInput_ShouldThrowArgumentNullException(string input)
        {
            // Arrange
            var animalType = new AnimalType(name: "Gato");

            // Act & Assert
            Assert.Throws<ArgumentNullException>(() =>
            {
                animalType.UpdateName(input);
            });
        }

        [Fact]
        public void UpdateName_WithInvalidInputLength_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var animalType = new AnimalType(name: "Cachorro");
            string invalidInput = new string('a', 101);

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                animalType.UpdateName(invalidInput);
            });
        }
    }
}
