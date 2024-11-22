using System.Text;
using System.Text.Json;
using PlatformService.AsyncDataServices;
using PlatformService.DTO;

namespace PlatformService.SyncDataServices.Http
{
    public class HttpCommandDataClient : ICommandDataClient
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly IMessageBusClient _messageBusClient;

        public HttpCommandDataClient(
            HttpClient httpClient, 
            IConfiguration configuration,
            IMessageBusClient messageBusClient
        )
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _messageBusClient = messageBusClient;
        }

        public async Task SendPlatformToCommand(PlatformReadDTO plat)
        {
            var httpContent = new StringContent(
                JsonSerializer.Serialize(plat),
                Encoding.UTF8,
                "application/json");

            var response = await _httpClient.PostAsync($"{_configuration["CommandService"]}", httpContent);

            if(response.IsSuccessStatusCode) 
            {
                Console.WriteLine("-- Sync POST to CommandService was OK");
            } else {
                Console.WriteLine("-- Sync POST to CommandService was NOT OK");
            }
        }
    }
}