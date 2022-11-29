using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace Com.Efrata.Service.Purchasing.Lib.Migrations
{
    public partial class Delivery_Order : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeliveryOrders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    ArrivalDate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DODate = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    DONo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SupplierCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SupplierId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    SupplierName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    UId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryOrders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryOrderItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DOId = table.Column<long>(type: "bigint", nullable: false),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EPOId = table.Column<long>(type: "bigint", nullable: false),
                    EPONo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryOrderItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryOrderItems_DeliveryOrders_DOId",
                        column: x => x.DOId,
                        principalTable: "DeliveryOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "DeliveryOrderDetails",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Active = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DOItemId = table.Column<long>(type: "bigint", nullable: false),
                    DOQuantity = table.Column<double>(type: "float", nullable: false),
                    DealQuantity = table.Column<double>(type: "float", nullable: false),
                    DeletedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    DeletedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EPODetailId = table.Column<long>(type: "bigint", nullable: false),
                    IsClosed = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LastModifiedAgent = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedBy = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LastModifiedUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    POItemId = table.Column<long>(type: "bigint", nullable: false),
                    PRId = table.Column<long>(type: "bigint", nullable: false),
                    PRItemId = table.Column<long>(type: "bigint", nullable: false),
                    PRNo = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProductCode = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProductId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    ProductName = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ProductRemark = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceiptQuantity = table.Column<double>(type: "float", nullable: false),
                    UomId = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    UomUnit = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeliveryOrderDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DeliveryOrderDetails_DeliveryOrderItems_DOItemId",
                        column: x => x.DOItemId,
                        principalTable: "DeliveryOrderItems",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOrderDetails_DOItemId",
                table: "DeliveryOrderDetails",
                column: "DOItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DeliveryOrderItems_DOId",
                table: "DeliveryOrderItems",
                column: "DOId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeliveryOrderDetails");

            migrationBuilder.DropTable(
                name: "DeliveryOrderItems");

            migrationBuilder.DropTable(
                name: "DeliveryOrders");
        }
    }
}
