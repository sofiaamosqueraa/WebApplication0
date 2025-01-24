using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication0.Migrations
{
    /// <inheritdoc />
    public partial class AddDataCriacaoToInvitation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DataCriacao",
                table: "Invitations",
                type: "datetime2",
                nullable: false,
                defaultValue: DateTime.UtcNow);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataCriacao",
                table: "Invitations");
        }
    }
}
