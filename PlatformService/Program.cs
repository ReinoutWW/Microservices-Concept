using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PlatformService.AsyncDataServices;
using PlatformService.Data;
using PlatformService.DTO;
using PlatformService.Services;
using PlatformService.SyncDataServices.Grpc;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

// builder.WebHost.ConfigureKestrel(options =>
// {
//     // HTTP/2 for gRPC on port 777
//     options.ListenLocalhost(777, listenOptions =>
//     {
//         listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http2;
//         listenOptions.UseHttps(); // gRPC requires HTTPS
//     });

//     // HTTP/1.1 for Minimal API on port 5086
//     options.ListenLocalhost(5086, listenOptions =>
//     {
//         listenOptions.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1;
//     });
// });

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if(builder.Environment.IsProduction()) {
    Console.WriteLine("-- Production DB SqlServer");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseSqlServer(builder.Configuration.GetConnectionString("PlatformsConn")));
}
else {
    Console.WriteLine("-- Dev DB InMemmory");
    builder.Services.AddDbContext<AppDbContext>(opt =>
        opt.UseInMemoryDatabase("InMem"));
}

builder.Services.AddScoped<IPlatformRepo, PlatformRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddScoped<PlatformsService>();
builder.Services.AddHttpClient<ICommandDataClient, HttpCommandDataClient>();
builder.Services.AddSingleton<IMessageBusClient, MessageBusClient>();
builder.Services.AddGrpc();

var app = builder.Build();

PrepDb.PrepPopulation(app, app.Environment.IsProduction());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

Console.WriteLine($"-- CommandService Endpoint {app.Configuration["CommandService"]}");

RegisterPlatformAPI(app);

app.MapGrpcService<GrpcPlatformServie>();
app.MapGet("/protos/platforms.proto", async context =>
{
    await context.Response.WriteAsync(File.ReadAllText("Protos/platforms.proto"));
});

app.Run();

static void RegisterPlatformAPI(WebApplication app)
{
    app.MapGet("/platforms", (PlatformsService service) =>
    {
        return service.GetPlatforms();
    })
    .WithName("platforms")
    .WithOpenApi();

    app.MapGet("/platforms/{id:int}", (int id, PlatformsService service) =>
    {
        var platform = service.GetPlaformById(id);
        return platform is not null
            ? Results.Ok(platform)
            : Results.NotFound($"Platform with ID {id} not found.");
    })
    .WithName("GetPlatformById")
    .WithOpenApi();

    app.MapPost("/platforms", async (
        PlatformCreateDTO platform, 
        PlatformsService service, 
        ICommandDataClient commandDataClient,
        IMapper mapper,
        IMessageBusClient messageBusClient
    ) =>
    {
        PlatformReadDTO platformReadDto = service.CreatePlatform(platform);
        
        // Send Sync Message
        try {
            await commandDataClient.SendPlatformToCommand(platformReadDto);
        } catch(Exception ex) {
            Console.WriteLine($"-- Could not send synchronously: {ex.Message}");
        }

        // Send Async message
        try
        {
            var platformPublishedDTO = mapper.Map<PlatformPublishedDTO>(platformReadDto);
            platformPublishedDTO.Event = "Platform_Published";
            messageBusClient.PublishNewPlatform(platformPublishedDTO);
        } catch(Exception ex)
        {
            Console.WriteLine($"-- Could not send asynchronously: {ex.Message}");
        }

        return Results.CreatedAtRoute(
            "GetPlatformById",
            new { id = platformReadDto.Id },
            platformReadDto
        );
    })
    .WithName("CreatePlatform")
    .WithOpenApi();
}