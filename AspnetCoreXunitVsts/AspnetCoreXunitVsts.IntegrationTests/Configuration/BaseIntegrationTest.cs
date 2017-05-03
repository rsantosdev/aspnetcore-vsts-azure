using System.Net.Http;
using System.Threading.Tasks;
using AspnetCoreXunitVsts.Api.Models;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace AspnetCoreXunitVsts.IntegrationTests.Configuration
{
    [Collection("Base collection")]
    public abstract class BaseIntegrationTest
    {
        protected readonly TestServer Server;
        protected readonly HttpClient Client;
        protected readonly ApiContext TestDbContext;

        protected BaseTestFixture Fixture { get; }

        protected BaseIntegrationTest(BaseTestFixture fixture)
        {
            Fixture = fixture;

            TestDbContext = Fixture.TestDbContext;
            Server = Fixture.Server;
            Client = Fixture.Client;

            ClearDb().Wait();
        }

        private async Task ClearDb()
        {
            var commands = new[]
            {
                "EXEC sp_MSForEachTable 'ALTER TABLE ? NOCHECK CONSTRAINT ALL'",
                "EXEC sp_MSForEachTable 'DELETE FROM ?'",
                "EXEC sp_MSForEachTable 'ALTER TABLE ? CHECK CONSTRAINT ALL'"
            };

            await TestDbContext.Database.OpenConnectionAsync();

            foreach (var command in commands)
            {
                await TestDbContext.Database.ExecuteSqlCommandAsync(command);
            }

            TestDbContext.Database.CloseConnection();
        }
    }
}
