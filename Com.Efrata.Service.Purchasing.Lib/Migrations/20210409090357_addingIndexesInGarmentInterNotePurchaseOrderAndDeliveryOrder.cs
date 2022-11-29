using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class addingIndexesInGarmentInterNotePurchaseOrderAndDeliveryOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "DeliveryOrderNo",
                table: "GarmentInvoiceItems",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GarmentInvoices_CurrencyId_IncomeTaxId_SupplierId_InvoiceDate_IncomeTaxDate",
                table: "GarmentInvoices",
                columns: new[] { "CurrencyId", "IncomeTaxId", "SupplierId", "InvoiceDate", "IncomeTaxDate" });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentInvoiceItems_ArrivalDate_DeliveryOrderId_DeliveryOrderNo_InvoiceId",
                table: "GarmentInvoiceItems",
                columns: new[] { "ArrivalDate", "DeliveryOrderId", "DeliveryOrderNo", "InvoiceId" });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentInvoiceDetails_EPOId_IPOId_PRItemId_DODetailId_ProductId_UomId",
                table: "GarmentInvoiceDetails",
                columns: new[] { "EPOId", "IPOId", "PRItemId", "DODetailId", "ProductId", "UomId" });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentInternNotes_INDate_CurrencyId_CurrencyCode_SupplierId",
                table: "GarmentInternNotes",
                columns: new[] { "INDate", "CurrencyId", "CurrencyCode", "SupplierId" });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentInternalPurchaseOrderItems_CategoryId_GPOId_ProductId",
                table: "GarmentInternalPurchaseOrderItems",
                columns: new[] { "CategoryId", "GPOId", "ProductId" });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentExternalPurchaseOrders_CurrencyId_IncomeTaxId_SupplierId_UENId",
                table: "GarmentExternalPurchaseOrders",
                columns: new[] { "CurrencyId", "IncomeTaxId", "SupplierId", "UENId" });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentExternalPurchaseOrderItems_POId_PRId",
                table: "GarmentExternalPurchaseOrderItems",
                columns: new[] { "POId", "PRId" });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentDeliveryOrders_ArrivalDate_CustomsId_DOCurrencyId_DODate_IncomeTaxId_SupplierId",
                table: "GarmentDeliveryOrders",
                columns: new[] { "ArrivalDate", "CustomsId", "DOCurrencyId", "DODate", "IncomeTaxId", "SupplierId" });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentDeliveryOrderItems_CurrencyId_EPOId_GarmentDOId",
                table: "GarmentDeliveryOrderItems",
                columns: new[] { "CurrencyId", "EPOId", "GarmentDOId" });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentDeliveryOrderDetails_POId_PRId_ProductId",
                table: "GarmentDeliveryOrderDetails",
                columns: new[] { "POId", "PRId", "ProductId" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GarmentInvoices_CurrencyId_IncomeTaxId_SupplierId_InvoiceDate_IncomeTaxDate",
                table: "GarmentInvoices");

            migrationBuilder.DropIndex(
                name: "IX_GarmentInvoiceItems_ArrivalDate_DeliveryOrderId_DeliveryOrderNo_InvoiceId",
                table: "GarmentInvoiceItems");

            migrationBuilder.DropIndex(
                name: "IX_GarmentInvoiceDetails_EPOId_IPOId_PRItemId_DODetailId_ProductId_UomId",
                table: "GarmentInvoiceDetails");

            migrationBuilder.DropIndex(
                name: "IX_GarmentInternNotes_INDate_CurrencyId_CurrencyCode_SupplierId",
                table: "GarmentInternNotes");

            migrationBuilder.DropIndex(
                name: "IX_GarmentInternalPurchaseOrderItems_CategoryId_GPOId_ProductId",
                table: "GarmentInternalPurchaseOrderItems");

            migrationBuilder.DropIndex(
                name: "IX_GarmentExternalPurchaseOrders_CurrencyId_IncomeTaxId_SupplierId_UENId",
                table: "GarmentExternalPurchaseOrders");

            migrationBuilder.DropIndex(
                name: "IX_GarmentExternalPurchaseOrderItems_POId_PRId",
                table: "GarmentExternalPurchaseOrderItems");

            migrationBuilder.DropIndex(
                name: "IX_GarmentDeliveryOrders_ArrivalDate_CustomsId_DOCurrencyId_DODate_IncomeTaxId_SupplierId",
                table: "GarmentDeliveryOrders");

            migrationBuilder.DropIndex(
                name: "IX_GarmentDeliveryOrderItems_CurrencyId_EPOId_GarmentDOId",
                table: "GarmentDeliveryOrderItems");

            migrationBuilder.DropIndex(
                name: "IX_GarmentDeliveryOrderDetails_POId_PRId_ProductId",
                table: "GarmentDeliveryOrderDetails");

            migrationBuilder.AlterColumn<string>(
                name: "DeliveryOrderNo",
                table: "GarmentInvoiceItems",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
