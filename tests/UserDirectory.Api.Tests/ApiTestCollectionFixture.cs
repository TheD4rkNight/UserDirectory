using Xunit;

namespace UserDirectory.Api.Tests;

// Renamed class to avoid ending with 'Collection' as per CA1711
[CollectionDefinition("API", DisableParallelization = true)]
public sealed class ApiTestCollectionFixture : ICollectionFixture<CustomWebApplicationFactory> { }
