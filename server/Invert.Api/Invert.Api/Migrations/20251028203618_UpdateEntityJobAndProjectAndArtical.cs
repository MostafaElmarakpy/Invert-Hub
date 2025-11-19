using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Invert.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEntityJobAndProjectAndArtical : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Salary",
                table: "Jobs",
                type: "decimal(18,2)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "BenefitsJson",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequirementsJson",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "SkillsJson",
                table: "Jobs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Author",
                table: "Articles",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BenefitsJson",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "RequirementsJson",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "SkillsJson",
                table: "Jobs");

            migrationBuilder.DropColumn(
                name: "Author",
                table: "Articles");

            migrationBuilder.AlterColumn<string>(
                name: "Salary",
                table: "Jobs",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldMaxLength: 50);
        }
    }
}
