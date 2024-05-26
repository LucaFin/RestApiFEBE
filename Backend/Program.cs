using Backend.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.WebHost.UseUrls("http://*:8080");

// Configure Serilog
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .WriteTo.File("logs/log-{Date}.txt", fileSizeLimitBytes: 10 * 1024 * 1024)
    .CreateLogger();

builder.Host.UseSerilog();

// Add services to the container.
builder.Services.AddHttpClient<IJsonPlaceholderService, JsonPlaceholderService>();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
});

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policy => policy
            .WithOrigins("http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

// Configure logging
builder.Services.AddSingleton(Log.Logger);
builder.Services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "API V1");
        c.RoutePrefix = "swagger";
    });

    app.MapGet("/", context =>
    {
        context.Response.Redirect("/swagger");
        return Task.CompletedTask;
    });
}

app.UseRouting();

app.UseCors("AllowSpecificOrigin"); // Utilizza la policy CORS configurata

app.UseAuthorization();

app.MapControllers();

app.Run();
