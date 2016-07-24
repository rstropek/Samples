namespace AspNetCore1Angular2Intro.Services
{
    /// <summary>
    /// Defines the interface for name generators.
    /// </summary>
    /// <remarks>
    /// Is an interface to enable mocking in unit tests.
    /// </remarks>
    public interface INameGenerator
    {
        string GenerateRandomBookTitle();
    }
}
