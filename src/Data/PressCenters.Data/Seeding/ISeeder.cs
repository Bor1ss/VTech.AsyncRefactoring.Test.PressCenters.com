namespace PressCenters.Data.Seeding
{
    using System;
    using System.Threading.Tasks;

    public interface ISeeder
    {
        Task Seed(ApplicationDbContext dbContext, IServiceProvider serviceProvider);
    }
}
