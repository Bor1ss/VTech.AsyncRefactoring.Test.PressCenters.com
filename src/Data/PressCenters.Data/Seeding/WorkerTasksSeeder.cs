﻿namespace PressCenters.Data.Seeding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using PressCenters.Data.Models;

    public class WorkerTasksSeeder : ISeeder
    {
        public void Seed(ApplicationDbContext dbContext, IServiceProvider serviceProvider)
        {
            var workerTasks = new List<WorkerTask>
                              {
                                  new WorkerTask
                                  {
                                      TypeName = "PressCenters.Worker.Tasks.DbCleanupTask",
                                      Parameters = "{\"Recreate\":true}",
                                      Priority = 0,
                                  },
                                  new WorkerTask
                                  {
                                      TypeName = "PressCenters.Worker.Tasks.MainNewsGetterTask",
                                      Parameters = "{\"Recreate\":true}",
                                      Priority = 10000,
                                  },
                              };

            foreach (var workerTask in workerTasks)
            {
                if (!dbContext.WorkerTasks.Any(x => x.TypeName == workerTask.TypeName))
                {
                    dbContext.WorkerTasks.Add(workerTask);
                }
            }
        }
    }
}
