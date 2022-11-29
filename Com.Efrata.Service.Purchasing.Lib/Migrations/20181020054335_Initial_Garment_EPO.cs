using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Initial_Garment_EPO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GarmentExternalPurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    CurrencyRate = table.Column<double>(type: "float", nullable: false),
                    DarkPerspiration = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DryRubbing = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EPONo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FreightCostBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncomeTaxId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IncomeTaxName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IncomeTaxRate = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IsApproved = table.Column<bool>(type: "bit", nullable: false),
                    IsCanceled = table.Column<bool>(type: "bit", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsIncomeTax = table.Column<bool>(type: "bit", nullable: false),
                    IsOverBudget = table.Column<bool>(type: "bit", nullable: false),
                    IsPosted = table.Column<bool>(type: "bit", nullable: false),
                    IsUseVat = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LightMedPerspiration = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    OrderDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PaymentDueDays = table.Column<int>(type: "int", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PieceLength = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    QualityStandardType = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Shrinkage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SupplierCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SupplierId = table.Column<int>(type: "int", maxLength: 255, nullable: false),
                    SupplierImport = table.Column<bool>(type: "bit", maxLength: 255, nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Washing = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    WetRubbing = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentExternalPurchaseOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentExternalPurchaseOrderItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Article = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    BudgetPrice = table.Column<double>(type: "float", nullable: false),
                    Conversion = table.Column<double>(type: "float", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DOQuantity = table.Column<double>(type: "float", nullable: false),
                    DealQuantity = table.Column<double>(type: "float", nullable: false),
                    DealUomId = table.Column<int>(type: "int", nullable: false),
                    DealUomUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultQuantity = table.Column<double>(type: "float", nullable: false),
                    DefaultUomId = table.Column<int>(type: "int", nullable: false),
                    DefaultUomUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    GarmentEPOId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsOverBudget = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    POId = table.Column<int>(type: "int", nullable: false),
                    PONo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PO_SerialNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PRId = table.Column<int>(type: "int", nullable: false),
                    PRNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PricePerDealUnit = table.Column<double>(type: "float", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProductId = table.Column<int>(type: "int", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    RONo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SmallQuantity = table.Column<double>(type: "float", nullable: false),
                    SmallUomId = table.Column<int>(type: "int", nullable: false),
                    SmallUomUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UsedBudget = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentExternalPurchaseOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentExternalPurchaseOrderItems_GarmentExternalPurchaseOrders_GarmentEPOId",
                        column: x => x.GarmentEPOId,
                        principalTable: "GarmentExternalPurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentExternalPurchaseOrderItems_GarmentEPOId",
                table: "GarmentExternalPurchaseOrderItems",
                column: "GarmentEPOId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarmentExternalPurchaseOrderItems");

            migrationBuilder.DropTable(
                name: "GarmentExternalPurchaseOrders");
        }
    }
}
