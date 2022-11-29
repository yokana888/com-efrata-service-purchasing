using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class addvatcorrectionnote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "VatId",
                table: "GarmentCorrectionNotes",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VatRate",
                table: "GarmentCorrectionNotes",
                maxLength: 255,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "VatId",
                table: "GarmentCorrectionNotes");

            migrationBuilder.DropColumn(
                name: "VatRate",
                table: "GarmentCorrectionNotes");
        }
    }
}
