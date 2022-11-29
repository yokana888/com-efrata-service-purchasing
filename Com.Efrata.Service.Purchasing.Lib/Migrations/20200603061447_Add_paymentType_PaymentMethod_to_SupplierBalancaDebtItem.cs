using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Add_paymentType_PaymentMethod_to_SupplierBalancaDebtItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "GarmentSupplierBalanceDebtItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentType",
                table: "GarmentSupplierBalanceDebtItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "GarmentSupplierBalanceDebtItems");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "GarmentSupplierBalanceDebtItems");
        }
    }
}
