using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace _3.WebShop.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddedCustomersSeeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Address", "Age", "City", "Country", "Email", "Gender", "MaskedCreditCard", "Name", "OtherContactInfo", "PreferredPaymentMethod", "phone" },
                values: new object[,]
                {
                    { 1, "Storgatan 1", 28, "Stockholm", "Sverige", "anna@mail.se", "Female", "1111", "Anna Andersson", "-", "Card", "070-1234567" },
                    { 2, "Lilla Torget 5", 45, "Göteborg", "Sverige", "erik@mail.se", "Male", "2222", "Erik Karlsson", "-", "Swish", "073-9876543" },
                    { 3, "Trädgårdsgatan 12", 32, "Malmö", "Sverige", "sara@mail.se", "Female", "3333", "Sara Nilsson", "-", "Swish", "076-5554433" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 3);
        }
    }
}
