using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Change_Column_Garment_Intern_Note_Details : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TermOfPayment",
                table: "GarmentInternNoteDetails");

            migrationBuilder.AddColumn<string>(
                name: "PaymentMethod",
                table: "GarmentInternNoteDetails",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentMethod",
                table: "GarmentInternNoteDetails");

            migrationBuilder.AddColumn<string>(
                name: "TermOfPayment",
                table: "GarmentInternNoteDetails",
                nullable: true);
        }
    }
}
