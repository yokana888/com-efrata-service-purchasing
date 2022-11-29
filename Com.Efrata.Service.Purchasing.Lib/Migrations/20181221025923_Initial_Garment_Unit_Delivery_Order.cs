using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Initial_Garment_Unit_Delivery_Order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GarmentUnitDeliveryOrders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    Article = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RONo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorageCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StorageId = table.Column<long>(type: "bigint", nullable: false),
                    StorageName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UnitDODate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UnitDONo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitDOType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitRequestCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitRequestId = table.Column<long>(type: "bigint", nullable: false),
                    UnitRequestName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitSenderCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitSenderId = table.Column<long>(type: "bigint", nullable: false),
                    UnitSenderName = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentUnitDeliveryOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "GarmentUnitDeliveryOrderItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
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
                    URNId = table.Column<long>(type: "bigint", nullable: false),
                    URNItemId = table.Column<long>(type: "bigint", nullable: false),
                    URNNo = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UnitDOId = table.Column<long>(type: "bigint", nullable: false),
                    UomId = table.Column<long>(type: "bigint", nullable: false),
                    UomUnit = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GarmentUnitDeliveryOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GarmentUnitDeliveryOrderItems_GarmentUnitDeliveryOrders_UnitDOId",
                        column: x => x.UnitDOId,
                        principalTable: "GarmentUnitDeliveryOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GarmentUnitDeliveryOrderItems_UnitDOId",
                table: "GarmentUnitDeliveryOrderItems",
                column: "UnitDOId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GarmentUnitDeliveryOrderItems");

            migrationBuilder.DropTable(
                name: "GarmentUnitDeliveryOrders");
        }
    }
}
