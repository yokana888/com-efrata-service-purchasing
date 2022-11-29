using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Edit_GarmentPurchaseRequestItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUsed",
                table: "GarmentPurchaseRequestItems",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AlterColumn<double>(
                name: "Quantity",
                table: "GarmentPurchaseRequestItems",
                type: "float",
                nullable: false,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<double>(
                name: "BudgetPrice",
                table: "GarmentPurchaseRequestItems",
                type: "float",
                nullable: false,
                oldClrType: typeof(long));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUsed",
                table: "GarmentPurchaseRequestItems");

            migrationBuilder.AlterColumn<long>(
                name: "Quantity",
                table: "GarmentPurchaseRequestItems",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<long>(
                name: "BudgetPrice",
                table: "GarmentPurchaseRequestItems",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
