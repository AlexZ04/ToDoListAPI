using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ToDoList.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddCheckedField : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsChecked",
                table: "Tasks",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsChecked",
                table: "Tasks");
        }
    }
}
