
using Ct.Gadgets.SignalR.Host.SignalR;
using CT.Application.Interfaces;

namespace Ct.Gadgets.SignalR.Host
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // ----------------------
            // Add services
            // ----------------------
            builder.Services.AddControllers();

            // Swagger/OpenAPI
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            //string allowedOrigin = "http://localhost:7230";
            string allowedOrigin = "http://172.18.160.1:8080";

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowSpecificOrigin", policy =>
                {
                    policy.WithOrigins(allowedOrigin) // must be exact origin
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // SignalR requires credentials for WebSockets
                });
            });

            // 🔹 Add SignalR services
            builder.Services.AddSignalR();

            var app = builder.Build();

            // ----------------------
            // Configure pipeline
            // ----------------------
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseAuthorization();

            app.MapControllers();

            app.UseCors("AllowSpecificOrigin");

            // 🔹 Map SignalR hub
            app.MapHub<GadgetHub>("/gadgetHub");

            app.Run();
        }
    }
}
