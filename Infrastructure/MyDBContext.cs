using AspNetCoreApiExample.Domain;
using Microsoft.EntityFrameworkCore;

namespace AspNetCoreApiExample.Infrastructure
{
    public class MyDBContext : DbContext
    {
        public MyDBContext(DbContextOptions<MyDBContext> options)
            : base(options)
        {

        }


        public DbSet<GeoHistory> GeoHistories { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserToken> UserTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserToken>(entity =>
            {
                entity.HasOne(ut => ut.User)
                    .WithMany(u => u.UserTokens)
                    .HasForeignKey(ut => ut.OwnerUserId);
            });

            builder.Entity<GeoHistory>(entity =>
            {
                entity.HasOne(ut => ut.User)
                    .WithMany(u => u.GeoHistories)
                    .HasForeignKey(ut => ut.UserId);
            });


        }

        protected override void OnConfiguring(DbContextOptionsBuilder builder)
        {
            base.OnConfiguring(builder);


        }
    }
}