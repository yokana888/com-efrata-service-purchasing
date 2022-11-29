using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Add_GarmentSupplierBalanceDebt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GarmentSupplierBalanceDebts",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    CodeRequirment = table.Column<string>(nullable: true),
                    CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    DOCurrencyCode = table.Column<string>(nullable: true),
                    DOCurrencyId = table.Column<long>(nullable: true),
                    DOCurrencyRate = table.Column<double>(nullable: false),
                    DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(nullable: false),
                    Import = table.Column<bool>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    SupplierCode = table.Column<string>(maxLength: 255, nullable: true),
                    SupplierId = table.Column<long>(maxLength: 255, nullable: false),
                    SupplierName = table.Column<string>(maxLength: 1000, nullable: true),
                    TotalAmountIDR = table.Column<double>(nullable: false),
                    TotalValas = table.Column<double>(nullable: false),
                    UId = table.Column<string>(maxLength: 255, nullable: true),
                    Year = table.Column<long>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentSupplierBalanceDebts", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentSupplierBalanceDebtItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    ArrivalDate = table.Column<DateTimeOffset>(nullable: false),
                    BillNo = table.Column<string>(maxLength: 50, nullable: true),
                    CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    DOId = table.Column<long>(nullable: false),
                    DONo = table.Column<string>(nullable: true),
                    DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(nullable: false),
                    GarmentDebtId = table.Column<long>(nullable: false),
                    IDR = table.Column<double>(nullable: false),
                    InternNo = table.Column<string>(nullable: true),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    UId = table.Column<string>(maxLength: 255, nullable: true),
                    Valas = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentSupplierBalanceDebtItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentSupplierBalanceDebtItems_GarmentSupplierBalanceDebts_GarmentDebtId",
                        column: x => x.GarmentDebtId,
                        principalTable: "GarmentSupplierBalanceDebts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentSupplierBalanceDebtItems_GarmentDebtId",
                table: "GarmentSupplierBalanceDebtItems",
                column: "GarmentDebtId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarmentSupplierBalanceDebtItems");

            migrationBuilder.DropTable(
                name: "GarmentSupplierBalanceDebts");
        }
    }
}
