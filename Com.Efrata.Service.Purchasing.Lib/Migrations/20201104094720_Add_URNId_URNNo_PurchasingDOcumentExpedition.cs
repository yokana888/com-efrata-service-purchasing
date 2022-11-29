using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Add_URNId_URNNo_PurchasingDOcumentExpedition : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "URNId",
                table: "PurchasingDocumentExpeditions",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "URNNo",
                table: "PurchasingDocumentExpeditions",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "URNId",
                table: "PurchasingDocumentExpeditions");

            migrationBuilder.DropColumn(
                name: "URNNo",
                table: "PurchasingDocumentExpeditions");
        }
    }
}
