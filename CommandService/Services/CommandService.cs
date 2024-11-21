using AutoMapper;
using CommandService.Data;

namespace CommandService.Services
{
    public class CommandService 
    {
        private readonly ICommandRepo _repository;
        private readonly IMapper _mapper;

        public CommandService(ICommandRepo repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        
        
    }
}