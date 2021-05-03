using System;
using System.Linq;
using CodingChainApi.Infrastructure.Models;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using Right = CodingChainApi.Infrastructure.Models.Right;
using User = CodingChainApi.Infrastructure.Models.User;

namespace CodingChainApi.Infrastructure.Contexts
{
    public class CodingChainContext : DbContext
    {
        public CodingChainContext(DbContextOptions<CodingChainContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
            => optionsBuilder.UseSnakeCaseNamingConvention();

        public DbSet<User> Users { get; set; }

        public DbSet<ProgrammingLanguage> ProgrammingLanguages { get; set; }
        public DbSet<Right> Rights { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<UserTeam> UserTeams { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToTable("user");
            modelBuilder.Entity<ProgrammingLanguage>()
                .ToTable("programming_language");
            modelBuilder.Entity<Team>()
                .ToTable("team");
            modelBuilder.Entity<Right>()
                .ToTable("right");
            modelBuilder.Entity<UserTeam>()
                .ToTable("user_team");
            modelBuilder.Entity<UserFunction>()
                .ToTable("user_function");
            modelBuilder.Entity<Function>()
                .ToTable("function");
            modelBuilder.Entity<Participation>()
                .ToTable("participation");
            modelBuilder.Entity<Tournament>()
                .ToTable("tournament");
            modelBuilder.Entity<Step>()
                .ToTable("step");
            modelBuilder.Entity<Test>()
                .ToTable("test");


            modelBuilder.Entity<User>()
                .HasMany(s => s.Rights)
                .WithMany(c => c.Users);

            modelBuilder.Entity<UserTeam>()
                .HasKey(bc => new {bc.UserId, bc.TeamId});
            modelBuilder.Entity<UserTeam>()
                .HasOne(bc => bc.User)
                .WithMany(b => b.UserTeams)
                .HasForeignKey(bc => bc.UserId);
            modelBuilder.Entity<UserTeam>()
                .HasOne(bc => bc.Team)
                .WithMany(c => c.UserTeams)
                .HasForeignKey(bc => bc.TeamId);

            modelBuilder.Entity<UserFunction>()
                .HasKey(bc => new {bc.UserId, bc.FunctionId});
            modelBuilder.Entity<UserFunction>()
                .HasOne(bc => bc.User)
                .WithMany(b => b.UserFunctions)
                .HasForeignKey(bc => bc.UserId);
            modelBuilder.Entity<UserFunction>()
                .HasOne(bc => bc.Function)
                .WithMany(c => c.UserFunctions)
                .HasForeignKey(bc => bc.FunctionId);

            modelBuilder.Entity<Right>()
                .Property(c => c.Name)
                .HasConversion<string>();
            
            InitRights(modelBuilder);
            InitDecimalPrecisions(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void InitRights(ModelBuilder modelBuilder)
        {
            var rights = Enum.GetValues(typeof(RightEnum)) as RightEnum[];
            modelBuilder.Entity<Right>().HasData(
                (rights ?? Array.Empty<RightEnum>()).Select(r => new Right {Id = Guid.NewGuid(), Name = r}));
        }

        private void InitDecimalPrecisions(ModelBuilder modelBuilder)
        {
            modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(type => type.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?))
                .ToList()
                .ForEach(property => { property.SetPrecision(18); property.SetScale(6); });
        }
    }
}