using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CourseScheduleSystem.Web.Migrations
{
    /// <inheritdoc />
    public partial class AddCpModule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Intake",
                table: "ClassRepresentatives",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Level",
                table: "ClassRepresentatives",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ClassGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    ClassRepresentativeId = table.Column<int>(type: "int", nullable: false),
                    GroupName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Intake = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Level = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    GroupLink = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsPublished = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClassGroups_ClassRepresentatives_ClassRepresentativeId",
                        column: x => x.ClassRepresentativeId,
                        principalTable: "ClassRepresentatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClassGroups_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CourseCompletions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseId = table.Column<int>(type: "int", nullable: false),
                    ClassRepresentativeId = table.Column<int>(type: "int", nullable: false),
                    LecturerId = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Remarks = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    SubmittedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ConfirmedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseCompletions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CourseCompletions_ClassRepresentatives_ClassRepresentativeId",
                        column: x => x.ClassRepresentativeId,
                        principalTable: "ClassRepresentatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseCompletions_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CourseCompletions_Lecturers_LecturerId",
                        column: x => x.LecturerId,
                        principalTable: "Lecturers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "HodMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ClassRepresentativeId = table.Column<int>(type: "int", nullable: false),
                    HodId = table.Column<int>(type: "int", nullable: false),
                    Message = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    SentAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HodMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HodMessages_ClassRepresentatives_ClassRepresentativeId",
                        column: x => x.ClassRepresentativeId,
                        principalTable: "ClassRepresentatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassGroups_ClassRepresentativeId",
                table: "ClassGroups",
                column: "ClassRepresentativeId");

            migrationBuilder.CreateIndex(
                name: "IX_ClassGroups_CourseId_ClassRepresentativeId_Intake_Level",
                table: "ClassGroups",
                columns: new[] { "CourseId", "ClassRepresentativeId", "Intake", "Level" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseCompletions_ClassRepresentativeId",
                table: "CourseCompletions",
                column: "ClassRepresentativeId");

            migrationBuilder.CreateIndex(
                name: "IX_CourseCompletions_CourseId_ClassRepresentativeId",
                table: "CourseCompletions",
                columns: new[] { "CourseId", "ClassRepresentativeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CourseCompletions_LecturerId",
                table: "CourseCompletions",
                column: "LecturerId");

            migrationBuilder.CreateIndex(
                name: "IX_HodMessages_ClassRepresentativeId",
                table: "HodMessages",
                column: "ClassRepresentativeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassGroups");

            migrationBuilder.DropTable(
                name: "CourseCompletions");

            migrationBuilder.DropTable(
                name: "HodMessages");

            migrationBuilder.DropColumn(
                name: "Intake",
                table: "ClassRepresentatives");

            migrationBuilder.DropColumn(
                name: "Level",
                table: "ClassRepresentatives");
        }
    }
}
