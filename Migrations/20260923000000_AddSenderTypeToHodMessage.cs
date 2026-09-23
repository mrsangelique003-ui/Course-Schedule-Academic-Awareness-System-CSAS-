using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseScheduleSystem.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddSenderTypeToHodMessage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SenderType",
                table: "HodMessages",
                type: "int",
                nullable: false,
                defaultValue: 0);   // 0 = MessageSenderType.Hod
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SenderType",
                table: "HodMessages");
        }
    }
}
