using CarManagmentAPI.DataContext;
using Microsoft.EntityFrameworkCore;

namespace CarManagmentAPI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddCors(o => o.AddPolicy("Frontend", p =>
                p.WithOrigins("http://localhost:7171")
                 .AllowAnyHeader()
                 .AllowAnyMethod()
            ));

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseStaticFiles();  
            app.UseDefaultFiles(); 

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}