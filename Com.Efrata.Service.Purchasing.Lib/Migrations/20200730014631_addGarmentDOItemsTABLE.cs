using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class addGarmentDOItemsTABLE : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GarmentDOItems",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(nullable: false),
                    CreatedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(nullable: false),
                    DOCurrencyRate = table.Column<double>(nullable: false),
                    DOItemNo = table.Column<string>(maxLength: 255, nullable: true),
                    DeletedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(nullable: false),
                    DesignColor = table.Column<string>(nullable: true),
                    DetailReferenceId = table.Column<string>(nullable: true),
                    EPOItemId = table.Column<long>(nullable: false),
                    IsDeleted = table.Column<bool>(nullable: false),
                    LastModifiedAgent = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(nullable: false),
                    POId = table.Column<long>(nullable: false),
                    POItemId = table.Column<long>(nullable: false),
                    POSerialNumber = table.Column<string>(maxLength: 100, nullable: true),
                    PRItemId = table.Column<long>(nullable: false),
                    ProductCode = table.Column<string>(maxLength: 255, nullable: true),
                    ProductId = table.Column<long>(nullable: false),
                    ProductName = table.Column<string>(maxLength: 1000, nullable: true),
                    RemainingQuantity = table.Column<decimal>(nullable: false),
                    SmallQuantity = table.Column<decimal>(nullable: false),
                    SmallUomId = table.Column<long>(nullable: false),
                    SmallUomUnit = table.Column<string>(maxLength: 100, nullable: true),
                    StorageCode = table.Column<string>(maxLength: 255, nullable: true),
                    StorageId = table.Column<long>(nullable: false),
                    StorageName = table.Column<string>(maxLength: 100, nullable: true),
                    UId = table.Column<string>(nullable: true),
                    UnitCode = table.Column<string>(maxLength: 255, nullable: true),
                    UnitId = table.Column<long>(nullable: false),
                    UnitName = table.Column<string>(maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentDOItems", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarmentDOItems");
        }
    }
}
