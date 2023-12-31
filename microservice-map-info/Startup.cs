﻿using Google.Protobuf.WellKnownTypes;
using microservice_map_info.Services;
using GoogleMapInfo;
using Prometheus;

namespace microservice_map_info
{
    public class Startup
    {
        public static void Main(string[] args)
        {
           

            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddTransient<GoogleDistanceApi>();
            builder.Services.AddControllers();            
            builder.Services.AddGrpc();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();

            app.UseAuthorization();
            
            app.UseEndpoints(endpoints =>
            {
                // ...
                endpoints.MapControllers();
                endpoints.MapMetrics(); // <-- add this line
            });

            app.MapGrpcService<DistanceInfoService>();


            app.Run();

        }
    }
}
