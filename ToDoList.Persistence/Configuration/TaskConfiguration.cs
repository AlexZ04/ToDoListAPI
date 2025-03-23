using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ToDoList.Persistence.Entities;

namespace ToDoList.Persistence.Configuration
{
    public class TaskConfiguration : IEntityTypeConfiguration<TaskEntity>
    {
        public void Configure(EntityTypeBuilder<TaskEntity> builder)
        {
            builder.HasKey(t => t.Id);

            builder.Property(t => t.Name).IsRequired();
            builder.Property(t => t.Deadline).IsRequired();
            builder.Property(t => t.Status).IsRequired();
            builder.Property(t => t.Priority).IsRequired();
            builder.Property(t => t.CreateTime).IsRequired();
            builder.Property(t => t.UpdateTime).IsRequired();
        }
    }
}
