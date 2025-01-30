﻿// <auto-generated />
using System;
using FootballManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace FootballManager.Infrastructure.Data.Migrations
{
    [DbContext(typeof(AppDbContext))]
    partial class AppDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "9.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("FootballManager.Domain.Entities.Club", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<decimal>("Budget")
                        .HasColumnType("numeric");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.ToTable("Clubs");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            Budget = 100000000m,
                            Name = "Manchester United"
                        },
                        new
                        {
                            Id = 2,
                            Budget = 90000000m,
                            Name = "Liverpool FC"
                        },
                        new
                        {
                            Id = 3,
                            Budget = 85000000m,
                            Name = "Arsenal FC"
                        },
                        new
                        {
                            Id = 4,
                            Budget = 100m,
                            Name = "Orihuela Club de Futbol"
                        });
                });

            modelBuilder.Entity("FootballManager.Domain.Entities.Coach", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("ClubId")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("DateOfBirth")
                        .HasColumnType("date");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<decimal>("Salary")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("ClubId");

                    b.ToTable("Coaches");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ClubId = 1,
                            DateOfBirth = new DateOnly(1970, 2, 2),
                            Email = "erik.tenhag@manutd.com",
                            FirstName = "Erik",
                            LastName = "Ten Hag",
                            Salary = 8000000m
                        },
                        new
                        {
                            Id = 2,
                            ClubId = 2,
                            DateOfBirth = new DateOnly(1967, 6, 16),
                            Email = "jurgen.klopp@liverpoolfc.com",
                            FirstName = "Jurgen",
                            LastName = "Klopp",
                            Salary = 9000000m
                        },
                        new
                        {
                            Id = 3,
                            ClubId = 3,
                            DateOfBirth = new DateOnly(1982, 3, 26),
                            Email = "mikel.arteta@arsenal.com",
                            FirstName = "Mikel",
                            LastName = "Arteta",
                            Salary = 7500000m
                        });
                });

            modelBuilder.Entity("FootballManager.Domain.Entities.Player", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<int?>("ClubId")
                        .HasColumnType("integer");

                    b.Property<DateOnly>("DateOfBirth")
                        .HasColumnType("date");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Position")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<decimal>("Salary")
                        .HasColumnType("numeric");

                    b.HasKey("Id");

                    b.HasIndex("ClubId");

                    b.ToTable("Players");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            ClubId = 1,
                            DateOfBirth = new DateOnly(1990, 11, 7),
                            Email = "david.degea@manutd.com",
                            FirstName = "David",
                            LastName = "De Gea",
                            Position = "Goalkeeper",
                            Salary = 375000m
                        },
                        new
                        {
                            Id = 2,
                            ClubId = 1,
                            DateOfBirth = new DateOnly(1993, 3, 5),
                            Email = "harry.maguire@manutd.com",
                            FirstName = "Harry",
                            LastName = "Maguire",
                            Position = "Defender",
                            Salary = 290000m
                        },
                        new
                        {
                            Id = 3,
                            ClubId = 1,
                            DateOfBirth = new DateOnly(1994, 9, 8),
                            Email = "bruno.fernandes@manutd.com",
                            FirstName = "Bruno",
                            LastName = "Fernandes",
                            Position = "Midfielder",
                            Salary = 350000m
                        },
                        new
                        {
                            Id = 4,
                            ClubId = 1,
                            DateOfBirth = new DateOnly(1997, 10, 31),
                            Email = "marcus.rashford@manutd.com",
                            FirstName = "Marcus",
                            LastName = "Rashford",
                            Position = "Forward",
                            Salary = 330000m
                        },
                        new
                        {
                            Id = 5,
                            ClubId = 2,
                            DateOfBirth = new DateOnly(1992, 10, 2),
                            Email = "alisson.becker@liverpoolfc.com",
                            FirstName = "Alisson",
                            LastName = "Becker",
                            Position = "Goalkeeper",
                            Salary = 350000m
                        },
                        new
                        {
                            Id = 6,
                            ClubId = 2,
                            DateOfBirth = new DateOnly(1991, 7, 8),
                            Email = "virgil.vandijk@liverpoolfc.com",
                            FirstName = "Virgil",
                            LastName = "van Dijk",
                            Position = "Defender",
                            Salary = 380000m
                        },
                        new
                        {
                            Id = 7,
                            ClubId = 2,
                            DateOfBirth = new DateOnly(1990, 6, 17),
                            Email = "jordan.henderson@liverpoolfc.com",
                            FirstName = "Jordan",
                            LastName = "Henderson",
                            Position = "Midfielder",
                            Salary = 290000m
                        },
                        new
                        {
                            Id = 8,
                            ClubId = 2,
                            DateOfBirth = new DateOnly(1992, 6, 15),
                            Email = "mohamed.salah@liverpoolfc.com",
                            FirstName = "Mohamed",
                            LastName = "Salah",
                            Position = "Forward",
                            Salary = 400000m
                        },
                        new
                        {
                            Id = 9,
                            ClubId = 3,
                            DateOfBirth = new DateOnly(1998, 5, 14),
                            Email = "aaron.ramsdale@arsenal.com",
                            FirstName = "Aaron",
                            LastName = "Ramsdale",
                            Position = "Goalkeeper",
                            Salary = 280000m
                        },
                        new
                        {
                            Id = 10,
                            ClubId = 3,
                            DateOfBirth = new DateOnly(2001, 3, 24),
                            Email = "william.saliba@arsenal.com",
                            FirstName = "William",
                            LastName = "Saliba",
                            Position = "Defender",
                            Salary = 290000m
                        },
                        new
                        {
                            Id = 11,
                            ClubId = 3,
                            DateOfBirth = new DateOnly(1998, 12, 17),
                            Email = "martin.odegaard@arsenal.com",
                            FirstName = "Martin",
                            LastName = "Odegaard",
                            Position = "Midfielder",
                            Salary = 315000m
                        },
                        new
                        {
                            Id = 12,
                            ClubId = 3,
                            DateOfBirth = new DateOnly(2001, 9, 5),
                            Email = "bukayo.saka@arsenal.com",
                            FirstName = "Bukayo",
                            LastName = "Saka",
                            Position = "Forward",
                            Salary = 330000m
                        });
                });

            modelBuilder.Entity("FootballManager.Domain.Entities.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime?>("LastLoginAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("PasswordHash")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Role")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("character varying(50)");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("character varying(100)");

                    b.HasKey("Id");

                    b.HasIndex("Username")
                        .IsUnique();

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = 1,
                            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc),
                            PasswordHash = "$2a$11$K0hLTJgkZQQpkgEYz0BWwe/zYhisFCbwE3/UJ3gvB4y.f89acdLA2",
                            Role = "Admin",
                            Username = "admin"
                        });
                });

            modelBuilder.Entity("RefreshToken", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                    b.Property<DateTime>("ExpiresAt")
                        .HasColumnType("timestamp with time zone");

                    b.Property<string>("Token")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<int?>("UserId")
                        .HasColumnType("integer");

                    b.Property<string>("Username")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("RefreshTokens");
                });

            modelBuilder.Entity("FootballManager.Domain.Entities.Coach", b =>
                {
                    b.HasOne("FootballManager.Domain.Entities.Club", null)
                        .WithMany("Coaches")
                        .HasForeignKey("ClubId");
                });

            modelBuilder.Entity("FootballManager.Domain.Entities.Player", b =>
                {
                    b.HasOne("FootballManager.Domain.Entities.Club", null)
                        .WithMany("Players")
                        .HasForeignKey("ClubId");
                });

            modelBuilder.Entity("RefreshToken", b =>
                {
                    b.HasOne("FootballManager.Domain.Entities.User", null)
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId");
                });

            modelBuilder.Entity("FootballManager.Domain.Entities.Club", b =>
                {
                    b.Navigation("Coaches");

                    b.Navigation("Players");
                });

            modelBuilder.Entity("FootballManager.Domain.Entities.User", b =>
                {
                    b.Navigation("RefreshTokens");
                });
#pragma warning restore 612, 618
        }
    }
}
