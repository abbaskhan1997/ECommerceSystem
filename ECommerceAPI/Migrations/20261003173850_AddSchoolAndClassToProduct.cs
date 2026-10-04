using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ECommerceAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddSchoolAndClassToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolClasses_Schools_SchoolId",
                table: "SchoolClasses");

            migrationBuilder.AddColumn<int>(
                name: "SchoolClassId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "SchoolId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Products_SchoolClassId",
                table: "Products",
                column: "SchoolClassId");

            migrationBuilder.CreateIndex(
                name: "IX_Products_SchoolId",
                table: "Products",
                column: "SchoolId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_SchoolClasses_SchoolClassId",
                table: "Products",
                column: "SchoolClassId",
                principalTable: "SchoolClasses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Schools_SchoolId",
                table: "Products",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolClasses_Schools_SchoolId",
                table: "SchoolClasses",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_SchoolClasses_SchoolClassId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_Products_Schools_SchoolId",
                table: "Products");

            migrationBuilder.DropForeignKey(
                name: "FK_SchoolClasses_Schools_SchoolId",
                table: "SchoolClasses");

            migrationBuilder.DropIndex(
                name: "IX_Products_SchoolClassId",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "IX_Products_SchoolId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SchoolClassId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "SchoolId",
                table: "Products");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Categories_CategoryId",
                table: "Products",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SchoolClasses_Schools_SchoolId",
                table: "SchoolClasses",
                column: "SchoolId",
                principalTable: "Schools",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
