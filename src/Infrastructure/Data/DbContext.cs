namespace FootballManager.Infrastructure.Data;

using FootballManager.Domain.Entities;
using FootballManager.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using BCrypt.Net;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<Club> Clubs { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Coach> Coaches { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<RefreshToken> RefreshTokens {get; set;}

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Club>(entity =>
        {
            entity.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.Budget)
                .IsRequired();

            entity.HasMany(c => c.Players)
                .WithOne()  
                .HasForeignKey(e => e.ClubId)
                .IsRequired(false);

            entity.HasMany(c => c.Coaches)
                .WithOne() 
                .HasForeignKey(e => e.ClubId)
                .IsRequired(false);
        });

        modelBuilder.Entity<Player>(entity =>
        {
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.Position)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            entity.Property(e => e.DateOfBirth)
                .IsRequired();

            entity.Property(e => e.Salary)
                .IsRequired();

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<Coach>(entity =>
        {
            entity.Property(e => e.FirstName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.LastName)
                .IsRequired()
                .HasMaxLength(50);

            entity.Property(e => e.DateOfBirth)
                .IsRequired();

            entity.Property(e => e.Salary)
                .IsRequired();

            entity.Property(e => e.Email)
                .IsRequired()
                .HasMaxLength(100);
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username)
                .IsUnique();

            entity.Property(e => e.Username)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.PasswordHash)
                .IsRequired();

            entity.Property(e => e.Role)
                .IsRequired()
                .HasMaxLength(50);
        });

        // Seed Clubs
        modelBuilder.Entity<Club>().HasData(
            new { Id = 1, Name = "Manchester United", Budget = 100000000M },
            new { Id = 2, Name = "Liverpool FC", Budget = 90000000M },
            new { Id = 3, Name = "Arsenal FC", Budget = 85000000M },
            new { Id = 4, Name = "Orihuela Club de Futbol", Budget = 100M }
        );

        // Seed Coaches
        modelBuilder.Entity<Coach>().HasData(
            new { 
                Id = 1, 
                FirstName = "Erik", 
                LastName = "Ten Hag", 
                DateOfBirth = new DateOnly(1970, 2, 2), 
                Salary = 8000000M, 
                ClubId = 1,
                Email = "erik.tenhag@manutd.com"
            },
            new { 
                Id = 2, 
                FirstName = "Jurgen", 
                LastName = "Klopp", 
                DateOfBirth = new DateOnly(1967, 6, 16), 
                Salary = 9000000M, 
                ClubId = 2,
                Email = "jurgen.klopp@liverpoolfc.com"
            },
            new { 
                Id = 3, 
                FirstName = "Mikel", 
                LastName = "Arteta", 
                DateOfBirth = new DateOnly(1982, 3, 26), 
                Salary = 7500000M, 
                ClubId = 3,
                Email = "mikel.arteta@arsenal.com"
            }
        );

        // Seed Players
        modelBuilder.Entity<Player>().HasData(
            // Manchester United Players
            new { Id = 1, FirstName = "David", LastName = "De Gea", DateOfBirth = new DateOnly(1990, 11, 7), 
                  Position = Position.Goalkeeper, Salary = 375000M, ClubId = 1, Email = "david.degea@manutd.com" },
            new { Id = 2, FirstName = "Harry", LastName = "Maguire", DateOfBirth = new DateOnly(1993, 3, 5), 
                  Position = Position.Defender, Salary = 290000M, ClubId = 1, Email = "harry.maguire@manutd.com" },
            new { Id = 3, FirstName = "Bruno", LastName = "Fernandes", DateOfBirth = new DateOnly(1994, 9, 8), 
                  Position = Position.Midfielder, Salary = 350000M, ClubId = 1, Email = "bruno.fernandes@manutd.com" },
            new { Id = 4, FirstName = "Marcus", LastName = "Rashford", DateOfBirth = new DateOnly(1997, 10, 31), 
                  Position = Position.Forward, Salary = 330000M, ClubId = 1, Email = "marcus.rashford@manutd.com" },

            // Liverpool Players
            new { Id = 5, FirstName = "Alisson", LastName = "Becker", DateOfBirth = new DateOnly(1992, 10, 2), 
                  Position = Position.Goalkeeper, Salary = 350000M, ClubId = 2, Email = "alisson.becker@liverpoolfc.com" },
            new { Id = 6, FirstName = "Virgil", LastName = "van Dijk", DateOfBirth = new DateOnly(1991, 7, 8), 
                  Position = Position.Defender, Salary = 380000M, ClubId = 2, Email = "virgil.vandijk@liverpoolfc.com" },
            new { Id = 7, FirstName = "Jordan", LastName = "Henderson", DateOfBirth = new DateOnly(1990, 6, 17), 
                  Position = Position.Midfielder, Salary = 290000M, ClubId = 2, Email = "jordan.henderson@liverpoolfc.com" },
            new { Id = 8, FirstName = "Mohamed", LastName = "Salah", DateOfBirth = new DateOnly(1992, 6, 15), 
                  Position = Position.Forward, Salary = 400000M, ClubId = 2, Email = "mohamed.salah@liverpoolfc.com" },

            // Arsenal Players
            new { Id = 9, FirstName = "Aaron", LastName = "Ramsdale", DateOfBirth = new DateOnly(1998, 5, 14), 
                  Position = Position.Goalkeeper, Salary = 280000M, ClubId = 3, Email = "aaron.ramsdale@arsenal.com" },
            new { Id = 10, FirstName = "William", LastName = "Saliba", DateOfBirth = new DateOnly(2001, 3, 24), 
                  Position = Position.Defender, Salary = 290000M, ClubId = 3, Email = "william.saliba@arsenal.com" },
            new { Id = 11, FirstName = "Martin", LastName = "Odegaard", DateOfBirth = new DateOnly(1998, 12, 17), 
                  Position = Position.Midfielder, Salary = 315000M, ClubId = 3, Email = "martin.odegaard@arsenal.com" },
            new { Id = 12, FirstName = "Bukayo", LastName = "Saka", DateOfBirth = new DateOnly(2001, 9, 5), 
                  Position = Position.Forward, Salary = 330000M, ClubId = 3, Email = "bukayo.saka@arsenal.com" }
        );
        

        // Seed admin user (password: "admin123")
        // Using salt here to make the call deterministic so migrations work as expected
        modelBuilder.Entity<User>().HasData(
            new
            {
                Id = 1,
                Username = "admin",
                PasswordHash = BCrypt.HashPassword("admin123", salt: "$2a$11$K0hLTJgkZQQpkgEYz0BWwe"),
                Role = "Admin",
                CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc)
            }
        );
    }
}
