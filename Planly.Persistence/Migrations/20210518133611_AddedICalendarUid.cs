using Microsoft.EntityFrameworkCore.Migrations;

namespace Planly.Persistence.Migrations
{
    public partial class AddedICalendarUid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ActiveHours_Start_durationSinceMidnight",
                table: "Schedules",
                newName: "ActiveHours_Start_DurationSinceMidnight");

            migrationBuilder.RenameColumn(
                name: "ActiveHours_End_durationSinceMidnight",
                table: "Schedules",
                newName: "ActiveHours_End_DurationSinceMidnight");

            migrationBuilder.AddColumn<string>(
                name: "ICalendarId",
                table: "Sessions",
                type: "nvarchar(512)",
                maxLength: 512,
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ICalendarId",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "ActiveHours_Start_DurationSinceMidnight",
                table: "Schedules",
                newName: "ActiveHours_Start_durationSinceMidnight");

            migrationBuilder.RenameColumn(
                name: "ActiveHours_End_DurationSinceMidnight",
                table: "Schedules",
                newName: "ActiveHours_End_durationSinceMidnight");
        }
    }
}
