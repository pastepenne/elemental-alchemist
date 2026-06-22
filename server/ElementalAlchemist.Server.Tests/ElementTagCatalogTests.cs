using ElementalAlchemist.Server.Generation;
using Microsoft.Extensions.Options;
using Xunit;

namespace ElementalAlchemist.Server.Tests;

public class ElementTagCatalogTests
{
    [Fact]
    public void Constructor_MissingFile_ProducesEmptyCatalog()
    {
        // Arrange
        var options = Options.Create(new ContentOptions { TagsPath = "not-exist.json" });
        
        // Act
        var catalog = new ElementTagCatalog(options);

        // Assert
        Assert.Empty(catalog.Tags);
    }

    [Fact]
    public void Constructor_ValidFile_LoadsTags()
    {
        // Arrange
        var path = Path.GetTempFileName();
        File.WriteAllText(path, "[\"fire\", \"water\", \"earth\"]");
        var options = Options.Create(new ContentOptions { TagsPath = path });

        try
        {
            // Act
            var catalog = new ElementTagCatalog(options);

            // Assert
            Assert.True(catalog.Tags.SetEquals(["fire", "water", "earth"]));
        }
        finally
        {
            File.Delete(path);
        }
    }
}
