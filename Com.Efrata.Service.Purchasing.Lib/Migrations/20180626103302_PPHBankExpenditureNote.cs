using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class PPHBankExpenditureNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "BankExpenditureNoteDate",
                table: "PurchasingDocumentExpeditions",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "BankExpenditureNotePPHDate",
                table: "PurchasingDocumentExpeditions",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "BankExpenditureNotePPHNo",
                table: "PurchasingDocumentExpeditions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "PPHBankExpenditureNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    BGNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankAccountName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankAccountNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IncomeTaxId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncomeTaxName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IncomeTaxRate = table.Column<double>(type: "float", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    No = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TotalDPP = table.Column<double>(type: "float", nullable: false),
                    TotalIncomeTax = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPHBankExpenditureNotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PPHBankExpenditureNoteItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
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
                    PPHBankExpenditureNoteId = table.Column<int>(type: "int", nullable: false),
                    PurchasingDocumentExpeditionId = table.Column<int>(type: "int", nullable: false),
                    UnitPaymentOrderNo = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PPHBankExpenditureNoteItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PPHBankExpenditureNoteItems_PPHBankExpenditureNotes_PPHBankExpenditureNoteId",
                        column: x => x.PPHBankExpenditureNoteId,
                        principalTable: "PPHBankExpenditureNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_PPHBankExpenditureNoteItems_PurchasingDocumentExpeditions_PurchasingDocumentExpeditionId",
                        column: x => x.PurchasingDocumentExpeditionId,
                        principalTable: "PurchasingDocumentExpeditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PPHBankExpenditureNoteItems_PPHBankExpenditureNoteId",
                table: "PPHBankExpenditureNoteItems",
                column: "PPHBankExpenditureNoteId");

            migrationBuilder.CreateIndex(
                name: "IX_PPHBankExpenditureNoteItems_PurchasingDocumentExpeditionId",
                table: "PPHBankExpenditureNoteItems",
                column: "PurchasingDocumentExpeditionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PPHBankExpenditureNoteItems");

            migrationBuilder.DropTable(
                name: "PPHBankExpenditureNotes");

            migrationBuilder.DropColumn(
                name: "BankExpenditureNoteDate",
                table: "PurchasingDocumentExpeditions");

            migrationBuilder.DropColumn(
                name: "BankExpenditureNotePPHDate",
                table: "PurchasingDocumentExpeditions");

            migrationBuilder.DropColumn(
                name: "BankExpenditureNotePPHNo",
                table: "PurchasingDocumentExpeditions");
        }
    }
}
