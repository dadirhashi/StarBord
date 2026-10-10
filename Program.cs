using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using StarBord.Data;
using System.Text;
using Microsoft.OpenApi.Models;
using Microsoft.IdentityModel.Tokens;
using StarBord.Services.IService;
using StarBord.Services;
using StarBord.Middleware;
namespace StarBord
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Npgsql kräver annars DateTime med Kind=Utc; detta behåller samma beteende som med SQL Server
            AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

            var builder = WebApplication.CreateBuilder(args);

            // PostgreSQL istället för SQL Server. Connection string kommer från miljövariabeln
            // ConnectionStrings__DefaultConnection i docker-compose (aldrig hårdkodad i koden).
            builder.Services.AddDbContext<StarBordDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
            
            builder.Services.AddScoped<IAuthResponse, AuthService>();
            builder.Services.AddScoped<IPlatformTokenService, PlatformTokenService>();
            builder.Services.AddScoped<IGoogleReviewService, MockGoogleReviewService>();
            builder.Services.AddHttpClient();
            builder.Services.AddScoped<ITrustpilotService, TrustpilotService>();
            builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.WithOrigins("http://localhost:5173")
                          .AllowAnyHeader()
                          .AllowAnyMethod();
                });
            });

            var jwtSettings = builder.Configuration.GetSection("Jwt");
            var key = Encoding.UTF8.GetBytes(jwtSettings["Key"]!);

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
                .AddJwtBearer(option =>
                {
                    option.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings["Issuer"],
                        ValidAudience = jwtSettings["Audience"],
                        IssuerSigningKey = new SymmetricSecurityKey(key),

                    };
                });

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
           
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "StarBord API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your token}"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference
                            {
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            builder.Services.AddOpenApi();

            var app = builder.Build();

            ApplyDatabaseMigrations(app);

            app.UseMiddleware<GlobalExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(
                    c =>
                        {
                            c.SwaggerEndpoint("/swagger/v1/swagger.json", "StarBord API V1");
                            c.RoutePrefix = "swagger"; 
                        }
                    );
                app.MapOpenApi();
            }

            app.UseCors("AllowFrontend");
           
            // I containern körs bara HTTP bakom nginx, så ingen https-redirect där.
            // DOTNET_RUNNING_IN_CONTAINER sätts automatiskt av Microsofts images.
            if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
            {
                app.UseHttpsRedirection();
            }
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            // Health endpoint. "instance" = containerns hostname, så man ser vilken
            // replika som svarade när api är skalad (docker compose up --scale api=3).
            app.MapGet("/api/health", () => Results.Ok(new
            {
                status = "ok",
                instance = Environment.MachineName
            }));

            app.Run();
        }

        // Skapar/uppdaterar databasschemat vid start. Försöker igen om databasen inte är redo än.
        private static void ApplyDatabaseMigrations(WebApplication app)
        {
            var logger = app.Logger;
            for (var attempt = 1; attempt <= 10; attempt++)
            {
                try
                {
                    using var scope = app.Services.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<StarBordDbContext>();
                    db.Database.Migrate();
                    logger.LogInformation("Database is ready");
                    return;
                }
                catch (Exception ex) when (attempt < 10)
                {
                    logger.LogWarning(ex, "Database not ready (attempt {Attempt}/10), retrying in 3s", attempt);
                    Thread.Sleep(TimeSpan.FromSeconds(3));
                }
            }
        }
    }
}