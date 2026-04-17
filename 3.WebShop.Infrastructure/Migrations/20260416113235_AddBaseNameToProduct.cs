using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace _3.WebShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddBaseNameToProduct : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StockBalance",
                table: "Products");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StockBalance",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
