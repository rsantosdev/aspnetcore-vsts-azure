using Xunit;

namespace AspnetCoreXunitVsts.IntegrationTests.Configuration
{
    [CollectionDefinition("Base collection")]
    public abstract class BaseTestCollection : ICollectionFixture<BaseTestFixture>
    {
    }
}