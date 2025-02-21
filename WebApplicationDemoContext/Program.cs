using Microsoft.EntityFrameworkCore;
using WebApplicationDemoContext.DBContext;
using WebApplicationDemoContext.Middleware;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddTransient<BasicMiddleware>();//

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<BasicMiddleware>();//

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();//

app.Run();