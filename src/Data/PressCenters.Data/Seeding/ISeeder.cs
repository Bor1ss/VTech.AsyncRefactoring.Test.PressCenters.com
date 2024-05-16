namespace PressCenters.Data.Seeding
{
    using System.Threading.Tasks;

    using System;

    public interface ISeeder
    {
        Task SeedAsync(ApplicationDbContext dbContext, IServiceProvider serviceProvider);
    }
}
