using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class addVatIncomeTaxInFulfillment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UnitPaymentOrderIncomeTaxDate",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitPaymentOrderIncomeTaxNo",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "UnitPaymentOrderIncomeTaxRate",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UnitPaymentOrderUseIncomeTax",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "UnitPaymentOrderUseVat",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true);

            migrationBuilder.AddColumn<DateTimeOffset>(
                name: "UnitPaymentOrderVatDate",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UnitPaymentOrderVatNo",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitPaymentOrderIncomeTaxDate",
                table: "InternalPurchaseOrderFulfillments");

            migrationBuilder.DropColumn(
                name: "UnitPaymentOrderIncomeTaxNo",
                table: "InternalPurchaseOrderFulfillments");

            migrationBuilder.DropColumn(
                name: "UnitPaymentOrderIncomeTaxRate",
                table: "InternalPurchaseOrderFulfillments");

            migrationBuilder.DropColumn(
                name: "UnitPaymentOrderUseIncomeTax",
                table: "InternalPurchaseOrderFulfillments");

            migrationBuilder.DropColumn(
                name: "UnitPaymentOrderUseVat",
                table: "InternalPurchaseOrderFulfillments");

            migrationBuilder.DropColumn(
                name: "UnitPaymentOrderVatDate",
                table: "InternalPurchaseOrderFulfillments");

            migrationBuilder.DropColumn(
                name: "UnitPaymentOrderVatNo",
                table: "InternalPurchaseOrderFulfillments");
        }
    }
}
