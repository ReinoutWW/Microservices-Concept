using CommandService.Data;
using CommandService.DTO;
using CommandService.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<PlatformsService>();
builder.Services.AddScoped<CommandService.Services.CommandService>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseInMemoryDatabase("InMem"));

builder.Services.AddScoped<ICommandRepo, CommandRepo>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

var baseUrl = "/c/platforms";


app.MapPost("/c/testinboundconnection", (PlatformsService service) =>
{
    service.TestInboundConnection();
    return Results.Ok("Inbound test ok from Platforms controller");
}).WithOpenApi();;


app.MapGet(baseUrl, (PlatformsService service) => {
    Console.WriteLine("Gettings platforms from CommandService");

    var platformItems = service.GetAllPlatforms();

    return Results.Ok(platformItems);
}).WithOpenApi();;


app.MapGet(baseUrl + "/{id:int}/commands", (int id, [FromServices] CommandService.Services.CommandService service) =>
{
    try
    {
        Console.WriteLine($"-- Hit GetCommandsForPlatform: {id}");

        // Get platform commands from service
        var platformCommands = service.GetCommandsForPlatform(id);

        return Results.Ok(platformCommands);
    }
    catch (KeyNotFoundException ex)
    {
        Console.WriteLine($"KeyNotFoundException: {ex.Message}");
        return Results.NotFound(new { message = "Platform not found", id });
    }
}).WithName("GetCommandsByPlatformId").WithOpenApi();;

app.MapGet(baseUrl + "/{platformId:int}/commands/{commandId:int}", (int platformId, int commandId, [FromServices] CommandService.Services.CommandService service) =>
{
    try
    {
        Console.WriteLine($"-- Hit GetCommandForPlatform: {platformId} {commandId}");

        // Get platform commands from service
        var command = service.GetCommandForPlatform(platformId, commandId);

        return Results.Ok(command);
    }
    catch (KeyNotFoundException ex)
    {
        Console.WriteLine($"KeyNotFoundException: {ex.Message}");
        return Results.NotFound(new { message = $"Command ({commandId}) not found for Platform ({platformId}) not found" });
    }
}).WithName("GetCommandForPlatform").WithOpenApi();;

app.MapPost(baseUrl + "/{platformId}/commands", async (int platformId, [FromBody] CommandCreateDTO commandDto, [FromServices] CommandService.Services.CommandService service) =>
{
    try
    {
        Console.WriteLine($"Creating a new command for platform with ID {platformId}");

        // Use the service to create the command
        var createdCommand = service.CreateCommandForPlatform(platformId, commandDto);

        return Results.CreatedAtRoute(
            $"{baseUrl}/{platformId}/command/{createdCommand.Id}", 
            new { platformId = platformId, commandId = createdCommand.Id }, createdCommand);
    }
    catch (KeyNotFoundException ex)
    {
        Console.WriteLine($"KeyNotFoundException: {ex.Message}");
        return Results.NotFound(new { message = "Platform not found", platformId });
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
        return Results.Problem("An error occurred while creating the command.");
    }
});






app.Run();