using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class fixDataTypeFulfillmentNullable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "UnitReceiptNoteDeliveredQuantity",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "UnitPaymentOrderUseVat",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "UnitPaymentOrderUseIncomeTax",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(bool),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "UnitPaymentOrderIncomeTaxRate",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "InterNoteValue",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(double),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "UnitReceiptNoteDeliveredQuantity",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<bool>(
                name: "UnitPaymentOrderUseVat",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<bool>(
                name: "UnitPaymentOrderUseIncomeTax",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true,
                oldClrType: typeof(bool));

            migrationBuilder.AlterColumn<double>(
                name: "UnitPaymentOrderIncomeTaxRate",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<double>(
                name: "InterNoteValue",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true,
                oldClrType: typeof(double));
        }
    }
}
