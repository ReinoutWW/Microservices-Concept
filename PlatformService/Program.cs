using Microsoft.EntityFrameworkCore;
using PlatformService.Data;
using PlatformService.DTO;
using PlatformService.Services;
using PlatformService.SyncDataServices.Http;

var builder = WebApplication.CreateBuilder(args);

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



var app = builder.Build();

PrepDb.PrepPopulation(app, app.Environment.IsProduction());

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

Console.WriteLine($"-- CommandService Endpoint {app.Configuration["CommandService"]}");

//app.UseHttpsRedirection();

RegisterPlatformAPI(app);

app.Run();

static void RegisterPlatformAPI(WebApplication app)
{
    app.MapGet("/platforms", (PlatformsService service) =>
    {
        return service.GetPlatforms();
    })
    .WithName("platforms")
    .WithOpenApi();

    app.MapGet("/platform/{id:int}", (int id, PlatformsService service) =>
    {
        var platform = service.GetPlaformById(id);
        return platform is not null
            ? Results.Ok(platform)
            : Results.NotFound($"Platform with ID {id} not found.");
    })
    .WithName("GetPlatformById")
    .WithOpenApi();

    app.MapPost("/platform", async (
        PlatformCreateDTO platform, 
        PlatformsService service, 
        ICommandDataClient commandDataClient 
    ) =>
    {
        PlatformReadDTO platformReadDto = service.CreatePlatform(platform);
        
        try {
            await commandDataClient.SendPlatformToCommand(platformReadDto);
        } catch(Exception ex) {
            Console.WriteLine($"-- Could not send synchronously: {ex.Message}");
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