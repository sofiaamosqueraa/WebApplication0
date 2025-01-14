using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication0.Migrations
{
    /// <inheritdoc />
    public partial class SyncDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           // migrationBuilder.CreateTable(
        //     name: "Users",
        //     columns: table => new
        //     {
        //         Id = table.Column<int>(nullable: false)
        //             .Annotation("SqlServer:Identity", "1, 1"),
        //         Name = table.Column<string>(nullable: false),
        //         Email = table.Column<string>(nullable: false),
        //         Password = table.Column<string>(nullable: false),
        //         IsAdmin = table.Column<bool>(nullable: false)
        //     },
        //     constraints: table =>
        //     {
        //         table.PrimaryKey("PK_Users", x => x.Id);
        //     });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // migrationBuilder.DropTable(
            //     name: "Users");
        }
    }
}
