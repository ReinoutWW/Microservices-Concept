using PlatformService.Models;

namespace PlatformService.Data 
{
    // This wouldn't normally push to production
    // Only for testing purposes

    public static class PrepDb
    {
        public static void PrepPopulation(IApplicationBuilder app) 
        {
            using(var serviceScope = app.ApplicationServices.CreateScope()) 
            {
                SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>()!);        
            }
        }

        private static void SeedData(AppDbContext context) 
        {
            if(!context.Platforms.Any()) 
            {
                Console.WriteLine("-- Seeding Data --");

                context.Platforms.AddRange(
                    new Platform() { Name = ".NET", Publisher="Microsoft", Cost="Free" },
                    new Platform() { Name = "DotNetNuke", Publisher="Microsoft", Cost="Free" },
                    new Platform() { Name = "SQL Server", Publisher="Microsoft", Cost="1.000.000 per day" },
                    new Platform() { Name = "Azure", Publisher="Microsoft", Cost="2 livers per day" }
                );

                context.SaveChanges();
            }
            else 
            {
                Console.WriteLine("-- We already have data --");
            }
        }
    }
}