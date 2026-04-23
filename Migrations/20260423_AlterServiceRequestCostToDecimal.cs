using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROG7311Part2.Migrations
{
    public partial class AlterServiceRequestCostToDecimal_Manual : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "CostTemp",
                table: "ServiceRequests",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE ServiceRequests
                SET CostTemp = TRY_CONVERT(decimal(18,2), Cost)
            ");

            migrationBuilder.Sql(@"
                UPDATE ServiceRequests
                SET CostTemp = 0.00
                WHERE CostTemp IS NULL
            ");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "ServiceRequests");

            migrationBuilder.RenameColumn(
                name: "CostTemp",
                table: "ServiceRequests",
                newName: "Cost");

            migrationBuilder.AlterColumn<decimal>(
                name: "Cost",
                table: "ServiceRequests",
                type: "decimal(18,2)",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,2)",
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CostTemp",
                table: "ServiceRequests",
                type: "float",
                nullable: true);

            migrationBuilder.Sql(@"
                UPDATE ServiceRequests
                SET CostTemp = TRY_CONVERT(float, Cost)
            ");

            migrationBuilder.DropColumn(
                name: "Cost",
                table: "ServiceRequests");

            migrationBuilder.RenameColumn(
                name: "CostTemp",
                table: "ServiceRequests",
                newName: "Cost");

            migrationBuilder.AlterColumn<double>(
                name: "Cost",
                table: "ServiceRequests",
                type: "float",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);
        }
    }
}