using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MCQs_generator.data;

namespace MCQs_generator.Factory
{
    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();

            optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Initial Catalog=Questions;Integrated Security=True;Encrypt=False;Trust Server Certificate=True");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
