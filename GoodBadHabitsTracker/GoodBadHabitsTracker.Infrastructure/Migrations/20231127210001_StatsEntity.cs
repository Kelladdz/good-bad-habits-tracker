using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoodBadHabitsTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class StatsEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Statistics_Complete",
                table: "Habits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Statistics_Failed",
                table: "Habits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Statistics_Skipped",
                table: "Habits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Statistics_Streak",
                table: "Habits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Statistics_Total",
                table: "Habits",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Statistics_Complete",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "Statistics_Failed",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "Statistics_Skipped",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "Statistics_Streak",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "Statistics_Total",
                table: "Habits");
        }
    }
}
