using Microsoft.EntityFrameworkCore;
using SharedModels;

namespace CRUDHCM_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options): base(options)
        {

        }
        public DbSet<UserDataModel> Users { get; set; }
    }
}
