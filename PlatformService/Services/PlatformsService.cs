using AutoMapper;
using PlatformService.Data;
using PlatformService.DTO;
using PlatformService.Models;
using PlatformService.SyncDataServices.Http;

namespace PlatformService.Services
{
    public class PlatformsService 
    {
        private IPlatformRepo _repository;
        private IMapper _mapper;
        public PlatformsService(
            IPlatformRepo repository, 
            IMapper mapper
        )
        {
            _repository = repository;
            _mapper = mapper;  
        }

        public IEnumerable<PlatformReadDTO> GetPlatforms() 
        {
            Console.WriteLine("Gettings platforms");

            var platformItems = _repository.GetAllPlatforms();

            return _mapper.Map<IEnumerable<PlatformReadDTO>>(platformItems);
        }

        public PlatformReadDTO GetPlaformById(int id) 
        {
            Console.WriteLine($"Getting platform with id {id}");

            var platform = _repository.GetPlatformById(id);

            return _mapper.Map<PlatformReadDTO>(platform);
        }

        public PlatformReadDTO CreatePlatform(PlatformCreateDTO platformCreateDto) 
        {
            Platform platform = _mapper.Map<Platform>(platformCreateDto);

            _repository.CreatePlatform(platform);
            _repository.SaveChanges();

            PlatformReadDTO platformReadDto = _mapper.Map<PlatformReadDTO>(platform);

            return platformReadDto;
        }
    }
}