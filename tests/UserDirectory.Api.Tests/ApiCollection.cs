namespace UserDirectory.Api.Tests;

[CollectionDefinition("API", DisableParallelization = true)]
public sealed class ApiCollection : ICollectionFixture<CustomWebApplicationFactory> { }
