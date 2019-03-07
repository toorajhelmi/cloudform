using System;
using Microsoft.EntityFrameworkCore;

namespace Cloudform.Api.Models
{
    public class CloudformContext : DbContext
    {
        public DbSet<BuildEvent> BuildEvents { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Filename=./cloudform.db");
        }
    }
}
