using MegaManager.Models;
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

        // Настройки для таблицы Entries
        builder.Entity<Entry>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Id).ValueGeneratedOnAdd();
            entity.Property(e => e.IdUser).IsRequired();
            entity.Property(e => e.URL).IsRequired();
            entity.Property(e => e.Login).IsRequired();
            entity.Property(e => e.Password).IsRequired();
            entity.Property(e => e.Notes).IsRequired(false);

        });
        // Настройка связи между Entry и ApplicationUser
        builder.Entity<Entry>()
            .HasOne<ApplicationUser>()
            .WithMany()
            .HasForeignKey(e => e.IdUser);
    }
}

public class ApplicationUserEntityConfiguration: IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.FirstName).HasMaxLength(100);
        builder.Property(x => x.LastName).HasMaxLength(100);


    }
    public void Configure(EntityTypeBuilder<Entry> builder)
    {
        builder.Property(x => x.Notes).HasMaxLength(256);

    }
}