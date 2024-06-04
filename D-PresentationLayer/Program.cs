using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using A_DomainLayer.Entities;
using A_DomainLayer.Interfaces;
using B_RepositoryLayer.Data;
using B_RepositoryLayer.Repositories;
using B_RepositoryLayer.Seeds;
using C_ServiceLayer.Abstractions;
using C_ServiceLayer.Concretes;
using C_ServiceLayer.Utils;
using D_PresentationLayer.Middlewares;
using D_PresentationLayer.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;

namespace D_PresentationLayer;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Db Connections Configuration
        builder.Services.AddDbContext<ApplicationDbContext>(options => 
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
        
        builder.Services.AddDbContext<AuthDbContext>(options => 
            options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));
        
        // builder.Services.AddSingleton<IConnectionMultiplexer>( option=>
        // {
        //     var connectionString = builder.Configuration.GetConnectionString("RedisConnection");
        //     return ConnectionMultiplexer.Connect(connectionString);
        // });
        builder.Services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
        {
            var configuration = serviceProvider.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("RedisConnection");
            return ConnectionMultiplexer.Connect(connectionString);
        });
        //////////////////////////////////////////////////////////////////////////////////////

        // appsettings.json Configuration
        builder.Services.Configure<Jwt>(builder.Configuration.GetSection("JWT"));
        //Email
        builder.Services.Configure<Email>(builder.Configuration.GetSection("Email"));
        builder.Services.AddScoped<IEmailService, EmailService>();
        //Sms
        builder.Services.Configure<Sms>(builder.Configuration.GetSection("Sms"));
        builder.Services.AddScoped<ISmsService, SmsService>();
        //Cloudinary
        builder.Services.Configure<cloudProvider>(builder.Configuration.GetSection("Cloudinary"));
        builder.Services.AddScoped<IUploadImgService, UploadImgService>();
        //Payment
        builder.Services.Configure<Payment>(builder.Configuration.GetSection("Payment"));
        builder.Services.AddScoped<IPaymentService, PaymentService>();

        //////////////////////////////////////////////////////////////////////////////////////
        
        // Identity Configuration
        builder.Services.AddIdentity<User, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequireLowercase = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequiredLength = 4;
            })
            .AddEntityFrameworkStores<AuthDbContext>();
        
        builder.Services.AddScoped<UserManager<User>>();
        builder.Services.AddScoped<SignInManager<User>>();
        builder.Services.AddScoped<RoleManager<IdentityRole>>();
        //////////////////////////////////////////////////////////////////////////////////////
        
        // Adding Services 
        builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        builder.Services.AddScoped(typeof(IBrandRepository), typeof(BrandRepository));
        builder.Services.AddScoped(typeof(IProductRepository), typeof(ProductRepository));
        builder.Services.AddScoped(typeof(ICartRepository), typeof(CartRepository));
        builder.Services.AddScoped(typeof(IDeliveryMethodRepository), typeof(DeliveryMethodRepository));
        builder.Services.AddScoped(typeof(IOrderRepository), typeof(OrderRepository));
        builder.Services.AddScoped(typeof(IOrderItemRepository), typeof(OrderItemRepository));
        builder.Services.AddScoped<IAuthService, AuthService>();
        builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
        builder.Services.AddAutoMapper(typeof(MappingProfile));
        //////////////////////////////////////////////////////////////////////////////////////



        // Configure Authentication and Authorization
        builder.Services.AddAuthentication(options => {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.SaveToken = true;
            options.RequireHttpsMetadata = false;
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                RequireExpirationTime = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"])),
                ClockSkew = TimeSpan.Zero,
        
            };
            options.Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    context.HandleResponse();
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.ContentType = "application/json";
                    var response = new { message = "Custom Unauthorized Message login first" };
                    return context.Response.WriteAsJsonAsync(response);
                },
                OnForbidden = context =>
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    var response = new { message = "Custom Forbidden Message Roles thing" };
                    return context.Response.WriteAsJsonAsync(response);
                }
            };
        });
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy("Admin", policy => policy.RequireClaim(ClaimTypes.Role, "Admin"));
            options.AddPolicy("User", policy => policy.RequireClaim(ClaimTypes.Role, "User"));
            options.AddPolicy("AdminOrUser", policy => policy.RequireClaim(ClaimTypes.Role, "Admin", "User"));

        });
            
        // For Eager Loading
        builder.Services.AddControllers().AddJsonOptions(x =>
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.Preserve);
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(option =>
        {
            option.SwaggerDoc("v1", new OpenApiInfo { Title = "Online Store", Version = "v1" });
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Description = "Please enter a valid access token",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type=ReferenceType.SecurityScheme,
                            Id="Bearer"
                        }
                    },
                    new string[]{}
                }
            });
        });


        
        // CORS
        builder.Services.AddCors(options => {
            options.AddDefaultPolicy(b => {
                b.AllowAnyHeader();
                b.AllowAnyOrigin();
                b.AllowAnyMethod();
            });
        });
        
        
        // Configure API Behavior
        builder.Services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .SelectMany(x => x.Value.Errors)
                    .Select(x => x.ErrorMessage).ToArray();
        
                return new BadRequestObjectResult(new { message = errors });
            };
        });



        var app = builder.Build();
        
        
        // Auto Migrate Database and Seed Admin
        // Making each time migration headache on the cpu so commented\
        var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        var userManager = services.GetRequiredService<UserManager<User>>();
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        var appDbContext = services.GetRequiredService<ApplicationDbContext>();
        await AdminSeed.SeedAdminAsync(userManager, roleManager);
        await DeliverySeed.SeedDeliveryAsync(appDbContext);

        // try
        // {
        //     var scope = app.Services.CreateScope();
        //     var services = scope.ServiceProvider;
        //     var appDbContext = services.GetRequiredService<ApplicationDbContext>();
        //     await appDbContext.Database.MigrateAsync();
        //     var authDbContext = services.GetRequiredService<AuthDbContext>();
        //     var userManager = services.GetRequiredService<UserManager<User>>();
        //     var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        //     await AdminSeed.SeedAdminAsync(userManager, roleManager);
        //     await authDbContext.Database.MigrateAsync();
        // }
        // catch (Exception ex)
        // {
        //     Console.WriteLine($"Exception: {ex.Message}");
        //     Console.WriteLine($"Stack Trace: {ex.StackTrace}");
        //     if (ex.InnerException != null)
        //         Console.WriteLine($"Inner Exception: {ex.InnerException.Message}");
        // }        


        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        
        app.UseMiddleware<ErrorHandlerMiddleware>();
        app.UseCors(); 

        app.UseStatusCodePages(async context => 
        {
            context.HttpContext.Response.ContentType = "application/json";
            await context.HttpContext.Response.WriteAsync("invalid route.");
        });


        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        // var summaries = new[]
        // {
        //     "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        // };
        
        // app.MapGet("/weatherforecast", (HttpContext httpContext) =>
        //     {
        //         var forecast = Enumerable.Range(1, 5).Select(index =>
        //                 new WeatherForecast
        //                 {
        //                     Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
        //                     TemperatureC = Random.Shared.Next(-20, 55),
        //                     Summary = summaries[Random.Shared.Next(summaries.Length)]
        //                 })
        //             .ToArray();
        //         return forecast;
        //     })
        //     .WithName("GetWeatherForecast")
        //     .WithOpenApi();

        app.MapControllers();

        await app.RunAsync();
    }
}