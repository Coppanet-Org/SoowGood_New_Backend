using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoowGoodWeb.Migrations
{
    /// <inheritdoc />
    public partial class platformServices_platformfinancials_created : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SgPlatformServices",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ServiceName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ServiceDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SgPlatformServices", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SgPlatformFinancialSetups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlatformServicesId = table.Column<long>(type: "bigint", nullable: true),
                    PlatformServicesId1 = table.Column<int>(type: "int", nullable: true),
                    AmountIn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    FeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExternalAmountIn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalFeeAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: true),
                    CreationTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatorId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SgPlatformFinancialSetups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SgPlatformFinancialSetups_SgPlatformServices_PlatformServicesId1",
                        column: x => x.PlatformServicesId1,
                        principalTable: "SgPlatformServices",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_SgPlatformFinancialSetups_PlatformServicesId1",
                table: "SgPlatformFinancialSetups",
                column: "PlatformServicesId1");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SgPlatformFinancialSetups");

            migrationBuilder.DropTable(
                name: "SgPlatformServices");
        }
    }
}
