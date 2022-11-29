using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class FulfillmentDatetimeNotNull : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UnitReceiptNoteDate",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UnitPaymentOrderVatDate",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UnitPaymentOrderIncomeTaxDate",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "InvoiceDate",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "InterNoteDueDate",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "InterNoteDate",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UnitReceiptNoteDate",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UnitPaymentOrderVatDate",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UnitPaymentOrderIncomeTaxDate",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "InvoiceDate",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "InterNoteDueDate",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "InterNoteDate",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true,
                oldClrType: typeof(DateTimeOffset));
        }
    }
}
