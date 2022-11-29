using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class FixModelFulfillment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "UnitReceiptNoteItemId",
                table: "InternalPurchaseOrderFulfillments",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "UnitReceiptNoteId",
                table: "InternalPurchaseOrderFulfillments",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<double>(
                name: "UnitReceiptNoteDeliveredQuantity",
                table: "InternalPurchaseOrderFulfillments",
                type: "float",
                nullable: true,
                oldClrType: typeof(double));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UnitReceiptNoteDate",
                table: "InternalPurchaseOrderFulfillments",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<long>(
                name: "UnitPaymentOrderItemId",
                table: "InternalPurchaseOrderFulfillments",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "UnitPaymentOrderId",
                table: "InternalPurchaseOrderFulfillments",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "UnitPaymentOrderDetailId",
                table: "InternalPurchaseOrderFulfillments",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "InvoiceDate",
                table: "InternalPurchaseOrderFulfillments",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "InterNoteDueDate",
                table: "InternalPurchaseOrderFulfillments",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "InterNoteDate",
                table: "InternalPurchaseOrderFulfillments",
                type: "datetimeoffset",
                nullable: true,
                oldClrType: typeof(DateTimeOffset));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "UnitReceiptNoteItemId",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "UnitReceiptNoteId",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "UnitReceiptNoteDeliveredQuantity",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "UnitReceiptNoteDate",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "UnitPaymentOrderItemId",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "UnitPaymentOrderId",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "UnitPaymentOrderDetailId",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "InvoiceDate",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "InterNoteDueDate",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "InterNoteDate",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(DateTimeOffset),
                oldType: "datetimeoffset",
                oldNullable: true);
        }
    }
}
