using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using ToDoList.API;
using ToDoList.Application;
using ToDoList.Application.Implementations;
using ToDoList.Persistence;
using ToDoList.Persistence.Repositories;
using ToDoList.Persistence.Repositories.Implementations;

var builder = WebApplication.CreateBuilder(args);

var isTesting = builder.Environment.EnvironmentName == "Testing";

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

if (!isTesting)
    builder.Services.AddControllers()
        .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
else 
    builder.Services.AddControllers();

    builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

if (!isTesting)
{
    var connection = builder.Configuration.GetConnectionString("DefaultConnection");
    builder.Services.AddDbContext<DataContext>(options => options.UseNpgsql(connection));
}

builder.Services.AddTransient<ITaskService, TaskServiceImpl>();
builder.Services.AddScoped<ITaskRepository, TaskRepositoryImpl>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin();
        policy.AllowAnyHeader();
        policy.AllowAnyMethod();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (!isTesting)
{
    using var serviceScope = app.Services.CreateScope();
    var context = serviceScope.ServiceProvider.GetService<DataContext>();
    context?.Database.Migrate();
}

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseCors();
app.UseAuthorization();

app.MapControllers();

app.Run();
public partial class Program { }
