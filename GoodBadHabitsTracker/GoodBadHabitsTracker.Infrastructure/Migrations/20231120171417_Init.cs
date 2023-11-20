using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoodBadHabitsTracker.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Habits",
                columns: table => new
                {
                    HabitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsGood = table.Column<bool>(type: "bit", nullable: false),
                    IsGoalInTime = table.Column<bool>(type: "bit", nullable: true),
                    Quantity = table.Column<byte>(type: "tinyint", nullable: true),
                    Frequency = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IsRepeatDaily = table.Column<bool>(type: "bit", nullable: true),
                    RepeatDaysOfWeek = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RepeatDaysOfMonth = table.Column<byte[]>(type: "varbinary(max)", nullable: true),
                    StartDate = table.Column<DateOnly>(type: "date", nullable: false),
                    ReminderTime = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Habits", x => x.HabitId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Habits");
        }
    }
}
