using Microsoft.EntityFrameworkCore;
using WebApplicationDemoContext;
using WebApplicationDemoContext.DBContext;
using WebApplicationDemoContext.Middleware;
using WebApplicationDemoContext.Repositories;
using WebApplicationDemoContext.Repository;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<AppDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddTransient<IUserRepository, AdapterUser>();

builder.Services.AddTransient<Middleware>(); //
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<Middleware>(); //
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();