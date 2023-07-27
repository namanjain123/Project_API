using Microsoft.EntityFrameworkCore;
using Serilog.Events;
using Serilog.Sinks.MSSqlServer;
using Serilog;
using StackExchange.Redis;
using System.Configuration;
using Database_Layer;
using Buisness_Layer.Repository.Interface;
using Database_Layer.Model;
using Buisness_Layer.Repository;

class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        //Buisness layer addition
        builder.Services.AddScoped<ICustomers, Customers>();
        builder.Services.AddScoped<IOrders, Orders>();
        //Cache
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = "127.0.0.1:6379";
            options.InstanceName = "OrderAPI"; // name for your application
        });
        var conn = builder.Configuration.GetConnectionString("DefaultConnection");
        builder.Services.AddDbContext<DatabaseContext>
    (options => options.UseSqlServer("server=127.0.0.1;database=test;User ID=sa;Password=CEleb@L_TECH;Connect Timeout=30;Encrypt=False;Trust Server Certificate=False;Application Intent=ReadWrite;Multi Subnet Failover=False"));

        // Configure Serilog here
        Log.Logger = new LoggerConfiguration().
            MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning).
            MinimumLevel.Override("Microsoft", LogEventLevel.Information).
            Enrich.FromLogContext().
            ReadFrom.Configuration(builder.Configuration)
            .WriteTo.MSSqlServer
                (
                connectionString: builder.Configuration.GetSection("loggingDB").Value,
                sinkOptions: new MSSqlServerSinkOptions
                {
                    TableName = "Logs",
                    AutoCreateSqlTable = true
                }
                )
            .CreateLogger();
        builder.Services.AddScoped<DatabaseContext>();
        builder.Services.AddLogging(loggingBuilder =>
        {
            loggingBuilder.ClearProviders();
            
        });

        Log.Information("Application started.");
        var app = builder.Build();

        
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        // Log HTTP request details
        app.UseSerilogRequestLogging();
        app.UseHttpsRedirection();

        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}