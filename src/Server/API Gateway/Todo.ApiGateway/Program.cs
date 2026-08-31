using Serilog;
using Serilog.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

Log.Logger = new LoggerConfiguration()
     .MinimumLevel.Override("Microsoft", LogEventLevel.Error)
     .MinimumLevel.Override("System", LogEventLevel.Error)
     .Enrich.FromLogContext()
     .ReadFrom.Configuration(builder.Configuration)
     .CreateLogger();

var app = builder.Build();

app.MapReverseProxy();

app.Run();