using Todo.Identity;
using Todo.Identity.Infrastructure.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddOpenApi(options =>
{
    options.AddDocumentTransformer((document, _, _) =>
    {
        document.Info = new Microsoft.OpenApi.OpenApiInfo
        {
            Title = "Todo Identity API",
            Version = "v1",
            Description = "ASP.NET WebAPI for Todo.Identity service (JWT Bearer Authentication)"
        };
        return Task.CompletedTask;
    });
});

builder.Services.AddServices(builder.Configuration);

var app = builder.Build();

await app.MigrateDatabaseAsync();

app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
