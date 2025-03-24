using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;
using ToDoList.API;
using ToDoList.Application;
using ToDoList.Application.Implementations;
using ToDoList.Persistence;
using ToDoList.Persistence.Repositories;
using ToDoList.Persistence.Repositories.Implementations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddControllers()
    .AddJsonOptions(options => options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var connection = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<DataContext>(options => options.UseNpgsql(connection));
builder.Services.AddTransient<ITaskService, TaskServiceImpl>();
builder.Services.AddScoped<ITaskRepository, TaskRepositoryImpl>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

using var serviceScope = app.Services.CreateScope();
var context = serviceScope.ServiceProvider.GetService<DataContext>();

context?.Database.Migrate();

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
