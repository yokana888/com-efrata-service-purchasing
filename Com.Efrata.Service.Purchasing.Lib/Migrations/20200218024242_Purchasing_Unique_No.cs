using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Purchasing_Unique_No : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UPCNo",
                table: "UnitPaymentCorrectionNotes",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UnitReceiptNotes_URNNo",
                table: "UnitReceiptNotes",
                column: "URNNo",
                unique: true,
                filter: "[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2020-02-01 00:00:00.0000000')");

            migrationBuilder.CreateIndex(
                name: "IX_UnitPaymentOrders_UPONo",
                table: "UnitPaymentOrders",
                column: "UPONo",
                unique: true,
                filter: "[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2020-02-01 00:00:00.0000000')");

            migrationBuilder.CreateIndex(
                name: "IX_UnitPaymentCorrectionNotes_UPCNo",
                table: "UnitPaymentCorrectionNotes",
                column: "UPCNo",
                unique: true,
                filter: "[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2020-02-01 00:00:00.0000000')");

            migrationBuilder.CreateIndex(
                name: "IX_PurchaseRequests_No",
                table: "PurchaseRequests",
                column: "No",
                unique: true,
                filter: "[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2020-02-01 00:00:00.0000000')");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalPurchaseOrders_EPONo",
                table: "ExternalPurchaseOrders",
                column: "EPONo",
                unique: true,
                filter: "[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2020-02-01 00:00:00.0000000')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UnitReceiptNotes_URNNo",
                table: "UnitReceiptNotes");

            migrationBuilder.DropIndex(
                name: "IX_UnitPaymentOrders_UPONo",
                table: "UnitPaymentOrders");

            migrationBuilder.DropIndex(
                name: "IX_UnitPaymentCorrectionNotes_UPCNo",
                table: "UnitPaymentCorrectionNotes");

            migrationBuilder.DropIndex(
                name: "IX_PurchaseRequests_No",
                table: "PurchaseRequests");

            migrationBuilder.DropIndex(
                name: "IX_ExternalPurchaseOrders_EPONo",
                table: "ExternalPurchaseOrders");

            migrationBuilder.AlterColumn<string>(
                name: "UPCNo",
                table: "UnitPaymentCorrectionNotes",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
