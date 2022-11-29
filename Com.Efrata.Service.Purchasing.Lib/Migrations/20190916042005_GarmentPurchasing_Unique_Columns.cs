using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class GarmentPurchasing_Unique_Columns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_GarmentUnitReceiptNotes_URNNo",
                table: "GarmentUnitReceiptNotes",
                column: "URNNo",
                unique: true,
                filter: "[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2019-10-04 00:00:00.0000000')");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentUnitExpenditureNotes_UENNo",
                table: "GarmentUnitExpenditureNotes",
                column: "UENNo",
                unique: true,
                filter: "[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2019-10-01 00:00:00.0000000')");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentUnitDeliveryOrders_UnitDONo",
                table: "GarmentUnitDeliveryOrders",
                column: "UnitDONo",
                unique: true,
                filter: "[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2019-10-01 00:00:00.0000000')");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentReceiptCorrections_CorrectionNo",
                table: "GarmentReceiptCorrections",
                column: "CorrectionNo",
                unique: true,
                filter: "[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2019-10-01 00:00:00.0000000')");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentPurchaseRequests_PRNo",
                table: "GarmentPurchaseRequests",
                column: "PRNo",
                unique: true,
                filter: "[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2019-10-01 00:00:00.0000000')");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentPurchaseRequests_RONo",
                table: "GarmentPurchaseRequests",
                column: "RONo",
                unique: true,
                filter: "[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2019-10-01 00:00:00.0000000')");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentInternNotes_INNo",
                table: "GarmentInternNotes",
                column: "INNo",
                unique: true,
                filter: "[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2019-10-01 00:00:00.0000000')");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentExternalPurchaseOrders_EPONo",
                table: "GarmentExternalPurchaseOrders",
                column: "EPONo",
                unique: true,
                filter: "[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2019-10-01 00:00:00.0000000')");

            migrationBuilder.CreateIndex(
                name: "IX_GarmentCorrectionNotes_CorrectionNo",
                table: "GarmentCorrectionNotes",
                column: "CorrectionNo",
                unique: true,
                filter: "[IsDeleted]=(0) AND [CreatedUtc]>CONVERT([datetime2],'2019-10-01 00:00:00.0000000')");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GarmentUnitReceiptNotes_URNNo",
                table: "GarmentUnitReceiptNotes");

            migrationBuilder.DropIndex(
                name: "IX_GarmentUnitExpenditureNotes_UENNo",
                table: "GarmentUnitExpenditureNotes");

            migrationBuilder.DropIndex(
                name: "IX_GarmentUnitDeliveryOrders_UnitDONo",
                table: "GarmentUnitDeliveryOrders");

            migrationBuilder.DropIndex(
                name: "IX_GarmentReceiptCorrections_CorrectionNo",
                table: "GarmentReceiptCorrections");

            migrationBuilder.DropIndex(
                name: "IX_GarmentPurchaseRequests_PRNo",
                table: "GarmentPurchaseRequests");

            migrationBuilder.DropIndex(
                name: "IX_GarmentPurchaseRequests_RONo",
                table: "GarmentPurchaseRequests");

            migrationBuilder.DropIndex(
                name: "IX_GarmentInternNotes_INNo",
                table: "GarmentInternNotes");

            migrationBuilder.DropIndex(
                name: "IX_GarmentExternalPurchaseOrders_EPONo",
                table: "GarmentExternalPurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_GarmentCorrectionNotes_CorrectionNo",
                table: "GarmentCorrectionNotes");
        }
    }
}
