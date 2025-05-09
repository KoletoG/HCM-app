using System;
using HCM_app.Models.DataModels;
using Microsoft.EntityFrameworkCore;

namespace HCM_app.Data
{
    public class ApplicationDbContext : DbContext
    {

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) 
        { 

        }
        public DbSet<UserDataModel> Users { get; set; }
    }
}
