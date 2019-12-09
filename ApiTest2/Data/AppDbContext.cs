using ApiTest2.Entities;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ApiTest2.Data
{
    public class AppDbContext : IdentityDbContext<AppUser>
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {
        }


        public DbSet<Glossary> Glossary { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);


            builder.Entity<Glossary>()
                .Property(p => p.Term)
                .HasMaxLength(50)
                .IsRequired();

            builder.Entity<Glossary>()
                .Property(p => p.Description)
                .IsRequired();
        }
    }
}
