using Synapse.Application.Interfaces;
using Synapse.Application.Services;
using Synapse.Application.UseCases;
using Synapse.Infrastructure.Data;
using Synapse.Infrastructure.Services;
using Synapse.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi.Models;
using DotNetEnv;
using Synapse.Infrastructure.Settings;
using Synapse.Api.Providers;
using Microsoft.AspNetCore.SignalR;
using Synapse.Infrastructure.Realtime;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.MicrosoftAccount;


Env.Load("../../../.env");

var builder = WebApplication.CreateBuilder(args);

builder.WebHost.UseUrls("http://0.0.0.0:8080");
// Add services
builder.Services.AddControllers();
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IBlobService, BlobService>();
builder.Services.AddScoped<IMessageBus, ServiceBus>();
builder.Services.AddScoped<INotificationService, SignalRNotificationService>();

builder.Services.AddScoped<CreateNoteUseCase>();
builder.Services.AddScoped<DeleteNoteUseCase>();

builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

var ServiceBusSettingsSection = builder.Configuration.GetSection("ServiceBus");
builder.Services.Configure<ServiceBusSettings>(ServiceBusSettingsSection);

var OAuthSettingsSection = builder.Configuration.GetSection("OAuth");
builder.Services.Configure<OAuthSettings>(OAuthSettingsSection);

// Add Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
{
    Name = "Authorization",
    Type = SecuritySchemeType.Http,
    Scheme = "bearer",
    BearerFormat = "JWT",
    In = ParameterLocation.Header,
    Description = "Enter: {your token}"
});

    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// Add DbContext
// move to infrastructure project
builder.Services.AddInfrastructure(builder.Configuration);


var jwtSettings = builder.Configuration
                        .GetSection("Jwt")
                        .Get<JwtSettings>()
                        ?? throw new Exception("Jwt settings missing.");
if (string.IsNullOrEmpty(jwtSettings.Key))
{
    throw new Exception("Jwt: Key is missing");
}
var key = jwtSettings.Key;
// Add JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.Zero
        };

        //read token from query
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];

                if (!string.IsNullOrEmpty(accessToken) &&
                    context.HttpContext.Request.Path.StartsWithSegments("/hubs/notes"))
                {
                    context.Token = accessToken;
                }
                return Task.CompletedTask;
            }
        };
    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["OAuth:Google:ClientId"] ?? "";
        options.ClientSecret = builder.Configuration["OAuth:Google:ClientSecret"] ?? "";
        options.CallbackPath = "/api/auth/oauth/google/callback";
        options.SaveTokens = true;
        options.Events.OnCreatingTicket = context =>
        {
            var tokens = context.Properties?.Tokens ?? new Microsoft.AspNetCore.Authentication.AuthenticationTokens();
            tokens.Add(new Microsoft.AspNetCore.Authentication.AuthenticationToken
            {
                Name = "TicketCreated",
                Value = DateTime.UtcNow.ToString()
            });
            context.Properties?.Tokens = tokens;
            return Task.CompletedTask;
        };
    })
    .AddMicrosoftAccount(options =>
    {
        options.ClientId = builder.Configuration["OAuth:Microsoft:ClientId"] ?? "";
        options.ClientSecret = builder.Configuration["OAuth:Microsoft:ClientSecret"] ?? "";
        options.CallbackPath = "/api/auth/oauth/microsoft/callback";
        options.SaveTokens = true;
    });

builder.Services.AddSignalR();
builder.Services.AddSingleton<IUserIdProvider, CustomerUserIdProvider>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            var allowedOrigins = builder.Configuration["OAuth:Frontend:BaseUrl"] ?? "http://localhost:3000";
            policy.WithOrigins(allowedOrigins)
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecific", policy =>
    {
        var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>() 
            ?? new[] { "http://localhost:3000" };
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});



var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

app.UseCors("AllowSpecific"); 
// Configure pipeline
//if (app.Environment.IsDevelopment())
if (true) //for cloud
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext httpContext) =>
{
    var exception = httpContext.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
    return Results.Problem(detail: exception?.Message);
});

app.MapHub<NoteHub>("/hubs/notes");

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.MapGet("/", () => "Synapse API is running!");

app.Run();

public partial class Program
{
}