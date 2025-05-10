using Microsoft.EntityFrameworkCore;
using SharedModels;

namespace AuthAPIHCM.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<UserDataModel> Users { get; set; }
    }
}
