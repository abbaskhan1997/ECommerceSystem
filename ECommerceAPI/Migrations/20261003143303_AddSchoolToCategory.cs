using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSchoolToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SchoolId",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Categories_SchoolId",
                table: "Categories",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_Schools_SchoolId",
                table: "Categories",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_Schools_SchoolId",
                table: "Categories");

            migrationBuilder.DropIndex(
                name: "IX_Categories_SchoolId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "Categories");
        }
    }
}
