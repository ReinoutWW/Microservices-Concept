using AutoMapper;
using CommandService.Data;

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
    }
}