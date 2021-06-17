using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CryptoFaucet.Database.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FaucetAccount",
                columns: table => new
                {
                    AccountId = table.Column<string>(type: "TEXT", nullable: false),
                    BtcBalance = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaucetAccount", x => x.AccountId);
                });

            migrationBuilder.CreateTable(
                name: "FaucetClaim",
                columns: table => new
                {
                    ClaimId = table.Column<string>(type: "TEXT", nullable: false),
                    Email = table.Column<string>(type: "TEXT", nullable: true),
                    ClaimTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Status = table.Column<short>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaucetClaim", x => x.ClaimId);
                });

            migrationBuilder.CreateTable(
                name: "FaucetTransaction",
                columns: table => new
                {
                    TransactionTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    InitialBalance = table.Column<decimal>(type: "TEXT", nullable: false),
                    CurrentBalance = table.Column<decimal>(type: "TEXT", nullable: false),
                    BtcExchangeRate = table.Column<decimal>(type: "TEXT", nullable: false),
                    AccountId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FaucetTransaction", x => x.TransactionTime);
                    table.ForeignKey(
                        name: "FK_FaucetTransaction_FaucetAccount_AccountId",
                        column: x => x.AccountId,
                        principalTable: "FaucetAccount",
                        principalColumn: "AccountId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "FaucetAccount",
                columns: new[] { "AccountId", "BtcBalance" },
                values: new object[] { "396806e5-03f6-40ff-ae8f-2a5cb736dfe4", 0m });

            migrationBuilder.CreateIndex(
                name: "IX_FaucetTransaction_AccountId",
                table: "FaucetTransaction",
                column: "AccountId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FaucetClaim");

            migrationBuilder.DropTable(
                name: "FaucetTransaction");

            migrationBuilder.DropTable(
                name: "FaucetAccount");
        }
    }
}
