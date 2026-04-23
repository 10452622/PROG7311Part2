using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROG7311Part2.Migrations
{
    public partial class SafeConvertContactDetailsToInt : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ContactDetailsTemp",
                table: "Clients",
                type: "int",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE Clients
                SET ContactDetailsTemp = TRY_CAST(ContactDetails AS int)
            ");

            migrationBuilder.Sql(@"
                UPDATE Clients
                SET ContactDetailsTemp = 0
                WHERE ContactDetailsTemp IS NULL
            ");

            migrationBuilder.DropColumn(
                name: "ContactDetails",
                table: "Clients");

            migrationBuilder.RenameColumn(
                name: "ContactDetailsTemp",
                table: "Clients",
                newName: "ContactDetails");

            migrationBuilder.AlterColumn<int>(
                name: "ContactDetails",
                table: "Clients",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.AddColumn<string>(
                name: "ContactDetailsTemp",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE Clients
                SET ContactDetailsTemp = CAST(ContactDetails AS nvarchar(max))
            ");

            migrationBuilder.DropColumn(
                name: "ContactDetails",
                table: "Clients");

            migrationBuilder.RenameColumn(
                name: "ContactDetailsTemp",
                table: "Clients",
                newName: "ContactDetails");

            migrationBuilder.AlterColumn<string>(
                name: "ContactDetails",
                table: "Clients",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
