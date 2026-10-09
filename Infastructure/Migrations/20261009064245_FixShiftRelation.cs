using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixShiftRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShiftId",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Shifts",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Shifts");

            migrationBuilder.AddColumn<int>(
                name: "ShiftId",
                table: "Users",
                type: "integer",
                nullable: true);
        }
    }
}
