using Microsoft.EntityFrameworkCore;
using MCQs_generator.model;


namespace MCQs_generator.data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<mcq> MCQs { get; set; }

    }
}
