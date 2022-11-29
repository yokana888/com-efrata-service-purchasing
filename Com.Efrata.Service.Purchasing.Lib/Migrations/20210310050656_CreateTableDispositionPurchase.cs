using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class CreateTableDispositionPurchase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GarmentDispositionPurchases",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    Amount = table.Column<double>(nullable: false),
                    Bank = table.Column<string>(nullable: true),
                    Category = table.Column<string>(nullable: true),
                    ConfirmationOrderNo = table.Column<string>(nullable: true),
                    CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    CurrencyId = table.Column<int>(nullable: false),
                    CurrencyName = table.Column<string>(nullable: true),
                    DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    DispositionNo = table.Column<string>(nullable: true),
                    Dpp = table.Column<double>(nullable: false),
                    DueDate = table.Column<DateTimeOffset>(nullable: false),
                    IncomeTax = table.Column<double>(nullable: false),
                    InvoiceProformaNo = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    OtherCost = table.Column<double>(nullable: false),
                    PaymentType = table.Column<string>(nullable: true),
                    SupplierId = table.Column<int>(nullable: false),
                    SupplierName = table.Column<string>(nullable: true),
                    VAT = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentDispositionPurchases", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentDispositionPurchaseItem",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(nullable: false),
                    EPOId = table.Column<int>(nullable: false),
                    EPONo = table.Column<string>(nullable: true),
                    GarmentDispositionPurchaseId = table.Column<int>(nullable: false),
                    IncomeTaxAmount = table.Column<double>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    IsDispositionCreated = table.Column<bool>(nullable: false),
                    IsDispositionPaid = table.Column<bool>(nullable: false),
                    IsIncomeTax = table.Column<bool>(nullable: false),
                    IsVAT = table.Column<bool>(nullable: false),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    VATAmount = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentDispositionPurchaseItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentDispositionPurchaseItem_GarmentDispositionPurchases_GarmentDispositionPurchaseId",
                        column: x => x.GarmentDispositionPurchaseId,
                        principalTable: "GarmentDispositionPurchases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GarmentDispositionPurchaseDetail",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(nullable: false),
                    GarmentDispositionPurchaseItemId = table.Column<int>(nullable: false),
                    IPOId = table.Column<int>(nullable: false),
                    IPONo = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    PaidPrice = table.Column<double>(nullable: false),
                    PercentageOverQTY = table.Column<double>(nullable: false),
                    PricePerQTY = table.Column<double>(nullable: false),
                    PriceTotal = table.Column<double>(nullable: false),
                    ProductId = table.Column<int>(nullable: false),
                    ProductName = table.Column<string>(nullable: true),
                    QTYOrder = table.Column<double>(nullable: false),
                    QTYPaid = table.Column<double>(nullable: false),
                    QTYRemains = table.Column<double>(nullable: false),
                    QTYUnit = table.Column<string>(nullable: true),
                    ROId = table.Column<int>(nullable: false),
                    RONo = table.Column<string>(nullable: true),
                    UnitId = table.Column<int>(nullable: false),
                    UnitName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentDispositionPurchaseDetail", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentDispositionPurchaseDetail_GarmentDispositionPurchaseItem_GarmentDispositionPurchaseItemId",
                        column: x => x.GarmentDispositionPurchaseItemId,
                        principalTable: "GarmentDispositionPurchaseItem",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentDispositionPurchaseDetail_GarmentDispositionPurchaseItemId",
                table: "GarmentDispositionPurchaseDetail",
                column: "GarmentDispositionPurchaseItemId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentDispositionPurchaseItem_GarmentDispositionPurchaseId",
                table: "GarmentDispositionPurchaseItem",
                column: "GarmentDispositionPurchaseId");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentDispositionPurchases_DispositionNo",
                table: "GarmentDispositionPurchases",
                column: "DispositionNo",
                unique: true,
                filter: "[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2020-02-01 00:00:00.0000000')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarmentDispositionPurchaseDetail");

            migrationBuilder.DropTable(
                name: "GarmentDispositionPurchaseItem");

            migrationBuilder.DropTable(
                name: "GarmentDispositionPurchases");
        }
    }
}
