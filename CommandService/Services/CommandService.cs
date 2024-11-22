using AutoMapper;
using CommandService.Data;
using CommandService.DTO;
using CommandService.Models;

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
        
        public IEnumerable<CommandReadDTO> GetCommandsForPlatform(int platformId) 
        {
            if(!_repository.PlatformExists(platformId)) 
            {
                throw new KeyNotFoundException(platformId.ToString());
            }

            var commands = _repository.GetCommandsForPlatform(platformId);

            return _mapper.Map<IEnumerable<CommandReadDTO>>(commands);
        }

        public CommandReadDTO GetCommandForPlatform(int platformId, int commandId)
        {
            if(!_repository.PlatformExists(platformId)) 
            {
                throw new KeyNotFoundException(platformId.ToString());
            }

            var command = _repository.GetCommand(platformId, commandId);

            if(command == null) 
            {
                throw new KeyNotFoundException(platformId.ToString());
            }

            return _mapper.Map<CommandReadDTO>(command);
        }

        public CommandReadDTO CreateCommandForPlatform(int platformId, CommandCreateDTO commandDTO)
        {
            if(!_repository.PlatformExists(platformId)) 
            {
                throw new KeyNotFoundException(platformId.ToString());
            }

            var commandModel = _mapper.Map<Command>(commandDTO);

            _repository.CreateCommand(platformId, commandModel);
            _repository.SaveChanges();

            var commandReadDTO = _mapper.Map<CommandReadDTO>(commandModel);
            return commandReadDTO;
        }
    }
}