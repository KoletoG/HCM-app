using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using SharedModels;
using BCrypt.Net;
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
