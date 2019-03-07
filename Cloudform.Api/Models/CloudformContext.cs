using System;
using Microsoft.EntityFrameworkCore;

namespace Cloudform.Api.Models
{
    public class CloudformContext : DbContext
    {
        public DbSet<Build> Builds { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./cloudform.db");
        }
    }
}
