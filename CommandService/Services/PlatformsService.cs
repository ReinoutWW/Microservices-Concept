using AutoMapper;
using CommandService.Data;
using CommandService.DTO;
using CommandService.Models;

namespace CommandService.Services
{
    public class PlatformsService
    {
        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;

        public PlatformsService(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public void TestInboundConnection() 
        {
            Console.WriteLine("-- Inbound POST in the CommandService service --");
        }

        public List<PlatformReadDTO> GetAllPlatforms() 
        {
            var platformItems = _repository.GetAllPlatforms();

            return _mapper.Map<IEnumerable<PlatformReadDTO>>(platformItems).ToList();
        }
    }
}