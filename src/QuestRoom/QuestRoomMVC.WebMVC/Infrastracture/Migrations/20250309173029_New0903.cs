using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuestRoomMVC.WebMVC.Infrastracture.Migrations
{
    /// <inheritdoc />
    public partial class New0903 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Adress",
                table: "Location",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Adress",
                table: "Location");
        }
    }
}
