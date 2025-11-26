using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Rise.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class EventsGefixt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Time",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Events",
                newName: "StartDateTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDateTime",
                table: "Events",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndDateTime",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "StartDateTime",
                table: "Events",
                newName: "Date");

            migrationBuilder.AddColumn<string>(
                name: "Time",
                table: "Events",
                type: "varchar(10)",
                maxLength: 10,
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
