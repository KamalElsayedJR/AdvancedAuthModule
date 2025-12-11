using AUTH.API.Extentions;
using CORE.DTOs;
using CORE.Entities;
using CORE.Interfaces;
using Microsoft.EntityFrameworkCore;
using REPOSITORY.Data;
using System.Threading.Tasks;
namespace AUTH.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            #region // Add services to the container.
            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
            });
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            builder.Services.AddIdentityServices(builder.Configuration);
            builder.Services.AddServices();
            builder.Services.Configure<MailSettings>(builder.Configuration.GetSection("MailSettings"));


            using var app = builder.Build();
            var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var DbContext = services.GetRequiredService<IdentityDbContext>();
                await DbContext.Database.MigrateAsync();
                await IdentityDataSeed.SeedDataAsync(DbContext);
            }
            catch (Exception ex)
            {
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var Logger = loggerFactory.CreateLogger<Program>();
                Logger.LogError(ex.Message);
            }

            #endregion
            // Configure the HTTP request pipeline.
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
