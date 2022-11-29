using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class fixDataTypeFulfillment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "UnitReceiptNoteItemId",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "UnitReceiptNoteId",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "UnitPaymentOrderItemId",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "UnitPaymentOrderId",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);

            migrationBuilder.AlterColumn<long>(
                name: "UnitPaymentOrderDetailId",
                table: "InternalPurchaseOrderFulfillments",
                nullable: false,
                oldClrType: typeof(long),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "UnitReceiptNoteItemId",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "UnitReceiptNoteId",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "UnitPaymentOrderItemId",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "UnitPaymentOrderId",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true,
                oldClrType: typeof(long));

            migrationBuilder.AlterColumn<long>(
                name: "UnitPaymentOrderDetailId",
                table: "InternalPurchaseOrderFulfillments",
                nullable: true,
                oldClrType: typeof(long));
        }
    }
}
