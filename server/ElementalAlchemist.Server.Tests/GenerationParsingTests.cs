using ElementalAlchemist.Server.Generation;
using ElementalAlchemist.Shared;
using Xunit;

namespace ElementalAlchemist.Server.Tests;

public class GenerationParsingTests
{
    [Fact]
    public void Parse_JsonWrappedInExtraText_ExtractsJsonObject()
    {
        // Arrange
        const string raw = "Here you go: ```{\"possible\": true, \"id\": \"molten-glass\"}``` hope that helps.";

        // Act
        var result = GenerationParsing.Parse(raw);

        // Assert
        Assert.True(result.Possible);
        Assert.Equal("molten-glass", result.Id);
    }

    [Fact]
    public void Parse_NoJsonObject_ThrowsGeneratorException()
    {
        // Act
        var act = () => GenerationParsing.Parse("invalid response");
        
        // Assert
        Assert.Throws<GeneratorException>(act);
    }

    [Fact]
    public void Parse_MalformedJson_ThrowsGeneratorException()
    {
        // Act
        var act = () => GenerationParsing.Parse("{\"possible\": tru");
        
        // Assert
        Assert.Throws<GeneratorException>(act);
    }
    
    [Fact]
    public void TryBuildDefinition_ValidInput_ReturnsTrue()
    {
        // Arrange
        var result = Valid();

        // Act
        var ok = GenerationParsing.TryBuildDefinition(result, ValidTags, out var definition);

        // Assert
        Assert.True(ok);
        Assert.NotNull(definition);
        Assert.Equal(result.Id, definition.Id);
        Assert.Equal(ElementTier.Exotic, definition.Tier);
        Assert.Equal(["glass", "lava"], definition.Tags);
    }

    [Theory]
    [InlineData("Molten-Glass")]
    [InlineData("molten_glass")]
    [InlineData("molten glass")]
    [InlineData("-molten-glass")]
    [InlineData("molten-glass-")]
    [InlineData("glass2")]
    [InlineData("")]
    public void TryBuildDefinition_InvalidId_ReturnsFalse(string id)
    {
        // Arrange
        var result = Valid() with { Id = id };

        // Act
        var ok = GenerationParsing.TryBuildDefinition(result, ValidTags, out var definition);
        
        // Assert
        Assert.False(ok);
        Assert.Null(definition);
    }

    [Fact]
    public void TryBuildDefinition_MissingName_ReturnsFalse()
    {
        // Arrange
        var result = Valid() with { Name = "" };

        // Act
        var ok = GenerationParsing.TryBuildDefinition(result, ValidTags, out _);
        
        // Assert
        Assert.False(ok);
    }

    [Fact]
    public void TryBuildDefinition_MissingDescription_ReturnsFalse()
    {
        // Arrange
        var result = Valid() with { Description = "" };

        // Act
        var ok = GenerationParsing.TryBuildDefinition(result, ValidTags, out _);
        
        // Assert
        Assert.False(ok);
    }

    [Fact]
    public void TryBuildDefinition_SomeInvalidTags_RemovesThem()
    {
        // Arrange
        var result = Valid() with { Tags = ["glass", "invalid"] };

        // Act
        var ok = GenerationParsing.TryBuildDefinition(result, ValidTags, out var definition);

        // Assert
        Assert.True(ok);
        Assert.NotNull(definition);
        Assert.Equal(["glass"], definition.Tags);
    }

    [Fact]
    public void TryBuildDefinition_MoreThanTwoValidTags_KeepsOnlyTwo()
    {
        // Arrange
        var result = Valid() with { Tags = ["glass", "lava", "fire"] };

        // Act
        var ok = GenerationParsing.TryBuildDefinition(result, ValidTags, out var definition);

        // Assert
        Assert.True(ok);
        Assert.NotNull(definition);
        Assert.Equal(2, definition.Tags.Length);
        Assert.Equal(["glass", "lava"], definition.Tags);
    }

    [Fact]
    public void TryBuildDefinition_DuplicateTags_RemovesDuplicates()
    {
        // Arrange
        var result = Valid() with { Tags = ["glass", "glass"] };

        // Act
        var ok = GenerationParsing.TryBuildDefinition(result, ValidTags, out var definition);

        // Assert
        Assert.True(ok);
        Assert.NotNull(definition);
        Assert.Equal(["glass"], definition.Tags);
    }

    [Fact]
    public void TryBuildDefinition_NoValidTagsRemain_ReturnsFalse()
    {
        // Arrange
        var result = Valid() with { Tags = ["invalid", "not-exist"] };

        // Act
        var ok = GenerationParsing.TryBuildDefinition(result, ValidTags, out var definition);
        
        // Assert
        Assert.False(ok);
        Assert.Null(definition);
    }

    [Fact]
    public void TryBuildDefinition_NullTags_ReturnsFalse()
    {
        // Arrange
        var result = Valid() with { Tags = null };

        // Act
        var ok = GenerationParsing.TryBuildDefinition(result, ValidTags, out _);
        
        // Assert
        Assert.False(ok);
    }
 
    // The tag whitelist the server would load from the exported tag library.
    private static readonly IReadOnlySet<string> ValidTags =
        new HashSet<string>(StringComparer.Ordinal) { "glass", "lava", "fire" };

    private static GenerationResult Valid() => new()
    {
        Possible = true,
        Id = "molten-glass",
        Name = "Molten Glass",
        Description = "Glowing liquid silica.",
        Tags = ["glass", "lava"],
    };
}
