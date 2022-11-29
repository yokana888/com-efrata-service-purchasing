using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class GarmentPOMasterDistribution : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GarmentPOMasterDistributions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DODate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DOId = table.Column<long>(type: "bigint", nullable: false),
                    DONo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SupplierId = table.Column<long>(type: "bigint", nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentPOMasterDistributions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentPOMasterDistributionItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DODetailId = table.Column<long>(type: "bigint", nullable: false),
                    DOItemId = table.Column<long>(type: "bigint", nullable: false),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    POMasterDistributionId = table.Column<long>(type: "bigint", nullable: false),
                    SCId = table.Column<long>(type: "bigint", nullable: false),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentPOMasterDistributionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentPOMasterDistributionItems_GarmentPOMasterDistributions_POMasterDistributionId",
                        column: x => x.POMasterDistributionId,
                        principalTable: "GarmentPOMasterDistributions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GarmentPOMasterDistributionDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Conversion = table.Column<double>(type: "float", nullable: false),
                    CostCalculationId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    POMasterDistributionItemId = table.Column<long>(type: "bigint", nullable: false),
                    POSerialNumber = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    ProductCode = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    Quantity = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    QuantityCC = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    RONo = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: true),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UomCCId = table.Column<long>(type: "bigint", nullable: false),
                    UomCCUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    UomId = table.Column<long>(type: "bigint", nullable: false),
                    UomUnit = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentPOMasterDistributionDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentPOMasterDistributionDetails_GarmentPOMasterDistributionItems_POMasterDistributionItemId",
                        column: x => x.POMasterDistributionItemId,
                        principalTable: "GarmentPOMasterDistributionItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentPOMasterDistributionDetails_POMasterDistributionItemId",
                table: "GarmentPOMasterDistributionDetails",
                column: "POMasterDistributionItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentPOMasterDistributionItems_POMasterDistributionId",
                table: "GarmentPOMasterDistributionItems",
                column: "POMasterDistributionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarmentPOMasterDistributionDetails");

            migrationBuilder.DropTable(
                name: "GarmentPOMasterDistributionItems");

            migrationBuilder.DropTable(
                name: "GarmentPOMasterDistributions");
        }
    }
}
