using Memory_Cache.Model;
using Microsoft.AspNetCore.Mvc;

namespace Memory_Cache;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddSingleton<IMemCache, MemCache>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.MapGet("/cache/{key}", (string key, [FromServices] IMemCache cacheService)
            => cacheService.Get<object>(key) ?? Results.NotFound("No data found."));


        app.MapPost("/cache", async (CacheItem cacheItem, [FromServices] IMemCache cacheService) =>
        {
            if (string.IsNullOrEmpty(cacheItem.Key))
            {
                return Results.BadRequest("Invalid key or value.");
            }

            cacheService.Set(cacheItem.Key, cacheItem.Value, new CacheOptions { AbsoluteExpiration = TimeSpan.FromMinutes(5) });
            return Results.Ok("Cached successfully");
        });


        app.Run();
    }
}