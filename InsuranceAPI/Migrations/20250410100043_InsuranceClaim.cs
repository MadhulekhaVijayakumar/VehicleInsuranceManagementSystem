using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace InsuranceAPI.Migrations
{
    /// <inheritdoc />
    public partial class InsuranceClaim : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ClaimId",
                table: "Documents",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "InsuranceClaims",
                columns: table => new
                {
                    ClaimId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    InsurancePolicyNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    IncidentDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Pending"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InsuranceClaims", x => x.ClaimId);
                    table.ForeignKey(
                        name: "FK_InsuranceClaims_Insurances_InsurancePolicyNumber",
                        column: x => x.InsurancePolicyNumber,
                        principalTable: "Insurances",
                        principalColumn: "InsurancePolicyNumber",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ClaimId",
                table: "Documents",
                column: "ClaimId");

            migrationBuilder.CreateIndex(
                name: "IX_InsuranceClaims_InsurancePolicyNumber",
                table: "InsuranceClaims",
                column: "InsurancePolicyNumber");

            migrationBuilder.AddForeignKey(
                name: "FK_Documents_InsuranceClaims_ClaimId",
                table: "Documents",
                column: "ClaimId",
                principalTable: "InsuranceClaims",
                principalColumn: "ClaimId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Documents_InsuranceClaims_ClaimId",
                table: "Documents");

            migrationBuilder.DropTable(
                name: "InsuranceClaims");

            migrationBuilder.DropIndex(
                name: "IX_Documents_ClaimId",
                table: "Documents");

            migrationBuilder.DropColumn(
                name: "ClaimId",
                table: "Documents");
        }
    }
}
