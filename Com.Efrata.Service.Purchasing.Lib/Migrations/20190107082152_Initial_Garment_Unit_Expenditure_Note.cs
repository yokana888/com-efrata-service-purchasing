using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Initial_Garment_Unit_Expenditure_Note : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GarmentUnitExpenditureNote",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpenditureDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    ExpenditureTo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpenditureType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StorageCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorageId = table.Column<long>(type: "bigint", nullable: false),
                    StorageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorageRequestCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorageRequestId = table.Column<long>(type: "bigint", nullable: false),
                    StorageRequestName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UENNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitDOId = table.Column<long>(type: "bigint", nullable: false),
                    UnitDONo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitRequestCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitRequestId = table.Column<long>(type: "bigint", nullable: false),
                    UnitRequestName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitSenderCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitSenderId = table.Column<long>(type: "bigint", nullable: false),
                    UnitSenderName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentUnitExpenditureNote", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentUnitExpenditureNoteItem",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    BuyerCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BuyerId = table.Column<long>(type: "bigint", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DODetailId = table.Column<long>(type: "bigint", nullable: false),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EPOItemId = table.Column<long>(type: "bigint", nullable: false),
                    FabricType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    POItemId = table.Column<long>(type: "bigint", nullable: false),
                    POSerialNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PRItemId = table.Column<long>(type: "bigint", nullable: false),
                    PricePerDealUnit = table.Column<double>(type: "float", nullable: false),
                    ProductCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductId = table.Column<long>(type: "bigint", nullable: false),
                    ProductName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProductRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quantity = table.Column<double>(type: "float", nullable: false),
                    RONo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UENId = table.Column<long>(type: "bigint", nullable: false),
                    URNItemId = table.Column<long>(type: "bigint", nullable: false),
                    UnitDOItemId = table.Column<long>(type: "bigint", nullable: false),
                    UomId = table.Column<long>(type: "bigint", nullable: false),
                    UomUnit = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentUnitExpenditureNoteItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentUnitExpenditureNoteItem_GarmentUnitExpenditureNote_UENId",
                        column: x => x.UENId,
                        principalTable: "GarmentUnitExpenditureNote",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentUnitExpenditureNoteItem_UENId",
                table: "GarmentUnitExpenditureNoteItem",
                column: "UENId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarmentUnitExpenditureNoteItem");

            migrationBuilder.DropTable(
                name: "GarmentUnitExpenditureNote");
        }
    }
}
