namespace TestTechniqueApi.DbContext
{
    using Microsoft.EntityFrameworkCore;
    using TestTechniqueApi.Model;

    class Context : DbContext
    {
        public Context(DbContextOptions<Context> options)
            : base(options) { }

        public DbSet<Book> Books => Set<Book>();
    }
}
