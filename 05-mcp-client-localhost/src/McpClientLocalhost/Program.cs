using McpClientLocalhost.Endpoints;
using McpClientLocalhost.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddMcpClientInfrastructure(builder.Configuration);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddProblemDetails();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.UseExceptionHandler();
app.UseHttpsRedirection();
app.UseCors();

app.MapChatEndpoints();

app.Run();