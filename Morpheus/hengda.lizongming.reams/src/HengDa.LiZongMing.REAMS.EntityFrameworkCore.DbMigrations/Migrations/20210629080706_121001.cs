using Microsoft.EntityFrameworkCore.Migrations;

namespace HengDa.LiZongMing.REAMS.Migrations
{
    public partial class _121001 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "xml2",
                table: "Iot_NaiRecord",
                type: "longtext CHARACTER SET utf8mb4",
                maxLength: 8000,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xml2",
                table: "Iot_NaiRecord");
        }
    }
}
