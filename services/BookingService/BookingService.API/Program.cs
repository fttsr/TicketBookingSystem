
using BookingService.API.Middleware;
using BookingService.Application.Interfaces;
using BookingService.Application.Mapper;
using BookingService.Application.Services;
using BookingService.Infrastructure.Clients;
using BookingService.Infrastructure.Data;
using BookingService.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using Microsoft.OpenApi.Models;
using BookingService.Infrastructure.Messaging;

namespace BookingService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();


            builder.Services.AddScoped<IBookingRepository, BookingRepository>();
            builder.Services.AddScoped<IBookingService, BookingService.Application.Services.BookingService>();

            builder.Services.AddAutoMapper(config => config.AddProfile<BookingProfile>());

            builder.Services.AddDbContext<BookingDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"),
                npgsqlOptions => npgsqlOptions.MigrationsAssembly(
                        typeof(BookingDbContext).Assembly.FullName)
                ));

            builder.Services.AddHttpClient<IEventClient, EventClient>(client =>
            {
                client.BaseAddress = new Uri("http://event-api:8080");
            });

            builder.Services.AddScoped<IEventPublisher, EventPublisher>();

            builder.Services
                .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,

                        ValidIssuer = builder.Configuration["Jwt:Issuer"],
                        ValidAudience = builder.Configuration["Jwt:Audience"],

                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(
                                builder.Configuration["Jwt:Key"]!))
                    };

                    options.Events = new JwtBearerEvents
                    {
                        OnAuthenticationFailed = context =>
                        {
                            Console.WriteLine("JWT Err:");
                            Console.WriteLine(context.Exception.ToString());

                            return Task.CompletedTask;
                        }
                    };
                });

            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Введите JWT токен"
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
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
                        Array.Empty<String>()
                    }
                });
            });

            var app = builder.Build();

            app.UseMiddleware<ExceptionHandlingMiddleware>();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<BookingDbContext>();

                dbContext.Database.Migrate();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
