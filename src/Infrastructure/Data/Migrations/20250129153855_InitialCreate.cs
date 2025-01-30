using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FootballManager.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Clubs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Budget = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Clubs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    Role = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Coaches",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Salary = table.Column<decimal>(type: "numeric", nullable: false),
                    ClubId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Coaches", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Coaches_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Position = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FirstName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    LastName = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Email = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    DateOfBirth = table.Column<DateOnly>(type: "date", nullable: false),
                    Salary = table.Column<decimal>(type: "numeric", nullable: false),
                    ClubId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Players_Clubs_ClubId",
                        column: x => x.ClubId,
                        principalTable: "Clubs",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Token = table.Column<string>(type: "text", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.InsertData(
                table: "Clubs",
                columns: new[] { "Id", "Budget", "Name" },
                values: new object[,]
                {
                    { 1, 100000000m, "Manchester United" },
                    { 2, 90000000m, "Liverpool FC" },
                    { 3, 85000000m, "Arsenal FC" },
                    { 4, 100m, "Orihuela Club de Futbol" }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "LastLoginAt", "PasswordHash", "Role", "Username" },
                values: new object[] { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), null, "$2a$11$K0hLTJgkZQQpkgEYz0BWwe/zYhisFCbwE3/UJ3gvB4y.f89acdLA2", "Admin", "admin" });

            migrationBuilder.InsertData(
                table: "Coaches",
                columns: new[] { "Id", "ClubId", "DateOfBirth", "Email", "FirstName", "LastName", "Salary" },
                values: new object[,]
                {
                    { 1, 1, new DateOnly(1970, 2, 2), "erik.tenhag@manutd.com", "Erik", "Ten Hag", 8000000m },
                    { 2, 2, new DateOnly(1967, 6, 16), "jurgen.klopp@liverpoolfc.com", "Jurgen", "Klopp", 9000000m },
                    { 3, 3, new DateOnly(1982, 3, 26), "mikel.arteta@arsenal.com", "Mikel", "Arteta", 7500000m }
                });

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "Id", "ClubId", "DateOfBirth", "Email", "FirstName", "LastName", "Position", "Salary" },
                values: new object[,]
                {
                    { 1, 1, new DateOnly(1990, 11, 7), "david.degea@manutd.com", "David", "De Gea", "Goalkeeper", 375000m },
                    { 2, 1, new DateOnly(1993, 3, 5), "harry.maguire@manutd.com", "Harry", "Maguire", "Defender", 290000m },
                    { 3, 1, new DateOnly(1994, 9, 8), "bruno.fernandes@manutd.com", "Bruno", "Fernandes", "Midfielder", 350000m },
                    { 4, 1, new DateOnly(1997, 10, 31), "marcus.rashford@manutd.com", "Marcus", "Rashford", "Forward", 330000m },
                    { 5, 2, new DateOnly(1992, 10, 2), "alisson.becker@liverpoolfc.com", "Alisson", "Becker", "Goalkeeper", 350000m },
                    { 6, 2, new DateOnly(1991, 7, 8), "virgil.vandijk@liverpoolfc.com", "Virgil", "van Dijk", "Defender", 380000m },
                    { 7, 2, new DateOnly(1990, 6, 17), "jordan.henderson@liverpoolfc.com", "Jordan", "Henderson", "Midfielder", 290000m },
                    { 8, 2, new DateOnly(1992, 6, 15), "mohamed.salah@liverpoolfc.com", "Mohamed", "Salah", "Forward", 400000m },
                    { 9, 3, new DateOnly(1998, 5, 14), "aaron.ramsdale@arsenal.com", "Aaron", "Ramsdale", "Goalkeeper", 280000m },
                    { 10, 3, new DateOnly(2001, 3, 24), "william.saliba@arsenal.com", "William", "Saliba", "Defender", 290000m },
                    { 11, 3, new DateOnly(1998, 12, 17), "martin.odegaard@arsenal.com", "Martin", "Odegaard", "Midfielder", 315000m },
                    { 12, 3, new DateOnly(2001, 9, 5), "bukayo.saka@arsenal.com", "Bukayo", "Saka", "Forward", 330000m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_ClubId",
                table: "Coaches",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_Players_ClubId",
                table: "Players",
                column: "ClubId");

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Coaches");

            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.DropTable(
                name: "Clubs");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
