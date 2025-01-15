using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication0.Migrations
{
    public partial class AddInitialCompanies : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Companies",
                columns: new[] { "Name", "Address" },
                values: new object[,]
                {
                    { "Lusocargo", "Endereço 1" },
                    { "Infraspeak", "Endereço 2" },
                    { "Corkart", "Endereço 3" },
                    { "BSK Medical", "Endereço 4" },
                    { "Nucase grupo", "Endereço 5" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Companies",
                keyColumn: "Name",
                keyValues: new object[] { "Lusocargo", "Infraspeak", "Corkart", "BSK Medical", "Nucase grupo" });
        }
    }
}


