using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PROG7311Part2.Migrations
{
    public partial class AddPdfAttachmentToServiceRequest : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PDFAttachmentFilePath",
                table: "ServiceRequests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PDFAttachmentFilePath",
                table: "ServiceRequests");
        }
    }
}
