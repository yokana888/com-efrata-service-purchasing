using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class GarmentUnitReceiptNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GarmentInventoryDocuments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    No = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ReferenceNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ReferenceType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorageCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StorageId = table.Column<long>(type: "bigint", nullable: false),
                    StorageName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentInventoryDocuments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentInventoryMovements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    After = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    Before = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Date = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    No = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProductCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    ReferenceNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ReferenceType = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StockPlanning = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    StorageCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StorageId = table.Column<long>(type: "bigint", nullable: false),
                    StorageName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UomId = table.Column<long>(type: "bigint", nullable: false),
                    UomUnit = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentInventoryMovements", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentInventorySummaries",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
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
                    No = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProductCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    StockPlanning = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    StorageCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StorageId = table.Column<long>(type: "bigint", nullable: false),
                    StorageName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UomId = table.Column<long>(type: "bigint", nullable: false),
                    UomUnit = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentInventorySummaries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentUnitReceiptNotes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DOId = table.Column<long>(type: "bigint", nullable: false),
                    DONo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCorrection = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsStorage = table.Column<bool>(type: "bit", nullable: false),
                    IsUnitDO = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReceiptDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorageCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    StorageId = table.Column<long>(type: "bigint", nullable: false),
                    StorageName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SupplierCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SupplierId = table.Column<long>(type: "bigint", nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    URNNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitId = table.Column<long>(type: "bigint", nullable: false),
                    UnitName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentUnitReceiptNotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentInventoryDocumentItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GarmentInventoryDocumentId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProductRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    UomId = table.Column<long>(type: "bigint", nullable: false),
                    UomUnit = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentInventoryDocumentItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentInventoryDocumentItems_GarmentInventoryDocuments_GarmentInventoryDocumentId",
                        column: x => x.GarmentInventoryDocumentId,
                        principalTable: "GarmentInventoryDocuments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GarmentUnitReceiptNoteItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Conversion = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DODetailId = table.Column<long>(type: "bigint", nullable: false),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DesignColor = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EPOItemId = table.Column<long>(type: "bigint", nullable: false),
                    IsCorrection = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    POId = table.Column<long>(type: "bigint", nullable: false),
                    POItemId = table.Column<long>(type: "bigint", nullable: false),
                    POSerialNumber = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PRId = table.Column<long>(type: "bigint", nullable: false),
                    PRItemId = table.Column<long>(type: "bigint", nullable: false),
                    PRNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PricePerDealUnit = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProductRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RONo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ReceiptQuantity = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    SmallQuantity = table.Column<decimal>(type: "decimal(18, 2)", nullable: false),
                    SmallUomId = table.Column<long>(type: "bigint", nullable: false),
                    SmallUomUnit = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    URNId = table.Column<long>(type: "bigint", nullable: false),
                    UomId = table.Column<long>(type: "bigint", nullable: false),
                    UomUnit = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentUnitReceiptNoteItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentUnitReceiptNoteItems_GarmentUnitReceiptNotes_URNId",
                        column: x => x.URNId,
                        principalTable: "GarmentUnitReceiptNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentInventoryDocumentItems_GarmentInventoryDocumentId",
                table: "GarmentInventoryDocumentItems",
                column: "GarmentInventoryDocumentId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentUnitReceiptNoteItems_URNId",
                table: "GarmentUnitReceiptNoteItems",
                column: "URNId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarmentInventoryDocumentItems");

            migrationBuilder.DropTable(
                name: "GarmentInventoryMovements");

            migrationBuilder.DropTable(
                name: "GarmentInventorySummaries");

            migrationBuilder.DropTable(
                name: "GarmentUnitReceiptNoteItems");

            migrationBuilder.DropTable(
                name: "GarmentInventoryDocuments");

            migrationBuilder.DropTable(
                name: "GarmentUnitReceiptNotes");
        }
    }
}
