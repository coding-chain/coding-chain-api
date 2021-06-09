using System;
using System.Linq;
using CodingChainApi.Infrastructure.Models;
using Domain.Cron;
using Domain.ProgrammingLanguages;
using Domain.Users;
using Microsoft.EntityFrameworkCore;
using CronStatus = CodingChainApi.Infrastructure.Models.CronStatus;
using ProgrammingLanguage = CodingChainApi.Infrastructure.Models.ProgrammingLanguage;
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
        public DbSet<UserFunction> UserFunctions { get; set; }
        public DbSet<Function> Functions { get; set; }
        public DbSet<Participation> Participations { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<TournamentStep> TournamentSteps { get; set; }
        public DbSet<Step> Steps { get; set; }
        public DbSet<Test> Tests { get; set; }

        public DbSet<PlagiarismFunction> PlagiarismFunctions { get; set; }
        public DbSet<CronStatus> CronStatus { get; set; }
        public DbSet<Cron> Crons { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .ToTable("user")
                .Property(p => p.Id)
                .ValueGeneratedNever();
            modelBuilder.Entity<ProgrammingLanguage>()
                .ToTable("programming_language");
            modelBuilder.Entity<Team>()
                .ToTable("team")
                .Property(p => p.Id)
                .ValueGeneratedNever();
            modelBuilder.Entity<Right>()
                .ToTable("right");
            modelBuilder.Entity<UserTeam>()
                .ToTable("user_team");
            modelBuilder.Entity<UserFunction>()
                .ToTable("user_function");
            modelBuilder.Entity<Function>()
                .ToTable("function")
                .Property(p => p.Id)
                .ValueGeneratedNever();
            modelBuilder.Entity<Participation>()
                .ToTable("participation")
                .Property(p => p.Id)
                .ValueGeneratedNever();
            modelBuilder.Entity<Tournament>()
                .ToTable("tournament")
                .Property(p => p.Id)
                .ValueGeneratedNever();
            modelBuilder.Entity<TournamentStep>()
                .ToTable("tournament_step");
            modelBuilder.Entity<Step>()
                .ToTable("step")
                .Property(p => p.Id)
                .ValueGeneratedNever();
            modelBuilder.Entity<Test>()
                .ToTable("test")
                .Property(p => p.Id)
                .ValueGeneratedNever();
            modelBuilder.Entity<PlagiarismFunction>()
                .ToTable("plagiarism_function");
            modelBuilder.Entity<CronStatus>().ToTable("cron_status")
                .Property(p => p.Id)
                .ValueGeneratedNever();
            modelBuilder.Entity<Cron>()
                .ToTable("cron")
                .Property(c => c.Id)
                .ValueGeneratedNever();

            modelBuilder.Entity<User>()
                .HasMany(s => s.Rights)
                .WithMany(c => c.Users);


            modelBuilder.Entity<Tournament>()
                .HasMany(t => t.Participations)
                .WithOne(p => p.Tournament);
            modelBuilder.Entity<Team>()
                .Ignore(t => t.ActiveParticipations)
                .Ignore(t => t.ActiveMembers)
                .HasMany(t => t.Participations)
                .WithOne(p => p.Team);
            modelBuilder.Entity<Step>()
                .HasMany(t => t.Participations)
                .WithOne(p => p.Step);

            modelBuilder.Entity<UserTeam>()
                .HasOne(bc => bc.User)
                .WithMany(b => b.UserTeams)
                .HasForeignKey(bc => bc.UserId);
            modelBuilder.Entity<UserTeam>()
                .HasOne(bc => bc.Team)
                .WithMany(c => c.UserTeams)
                .HasForeignKey(bc => bc.TeamId);


            modelBuilder.Entity<UserFunction>()
                .HasKey(uF => new {uF.UserId, uF.FunctionId});
            modelBuilder.Entity<UserFunction>()
                .HasOne(bc => bc.User)
                .WithMany(b => b.UserFunctions)
                .HasForeignKey(bc => bc.UserId);
            modelBuilder.Entity<UserFunction>()
                .HasOne(bc => bc.Function)
                .WithMany(c => c.UserFunctions)
                .HasForeignKey(bc => bc.FunctionId);

            modelBuilder.Entity<TournamentStep>()
                .HasOne(bc => bc.Tournament)
                .WithMany(b => b.TournamentSteps)
                .HasForeignKey(bc => bc.TournamentId);
            modelBuilder.Entity<TournamentStep>()
                .HasOne(bc => bc.Step)
                .WithMany(c => c.TournamentSteps)
                .HasForeignKey(bc => bc.StepId);

            modelBuilder.Entity<PlagiarismFunction>()
                .HasOne(bc => bc.CheatingFunction)
                .WithMany(b => b.CheatingFunctions)
                .HasForeignKey(bc => bc.CheatingFunctionId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<PlagiarismFunction>()
                .HasOne(bc => bc.PlagiarizedFunction)
                .WithMany(b => b.PlagiarizedFunctions)
                .HasForeignKey(bc => bc.PlagiarizedFunctionId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder
                .Entity<Cron>()
                .HasOne(c => c.Status)
                .WithMany(c => c.Crons);

            modelBuilder.Entity<Right>()
                .Property(c => c.Name)
                .HasConversion<string>();
            modelBuilder.Entity<CronStatus>()
                .Property(s => s.Code)
                .HasConversion<string>();

            modelBuilder.Entity<ProgrammingLanguage>()
                .Property(c => c.Name)
                .HasConversion<string>();


            InitRights(modelBuilder);
            InitLanguages(modelBuilder);
            InitDecimalPrecisions(modelBuilder);
            InitCronStatus(modelBuilder);
            base.OnModelCreating(modelBuilder);
        }

        private void InitRights(ModelBuilder modelBuilder)
        {
            var rights = Enum.GetValues(typeof(RightEnum)) as RightEnum[];
            modelBuilder.Entity<Right>().HasData(
                (rights ?? Array.Empty<RightEnum>()).Select(r => new Right {Id = Guid.NewGuid(), Name = r}));
        }

        private void InitLanguages(ModelBuilder modelBuilder)
        {
            var languages = Enum.GetValues(typeof(LanguageEnum)) as LanguageEnum[];
            modelBuilder.Entity<ProgrammingLanguage>().HasData(
                (languages ?? Array.Empty<LanguageEnum>()).Select(l => new ProgrammingLanguage
                    {Id = Guid.NewGuid(), Name = l}));
        }

        private void InitDecimalPrecisions(ModelBuilder modelBuilder)
        {
            modelBuilder.Model
                .GetEntityTypes()
                .SelectMany(type => type.GetProperties())
                .Where(p => p.ClrType == typeof(decimal) || p.ClrType == typeof(decimal?))
                .ToList()
                .ForEach(property =>
                {
                    property.SetPrecision(18);
                    property.SetScale(6);
                });
        }

        public void InitCronStatus(ModelBuilder modelBuilder)
        {
            var status = Enum.GetValues(typeof(CronStatusEnum)) as CronStatusEnum[];
            modelBuilder.Entity<CronStatus>().HasData(
                (status ?? Array.Empty<CronStatusEnum>()).Select(r => new CronStatus()
                    {Id = Guid.NewGuid(), Code = r}));
        }
    }
}