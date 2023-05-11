using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LicensePlateDataLibrary.Migrations
{
    public partial class createandseedlicenseplates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LicensePlates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    LicensePlateText = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TimeStamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicensePlates", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "LicensePlates",
                columns: new[] { "Id", "FileName", "IsProcessed", "LicensePlateText", "TimeStamp" },
                values: new object[,]
                {
                    { 1, "https://plateimagesyyyymmddxyz.blob.core.windows.net/images/FAKE1.jpg", true, "FUNTIME", new DateTime(2021, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "https://plateimagesyyyymmddxyz.blob.core.windows.net/images/FAKE2.jpg", true, "GVMESPD", new DateTime(2021, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "https://plateimagesyyyymmddxyz.blob.core.windows.net/images/FAKE3.jpg", true, "PULTOYS", new DateTime(2021, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "https://plateimagesyyyymmddxyz.blob.core.windows.net/images/FAKE4.jpg", true, "NCC1701", new DateTime(2021, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 5, "https://plateimagesyyyymmddxyz.blob.core.windows.net/images/FAKE5.jpg", true, "MYCAR01", new DateTime(2021, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 6, "https://plateimagesyyyymmddxyz.blob.core.windows.net/images/FAKE6.jpg", true, "FBIAGNT", new DateTime(2021, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 7, "https://plateimagesyyyymmddxyz.blob.core.windows.net/images/FAKE7.jpg", true, "FLYERS1", new DateTime(2021, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 8, "https://plateimagesyyyymmddxyz.blob.core.windows.net/images/FAKE8.jpg", true, "EMC2FST", new DateTime(2021, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 9, "https://plateimagesyyyymmddxyz.blob.core.windows.net/images/FAKE9.jpg", true, "FSTRNU", new DateTime(2021, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 10, "https://plateimagesyyyymmddxyz.blob.core.windows.net/images/FAKE10.jpg", true, "BACKOFF", new DateTime(2021, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LicensePlates");
        }
    }
}
