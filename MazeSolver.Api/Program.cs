using FluentValidation;
using MazeSolver.Api.Middleware;
using MazeSolver.Api.Models;
using MazeSolver.Api.Validators;
using MazeSolver.Domain;
using MazeSolver.Domain.DataAccess;
using MazeSolver.Domain.Models;
using MazeSolver.Domain.Services;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            });
}

public class Startup
{
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddTransient<IMazeSolver, BFSMazeSolver>();
        services.AddScoped<IMazeService<Guid, string?>, MazeService>();
        services.AddScoped<IValidator<MazeRequestModel>, MazeRequestValidator>();
        services.AddSingleton<IRepository<MazeConfiguration, Guid>, BasicInMemoryMazeRepository>();

        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseRouting();

        // Custom middleware should be placed here
        app.UseMiddleware<TimeoutMiddleware>();
        app.UseMiddleware<ExceptionHandlerMiddleware>();

        app.UseAuthorization();
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }
}
