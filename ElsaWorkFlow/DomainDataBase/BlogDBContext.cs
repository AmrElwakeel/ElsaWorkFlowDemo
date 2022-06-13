using ElsaWorkFlow.DomainDataBase.Entities;
using Microsoft.EntityFrameworkCore;

namespace ElsaWorkFlow.DomainDataBase
{
    public class BlogDBContext : DbContext
    {
        public BlogDBContext(DbContextOptions options ):base(options)
        { }
        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Request> Requests { get; set; }
    }
}
