using Microsoft.EntityFrameworkCore.Migrations;

namespace Lexicon_LMS.Migrations
{
    public partial class DocFilePath : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FilePath",
                table: "Documents",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FilePath",
                table: "Documents");
        }
    }
}
