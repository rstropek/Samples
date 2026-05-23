using System.Text.Json;

namespace DataAccess.Tests;

[Trait("Category", "Integration")]
public class JsonFileRepositoryTests : IDisposable
{
    private readonly string testFolder;
    private readonly JsonFileRepository repository;
    private readonly RepositorySettings settings;

    public JsonFileRepositoryTests()
    {
        testFolder = Path.Combine(Path.GetTempPath(), $"JsonFileRepositoryTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(testFolder);
        settings = new RepositorySettings
        {
            DataFolder = testFolder,
            NumberOfRetries = 3,
            RetryDelayMilliseconds = 1
        };
        repository = new JsonFileRepository(settings);
    }

    public void Dispose()
    {
        if (Directory.Exists(testFolder))
        {
            Directory.Delete(testFolder, recursive: true);
        }

        GC.SuppressFinalize(this);
    }

    private async Task<(string Id, TestEntity Entity, Item item)> CreateTestEntity(string name = "Test", int value = 42)
    {
        var id = Guid.NewGuid().ToString("N");
        var entity = new TestEntity { Name = name, Value = value };
        var item = await repository.Create(id, entity).ConfigureAwait(false);
        return (id, entity, item);
    }

    [Fact]
    public async Task Create_ShouldPersistEntity()
    {
        // Arrange, Act
        var (id, testEntity, item) = await CreateTestEntity();

        // Assert
        Assert.Equal(id, item.Id);
        Assert.True(File.Exists(item.FilePath));
        var json = await File.ReadAllTextAsync(item.FilePath);
        var deserializedEntity = JsonSerializer.Deserialize<TestEntity>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        Assert.Equal(testEntity.Name, deserializedEntity?.Name);
        Assert.Equal(testEntity.Value, deserializedEntity?.Value);
    }

    [Fact]
    public async Task Get_ShouldRetrieveEntity()
    {
        // Arrange
        var (id, testEntity, _) = await CreateTestEntity();

        // Act
        await using var stream = await repository.Open(id, false);
        var retrievedEntity = await repository.Get<TestEntity>(stream!);

        // Assert
        Assert.NotNull(retrievedEntity);
        Assert.Equal(testEntity.Name, retrievedEntity.Name);
        Assert.Equal(testEntity.Value, retrievedEntity.Value);
    }

    [Fact]
    public async Task Update_ShouldModifyEntity()
    {
        // Arrange
        var (id, _, _) = await CreateTestEntity();

        // Act
        var updatedEntity = new TestEntity { Name = "Updated", Value = 100 };
        await using (var stream = await repository.Open(id, true))
        {
            await repository.Update(stream!, updatedEntity);
        }

        // Verify
        await using var verifyStream = await repository.Open(id, false);
        var retrievedEntity = await repository.Get<TestEntity>(verifyStream!);

        // Assert
        Assert.NotNull(retrievedEntity);
        Assert.Equal(updatedEntity.Name, retrievedEntity.Name);
        Assert.Equal(updatedEntity.Value, retrievedEntity.Value);
    }

    [Fact]
    public async Task Delete_ShouldRemoveEntity()
    {
        // Arrange
        var (id, _, item) = await CreateTestEntity();

        // Act
        repository.Delete(id);

        // Assert
        Assert.False(File.Exists(item.FilePath));
    }

    [Fact]
    public async Task EnumerateAll_ShouldListAllEntities()
    {
        // Arrange
        var entities = Enumerable.Range(1, 3)
            .Select(i => (
                Guid.NewGuid().ToString("N"),
                new TestEntity { Name = $"Test{i}", Value = i }
            ))
            .ToList();

        foreach (var (id, entity) in entities)
        {
            await repository.Create(id, entity);
        }

        // Act
        var items = repository.EnumerateAll().ToList();

        // Assert
        Assert.Equal(entities.Count, items.Count);
        foreach (var (id, _) in entities)
        {
            Assert.Contains(items, item => item.Id == id);
        }
    }

    [Fact]
    public async Task Open_NonexistentEntity_ShouldReturnNull()
    {
        // Arrange
        var id = Guid.NewGuid().ToString("N");

        // Act
        var stream = await repository.Open(id, false);

        // Assert
        Assert.Null(stream);
    }

    [Fact]
    public async Task Open_WithWriteLock_ShouldPreventConcurrentAccess()
    {
        // Arrange
        var id = Guid.NewGuid().ToString("N");
        var testEntity = new TestEntity { Name = "Test", Value = 42 };
        await repository.Create(id, testEntity);

        // Act
        await using var firstStream = await repository.Open(id, true);
        var secondStream = await repository.Open(id, true);

        // Assert
        Assert.NotNull(firstStream);
        Assert.Null(secondStream); // Second attempt should fail due to file lock
    }

    [Fact]
    public async Task Create_WithNullId_ShouldThrowArgumentNullException()
    {
        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(() => repository.Create(null!, (TestEntity)null!));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("\t")]
    public async Task Create_WithInvalidId_ShouldThrowArgumentException(string invalidId)
    {
        // Arrange
        var testEntity = new TestEntity { Name = "Test", Value = 42 };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => repository.Create(invalidId, testEntity));
    }
}

public class TestEntity
{
    public required string Name { get; set; }
    public int Value { get; set; }
}