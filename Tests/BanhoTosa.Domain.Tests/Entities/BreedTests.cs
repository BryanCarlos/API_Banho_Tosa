using API_Banho_Tosa.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace BanhoTosa.Domain.Tests.Entities
{
    public class BreedTests
    {
        [Fact]
        public void Breed_WithValidInputs_ShouldCreateBreedInstance()
        {
            // Arrange
            string breedName = "Buldogue";
            int animalTypeId = 1;

            // Act
            var breed = Breed.Create(breedName, animalTypeId);

            // Assert
            Assert.NotNull(breed);
            Assert.Equal(breedName, breed.Name);
            Assert.Equal(animalTypeId, breed.AnimalTypeId);
        }

        [Fact]
        public void UpdateName_WithValidInputName_ShouldUpdateBreedName()
        {
            // Arrange
            string breedName = "Buldogue";
            int animalTypeId = 1;
            var breed = Breed.Create(breedName, animalTypeId);

            string newName = "Pitbull";

            // Act
            breed.UpdateName(newName);

            // Assert
            Assert.NotNull(breed);
            Assert.NotEqual(breedName, breed.Name);
            Assert.NotEqual(breed.CreatedAt, breed.UpdatedAt);
            Assert.Equal(newName, breed.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("    ")]
        public void UpdateName_WithInvalidName_ShouldThrowArgumentException(string input)
        {
            // Arrange
            var breed = Breed.Create(name: "Lhasa Apso", animalTypeId: 1);

            // Act & Assert
            var exception = Assert.Throws<ArgumentException>(() => breed.UpdateName(input));

            Assert.Equal("The breed name cannot be null or empty.", exception.Message);
        }

        [Fact]
        public void UpdateName_WithInvalidInputLength_ShouldThrowArgumentOutOfRangeException()
        {
            // Arrange
            var breed = Breed.Create(name: "Siamês", animalTypeId: 2);
            string invalidName = new string('a', 101);

            // Act & Assert
            var exception = Assert.Throws<ArgumentOutOfRangeException>(() => breed.UpdateName(invalidName));

            Assert.Equal("The breed name cannot exceed 100 characters. (Parameter 'Name')", exception.Message);
        }
    }
}
