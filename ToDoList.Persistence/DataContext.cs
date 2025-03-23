using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using ToDoList.Persistence.Configuration;
using ToDoList.Persistence.Entities;

namespace ToDoList.Persistence
{
    public class DataContext : DbContext
    {
        public DbSet<TaskEntity> Tasks { get; set; }

        public DataContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new TaskConfiguration());

            base.OnModelCreating(modelBuilder);
        }
    }
}
