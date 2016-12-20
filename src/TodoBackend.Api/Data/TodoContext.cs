using Microsoft.EntityFrameworkCore;
using TodoBackend.Core.Domain;

namespace TodoBackend.Api.Data
{
    public class TodoContext : DbContext
    {
        public DbSet<Todo> Todos { get; set; }

        public TodoContext(DbContextOptions<TodoContext> options)
            : base(options)
        { }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Todo>(entity =>
            {
                entity.HasKey("SequenceId");
                entity.HasAlternateKey(e => e.Id);

                entity.Property(e => e.Title).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Completed).IsRequired();
            });
        }
    }
}
