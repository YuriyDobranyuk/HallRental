using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace HallRental.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Halls",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Capacity = table.Column<int>(type: "int", nullable: false),
                    BaseHourlyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Halls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Services",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Services", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Bookings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HallId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StartUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndUtc = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bookings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Bookings_Halls_HallId",
                        column: x => x.HallId,
                        principalTable: "Halls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HallServices",
                columns: table => new
                {
                    HallId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HallServices", x => new { x.HallId, x.ServiceId });
                    table.ForeignKey(
                        name: "FK_HallServices_Halls_HallId",
                        column: x => x.HallId,
                        principalTable: "Halls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_HallServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BookingServices",
                columns: table => new
                {
                    BookingId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServiceId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PriceAtBooking = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BookingServices", x => new { x.BookingId, x.ServiceId });
                    table.ForeignKey(
                        name: "FK_BookingServices_Bookings_BookingId",
                        column: x => x.BookingId,
                        principalTable: "Bookings",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BookingServices_Services_ServiceId",
                        column: x => x.ServiceId,
                        principalTable: "Services",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Halls",
                columns: new[] { "Id", "BaseHourlyRate", "Capacity", "CreatedAtUtc", "IsDeleted", "Name" },
                values: new object[,]
                {
                    { new Guid("1f9bb736-7681-41ac-9e65-d086dc30903b"), 1500m, 30, new DateTime(2026, 9, 7, 16, 57, 31, 776, DateTimeKind.Utc).AddTicks(76), false, "Зал C" },
                    { new Guid("38e195b3-177d-4294-bfc3-dfb4fe81c723"), 3500m, 100, new DateTime(2026, 9, 7, 16, 57, 31, 776, DateTimeKind.Utc).AddTicks(76), false, "Зал B" },
                    { new Guid("6b388f3e-0fe7-4d01-9b6b-f1c4465cb815"), 2000m, 50, new DateTime(2026, 9, 7, 16, 57, 31, 776, DateTimeKind.Utc).AddTicks(76), false, "Зал A" }
                });

            migrationBuilder.InsertData(
                table: "Services",
                columns: new[] { "Id", "CreatedAtUtc", "Name", "Price" },
                values: new object[,]
                {
                    { new Guid("3e91b7d4-6a25-4f08-8c53-d129e7a46b81"), new DateTime(2026, 9, 7, 16, 57, 31, 776, DateTimeKind.Utc).AddTicks(76), "Wi-Fi", 300m },
                    { new Guid("a7f24c91-3d68-4e52-bf17-82c9d5a41e73"), new DateTime(2026, 9, 7, 16, 57, 31, 776, DateTimeKind.Utc).AddTicks(76), "Проєктор", 500m },
                    { new Guid("c5824f16-9d37-4b60-ae28-71f3c95d824a"), new DateTime(2026, 9, 7, 16, 57, 31, 776, DateTimeKind.Utc).AddTicks(76), "Звук", 700m }
                });

            migrationBuilder.InsertData(
                table: "HallServices",
                columns: new[] { "HallId", "ServiceId" },
                values: new object[,]
                {
                    { new Guid("1f9bb736-7681-41ac-9e65-d086dc30903b"), new Guid("3e91b7d4-6a25-4f08-8c53-d129e7a46b81") },
                    { new Guid("1f9bb736-7681-41ac-9e65-d086dc30903b"), new Guid("a7f24c91-3d68-4e52-bf17-82c9d5a41e73") },
                    { new Guid("1f9bb736-7681-41ac-9e65-d086dc30903b"), new Guid("c5824f16-9d37-4b60-ae28-71f3c95d824a") },
                    { new Guid("38e195b3-177d-4294-bfc3-dfb4fe81c723"), new Guid("3e91b7d4-6a25-4f08-8c53-d129e7a46b81") },
                    { new Guid("38e195b3-177d-4294-bfc3-dfb4fe81c723"), new Guid("a7f24c91-3d68-4e52-bf17-82c9d5a41e73") },
                    { new Guid("38e195b3-177d-4294-bfc3-dfb4fe81c723"), new Guid("c5824f16-9d37-4b60-ae28-71f3c95d824a") },
                    { new Guid("6b388f3e-0fe7-4d01-9b6b-f1c4465cb815"), new Guid("3e91b7d4-6a25-4f08-8c53-d129e7a46b81") },
                    { new Guid("6b388f3e-0fe7-4d01-9b6b-f1c4465cb815"), new Guid("a7f24c91-3d68-4e52-bf17-82c9d5a41e73") },
                    { new Guid("6b388f3e-0fe7-4d01-9b6b-f1c4465cb815"), new Guid("c5824f16-9d37-4b60-ae28-71f3c95d824a") }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bookings_HallId_StartUtc_EndUtc",
                table: "Bookings",
                columns: new[] { "HallId", "StartUtc", "EndUtc" });

            migrationBuilder.CreateIndex(
                name: "IX_BookingServices_ServiceId",
                table: "BookingServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Halls_Capacity",
                table: "Halls",
                column: "Capacity");

            migrationBuilder.CreateIndex(
                name: "IX_HallServices_ServiceId",
                table: "HallServices",
                column: "ServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_Services_Name",
                table: "Services",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BookingServices");

            migrationBuilder.DropTable(
                name: "HallServices");

            migrationBuilder.DropTable(
                name: "Bookings");

            migrationBuilder.DropTable(
                name: "Services");

            migrationBuilder.DropTable(
                name: "Halls");
        }
    }
}
