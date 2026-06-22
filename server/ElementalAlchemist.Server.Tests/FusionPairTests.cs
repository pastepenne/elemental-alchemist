using ElementalAlchemist.Shared;
using Xunit;

namespace ElementalAlchemist.Server.Tests;

public class FusionPairTests
{
    [Fact]
    public void Constructor_ElementsInReverseOrder_SortsOrder()
    {
        // Act
        var pair = new FusionPair("water", "fire");

        // Assert
        Assert.Equal("fire", pair.ElementA);
        Assert.Equal("water", pair.ElementB);
    }

    [Fact]
    public void Constructor_ElementsAlreadyInOrder_KeepsOrder()
    {
        // Act
        var pair = new FusionPair("fire", "water");

        // Assert
        Assert.Equal("fire", pair.ElementA);
        Assert.Equal("water", pair.ElementB);
    }

    [Fact]
    public void Equals_SameElementsInDifferentOrder_AreEqual()
    {
        // Arrange
        var ab = new FusionPair("fire", "water");
        var ba = new FusionPair("water", "fire");

        // Assert
        Assert.Equal(ab, ba);
        Assert.Equal(ab.GetHashCode(), ba.GetHashCode());
    }

    [Fact]
    public void Equals_DifferentElements_AreNotEqual()
    {
        // Arrange
        var fireWater = new FusionPair("fire", "water");
        var fireEarth = new FusionPair("fire", "earth");

        // Assert
        Assert.NotEqual(fireWater, fireEarth);
    }
}
