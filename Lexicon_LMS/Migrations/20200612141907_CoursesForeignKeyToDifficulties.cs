using Microsoft.EntityFrameworkCore.Migrations;

namespace Lexicon_LMS.Migrations
{
    public partial class CoursesForeignKeyToDifficulties : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DifficultyId",
                table: "Courses",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Courses_DifficultyId",
                table: "Courses",
                column: "DifficultyId");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Difficulties_DifficultyId",
                table: "Courses",
                column: "DifficultyId",
                principalTable: "Difficulties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Difficulties_DifficultyId",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_DifficultyId",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "DifficultyId",
                table: "Courses");
        }
    }
}
