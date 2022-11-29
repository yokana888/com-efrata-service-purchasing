using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Update_Quantity_Column_Type_on_UnitPaymentCorrectionNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Quantity",
                table: "UnitPaymentCorrectionNoteItems",
                type: "float",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<double>(
                name: "PriceTotalBefore",
                table: "UnitPaymentCorrectionNoteItems",
                type: "float",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<double>(
                name: "PriceTotalAfter",
                table: "UnitPaymentCorrectionNoteItems",
                type: "float",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<double>(
                name: "PricePerDealUnitBefore",
                table: "UnitPaymentCorrectionNoteItems",
                type: "float",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<double>(
                name: "PricePerDealUnitAfter",
                table: "UnitPaymentCorrectionNoteItems",
                type: "float",
                nullable: false,
                oldClrType: typeof(long));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "Quantity",
                table: "UnitPaymentCorrectionNoteItems",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<long>(
                name: "PriceTotalBefore",
                table: "UnitPaymentCorrectionNoteItems",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<long>(
                name: "PriceTotalAfter",
                table: "UnitPaymentCorrectionNoteItems",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<long>(
                name: "PricePerDealUnitBefore",
                table: "UnitPaymentCorrectionNoteItems",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<long>(
                name: "PricePerDealUnitAfter",
                table: "UnitPaymentCorrectionNoteItems",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
