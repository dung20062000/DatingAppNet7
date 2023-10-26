
using API.Data;
using API.Interfaces;
using API.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddDbContext<DataContext>(opt => 
{
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// fix lỗi CORS
builder.Services.AddCors();

//thêm dịch vụ tokens
builder.Services.AddScoped<ITokenService, TokenService>(); //dịch vụ kiểu ITokenService sử dụng trong ứng dụng, DI container sẽ cung cấp một phiên bản của TokenService

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseCors(x => x.AllowAnyHeader().AllowAnyMethod().WithOrigins("https://localhost:4200"));

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
