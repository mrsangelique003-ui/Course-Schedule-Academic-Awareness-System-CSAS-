using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseScheduleSystem.Web.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClassRepresentativeModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "StudySession",
                table: "ClassRepresentatives",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Faculty",
                table: "ClassRepresentatives",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SignatureImageData",
                table: "ClassRepresentatives",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "ClassRepresentatives",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Faculty",
                table: "ClassRepresentatives");

            migrationBuilder.DropColumn(
                name: "SignatureImageData",
                table: "ClassRepresentatives");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "ClassRepresentatives");

            migrationBuilder.AlterColumn<int>(
                name: "StudySession",
                table: "ClassRepresentatives",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);
        }
    }
}
