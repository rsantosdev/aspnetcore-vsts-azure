using Microsoft.EntityFrameworkCore;

namespace AspnetCoreXunitVsts.Api.Models
{
    public class ApiContext : DbContext
    {
        public ApiContext(DbContextOptions<ApiContext> options) : base(options)
        {

        }

        public DbSet<Person> People { get; set; }
    }
}