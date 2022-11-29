using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class UnitPaymentOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UnitPaymentOrders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CategoryCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CategoryId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CategoryName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CurrencyId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CurrencyRate = table.Column<double>(type: "float", nullable: false),
                    Date = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DivisionCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DivisionId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DivisionName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DueDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IncomeTaxDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IncomeTaxId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IncomeTaxName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    IncomeTaxNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IncomeTaxRate = table.Column<double>(type: "float", nullable: false),
                    InvoiceDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    InvoiceNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsCorrection = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    IsPaid = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PibNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SupplierId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UPONo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UseIncomeTax = table.Column<bool>(type: "bit", nullable: false),
                    UseVat = table.Column<bool>(type: "bit", nullable: false),
                    VatDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    VatNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitPaymentOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnitPaymentOrderItems",
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
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UPOId = table.Column<long>(type: "bigint", nullable: false),
                    URNId = table.Column<long>(type: "bigint", nullable: false),
                    URNNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitPaymentOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitPaymentOrderItems_UnitPaymentOrders_UPOId",
                        column: x => x.UPOId,
                        principalTable: "UnitPaymentOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UnitPaymentOrderDetails",
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
                    EPONo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PRId = table.Column<long>(type: "bigint", nullable: false),
                    PRItemId = table.Column<long>(type: "bigint", nullable: false),
                    PRNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    PricePerDealUnit = table.Column<double>(type: "float", nullable: false),
                    PricePerDealUnitCorrection = table.Column<double>(type: "float", nullable: false),
                    PriceTotal = table.Column<double>(type: "float", nullable: false),
                    PriceTotalCorrection = table.Column<double>(type: "float", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProductId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProductRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptQuantity = table.Column<double>(type: "float", nullable: false),
                    UPOItemId = table.Column<long>(type: "bigint", nullable: false),
                    URNDetailId = table.Column<long>(type: "bigint", nullable: false),
                    UomId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UomUnit = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitPaymentOrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitPaymentOrderDetails_UnitPaymentOrderItems_UPOItemId",
                        column: x => x.UPOItemId,
                        principalTable: "UnitPaymentOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnitPaymentOrderDetails_UPOItemId",
                table: "UnitPaymentOrderDetails",
                column: "UPOItemId");

            migrationBuilder.CreateIndex(
                name: "IX_UnitPaymentOrderItems_UPOId",
                table: "UnitPaymentOrderItems",
                column: "UPOId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnitPaymentOrderDetails");

            migrationBuilder.DropTable(
                name: "UnitPaymentOrderItems");

            migrationBuilder.DropTable(
                name: "UnitPaymentOrders");
        }
    }
}
