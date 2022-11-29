using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Add_URNId_URNNo_PurchasingDOcumentExpeditionItem : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "URNId",
                table: "PurchasingDocumentExpeditionItems",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "URNNo",
                table: "PurchasingDocumentExpeditionItems",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "URNId",
                table: "PurchasingDocumentExpeditionItems");

            migrationBuilder.DropColumn(
                name: "URNNo",
                table: "PurchasingDocumentExpeditionItems");
        }
    }
}
