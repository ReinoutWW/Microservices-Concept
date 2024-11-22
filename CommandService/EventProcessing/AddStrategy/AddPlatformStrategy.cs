using System.Text.Json;
using AutoMapper;
using CommandService.Data;
using CommandService.DTO;
using CommandService.Models;

namespace CommandService.EventProcessing.AddStrategy
{
    public class AddPlatformStrategy : IAddStrategy
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public AddPlatformStrategy(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        public void Add(string platformPublishedMessage)
        {
            using (var scope = _scopeFactory.CreateScope())
            {
                var repo = scope.ServiceProvider.GetRequiredService<ICommandRepo>();
                var mapper = scope.ServiceProvider.GetRequiredService<IMapper>();

                var platformPublishedDTO = JsonSerializer.Deserialize<PlatformPublishedDTO>(platformPublishedMessage);

                try
                {
                    AddPlatform(repo, mapper, platformPublishedDTO);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("-- Could not add Platform to DB");
                }
            }
        }

        private static void AddPlatform(ICommandRepo repo, IMapper mapper, PlatformPublishedDTO? platformPublishedDTO)
        {
            var platformModel = mapper.Map<Platform>(platformPublishedDTO);
            if (!repo.ExternalPlatformExists(platformModel.ExternalID))
            {
                repo.CreatePlatform(platformModel);
                repo.SaveChanges();
                Console.WriteLine("-- Persisted platform");
            }
            else
            {
                Console.WriteLine();
            }
        }
    }
}