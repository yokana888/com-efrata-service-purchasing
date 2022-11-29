using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class BankExpenditureNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BankExpenditureNotes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    BGCheckNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BankAccountName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BankCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BankCurrencyCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BankCurrencyId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BankCurrencyRate = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BankId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DocumentNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    GrandTotal = table.Column<double>(type: "float", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SupplierCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SupplierId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SupplierImport = table.Column<bool>(type: "bit", nullable: false),
                    SupplierName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankExpenditureNotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BankExpenditureNoteDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    BankExpenditureNoteId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DivisionCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DivisionName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DueDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    InvoiceNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    SupplierCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TotalPaid = table.Column<double>(type: "float", nullable: false),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UPODate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UnitPaymentOrderId = table.Column<long>(type: "bigint", nullable: false),
                    UnitPaymentOrderNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Vat = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankExpenditureNoteDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankExpenditureNoteDetails_BankExpenditureNotes_BankExpenditureNoteId",
                        column: x => x.BankExpenditureNoteId,
                        principalTable: "BankExpenditureNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "BankExpenditureNoteItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    BankExpenditureNoteDetailId = table.Column<long>(type: "bigint", nullable: false),
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
                    Price = table.Column<double>(type: "float", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProductId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitPaymentOrderItemId = table.Column<long>(type: "bigint", nullable: false),
                    Uom = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BankExpenditureNoteItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BankExpenditureNoteItems_BankExpenditureNoteDetails_BankExpenditureNoteDetailId",
                        column: x => x.BankExpenditureNoteDetailId,
                        principalTable: "BankExpenditureNoteDetails",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BankExpenditureNoteDetails_BankExpenditureNoteId",
                table: "BankExpenditureNoteDetails",
                column: "BankExpenditureNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_BankExpenditureNoteItems_BankExpenditureNoteDetailId",
                table: "BankExpenditureNoteItems",
                column: "BankExpenditureNoteDetailId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BankExpenditureNoteItems");

            migrationBuilder.DropTable(
                name: "BankExpenditureNoteDetails");

            migrationBuilder.DropTable(
                name: "BankExpenditureNotes");
        }
    }
}
