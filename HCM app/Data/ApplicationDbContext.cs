using System;
using Microsoft.EntityFrameworkCore;

namespace HCM_app.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        { 

        }
    }
}
