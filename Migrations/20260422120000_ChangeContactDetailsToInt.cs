using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROG7311Part2.Migrations
{
    public partial class ChangeContactDetailsToInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "ContactDetails",
                table: "Clients",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "ContactDetails",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
