using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class PurchasingDocumentExpeditionItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FinanceDivisionBy",
                table: "PurchasingDocumentExpeditions");

            migrationBuilder.DropColumn(
                name: "FinanceDivisionDate",
                table: "PurchasingDocumentExpeditions");

            migrationBuilder.DropColumn(
                name: "SendToFinanceDivisionBy",
                table: "PurchasingDocumentExpeditions");

            migrationBuilder.DropColumn(
                name: "SendToFinanceDivisionDate",
                table: "PurchasingDocumentExpeditions");

            migrationBuilder.AddColumn<string>(
                name: "AccountingDivisionBy",
                table: "PurchasingDocumentExpeditions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "AccountingDivisionDate",
                table: "PurchasingDocumentExpeditions",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "IncomeTax",
                table: "PurchasingDocumentExpeditions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<string>(
                name: "IncomeTaxId",
                table: "PurchasingDocumentExpeditions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IncomeTaxName",
                table: "PurchasingDocumentExpeditions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "IncomeTaxRate",
                table: "PurchasingDocumentExpeditions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaid",
                table: "PurchasingDocumentExpeditions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPaidPPH",
                table: "PurchasingDocumentExpeditions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SendToAccountingDivisionBy",
                table: "PurchasingDocumentExpeditions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "SendToAccountingDivisionDate",
                table: "PurchasingDocumentExpeditions",
                type: "datetimeoffset",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Vat",
                table: "PurchasingDocumentExpeditions",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.CreateTable(
                name: "PurchasingDocumentExpeditionItems",
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
                    Price = table.Column<double>(type: "float", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PurchasingDocumentExpeditionId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    UnitCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Uom = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PurchasingDocumentExpeditionItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PurchasingDocumentExpeditionItems_PurchasingDocumentExpeditions_PurchasingDocumentExpeditionId",
                        column: x => x.PurchasingDocumentExpeditionId,
                        principalTable: "PurchasingDocumentExpeditions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PurchasingDocumentExpeditionItems_PurchasingDocumentExpeditionId",
                table: "PurchasingDocumentExpeditionItems",
                column: "PurchasingDocumentExpeditionId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PurchasingDocumentExpeditionItems");

            migrationBuilder.DropColumn(
                name: "AccountingDivisionBy",
                table: "PurchasingDocumentExpeditions");

            migrationBuilder.DropColumn(
                name: "AccountingDivisionDate",
                table: "PurchasingDocumentExpeditions");

            migrationBuilder.DropColumn(
                name: "IncomeTax",
                table: "PurchasingDocumentExpeditions");

            migrationBuilder.DropColumn(
                name: "IncomeTaxId",
                table: "PurchasingDocumentExpeditions");

            migrationBuilder.DropColumn(
                name: "IncomeTaxName",
                table: "PurchasingDocumentExpeditions");

            migrationBuilder.DropColumn(
                name: "IncomeTaxRate",
                table: "PurchasingDocumentExpeditions");

            migrationBuilder.DropColumn(
                name: "IsPaid",
                table: "PurchasingDocumentExpeditions");

            migrationBuilder.DropColumn(
                name: "IsPaidPPH",
                table: "PurchasingDocumentExpeditions");

            migrationBuilder.DropColumn(
                name: "SendToAccountingDivisionBy",
                table: "PurchasingDocumentExpeditions");

            migrationBuilder.DropColumn(
                name: "SendToAccountingDivisionDate",
                table: "PurchasingDocumentExpeditions");

            migrationBuilder.DropColumn(
                name: "Vat",
                table: "PurchasingDocumentExpeditions");

            migrationBuilder.AddColumn<string>(
                name: "FinanceDivisionBy",
                table: "PurchasingDocumentExpeditions",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "FinanceDivisionDate",
                table: "PurchasingDocumentExpeditions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SendToFinanceDivisionBy",
                table: "PurchasingDocumentExpeditions",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "SendToFinanceDivisionDate",
                table: "PurchasingDocumentExpeditions",
                nullable: true);
        }
    }
}
