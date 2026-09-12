
using NotificationService.Application.Interfaces;
using NotificationService.Application.Services;
using NotificationService.Infrastructure.Email;
using NotificationService.Infrastructure.Messaging;

namespace NotificationService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();

            builder.Services.AddScoped<IEmailSender, SmtpEmailSender>();
            builder.Services.AddScoped<INotificationService, NotificationService.Application.Services.NotificationService>();

            builder.Services.AddHostedService<UserRegisteredConsumer>();
            builder.Services.AddHostedService<BookingCreatedConsumer>();
            builder.Services.AddHostedService<BookingCancelledConsumer>();

            builder.Services.AddSwaggerGen();
            
            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
