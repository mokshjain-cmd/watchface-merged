using Microsoft.AspNetCore.Mvc;
using System.Text;
using Newtonsoft.Json;
using WatchControlLibrary.Model;
using WatchBin.Model;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS policy for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins(
                "http://localhost:5173", // Vite default
                "http://localhost:5174", // ZHJL Frontend
                "http://localhost:5175", // ZhouHai Frontend
                "http://localhost:3002", // Alternative Vite port
                "http://localhost:3003", // React dev server
                "http://localhost:5000", // Alternative port
                "http://localhost:4173"  // Vite preview
            )
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();