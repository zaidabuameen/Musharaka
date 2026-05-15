using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Musharaka.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAdminIdToParty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AdminId",
                table: "PoliticalParties",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AdminId",
                table: "PoliticalParties");
        }
    }
}
