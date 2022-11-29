using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class updateCurrencyRate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "CurrencyRate",
                table: "ExternalPurchaseOrders",
                type: "float",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldMaxLength: 1000,
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CurrencyRate",
                table: "ExternalPurchaseOrders",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float",
                oldMaxLength: 1000);
        }
    }
}
