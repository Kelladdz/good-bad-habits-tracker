using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoodBadHabitsTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class LittleChangesOnModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Habits",
                keyColumn: "HabitId",
                keyValue: new Guid("6ecb8c16-d048-4e9b-8d30-26792bf54de4"));

            migrationBuilder.AlterColumn<string>(
                name: "RepeatDaysOfMonth",
                table: "Habits",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(byte[]),
                oldType: "varbinary(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<byte[]>(
                name: "RepeatDaysOfMonth",
                table: "Habits",
                type: "varbinary(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.InsertData(
                table: "Habits",
                columns: new[] { "HabitId", "Frequency", "IsGoalInTime", "IsGood", "IsRepeatDaily", "Name", "Quantity", "ReminderTime", "RepeatDaysOfMonth", "RepeatDaysOfWeek", "StartDate", "UserId" },
                values: new object[] { new Guid("6ecb8c16-d048-4e9b-8d30-26792bf54de4"), "daily", true, true, null, "TestHabit", (byte)1, new TimeOnly(12, 0, 0), null, "[\"monday\",\"saturday\"]", new DateOnly(2023, 11, 25), new Guid("62907db2-763d-4fcf-98fa-d8f0da76d00b") });
        }
    }
}
