using McpClientAuthenticated.Endpoints;
using McpClientAuthenticated.DependencyInjection;
using McpClientAuthenticated.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Identity.Web;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(options =>
    {
        var entraOptions = builder.Configuration.GetSection(EntraOptions.SectionName).Get<EntraOptions>()!;
        options.TokenValidationParameters.ValidateAudience = true;
        options.TokenValidationParameters.ValidateIssuer = true;
        options.TokenValidationParameters.ValidAudience = entraOptions.Audience;
    }, options =>
    {
        var entraOptions = builder.Configuration.GetSection(EntraOptions.SectionName).Get<EntraOptions>()!;
        options.Instance = "https://login.microsoftonline.com/";
        options.TenantId = entraOptions.TenantId;
        options.ClientId = entraOptions.ClientId;
        options.ClientSecret = entraOptions.ClientSecret;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuthenticatedUser", policy =>
        policy.RequireAuthenticatedUser());
});


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
app.UseAuthentication();
app.UseAuthorization();

app.MapChatEndpoints();

app.Run();
