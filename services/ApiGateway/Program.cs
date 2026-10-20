namespace ApiGateway
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddCors(options =>
                options.AddPolicy("Frontend", policy =>
                    policy.WithOrigins("https://fttsr.github.io", "http://localhost:5173")
                        .AllowAnyHeader()
                        .AllowAnyMethod()));

            builder.Services.AddReverseProxy().LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

            var app = builder.Build();

            app.UseCors("Frontend");
            app.MapReverseProxy();

            app.Run();
        }
    }
}
