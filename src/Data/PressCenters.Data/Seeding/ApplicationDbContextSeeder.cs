namespace PressCenters.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Logging;

    public static class ApplicationDbContextSeeder
    {
        public static async Task Seed(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            if (dbContext == null)
            {
                throw new ArgumentNullException(nameof(dbContext));
            }

            if (serviceProvider == null)
            {
                throw new ArgumentNullException(nameof(serviceProvider));
            }

            var logger = serviceProvider.GetService<ILoggerFactory>().CreateLogger(typeof(ApplicationDbContextSeeder));

            var seeders = new List<ISeeder>
                          {
                              new RolesSeeder(),
                              new SourcesSeeder(),
                              new MainNewsSourcesSeeder(),
                          };

            foreach (var seeder in seeders)
            {
                await seeder.Seed(dbContext, serviceProvider);
                logger.LogInformation($"Seeder {seeder.GetType().Name} done.");
                dbContext.SaveChanges();
            }
        }
    }
}
