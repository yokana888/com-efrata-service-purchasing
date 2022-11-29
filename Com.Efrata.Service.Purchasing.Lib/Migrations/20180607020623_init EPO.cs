using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class initEPO : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExternalPurchaseOrders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CurrencyId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CurrencyRate = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveryDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DivisionCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DivisionId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DivisionName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    EPONo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FreightCostBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsCanceled = table.Column<bool>(type: "bit", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsPosted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OrderDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    PaymentDueDays = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PaymentMethod = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SupplierId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UseIncomeTax = table.Column<bool>(type: "bit", nullable: false),
                    UseVat = table.Column<bool>(type: "bit", nullable: false),
                    VatId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    VatName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    VatRate = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalPurchaseOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ExternalPurchaseOrderItems",
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
                    EPOId = table.Column<long>(type: "bigint", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    POId = table.Column<long>(type: "bigint", nullable: false),
                    PONo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PRId = table.Column<long>(type: "bigint", nullable: false),
                    PRNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalPurchaseOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalPurchaseOrderItems_ExternalPurchaseOrders_EPOId",
                        column: x => x.EPOId,
                        principalTable: "ExternalPurchaseOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ExternalPurchaseOrderDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Conversion = table.Column<double>(type: "float", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DOQuantity = table.Column<double>(type: "float", nullable: false),
                    DealQuantity = table.Column<double>(type: "float", nullable: false),
                    DealUomId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DealUomUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultQuantity = table.Column<double>(type: "float", nullable: false),
                    DefaultUomId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DefaultUomUnit = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EPOItemId = table.Column<long>(type: "bigint", nullable: false),
                    IncludePpn = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    POItemId = table.Column<long>(type: "bigint", nullable: false),
                    PRItemId = table.Column<long>(type: "bigint", nullable: false),
                    PriceBeforeTax = table.Column<double>(type: "float", nullable: false),
                    PricePerDealUnit = table.Column<double>(type: "float", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProductId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ProductRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptQuantity = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalPurchaseOrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalPurchaseOrderDetails_ExternalPurchaseOrderItems_EPOItemId",
                        column: x => x.EPOItemId,
                        principalTable: "ExternalPurchaseOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ExternalPurchaseOrderDetails_EPOItemId",
                table: "ExternalPurchaseOrderDetails",
                column: "EPOItemId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalPurchaseOrderItems_EPOId",
                table: "ExternalPurchaseOrderItems",
                column: "EPOId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ExternalPurchaseOrderDetails");

            migrationBuilder.DropTable(
                name: "ExternalPurchaseOrderItems");

            migrationBuilder.DropTable(
                name: "ExternalPurchaseOrders");
        }
    }
}
