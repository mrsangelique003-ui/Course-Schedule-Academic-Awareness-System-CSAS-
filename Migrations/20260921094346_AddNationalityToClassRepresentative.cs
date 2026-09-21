using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseScheduleSystem.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddNationalityToClassRepresentative : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Nationality",
                table: "ClassRepresentatives",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nationality",
                table: "ClassRepresentatives");
        }
    }
}
