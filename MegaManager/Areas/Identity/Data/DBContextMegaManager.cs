﻿using MegaManager.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace MegaManager.Areas.Identity.Data;

public class DBContextMegaManager: IdentityDbContext<ApplicationUser>
{
    public DBContextMegaManager(DbContextOptions<DBContextMegaManager> options): base(options)
    {

    }
    public DbSet<Entry> Entries { get; set; }
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Настройка связи между Entry и ApplicationUser
        builder.Entity<Entry>()
            .HasOne(e => e.User)
            .WithMany()
            .HasForeignKey(e => e.IdUser);
        builder.ApplyConfiguration(new ApplicationUserEntityConfiguration());
    }
}

public class ApplicationUserEntityConfiguration: IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.FirstName).HasMaxLength(100);
        builder.Property(x => x.LastName).HasMaxLength(100);
    }
}