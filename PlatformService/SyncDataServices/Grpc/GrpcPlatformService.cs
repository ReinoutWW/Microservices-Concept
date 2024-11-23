using AutoMapper;
using Grpc.Core;
using PlatformService.Data;
using PlatformService.Models;

namespace PlatformService.SyncDataServices.Grpc
{
    public class GrpcPlatformServie : GrpcPlatform.GrpcPlatformBase
    {
        private readonly IPlatformRepo _repository;
        private readonly IMapper _mapper;

        public GrpcPlatformServie(IPlatformRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
            Console.WriteLine("-- GrpcPlatformService configured!");
        }

        public override Task<PlatformResponse> GetAllPlatforms(GetAllRequest request, ServerCallContext context)
        {
            var response = new PlatformResponse();
            var platforms = _repository.GetAllPlatforms();

            foreach(var platform in platforms)
            {
                response.Platform.Add(_mapper.Map<GrpcPlatformModel>(platform));
            }

            return Task.FromResult(response);
        }
    }
}