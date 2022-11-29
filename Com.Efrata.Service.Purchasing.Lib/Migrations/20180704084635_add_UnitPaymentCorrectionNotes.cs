using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class add_UnitPaymentCorrectionNotes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UnitPaymentCorrectionNotes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CorrectionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CorrectionType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DueDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    IncomeTaxCorrectionName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncomeTaxCorrectionNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InvoiceCorrectionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    InvoiceCorrectionNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReleaseOrderNoteNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReturNoteNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UPCNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UPOId = table.Column<long>(type: "bigint", nullable: false),
                    UPONo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    VatTaxCorrectionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    VatTaxCorrectionNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    useIncomeTax = table.Column<bool>(type: "bit", nullable: false),
                    useVat = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitPaymentCorrectionNotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UnitPaymentCorrectionNoteItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CurrencyCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CurrencyRate = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EPONo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PRDetailId = table.Column<long>(type: "bigint", nullable: false),
                    PRId = table.Column<long>(type: "bigint", nullable: false),
                    PRNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PricePerDealUnitAfter = table.Column<long>(type: "bigint", nullable: false),
                    PricePerDealUnitBefore = table.Column<long>(type: "bigint", nullable: false),
                    PriceTotalAfter = table.Column<long>(type: "bigint", nullable: false),
                    PriceTotalBefore = table.Column<long>(type: "bigint", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<long>(type: "bigint", nullable: false),
                    UPCId = table.Column<long>(type: "bigint", nullable: false),
                    UPODetailId = table.Column<long>(type: "bigint", nullable: false),
                    URNNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UomId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UomUnit = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UnitPaymentCorrectionNoteItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UnitPaymentCorrectionNoteItems_UnitPaymentCorrectionNotes_UPCId",
                        column: x => x.UPCId,
                        principalTable: "UnitPaymentCorrectionNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UnitPaymentCorrectionNoteItems_UPCId",
                table: "UnitPaymentCorrectionNoteItems",
                column: "UPCId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UnitPaymentCorrectionNoteItems");

            migrationBuilder.DropTable(
                name: "UnitPaymentCorrectionNotes");
        }
    }
}
