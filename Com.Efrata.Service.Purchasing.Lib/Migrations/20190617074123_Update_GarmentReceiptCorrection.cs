using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Update_GarmentReceiptCorrection : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GarmentReceiptCorrections",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CorrectionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CorrectionNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CorrectionType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
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
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorageCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StorageId = table.Column<long>(type: "bigint", nullable: false),
                    StorageName = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    URNId = table.Column<long>(type: "bigint", nullable: false),
                    URNNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitId = table.Column<long>(type: "bigint", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentReceiptCorrections", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentReceiptCorrectionItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Conversion = table.Column<double>(type: "float", nullable: false),
                    CorrectionConversion = table.Column<double>(type: "float", nullable: false),
                    CorrectionId = table.Column<long>(type: "bigint", nullable: false),
                    CorrectionQuantity = table.Column<double>(type: "float", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DODetailId = table.Column<long>(type: "bigint", nullable: false),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DesignColor = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EPOItemId = table.Column<long>(type: "bigint", nullable: false),
                    FabricType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    POItemId = table.Column<long>(type: "bigint", nullable: false),
                    POSerialNumber = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    PRItemId = table.Column<long>(type: "bigint", nullable: false),
                    PricePerDealUnit = table.Column<double>(type: "float", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    RONo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SmallQuantity = table.Column<double>(type: "float", nullable: false),
                    SmallUomId = table.Column<long>(type: "bigint", nullable: false),
                    SmallUomUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    URNItemId = table.Column<long>(type: "bigint", nullable: false),
                    UomId = table.Column<long>(type: "bigint", nullable: false),
                    UomUnit = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentReceiptCorrectionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentReceiptCorrectionItems_GarmentReceiptCorrections_CorrectionId",
                        column: x => x.CorrectionId,
                        principalTable: "GarmentReceiptCorrections",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentReceiptCorrectionItems_CorrectionId",
                table: "GarmentReceiptCorrectionItems",
                column: "CorrectionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarmentReceiptCorrectionItems");

            migrationBuilder.DropTable(
                name: "GarmentReceiptCorrections");
        }
    }
}
