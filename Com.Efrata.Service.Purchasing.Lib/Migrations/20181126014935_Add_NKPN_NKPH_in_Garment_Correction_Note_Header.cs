using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Add_NKPN_NKPH_in_Garment_Correction_Note_Header : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NKPH",
                table: "GarmentCorrectionNotes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NKPN",
                table: "GarmentCorrectionNotes",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NKPH",
                table: "GarmentCorrectionNotes");

            migrationBuilder.DropColumn(
                name: "NKPN",
                table: "GarmentCorrectionNotes");
        }
    }
}
