using System.Text.Json;
using AutoMapper;
using CommandService.DTO;
using CommandService.EventProcessing.AddStrategy;

namespace CommandService.EventProcessing
{
    public class EventProcessor : IEventProcessor
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMapper _mapper;

        public EventProcessor(IServiceScopeFactory scopeFactory, IMapper mapper)
        {
            _scopeFactory = scopeFactory;
            _mapper = mapper;
        }

        public void ProcessEvent(string message)
        {
            var eventType = DetermineEvent(message);
            AddStrategyCoordinator addStrategyCoordinator = new();

            switch (eventType)
            {
                case EventType.PlatformPublished:
                    addStrategyCoordinator.SetStrategy(new AddPlatformStrategy(_scopeFactory));
                    addStrategyCoordinator.Add(message);
                    break;
                default:
                    break;                
            }
        }

        private EventType DetermineEvent(string notificationMessage)
        {
            Console.WriteLine("-- Detemining Event");

            var eventType = JsonSerializer.Deserialize<GenericEventDTO>(notificationMessage) ?? new GenericEventDTO();

            switch(eventType.Event)
            {
                case "Platform_Published":
                    Console.WriteLine("-- Platform Published Event Detected");
                    return EventType.PlatformPublished;
                default:
                    Console.WriteLine("-- Could not detemine event type");
                    return EventType.Undetermined;
            }
        }
    }

    enum EventType
    {
        PlatformPublished,
        Undetermined
    }
}