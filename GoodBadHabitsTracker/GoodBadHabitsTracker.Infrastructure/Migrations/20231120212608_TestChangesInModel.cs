using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoodBadHabitsTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class TestChangesInModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "IsGoalInTime",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "IsGood",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "IsRepeatDaily",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "ReminderTime",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "RepeatDaysOfMonth",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "RepeatDaysOfWeek",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "Habits");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Habits");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Frequency",
                table: "Habits",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGoalInTime",
                table: "Habits",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsGood",
                table: "Habits",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsRepeatDaily",
                table: "Habits",
                type: "bit",
                nullable: true);

            migrationBuilder.AddColumn<byte>(
                name: "Quantity",
                table: "Habits",
                type: "tinyint",
                nullable: true);

            migrationBuilder.AddColumn<TimeOnly>(
                name: "ReminderTime",
                table: "Habits",
                type: "time",
                nullable: false,
                defaultValue: new TimeOnly(0, 0, 0));

            migrationBuilder.AddColumn<string>(
                name: "RepeatDaysOfMonth",
                table: "Habits",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RepeatDaysOfWeek",
                table: "Habits",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "StartDate",
                table: "Habits",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1));

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Habits",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }
    }
}
