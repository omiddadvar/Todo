using Serilog;
using Serilog.Events;
using Todo.Identity;
using Todo.Identity.Infrastructure.Middleware;

Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddServices(builder.Configuration);

builder.Host.UseSerilog((context, services, configuration) => configuration
    .ReadFrom.Configuration(context.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .Enrich.WithMachineName()
    .Enrich.WithThreadId()
    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName));

var app = builder.Build();

await app.MigrateDatabaseAsync();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseWebAppExtensions();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
