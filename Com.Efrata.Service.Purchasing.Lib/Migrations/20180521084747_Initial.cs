using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PurchasingDocumentExpeditions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    BankExpenditureNoteNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CashierDivisionBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    CashierDivisionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
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
                    FinanceDivisionBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FinanceDivisionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    InvoiceNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NotVerifiedReason = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    Position = table.Column<int>(type: "int", nullable: false),
                    SendToCashierDivisionBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SendToCashierDivisionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SendToFinanceDivisionBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SendToFinanceDivisionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SendToPurchasingDivisionBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SendToPurchasingDivisionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SendToVerificationDivisionBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SendToVerificationDivisionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    SupplierCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    TotalPaid = table.Column<double>(type: "float", nullable: false),
                    UPODate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UnitPaymentOrderNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    VerificationDivisionBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    VerificationDivisionDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    VerifyDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchasingDocumentExpeditions", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchasingDocumentExpeditions");
        }
    }
}
