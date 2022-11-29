using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class moveColumfromdetailtoiteminInvoice : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "GarmentInvoiceItems");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "GarmentInvoiceItems");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "GarmentInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "GarmentInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "GarmentDeliveryOrderItems");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "GarmentDeliveryOrderItems");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "GarmentDeliveryOrderItems");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "GarmentDeliveryOrderItems");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "GarmentInvoiceItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentType",
                table: "GarmentInvoiceItems",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "DODetailDOId",
                table: "GarmentInvoiceDetails",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "GarmentDeliveryOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CurrencyId",
                table: "GarmentDeliveryOrders",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "GarmentDeliveryOrders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentType",
                table: "GarmentDeliveryOrders",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "GarmentInvoiceItems");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "GarmentInvoiceItems");

            migrationBuilder.DropColumn(
                name: "DODetailDOId",
                table: "GarmentInvoiceDetails");

            migrationBuilder.DropColumn(
                name: "CurrencyCode",
                table: "GarmentDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "CurrencyId",
                table: "GarmentDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "GarmentDeliveryOrders");

            migrationBuilder.DropColumn(
                name: "PaymentType",
                table: "GarmentDeliveryOrders");

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "GarmentInvoiceItems",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CurrencyId",
                table: "GarmentInvoiceItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "GarmentInvoiceDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentType",
                table: "GarmentInvoiceDetails",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CurrencyCode",
                table: "GarmentDeliveryOrderItems",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "CurrencyId",
                table: "GarmentDeliveryOrderItems",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "GarmentDeliveryOrderItems",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PaymentType",
                table: "GarmentDeliveryOrderItems",
                nullable: true);
        }
    }
}
