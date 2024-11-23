using AutoMapper;
using CommandService.Models;
using Grpc.Net.Client;
using PlatformService;

namespace CommandService.SyncDataServices.Grpc
{
    public class PlatformDataClient : IPlatformDataClient
    {
        private readonly IConfiguration _configuration;
        private readonly IMapper _mapper;

        public PlatformDataClient(IConfiguration configuration, IMapper mapper)
        {
            _configuration = configuration;
            _mapper = mapper;
        }

        public IEnumerable<Platform> ReturnAllPlatforms()
        {
            var endpoint = _configuration["GrpcPlatform"];
            Console.WriteLine($"-- Calling GRPC Service from {endpoint}");
            var channel = GrpcChannel.ForAddress(endpoint);
            var client = new GrpcPlatform.GrpcPlatformClient(channel);
            var request = new GetAllRequest();

            try
            {
                var reply = client.GetAllPlatforms(request);
                return _mapper.Map<IEnumerable<Platform>>(reply.Platform);
            } catch(Exception ex)
            {
                Console.WriteLine($"-- Could not retreive platforms from Grpc {endpoint}, {ex.Message}");
                return null;
            }
        }
    }
}