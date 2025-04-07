using Draughts.Core.Engine;
using Draughts.Core.Service;
using Draughts.Infrastructure.Repositories;

namespace Draughts.API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        // Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Register dependencies
        builder.Services.AddSingleton<IGameRepository, InMemoryGameRepository>();
        builder.Services.AddSingleton<IGameEngine, MinimaxEngine>();
        builder.Services.AddScoped<IGameService, GameService>();

        // CORS policy
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder => builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
        });

        var app = builder.Build();

        // Enable Swagger
        app.UseSwagger();
        app.UseSwaggerUI();

        app.UseCors("AllowAll");

        //  app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}